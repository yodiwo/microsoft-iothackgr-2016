using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrovePi;
using GrovePi.Sensors;
using GrovePi.I2CDevices;

namespace App1
{
    public class LCD
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        IRgbLcdDisplay display;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        public LCD()
        {
            display = DeviceFactory.Build.RgbLcdDisplay();
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public void Display(string text, int[] rgb)
        {
            display.SetBacklightRgb((byte)rgb[0], (byte)rgb[1], (byte)rgb[2]);
            display.SetText(text);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
