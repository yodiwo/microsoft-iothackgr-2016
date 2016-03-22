using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrovePi;
using GrovePi.Sensors;

namespace App1
{
    public class RotaryWatcher
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        IRotaryAngleSensor rotaryAngleSensor;
        public delegate void OnNewValueAcquired(AzureIOTPayoad payload);
        public OnNewValueAcquired OnNewValueAcquiredCb = null;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        public RotaryWatcher(GrovePi.Pin pin)
        {
            rotaryAngleSensor = DeviceFactory.Build.RotaryAngleSensor(pin);
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
                    var sensorValue = rotaryAngleSensor.SensorValue();
                    var payload = new AzureIOTPayoad()
                    {
                        RotaryAngleSensor = sensorValue,
                        ThingName = "RotaryAngleSensor"
                    };
                    if (OnNewValueAcquiredCb != null)
                        OnNewValueAcquiredCb(payload);
                    await Task.Delay(2000);
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
