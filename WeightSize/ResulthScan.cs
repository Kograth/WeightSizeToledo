using System;
using System.Runtime.Serialization;

namespace Cargoscan
{
    [Serializable]
   // [DataContract(Name = "ResulthScan" ,Namespace="http:\\Cse.Cargoscan")]
    public class ResulthScan
    {
        private string l_barcode;
        private double l_weight;
        private double l_length;
        private double l_width;
        private double l_height;

     //   [DataMember]
        public string Barcode
        {
            get { return l_barcode; }
            set { l_barcode = value; }
        }

       // [DataMember]
        public double Weight
        {
            get { return l_weight; }
            set { l_weight = value; }
        }

        //[DataMember]
        public double Length
        {
            get { return l_length; }
            set { l_length = value; }
        }

        //[DataMember]
        public double Width
        {
            get { return l_width; }
            set { l_width = value; }
        }

        //[DataMember]
        public double Height
        {
            get { return l_height; }
            set { l_height = value; }
        }
         public override  string ToString()
         {
             return "Barcode:" + l_barcode + " Weight: " + l_weight+" Dimensions: " + l_length + " X " + l_width + " X " + l_height;
         }

    }
}

