//#define USE_Speech

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;
using Yodiwo.API.MediaStreaming;

namespace SampleNode
{
    public static class Helper
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public static Yodiwo.API.Plegma.Thing Button1Thing;
        public static Yodiwo.API.Plegma.Thing Button2Thing;
        public static Yodiwo.API.Plegma.Thing Button3Thing;
        public static Yodiwo.API.Plegma.Thing CheckBox1Thing;
        public static Yodiwo.API.Plegma.Thing CheckBox2Thing;
        public static Yodiwo.API.Plegma.Thing Slider1Thing;

        public static Yodiwo.API.Plegma.Thing Light1Thing;
        public static Yodiwo.API.Plegma.Thing Light2Thing;
        public static Yodiwo.API.Plegma.Thing Light3Thing;

        public static Yodiwo.API.Plegma.Thing TextThing;

        public static Yodiwo.API.Plegma.Thing SpeechRegThing;
        public static Yodiwo.API.Plegma.Thing Text2SpeechThing;

        public static Yodiwo.API.Plegma.Thing AccelerometerThing;
        public static Yodiwo.API.Plegma.Thing FallThing;

        private static List<Thing> Things = new List<Thing>();

        #endregion
        //------------------------------------------------------------------------------------------------------------------------

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        //initialize node's things
        public static List<Thing> GatherThings()
        {
            //setup CheckBox 1 thing
            #region Setup CheckBox 1 thing
            {
                var thing = CheckBox1Thing = new Yodiwo.API.Plegma.Thing()
                {
                    Type = "yodiwo.output.checkboxes",
                    Name = "Virtual CheckBox 1",
                    Config = null,
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "/Content/VirtualGateway/img/icon-thing-checkbox.png",
                    },
                };
                thing.Ports = new List<Yodiwo.API.Plegma.Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "CheckState",
                        State = "false",
                        Type = Yodiwo.API.Plegma.ePortType.Boolean,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0")
                    }
                };
                Things.Add(thing);
            }
            #endregion

            //setup Text 1 thing
            #region Setup CheckBox 1 thing
            {
                var thing = TextThing = new Yodiwo.API.Plegma.Thing()
                {
                    Type = "yodiwo.output.labels",
                    Name = "Text 1",
                    Config = null,
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "/Content/VirtualGateway/img/icon-thing-text.png",
                    },
                };
                thing.Ports = new List<Yodiwo.API.Plegma.Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                        Name = "Text",
                        State = "",
                        Type = Yodiwo.API.Plegma.ePortType.String,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0")
                    }
                };
                Things.Add(thing);
            }
            #endregion

            //setup slider 1 thing
            #region Setup slider 1 thing
            {
                var thing = Slider1Thing = new Yodiwo.API.Plegma.Thing()
                {
                    Type = "yodiwo.output.slider",
                    Name = "Slider 1",
                    Config = null,
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "/Content/VirtualGateway/img/icon-thing-slider.png",
                    },
                };
                thing.Ports = new List<Yodiwo.API.Plegma.Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "Value",
                        State = "0",
                        Type = Yodiwo.API.Plegma.ePortType.Decimal,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0")
                    }
                };
                Things.Add(thing);
            }
            #endregion

            //setup Light 1 thing
            #region Setup Light 1 thing
            {
                var thing = Light1Thing = new Yodiwo.API.Plegma.Thing()
                {
                    Type = "yodiwo.input.lights.dimmable",
                    Name = "Virtual Light 1",
                    Config = null,
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "/Content/VirtualGateway/img/icon-thing-genericlight.png",
                    },
                };
                thing.Ports = new List<Yodiwo.API.Plegma.Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                        Name = "LightState",
                        State = "0",
                        Type = Yodiwo.API.Plegma.ePortType.Decimal,
                        ConfFlags = ePortConf.PropagateAllEvents,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0")
                    }
                };
                Things.Add(thing);
            }
            #endregion

            //setup SpeechReg thing
            #region Setup SpeechReg thing
            {
                var thing = SpeechRegThing = new Yodiwo.API.Plegma.Thing()
                {
                    Type = "yodiwo.output.speechrecognition",
                    Name = "Virtual SpeechReg",
                    Config = null,
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "/Content/VirtualGateway/img/icon-thing-voicerecognition.png",
                    },
                };
                thing.Ports = new List<Yodiwo.API.Plegma.Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "Text",
                        State = "",
                        Type = Yodiwo.API.Plegma.ePortType.String,
                        ConfFlags = ePortConf.PropagateAllEvents,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0")
                    }
                };
                Things.Add(thing);
            }
            #endregion

            //setup SpeechReg thing
            #region Setup Text2Speech thing
            {
                var thing = Text2SpeechThing = new Yodiwo.API.Plegma.Thing()
                {
                    Type = "yodiwo.input.text2speech",
                    Name = "Virtual Text2Speech",
                    Config = null,
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "/Content/VirtualGateway/img/icon-thing-text2speech.png",
                    },
                };
                thing.Ports = new List<Yodiwo.API.Plegma.Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                        Name = "Text",
                        State = "",
                        Type = Yodiwo.API.Plegma.ePortType.String,
                        ConfFlags = ePortConf.PropagateAllEvents,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0")
                    }
                };
                Things.Add(thing);
            }
            #endregion


            //setup thing
            #region accell 1 thing
            {
                var thing = AccelerometerThing = new Yodiwo.API.Plegma.Thing()
                {
                    Type = "yodiwo.output.accelerometer",
                    Name = "Accelerometer",
                    Config = null,
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "/Content/VirtualGateway/img/icon-thing-slider.png",
                    },
                };
                thing.Ports = new List<Yodiwo.API.Plegma.Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "X",
                        State = "0",
                        Type = Yodiwo.API.Plegma.ePortType.Decimal,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0")
                    },
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "Y",
                        State = "0",
                        Type = Yodiwo.API.Plegma.ePortType.Decimal,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "1")
                    },
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "Z",
                        State = "0",
                        Type = Yodiwo.API.Plegma.ePortType.Decimal,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "2")
                    },
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "Length",
                        State = "0",
                        Type = Yodiwo.API.Plegma.ePortType.Decimal,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "3")
                    },
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "Shaken",
                        State = "0",
                        ConfFlags = ePortConf.PropagateAllEvents,
                        Type = Yodiwo.API.Plegma.ePortType.Boolean,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "4")
                    }
                };
                Things.Add(thing);
            }
            #endregion



            //setup  thing
            #region Setup fall thing
            {
                var thing = FallThing = new Yodiwo.API.Plegma.Thing()
                {
                    Type = "yodiwo.output.accelerometer",
                    Name = "Fall Detector",
                    Config = null,
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "/Content/VirtualGateway/img/icon-thing-checkbox.png",
                    },
                };
                thing.Ports = new List<Yodiwo.API.Plegma.Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "Fall Event",
                        State = "false",
                        ConfFlags = ePortConf.PropagateAllEvents,
                        Type = Yodiwo.API.Plegma.ePortType.Boolean,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0")
                    }
                };
                Things.Add(thing);
            }
            #endregion



            return Things;
        }
        //------------------------------------------------------------------------------------------------------------------------
#if USE_Speech
        public static void OnRecognized(string msg)
        {
            OnSpeechReccb(msg);
        }
#endif
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }


}
