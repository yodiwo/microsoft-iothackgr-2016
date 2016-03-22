using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrovePi;
using GrovePi.Sensors;

namespace App1
{
    public class ButtonWatcher
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        IButtonSensor buttonSensor;
        public delegate void OnNewValueAcquired(AzureIOTPayoad payload);
        public OnNewValueAcquired OnNewValueAcquiredCb = null;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        public ButtonWatcher(GrovePi.Pin pin)
        {
            buttonSensor = DeviceFactory.Build.ButtonSensor(pin);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public async void Watch()
        {
            //start polling
            while (true)
            {
                try
                {
                    var sensorValue = buttonSensor.CurrentState;
                    var payload = new AzureIOTPayoad()
                    {
                        Button = (sensorValue == SensorStatus.Off) ? 0 : 1,
                        ThingName = "Button"
                    };
                    if (OnNewValueAcquiredCb != null)
                        OnNewValueAcquiredCb(payload);

                    //sleep for 500ms
                    await Task.Delay(500);
                }
                catch (Exception ex)
                {
                }

            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
