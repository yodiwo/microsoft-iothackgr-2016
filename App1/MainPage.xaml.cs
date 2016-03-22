using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Azure.Devices.Client;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //------------------------------------------------------------------------------------------------------------------------
        private RotaryWatcher rotarywatcher;
        private ButtonWatcher buttonwatcher;
        private LightWatcher lightwatcher;
        private Led led;
        private LCD lcd;
        private DeviceClient deviceClient;
        //use the device id acquired from the Device Explorer
        public static string RaspName = "RaspberryIOT";
        //------------------------------------------------------------------------------------------------------------------------
        public MainPage()
        {
            this.InitializeComponent();
        }
        //------------------------------------------------------------------------------------------------------------------------
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //use the connection string aquired from the Device Explorer
            deviceClient = DeviceClient.CreateFromConnectionString("HostName=demoyodiwohub......", TransportType.Http1);
            rotarywatcher = new RotaryWatcher(GrovePi.Pin.AnalogPin2);
            rotarywatcher.OnNewValueAcquiredCb = OnSensedValue;
            rotarywatcher.Watch();
            buttonwatcher = new ButtonWatcher(GrovePi.Pin.DigitalPin3);
            buttonwatcher.OnNewValueAcquiredCb = OnSensedValue;
            //buttonwatcher.Watch();
            lightwatcher = new LightWatcher(GrovePi.Pin.DigitalPin4);
            lightwatcher.OnNewValueAcquiredCb = OnSensedValue;
            //lightwatcher.Watch();
            led = new Led(GrovePi.Pin.DigitalPin5);
            lcd = new LCD();
            //receive events from the Azure IOT hub
            ReceiveDataFromAzure();
        }
        //------------------------------------------------------------------------------------------------------------------------
        private async void OnSensedValue(AzureIOTPayoad payload)
        {
            payload.name = RaspName;
            await deviceClient.SendEventAsync(new Message(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(payload))));
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TXT.Text = "oooo";
        }
        //------------------------------------------------------------------------------------------------------------------------
        public async void ReceiveDataFromAzure()
        {
            Message receivedMessage;
            string messageData;

            while (true)
            {
                receivedMessage = await deviceClient.ReceiveAsync();

                if (receivedMessage != null)
                {
                    messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                    await deviceClient.CompleteAsync(receivedMessage);
                    var payload = JsonConvert.DeserializeObject<AzureIOTPayoad>(messageData);
                    if (payload.ThingName == "Led")
                        led.SetBrightness(payload.Led);
                    else if (payload.ThingName == "LCD")
                        lcd.Display(payload.LCD, new int[] { 255, 255, 0 });
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
    }
}
