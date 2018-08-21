using System;
using Cargoscan.BScaner;
using Cargoscan.DScanner;
using Cargoscan.Properties;
using Cargoscan.Cubiscan;
using log4net;
using System.Messaging;
//using System.Configuration;
using System.Web.Script.Serialization;

namespace Cargoscan

{
    public class Program
    {
       private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        private BarcodeScaner BrcScanner;
        private DimensionScaner DimScanner;
        private WeightMeasure WeightScanner;


        public Program()
        {
            BrcScanner = new BarcodeScaner();
            BrcScanner.ScanBarcodeEvnt += new BarcodeScanning(evScanbrcode);
            DimScanner = new DimensionScaner();
            WeightScanner = new WeightMeasure();

            try
            {
               MessageQueue Queue = new MessageQueue(StartProgram.queueName);
                if (!Queue.CanWrite)
                {
                    throw new Exception(Resurses.ErrorQueue);
                }
                BrcScanner.Open(Settings.Default.PortBarcodeScanner, 9600, 8);
                DimScanner.Open(Settings.Default.PortDimensionScaner, 9600, 8);
                WeightScanner.Open(Settings.Default.PortWeighr, 9600, 8);

            }
            catch (Exception error)
            {
               log.Error("Ошибка запуска программы..." + error.Message);

            }
        }

        private void evScanbrcode(string ebarcode)
        {

            bool flOkscan = true;
            ResulthScan rscan = new ResulthScan();
            rscan.Barcode = ebarcode;

            BrcScanner.SendingData = false;
            try
            {

                flOkscan = flOkscan & DimScanner.Scan(ref rscan);
                flOkscan = flOkscan & WeightScanner.Scan(ref rscan);
                if (flOkscan) 
                {
                    Message ordermsg = new Message();
                    ordermsg.Formatter = new XmlMessageFormatter(new Type[] { typeof(string)});

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    string json = serializer.Serialize(rscan);


                    ordermsg.Label = "ScanDimention";
                    ordermsg.Priority = MessagePriority.Normal;
                    ordermsg.Body = json;
                    //для начала создадим очередь
                    // Create the transacted MSMQ queue if necessary. 
                    MessageQueue Queue = new MessageQueue(StartProgram.queueName);

                    Queue.Send(ordermsg);
                }
                    else
                        {

                        }
                
             }
            catch (Exception error)
            {
                log.Error(error.Message);
                BrcScanner.SendingData = true;
            }

            BrcScanner.SendingData = true;
        }
    }
}
