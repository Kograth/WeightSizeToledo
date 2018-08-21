using System;
using System.IO.Ports;
using System.Text;
//using System.Text.RegularExpressions;
using log4net;
using System.Globalization;

namespace Cargoscan.Cubiscan
{
    class CubiscanWeightScan
    {
        //public delegate void DimensionScanning(DimensionEventArgs _eventParameters);
        public static readonly ILog log = LogManager.GetLogger(typeof(StartProgram));
        //public static readonly string queueName = ConfigurationManager.AppSettings["queueName"];
        private int msTimeout = 10000;

        public  string indata;

        SerialPort _serialport = new SerialPort();
        

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

            _serialport.DataReceived += _serialport_DataReceived;

            if (_serialport.IsOpen) return true; else return false;
        }

        private void _serialport_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                
                SerialPort sp = (SerialPort)sender;

                //if (sp.PortName = "COM4")
                //{
                //string test1;
                string test; //"MAH      ,L 2.0,W 4.6,H 9.4cm,K0.770,D 3.268kg,F6000,D";//sp.ReadExisting();
                //test1 = "";                                //if (test.Length>10)
                                                           //{
                //indata = indata + test;
                //while (sp.BytesToRead != 0)
                //{
                    indata = sp.ReadLine();
                    //indata = indata + test;
                //}

               //// if (sp.BytesToRead==0)
               // { 
                    DataPortFrame.ValueFrame = indata;
                    indata = "";
                   // sp.DiscardInBuffer();
                    //sp.DiscardOutBuffer();
                    //sp.Close();
                //}
                //else
                //{

                //}
            }

            catch {
                return; 
                    }//
        }

        public bool Scan(ref ResulthScan rs)
        {

            

            //byte[] CmdStartScan = Encoding.ASCII.GetBytes("<STX>M<ETX>");

            byte[] CmdStartScan = { 0x02, 0x4D, 0x03, 0x0D, 0x0A };
            
                 //Образец команды <STX>    M   <ETX> <CR>, <LF>

            bool flOkScan = false;
            string Resulscan = "";


            try
            {

                // иннициируем сканирование

               string FrameData = DataPortFrame.GetValueFrame();

               // if (FrameData != null)
                    if (FrameData.Length > 50)
                    {

                        Resulscan = FrameData;
                        FrameData = "";
                        DataPortFrame.ValueFrame = "";
                    }
                    else
                    {
                        if (!_serialport.IsOpen)
                        {
                            _serialport.Open();
                        }
                        _serialport.Write(CmdStartScan, 0, CmdStartScan.Length);
                    //Кубискан вернет такую строку
                    //"MAH      ,L 52.0,W 24.6,H 29.4cm,K20.770,D 6.268kg,F6000,D";
                    // "\n\u0002MAC      ,L 24.0,W 26.6,H-----cm,K 0.780,D 0.000kg,F6000,D\u0003"; //

                   string SoundData = DataPortFrame.GetValueFrame();

                    //Resulscan = SoundData;//_serialport.ReadLine();

                    if (SoundData.Length>50)
                    {
                        Resulscan = SoundData;
                    }
                    else
                    {
                        flOkScan = false;
                        return flOkScan;
                    }  
                    }
                               
                
                string[] FinishResult = Resulscan.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                log.Debug(Resulscan);

                //Хз какой массив будет получен!
                string H = FinishResult[3].Replace("H", "").Replace("cm", "");
                string L = FinishResult[1].Replace("L", "");
                string W = FinishResult[2].Replace("W", "");
                string K = FinishResult[4].Replace("K", "");

                NumberFormatInfo provider = new NumberFormatInfo();
                provider.NumberDecimalSeparator = ".";


                try
                {
                    rs.Height = Convert.ToDouble(H, provider); //outerdimensions.Height;
                }
                catch
                {
                    rs.Height = 0;
                }
                try
                {
                    rs.Width = Convert.ToDouble(W, provider);//outerdimensions.Width;
                }
                catch
                {
                    rs.Width = 0;
                }
                try
                {
                    rs.Length = Convert.ToDouble(L, provider);//outerdimensions.Length;
                }
                catch
                {
                    rs.Length = 0;
                }

                try
                {
                    rs.Weight = Convert.ToDouble(K, provider);
                }
                catch
                {
                    rs.Weight = 0;
                }

                flOkScan = true;
                
                              // Очистим данные порта, дабы не мешались
                _serialport.DiscardInBuffer();
                _serialport.DiscardOutBuffer();
                _serialport.Close();
               
                
            }
            catch (Exception error)
            {
                //this.Close();
                Resulscan = "error ";

                log.Error(error.Message + " " + Resulscan);
            }
            return flOkScan;
        }
    }
}
