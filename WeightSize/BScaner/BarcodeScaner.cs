using System;
using System.IO.Ports;
using log4net;

namespace Cargoscan.BScaner
{

    public delegate void BarcodeScanning(string  _eventBarcode);

    class BarcodeScaner
    {
       private static readonly ILog log = LogManager.GetLogger(typeof(BarcodeScaner));

        public event BarcodeScanning ScanBarcodeEvnt;

        private int msTimeout = 3500;
        private bool fsendData = true;

        SerialPort _serialport = new SerialPort();
        public BarcodeScaner()
        {
        }

        ~BarcodeScaner()
        {
            _serialport.Close();
        }

        protected virtual void OnBarcodeScan(string _MyEventParameters)
        {
            if (ScanBarcodeEvnt != null)
                ScanBarcodeEvnt(_MyEventParameters);
        }

        private void SP_DataRecieved(Object sender, SerialDataReceivedEventArgs e)
        {

            string data = "";
            if (fsendData) {
               
                try
                {
                    data = _serialport.ReadLine();
                }
                catch (TimeoutException error)
                {
                    log.Error(error.Message);
                }

                OnBarcodeScan(data);

                try
                {
                    _serialport.DiscardInBuffer();
                }
                catch (TimeoutException error)
                {
                    log.Error(error.Message);
                }
                
            }
        }

        private void SP_ErrorRecieved(Object sender, SerialErrorReceivedEventArgs e)
        {
            string data = "";
            if (fsendData)
            {
                try
                {
                    data = _serialport.ReadLine();
                }
                catch (TimeoutException error)
                {
                    log.Error(error.Message);
                }
            string retval = "err";
            OnBarcodeScan(retval);
            }            
        }

  
        public bool SendData(byte[] Data)
        {
            if (_serialport.IsOpen)
            {
                _serialport.Write(Data,0,Data.Length);
                return true;
            }
            else return false;
        }

        public bool Open(string PortName, int BaudRate, int DataBits)
        {
             char[] newLine = { (char)13 };
            _serialport.PortName = PortName;
            _serialport.BaudRate = BaudRate;
            _serialport.DataBits = DataBits;
            _serialport.Parity = Parity.None;
            _serialport.StopBits = StopBits.One;
            _serialport.ReadTimeout = msTimeout;
            _serialport.WriteTimeout = msTimeout;
            _serialport.NewLine = new string(newLine);
            //_serialport.ReadBufferSize = 256;
            //_serialport.WriteBufferSize = 4;
     
            //_serialport.DtrEnable = true;
            //_serialport.RtsEnable = true;
     
            _serialport.DataReceived += new SerialDataReceivedEventHandler(SP_DataRecieved);
            _serialport.ErrorReceived += new SerialErrorReceivedEventHandler(SP_ErrorRecieved);
            _serialport.Open();
     
            if (_serialport.IsOpen) return true; else return false; 
        }

        public int Timeout
        {
            get { return msTimeout; }
            set { msTimeout= value; }

        }

        public bool SendingData
        {
            get { return fsendData; }
            set { fsendData = value; }

        }


    }
}
