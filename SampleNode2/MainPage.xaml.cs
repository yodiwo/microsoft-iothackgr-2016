using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Yodiwo;
using Yodiwo.API.Plegma;
using Yodiwo.NodeLibrary;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SampleNode
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static VirtNodeConfig ActiveCfg;
        Yodiwo.API.Plegma.NodeKey NodeKey { get { return ActiveCfg.NodeKey; } }
        Yodiwo.NodeLibrary.Node node;
        Transport transport = Transport.YPCHANNEL;
        Yodiwo.Node.Pairing.CommonDevicePairingPolling pairmodule;
        Accelerometer accelerometer;
        Accelerometer accelerometer2;

        Yodiwo.Tools.StorageService _storageService = new Yodiwo.Tools.StorageService(false);

        // The speech recognizer used throughout this sample.
        private SpeechRecognizer speechRecognizer;


        public MainPage()
        {
            this.InitializeComponent();

            //load configs
            ActiveCfg = VirtNodeConfig.GetDefaultConfig();

            //load keys (untile a full configuration ui is implemented, just load these 2 for now)
            ActiveCfg.NodeKey = _storageService.LoadSettingString("NodeKey");
            ActiveCfg.NodeSecret = _storageService.LoadSettingString("SecretKey");

            //add me as entry assembly
            TypeCache.AddEntryAssembly(typeof(Program));

            //init nodelibrary confs
            NodeConfig conf = new NodeConfig()
            {
                uuid = "UniversalSampleNode2", //ActiveCfg.Uuid,
                Name = "Universal Sample Node",
                YpServer = ActiveCfg.ApiServer,
                YpchannelPort = ActiveCfg.YpchannelPort,
                SecureYpc = ActiveCfg.YpchannelSecure,
                CertificationServerName = ActiveCfg.CertificationServerName,
                FrontendServer = ActiveCfg.FrontendServer,
                CanSolveGraphs = ActiveCfg.CanSolveGraphs,
                Pairing_CompletionInstructions = "Close the browser tab and switch back to the App",
                Pairing_NoUUIDAuthentication = true, //same machine authentication, no uuid authentiation required

                EnableNodeDiscovery = false,
#if false
                NodeDiscovery_YPCPort_Start = 5000,
                NodeDiscovery_YPCPort_End = 65000,
#else
                NodeDiscovery_YPCPort_Start = 0,
                NodeDiscovery_YPCPort_End = 0,
#endif
            };

            //prepare pairing module
            pairmodule = new Yodiwo.Node.Pairing.CommonDevicePairingPolling();

            //create node
            node = new Yodiwo.NodeLibrary.Node(conf,
                                                Helper.GatherThings(),
                                                pairmodule,
                                                NodeDataLoad, NodeDataSave
                                                );

            //register node  cbs
            //node.OnChangedState += ChangedState;
            node.OnTransportConnected += OnConnected;
            node.OnTransportDisconnected += OnDisconnected;
            node.OnTransportError += OnTransportError;
            node.OnNodePaired += OnPaired;
            node.OnUnexpectedMessage = HandleUnknownMessage;
            node.OnThingActivated += Node_OnThingActivated;
            node.OnThingDeactivated += Node_OnThingDeactivated;

            //register port events
            RegisterThings();
        }


        public byte[] NodeDataLoad(string Identifier, bool Secure)
        {
            if (File.Exists(Identifier) == false)
                return null;
            else
                return File.ReadAllBytes(Identifier);
        }
        public bool NodeDataSave(string Identifier, byte[] Data, bool Secure)
        {
            try
            {
                File.WriteAllBytes(Identifier, Data);
                return true;
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Data save failed");
                return false;
            }
        }

        void RegisterThings()
        {
            node.PortEventHandlers[Helper.TextThing.Ports[0]] = async data =>
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                () =>
                {
                    txtbox1.Text = data;
                });
            };
            node.PortEventHandlers[Helper.Light1Thing.Ports[0]] = async data =>
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                () =>
                {
                    try
                    {
                        var val = data.ParseToFloat();
                        light1.Opacity = 1.0 - val;
                    }
                    catch { }
                });
            };
            node.PortEventHandlers[Helper.Text2SpeechThing.Ports[0]] = async data =>
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                () =>
                {
                    SpeakText(data);
                });
            };
        }

        //cb when node is paired
        void OnPaired(NodeKey nodekey, string secret)
        {
            ActiveCfg.NodeKey = nodekey;
            ActiveCfg.NodeSecret = secret;
            //ActiveCfg.Save();

            _storageService.SaveSetting("NodeKey", nodekey);
            _storageService.SaveSetting("SecretKey", secret);
        }

        void OnConnected(Transport Transport, string msg)
        {
            DebugEx.TraceLog("OnConnected transport=" + Transport.ToString() + " msg=" + msg);
        }
        async void OnDisconnected(Transport Transport, string msg)
        {
            DebugEx.TraceLog("OnDisconnected transport=" + Transport.ToString() + " msg=" + msg);
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                async () =>
                {
                    await MessageBox.Show("Transport (" + transport.ToStringInvariant() + ") disconnected");
                });
        }
        async void OnTransportError(Transport Transport, TransportErrors Error, string msg)
        {
            DebugEx.TraceLog("OnTransportError transport=" + Transport.ToString() + " msg=" + msg);
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                async () =>
                {
                    await MessageBox.Show("Transport (" + transport.ToStringInvariant() + ") Error (" + Error.ToStringInvariant() + "), msg=" + msg);
                });
        }

        void ChangedState(Thing thing, Port port, string state)
        {
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //show status form
            SetStatus("Initializing Virtual Node");

            //Check Pairing
            #region Check Pairing/connection
            if (NodeKey.IsInvalid)
            {
                SetStatus("Waiting for pairing completion");

                //start pairing
                await node.StartPairing(ActiveCfg.FrontendServer, null, ActiveCfg.LocalWebServer);

                //get result
                while (NodeKey.IsInvalid)
                    await Task.Delay(500);

                SetStatus("Pairing Completed");
            }
            else
            {
                //Setup node info from stored configurations
                node.SetupNodeKeys(ActiveCfg.NodeKey, ActiveCfg.NodeSecret);
            }


            //Connect
            if (this.transport != Transport.None)
            {
                SetStatus("Connecting " + this.transport.ToString() + " to worker");
                node.Transport = this.transport;
                node.Connect();
                //close status form
                SetStatus("Virtual Gateway started");
                //enable controls
                brdDisabler.Visibility = Visibility.Collapsed;
            }
            else
                await MessageBox.Show("Set your transport");

            speechRecognizer = new SpeechRecognizer(SpeechRecognizer.SystemSpeechLanguage);
            speechRecognizer.StateChanged += SpeechRecognizer_StateChanged; ;

            // Apply the dictation topic constraint to optimize for dictated freeform speech.
            var dictationConstraint = new SpeechRecognitionTopicConstraint(SpeechRecognitionScenario.Dictation, "dictation");
            speechRecognizer.Constraints.Add(dictationConstraint);
            SpeechRecognitionCompilationResult result = await speechRecognizer.CompileConstraintsAsync();
            if (result.Status != SpeechRecognitionResultStatus.Success)
                SetStatus("Speech recognition init failed");

            //start recogniser
            recognHeartBeat();

            //Workaround Alert : Accelerometers stop giving out events after a time for some reason.. so keep "refreshing" them every XXX time
            #region Start Accelerometers
            //start accelerometer
            Task.Run(() =>
            {
                while (true)
                {
                    if (accelerometer != null)
                    {
                        accelerometer.ReadingChanged -= accelerometer_ReadingChanged;
                        accelerometer.Shaken -= Accelerometer_Shaken;
                    }

                    accelerometer = Accelerometer.GetDefault();
                    accelerometer.ReportInterval = 300;
                    accelerometer.ReadingChanged += accelerometer_ReadingChanged;
                    accelerometer.Shaken += Accelerometer_Shaken;
                    Task.Delay(2000).Wait();
                }
            });


            //start accelerometer 2
            Task.Run(() =>
            {
                while (true)
                {
                    if (accelerometer2 != null)
                        accelerometer2.ReadingChanged -= accelerometer2_ReadingChanged;

                    accelerometer2 = Accelerometer.GetDefault();
                    accelerometer2.ReportInterval = 50;
                    accelerometer2.ReadingChanged += accelerometer2_ReadingChanged;
                    Task.Delay(2000).Wait();
                }
            });
            #endregion
        }

        private void Accelerometer_Shaken(Accelerometer sender, AccelerometerShakenEventArgs args)
        {
            try { node.SetState(Helper.AccelerometerThing.Ports[4], "true"); }
            catch (Exception ex) { }
        }

        void accelerometer_ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            try
            {
                var ev = new TupleS<Port, string>[4];
                ev[0].Item1 = Helper.AccelerometerThing.Ports[0];
                ev[0].Item2 = args.Reading.AccelerationX.ToString();

                ev[1].Item1 = Helper.AccelerometerThing.Ports[1];
                ev[1].Item2 = args.Reading.AccelerationY.ToString();

                ev[2].Item1 = Helper.AccelerometerThing.Ports[2];
                ev[2].Item2 = args.Reading.AccelerationZ.ToString();

                ev[3].Item1 = Helper.AccelerometerThing.Ports[3];
                var x = args.Reading.AccelerationX;
                var y = args.Reading.AccelerationY;
                var z = args.Reading.AccelerationZ;
                ev[3].Item2 = Math.Sqrt(x * x + y * y + z * z).ToString();

                node.SetState(ev);
            }
            catch (Exception ex) { }
        }

        DateTime lastEvent = DateTime.Now;
        void accelerometer2_ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            try
            {
                var x = args.Reading.AccelerationX;
                var y = args.Reading.AccelerationY;
                var z = args.Reading.AccelerationZ;
                var length = Math.Sqrt(x * x + y * y + z * z);
                if (length < 0.2f)
                {
                    if (DateTime.Now - lastEvent > TimeSpan.FromSeconds(1))
                    {
                        lastEvent = DateTime.Now;
                        node.SetState(Helper.FallThing.Ports[0], "true");
                    }
                }
            }
            catch (Exception ex) { }
        }

        private void SpeechRecognizer_StateChanged(SpeechRecognizer sender, SpeechRecognizerStateChangedEventArgs args)
        {
            SetStatus("SpeechReg : " + args.State.ToStringInvariant());
        }

        async void recognHeartBeat()
        {
            while (true)
            {
                try
                {
                    var res = await speechRecognizer.RecognizeAsync();
                    node.SetState(Helper.SpeechRegThing.Ports[0], res.Text);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("HRESULT: 0x80045509"))
                        return;

                    //penalty
                    await Task.Delay(200);
                }
            }
        }


        #endregion

        //configure MessageBox
        async void SetStatus(string status)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                    () =>
                    {
                        txtStatus.Text = status;
                    });
        }

        private async void Node_OnThingActivated(Thing thing)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
            () =>
            {
                chk_CheckBox1.Content = "Activated";
            });
        }

        private async void Node_OnThingDeactivated(Thing thing)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
            () =>
            {
                chk_CheckBox1.Content = "Deactivated";
            });
        }

        private void HandleUnknownMessage(object msg)
        {
            DebugEx.TraceError("Received unknown msg of type: " + msg.GetType());
        }

        #region UI Event Handlers
        private void chk_CheckBox1_Changed(object sender, RoutedEventArgs e)
        {
            node.SetState(Helper.CheckBox1Thing.Ports[0], chk_CheckBox1.IsChecked.Value.ToString());
        }
        #endregion

        private void slider1_Changed(object sender, RangeBaseValueChangedEventArgs e)
        {
            node.SetState(Helper.Slider1Thing.Ports[0], slider1.Value.ToString());
        }


        public async void SpeakText(string TTS)
        {
            try
            {
                var ttssynthesizer = new SpeechSynthesizer();

                //Set the Voice/Speaker
                using (var Speaker = new SpeechSynthesizer())
                {
                    Speaker.Voice = (SpeechSynthesizer.AllVoices.First(x => x.Gender == VoiceGender.Female));
                    ttssynthesizer.Voice = Speaker.Voice;
                }

                var ttsStream = await ttssynthesizer.SynthesizeTextToStreamAsync(TTS);

                //play the speech
                MediaElement media = new MediaElement();
                media.SetSource(ttsStream, " ");
            }
            catch (Exception ex)
            {
            }
        }

        private async void Unpair_Clicked(object sender, RoutedEventArgs e)
        {
            OnPaired(null, null);
            await MessageBox.Show("Keys deleted (localy). Closing app");
            Application.Current.Exit();
        }
    }
}
