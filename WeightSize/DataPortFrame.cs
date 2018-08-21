using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cargoscan
{
   static class DataPortFrame
    {
        public static string ValueFrame { get; set; }

        public static string GetValueFrame()
        {
            if (ValueFrame != null)
            {
                return string.Format(ValueFrame);
            }
            else
            {
                return string.Format("");
                    }
        }


    }
}
