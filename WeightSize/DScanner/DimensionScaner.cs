using System;
using System.IO.Ports;
using System.Text.RegularExpressions;
using log4net;


namespace Cargoscan.DScanner
{

    public delegate void DimensionScanning(DimensionEventArgs _eventParameters);

    public class DimensionScaner
    {
       private static readonly ILog log = LogManager.GetLogger(typeof(DimensionScaner));

        public event DimensionScanning ScanDimensionEvent;

        private Regex RegexResulth = new Regex(@"^.{6}(?<leght>\d{5})(?<width>\d{5})(?<height>\d{5}).{2}", RegexOptions.Compiled | RegexOptions.Singleline);
        private int msTimeout = 10000;
        private bool fsendData = true;

        SerialPort _serialport = new SerialPort();

        public DimensionScaner()
        {
        }

        ~DimensionScaner()
        {
            _serialport.Close();
        }

        protected virtual void OnDimensionScan(DimensionEventArgs _MyEventParameters)
        {
            if (ScanDimensionEvent != null)
                ScanDimensionEvent(_MyEventParameters);
        }



        private DimensionEventArgs ParseResults(string _inpstr)
        {

            string len ="0";
            string wid="0";
            string hei="0";
            DimensionEventArgs parseres;
                Match match = RegexResulth.Match(_inpstr);
                if (match.Success)
                {
                    len = match.Groups["leght"].Value;
                    wid = match.Groups["width"].Value;
                    hei = match.Groups["height"].Value;
                }
                else
                {
                    throw new Exception(Resurses.Error1);
                }


            parseres = new DimensionEventArgs(Convert.ToDouble(len)/10,
                                                Convert.ToDouble(wid)/10,
                                                Convert.ToDouble(hei)/10);


            return parseres;
        }

        public bool ScanCubiscan()
        {
            return false;
        }

        public bool Scan(ref ResulthScan rs)
		{
            
            byte[] CmdStartScan= { 2, 68, 3}; //Сканер объема

            bool flOkScan = false;
            string Resulscan="";
            DimensionEventArgs outerdimensions;

            //char[] ReadBuff = new char[256];
		    try
		    {
    			if (!_serialport.IsOpen)
	    		{
                    _serialport.Open();
		    	}
                // иннициируем сканирование
                _serialport.Write(CmdStartScan, 0, CmdStartScan.Length);



                Resulscan =_serialport.ReadLine();


                log.Debug(Resulscan);

                    outerdimensions = ParseResults(_serialport.ReadLine());
                    Resulscan = outerdimensions.ToString();
                    rs.Height = outerdimensions.Height;
                    rs.Width = outerdimensions.Width;
                    rs.Length = outerdimensions.Length;
                    flOkScan = true;

                // Очистим данные порта, дабы не мешались
                    _serialport.DiscardInBuffer();
                    _serialport.DiscardOutBuffer();
		    }
		    catch (Exception error)
		    {
    			//this.Close();
                Resulscan = "error ";

                log.Error(error.Message + " " + Resulscan);
		    }
            return flOkScan;
		}
        

        public bool Open(string PortName, int BaudRate, int DataBits)
        {
            if (!_serialport.IsOpen)
            {
                char[] newLine = { (char)13 };
                _serialport.PortName = PortName;
                _serialport.BaudRate = BaudRate;
                _serialport.DataBits = DataBits;
                _serialport.Parity = Parity.None;
                _serialport.Handshake = Handshake.None;
                _serialport.StopBits = StopBits.One;
                _serialport.ReadTimeout = msTimeout;
                _serialport.WriteTimeout = msTimeout;
                _serialport.ReadBufferSize = 256;
                _serialport.NewLine = new string(newLine);
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

        internal void Open(object portDimensionScaner, int v1, int v2)
        {
            throw new NotImplementedException();
        }
    }
}
