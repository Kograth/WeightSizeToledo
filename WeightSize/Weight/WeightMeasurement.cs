using System;
using System.IO.Ports;
using System.Text.RegularExpressions;
using log4net;
using System.Globalization;

namespace Cargoscan.Cubiscan
{

    public delegate void WeightMeasurement(WeightEventArgs _eventParameters);

    public class WeightMeasure
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WeightMeasurement));
        public event WeightMeasurement WeightMeasurementEvent;

        private int msTimeout = 3500;
        private bool fsendData = true;

        SerialPort _serialport = new SerialPort();
        private Regex RegexResulth = new Regex(@"^S.S.*\s(?<weight>\d{1,6}\.\d{0,3})\s(?<unit>\w{1,2})", RegexOptions.Compiled | RegexOptions.Singleline);

        public WeightMeasure()
        {
        }

        ~WeightMeasure()
        {
            _serialport.Close();
        }

        protected virtual void OnWeightMeasurement(WeightEventArgs _MyEventParameters)
        {
            if (WeightMeasurementEvent != null)
                WeightMeasurementEvent(_MyEventParameters);
        }


        private WeightEventArgs ParseResults(string _inpstr)
        {
            NumberFormatInfo  provider  = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";

            string sweight = "0";
            string sunit = "0";
            double lweit = 0;
            WeightEventArgs parseres =null;
            Match match = RegexResulth.Match(_inpstr);
            if (match.Success)
            {
                sweight = match.Groups["weight"].Value;
                sunit = match.Groups["unit"].Value;
            }

            try 
            {
                lweit = Convert.ToDouble(sweight, provider);  
		    }
		    catch (Exception error)
		    {
              log.Error(error);
              log.Debug("Получено с весов: "+_inpstr);
		    }
            parseres = new WeightEventArgs(lweit, sunit);
            return parseres;
        }


        public bool Scan(ref ResulthScan rs)
		{
            byte[] CmdInitWeight = { 0x54, 0x41, 0x43, 0x0D, 0x0A }; //Иннициализация весов
            byte[] CmdStartWeight = { 0x53, 0x0D, 0x0A }; //Запуск взвешивания весов

            bool flOkScan = false;

            string Resulscan ="";
            WeightEventArgs rscan;

		    try
		    {
    			if (!_serialport.IsOpen)
	    		{
                    _serialport.Open();
                    //
		    	}
                // иннициируем взвешивание
                _serialport.Write(CmdInitWeight, 0, CmdInitWeight.Length);

                //Прочитаем данные из портра 
                Resulscan = _serialport.ReadLine();


                _serialport.Write(CmdStartWeight, 0, CmdStartWeight.Length);

                //Если время чтения истекло необходимо получить проблему
                rscan = ParseResults(_serialport.ReadLine());
                Resulscan = rscan.ToString();
                rs.Weight = rscan.Weight;
                flOkScan = true;

                _serialport.DiscardInBuffer();
                _serialport.DiscardOutBuffer();

		    }
		    catch (Exception error)
		    {
    			//this.Close();
                Resulscan =  "error";
                log.Error(error);
		    }

            return flOkScan;
		}
        

        public bool Open(string PortName, int BaudRate, int DataBits)
        {
            if (!_serialport.IsOpen)
            {
                _serialport.PortName = PortName;
                _serialport.BaudRate = BaudRate;
                _serialport.DataBits = DataBits;
                _serialport.Parity = Parity.None;
                _serialport.StopBits = StopBits.One;
                _serialport.ReadTimeout = msTimeout;
                _serialport.WriteTimeout = msTimeout;
                _serialport.Open();
            }

            if (_serialport.IsOpen) return true; else return false;
        }

        public int Timeout
        {
            get { return msTimeout; }
            set { msTimeout = value; }

        }

        public bool SendingData
        {
            get { return fsendData; }
            set { fsendData = value; }

        }

        internal void Open(object portWeighr, int v1, int v2)
        {
            throw new NotImplementedException();
        }
    }
}
