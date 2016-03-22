using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrovePi;
using GrovePi.Sensors;

namespace App1
{
    public class LightWatcher
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        ILightSensor lightSensor;
        public delegate void OnNewValueAcquired(AzureIOTPayoad payload);
        public OnNewValueAcquired OnNewValueAcquiredCb = null;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        public LightWatcher(GrovePi.Pin pin)
        {
            lightSensor = DeviceFactory.Build.LightSensor(pin);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public async void Watch()
        {
            while (true)
            {
                try
                {
                    var sensorValue = lightSensor.SensorValue();
                    var payload = new AzureIOTPayoad()
                    {
                        Light = sensorValue,
                        ThingName = "Light"
                    };
                    if (OnNewValueAcquiredCb != null)
                        OnNewValueAcquiredCb(payload);
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
