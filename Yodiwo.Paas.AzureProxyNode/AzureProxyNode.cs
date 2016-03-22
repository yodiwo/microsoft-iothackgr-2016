using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.NodeLibrary;
using Yodiwo.API.Plegma;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Yodiwo.PaaS.Azure;
using System.Xml.Serialization;
using Yodiwo.PaaS.Azure.Application;


namespace Yodiwo.PaaS.AzureProxyNode
{

    public class AzureProxyNode
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        Yodiwo.NodeLibrary.Node proxyNode;
        ListTS<Thing> Things = new ListTS<Thing>();
        NodeDescriptor ActiveCfg;
        Azure.Application.AzureIOTApplication azureAppClient;
        //------------------------------------------------------------------------------------------------------------------------

        //------------------------------------------------------------------------------------------------------------------------
        public static Dictionary<string, ioPortDirection> LookupPortDirection = new Dictionary<string, ioPortDirection>()
        {
            {"input", ioPortDirection.Input},
            {"output",ioPortDirection.Output},
        };
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        public static Dictionary<string, ePortType> LookupThingType = new Dictionary<string, ePortType>()
        {
            {"integer", ePortType.Integer},
            {"float",ePortType.Decimal},
            {"bool",ePortType.Boolean },
            {"string",ePortType.String}
        };
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        public static Dictionary<ePortType, string> LookupThingState = new Dictionary<ePortType, string>()
        {
            { ePortType.Integer,"0"},
            {ePortType.Decimal,"0"},
            {ePortType.Boolean,"false"},
            {ePortType.String,""}
        };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public void Init()
        {
            //load config
            XmlSerializer ser = new XmlSerializer(typeof(NodeDescriptor));
            if (!File.Exists("config.xml"))
            {
                DebugEx.TraceLog("Could not find config.xml");
                return;
            }
            else {
                using (var fs = new FileStream("config.xml", FileMode.Open))
                    ActiveCfg = ser.Deserialize(fs) as NodeDescriptor;
            }

            //first check/validation in config.xml
            if (ActiveCfg?.AzureInfo?.AzureDeviceName == null || ActiveCfg?.AzureInfo?.AzureIOTApplicationConnectionString == null)
            {
                DebugEx.TraceLog("Not Valid Configuration in IBMInfo");
                return;
            }
            if (ActiveCfg?.YodiwoInfo?.YodiwoApiServer == null || ActiveCfg?.YodiwoInfo?.YodiwoNodeKey == null || ActiveCfg?.YodiwoInfo?.YodiwoRestUrl == null || ActiveCfg?.YodiwoInfo?.YodiwoSecretKey == null || ActiveCfg?.YodiwoInfo?.YPChannelPort == null)
            {
                DebugEx.TraceLog("Not Valid Configuration in YodiwoInfo");
                return;
            }
            if (ActiveCfg.ThingsDescription.ThingDescriptor.Count == 0)
            {
                DebugEx.TraceLog("List of Things is empty");
                return;
            }
            else
            {
                foreach (var thdesc in ActiveCfg.ThingsDescription.ThingDescriptor)
                {
                    if (thdesc?.Name == null || thdesc?.IO == null || thdesc?.Type == null)
                    {
                        DebugEx.TraceError("Check thing configurations!!! " + ((thdesc.Name != null) ? thdesc.Name : String.Empty));
                        return;
                    }
                    else
                    {
                        //reconstruct thing from thing config description
                        var th = ReconstructThing(thdesc);
                        if (th != null)
                            Things.Add(th);
                    }
                }
            }
            InitAzureConnection();
            InitYodiwoConnection();
        }
        //------------------------------------------------------------------------------------------------------------------------
        private Thing ReconstructThing(ThingDescriptor thdesc)
        {
            if (!LookupThingType.ContainsKey(thdesc.Type))
            {
                DebugEx.TraceError("Not Valid type in: " + thdesc.Name.ToLowerInvariant());
                return null;
            }
            if (!LookupPortDirection.ContainsKey(thdesc.IO.ToLowerInvariant()))
            {
                DebugEx.TraceError("Not Valid IO in: " + thdesc.Name);
                return null;
            }

            Thing thing = new Thing()
            {
                Name = thdesc?.Name,
                Config = null,
                UIHints = new ThingUIHints()
                {
                    IconURI = thdesc?.FriendlyIcon
                }
            };
            thing.Ports = new List<Port>()
            {
                new Port() {
                    ioDirection = LookupPortDirection[thdesc.IO],
                    Name =thing.Name +"Value",
                    Type = LookupThingType[thdesc.Type],
                     PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0"),
                     State = LookupThingState[LookupThingType[thdesc.Type]],
                }
            };
            return thing;
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void RegisterThings()
        {

            proxyNode.PortEventHandlers[Things.Find(i => i.Name == "Led").Ports[0]] = data =>
                    {
                        var payload = new AzureIOTPayoad()
                        {
                            name = ActiveCfg.AzureInfo.AzureDeviceName,
                            Led = Convert.ToInt32(data),
                            ThingName = "Led"
                        };
                        azureAppClient.SendC2DMessages(ActiveCfg.AzureInfo.AzureDeviceName, payload);
                    };
            proxyNode.PortEventHandlers[Things.Find(i => i.Name == "LCD").Ports[0]] = data =>
            {
                var payload = new AzureIOTPayoad()
                {
                    name = ActiveCfg.AzureInfo.AzureDeviceName,
                    LCD = data,
                    ThingName = "LCD"
                };
                azureAppClient.SendC2DMessages(ActiveCfg.AzureInfo.AzureDeviceName, payload);

            };

        }
        //------------------------------------------------------------------------------------------------------------------------
        private void InitYodiwoConnection()
        {
            //create node configuration
            NodeConfig conf = new NodeConfig()
            {
                Name = "Azure Clone of " + ActiveCfg.AzureInfo.AzureDeviceName,
                YpServer = ActiveCfg.YodiwoInfo.YodiwoApiServer,
                YpchannelPort = Convert.ToInt32(ActiveCfg.YodiwoInfo.YPChannelPort),
                FrontendServer = ActiveCfg.YodiwoInfo.YodiwoRestUrl,
                CanSolveGraphs = false,
                SecureYpc = ActiveCfg.YodiwoInfo.SecureYPC,
            };

            //create the proxy node
            proxyNode = new Yodiwo.NodeLibrary.Node(conf, Things, null, null, null);

            //set transport
            proxyNode.Transport = Transport.YPCHANNEL;

            //register cbs
            proxyNode.OnChangedState += OnChangedStateCb;
            proxyNode.OnTransportConnected += OnTransportConnectedCb;
            proxyNode.OnTransportDisconnected += OnTransportDisconnectedCb;
            proxyNode.OnTransportError += OnTransportErrorCb;
            proxyNode.OnUnexpectedMessage = OnUnexpectedMessageCb;
            proxyNode.OnThingActivated += OnThingActivatedCb;

            //use nodekey to setup things
            proxyNode.SetupNodeKeys(ActiveCfg.YodiwoInfo.YodiwoNodeKey, ActiveCfg.YodiwoInfo.YodiwoSecretKey);

            //connect
            proxyNode.Connect();
            RegisterThings();
        }

        //------------------------------------------------------------------------------------------------------------------------
        private void OnTransportConnectedCb(Yodiwo.NodeLibrary.Transport Transport, string msg)
        {
            DebugEx.TraceLog("OnConnected transport=" + Transport.ToString() + " msg=" + msg);
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void OnThingActivatedCb(Thing thing)
        {
            DebugEx.TraceLog("OnThingActivated: " + thing.ThingKey);
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void OnTransportDisconnectedCb(Yodiwo.NodeLibrary.Transport Transport, string msg)
        {
            DebugEx.TraceLog("OnDisconnected transport=" + Transport.ToString() + " msg=" + msg);
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void OnTransportErrorCb(Yodiwo.NodeLibrary.Transport Transport, TransportErrors Error, string msg)
        {
            DebugEx.TraceLog("OnTransportError transport=" + Transport.ToString() + " msg=" + msg);
        }
        //------------------------------------------------------------------------------------------------------------------------
        void OnChangedStateCb(Thing thing, Port port, string state)
        {
            DebugEx.TraceLog("On ChangedStatecb....");
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void OnUnexpectedMessageCb(object message)
        {
            DebugEx.TraceLog("Unexpected message received.");
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void InitAzureConnection()
        {
            //register Azure application, which Receives Messages
            azureAppClient = new Azure.Application.AzureIOTApplication(ActiveCfg.AzureInfo.AzureIOTApplicationConnectionString);
            azureAppClient.OnAzureIOTHubRxCb = OnAzureIOTHubThingRxMessage;
            //start rx messages from dev side
            azureAppClient.StartReceivingD2CMessages();

        }
        //------------------------------------------------------------------------------------------------------------------------
        private void OnAzureIOTHubThingRxMessage(string partition, string data)
        {
            DebugEx.TraceLog("On Azure IOT Hub MessageRx");
            //deserialize message
            var payload = data.FromJSON<AzureIOTPayoad>();
            var th = Things.Find(i => i.Name == payload.ThingName);
            if (payload.ThingName == "RotaryAngleSensor")
                proxyNode.SetState(th, th.Ports[0], payload.RotaryAngleSensor.ToString());
            else if (payload.ThingName == "Light")
                proxyNode.SetState(th, th.Ports[0], payload.Light.ToString());
            else if (payload.ThingName == "Button")
                proxyNode.SetState(th, th.Ports[0], payload.Button.ToString());

            //check name with deviceid
#if false
            if (payload["name"].ToString() != ActiveCfg.AzureInfo.AzureDeviceName)
                return;
            foreach (var th in Things)
            {
                try
                {
                    //TODO: scan all things and send one batch port event message to the YodiwoCloud
                    //is there any value for the specific thing
                    var sensor = payload[]
                    var y = payload[th.Name];
                    //send porteventmessage
                    proxyNode.SetState(th, th.Ports[0], y.ToString());
                }
                catch (Exception ex)
                {
                    DebugEx.TraceError(ex.Message);
                }
#endif

        }
    }
    //------------------------------------------------------------------------------------------------------------------------

    #endregion
}




