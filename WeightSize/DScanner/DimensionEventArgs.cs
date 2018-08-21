using System;


namespace Cargoscan.DScanner
{
    public class DimensionEventArgs : EventArgs
    {

        private double length = 0;
        private double width = 0;
        private double height = 0;
        private string msg;

        public DimensionEventArgs(double lgth, double wdth, double hght)
        {
            length = lgth;
            width = wdth;
            height = hght;
            msg = "" + length + " X " + width + " X " + height;

        }


        public override string ToString()
        {
            return msg;
        }


        public double Length
        {
            get { return length; }
        }

        public double Width
        {
            get { return width; }
        }

        public double Height
        {
            get { return height; }
        }

    }

}
