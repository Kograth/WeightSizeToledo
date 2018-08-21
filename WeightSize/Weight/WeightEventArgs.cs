using System;


namespace Cargoscan.Cubiscan
{
    public class WeightEventArgs : EventArgs
    {

        private double _weight = 0;
        private string _unit;


        private string msg;

        public WeightEventArgs(double wght, string unit)
        {

            _weight = wght;
            _unit = unit;
            msg = "" + _weight;

        }


        public override string ToString()
        {
            return msg;
        }


        public double Weight
        {
            get { return _weight; }
        }
        public string Unit
        {
            get { return _unit; }
        }
    }

}
