using System;
using System.Messaging;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Cargoscan.BScaner;
using Cargoscan.DScanner;
using Cargoscan.Properties;
using Cargoscan.Cubiscan;
//using System.Collections.ObjectModel;
using Apache.NMS;
using Apache.NMS.Util;

using log4net;

namespace Cargoscan
{
        

    public partial class MainForm : Form
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        //public delegate void evScanbrcode();

        delegate void OutputText(string text);
        private BarcodeScaner BrcScanner;
        private DimensionScaner DimScanner;
        private WeightMeasure WeightScanner;
        private CubiscanWeightScan DoMeizure;

        ActiveMQ activeMq = new ActiveMQ();
        //ActiveMQ.Publisher publisher = new ActiveMQ.Publisher();
        //listener = new Listener(this);
        //publisher = new Publisher();
            
        
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

            
            BrcScanner = new BarcodeScaner();
            BrcScanner.ScanBarcodeEvnt += new BarcodeScanning(evScanbrcode);
            DimScanner = new DimensionScaner();
            WeightScanner = new WeightMeasure();
            DoMeizure = new CubiscanWeightScan();
            checkBox1.Checked = false;


            // ЛОГИРОВАНИЕ ПОД ApacheMQ
            
          

            //publisher.SendMessage("Hello");*/



            try
            {
               /*MessageQueue Queue = new MessageQueue(StartProgram.queueName);
                if (!Queue.CanWrite)
                {
                    throw new Exception(Resurses.ErrorQueue);
                }*/
                
                BrcScanner.Open(Settings.Default.PortBarcodeScanner, 9600, 8); 

            }
            catch (Exception error)
            {
                log.Error("Ошибка запуска программы..." + error.Message);
                lblError.Text = error.Message;

            }


        }


        private void evScanbrcode(string ebarcode)
        {
            
            bool flOkscan = false;
            ResulthScan rscan = new ResulthScan();
            rscan.Barcode = ebarcode;
                        

            Barcode.Invoke(new OutputText((s)=>Barcode.Text=ebarcode), "newText");
            BrcScanner.SendingData = false;

            if (!checkBox1.Checked) 
                    
            /*{
                    //BrcScanner.SendingData = false;
                    lblError.Invoke(new OutputText((s) => lblError.Text = "укажите измеритель"), "newText");
                    //flOkscan = true;
                    BrcScanner.SendingData = true;
                    return;

                } */

            Weight.Invoke(new OutputText((s) => Weight.Text = ""), "newText");
            DimX.Invoke(new OutputText((s) => DimX.Text = ""), "newText");
            DimY.Invoke(new OutputText((s) => DimY.Text = ""), "newText");
            DimZ.Invoke(new OutputText((s) => DimZ.Text = ""), "newText");
            lblError.Invoke(new OutputText((s) => lblError.Text = ""), "newText");

            if (!checkBox1.Checked)
            {

                try
                {

                    flOkscan = flOkscan | DimScanner.Scan(ref rscan);
                    flOkscan = flOkscan | WeightScanner.Scan(ref rscan);
                    if (flOkscan)
                    {
                        System.Messaging.Message ordermsg = new System.Messaging.Message();
                        ordermsg.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });

                        //test only
                        /*rscan.Height = 12.3;
                        rscan.Length = 10.3;
                        rscan.Weight = 9.3;
                        rscan.Width  = 7.2;*/


                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        string json = serializer.Serialize(rscan);

                        //Uri connecturi = new Uri("activemq:tcp://localhost:61616");

                        //Console.WriteLine("About to connect to " + connecturi);

                        // NOTE: ensure the nmsprovider-activemq.config file exists in the executable folder.
                        IConnectionFactory factory = new NMSConnectionFactory("activemq:tcp://10.0.1.39:61616?wireFormat.tightEncodingEnabled=true");

                     

                       /* ((Apache.NMS.ActiveMQ.ConnectionFactory)((Apache.NMS.NMSConnectionFactory)factory).ConnectionFactory).UserName = StartProgram.userNameMq;
                        ((Apache.NMS.ActiveMQ.ConnectionFactory)((Apache.NMS.NMSConnectionFactory)factory).ConnectionFactory).Password = StartProgram.passwordMq;*/
                        using (IConnection connection = factory.CreateConnection(StartProgram.userNameMq, StartProgram.passwordMq))
                           
                        using (ISession session = connection.CreateSession())
                        {
                            IDestination destination = SessionUtil.GetDestination(session, StartProgram.queueName);
                            Console.WriteLine("Using destination: " + destination);

                            // Create a consumer and producer
                            //using (IMessageConsumer consumer = session.CreateConsumer(destination));
                            using (IMessageProducer producer = session.CreateProducer(destination))
                           
                            {
                                connection.Start();
                                producer.DeliveryMode = MsgDeliveryMode.Persistent;

                                // Send a message
                                ITextMessage request = session.CreateTextMessage(json);
                                request.NMSCorrelationID = "Store";
                                request.Properties["NMSXGroupID"] = "StoreLibra";
                                request.Properties["myHeader"] = "Sending measurements";

                                producer.Send(request);
                            }
                        }
                            //activeMq.IntializeActiveMQ();

                        //ActiveMQ.Publisher publisher;

                        
                       
                        /*ordermsg.Label = "ScanDimention";
                        ordermsg.Priority = MessagePriority.Normal;
                        ordermsg.Body = json;
                        //для начала создадим очередь
                        // Create the transacted MSMQ queue if necessary. 
                        MessageQueue Queue = new MessageQueue(StartProgram.queueName);
                        Queue.Send(ordermsg);*/

                        Weight.Text = rscan.Weight.ToString();
                        DimX.Text   = rscan.Length.ToString();
                        DimY.Text   = rscan.Width.ToString();
                        DimZ.Text   = rscan.Height.ToString();
                    }
                    else
                    {
                        Weight.Invoke(new OutputText((s) => Weight.Text = ""), "newText");
                        DimX.Invoke(new OutputText((s) => DimX.Text = ""), "newText");
                        DimY.Invoke(new OutputText((s) => DimY.Text = ""), "newText");
                        DimZ.Invoke(new OutputText((s) => DimZ.Text = ""), "newText");
                    }

                }
                catch (Exception error)
                {
                    log.Error(error.Message);
                    BrcScanner.SendingData = true;

                    this.lblError.Text = error.Message;
                }

                BrcScanner.SendingData = true;

            }
            else
            {
                try {

                    
                        flOkscan = flOkscan | DoMeizure.Scan(ref rscan);
                    
                        // Откроем повторно COM порт Cubiscan для получения новых порций событий
                        DoMeizure.Open(Settings.Default.PortCubiscan, 9600, 8);

                    if (flOkscan)
                    {
                       System.Messaging.Message ordermsg = new System.Messaging.Message();
                        ordermsg.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });

                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        string json = serializer.Serialize(rscan);

                        ordermsg.Label = "ScanDimentionCubiscan";
                        ordermsg.Priority = MessagePriority.Normal;
                        ordermsg.Body = json;

                        MessageQueue Queue = new MessageQueue(StartProgram.queueName);

                        Queue.Send(ordermsg);

                         Weight.Invoke(new OutputText((s) => Weight.Text = rscan.Weight.ToString()), "newText");

                        DimX.Invoke(new OutputText((s) => DimX.Text = rscan.Length.ToString() + " Д"), "newText");
                        DimY.Invoke(new OutputText((s) => DimY.Text = rscan.Width.ToString()  + " Ш"),  "newText");
                        DimZ.Invoke(new OutputText((s) => DimZ.Text = rscan.Height.ToString() + " В"), "newText");

                    }
                    else
                    {

                        Weight.Text = "";
                        DimX.Text = "";
                        DimY.Text = "";
                        DimZ.Text = "";

                    }
                }
                catch (Exception error)
                {
                    log.Error(error.Message);
                    BrcScanner.SendingData = true;

                    this.lblError.Text = error.Message;

                    Weight.Invoke(new OutputText((s) => Weight.Text = ""), "newText");
                    DimX.Invoke(new OutputText((s) => Weight.Text = ""), "newText");
                    DimY.Invoke(new OutputText((s) => Weight.Text = ""), "newText");
                    DimZ.Invoke(new OutputText((s) => Weight.Text = ""), "newText");

                    BrcScanner.SendingData = true;
                }

                BrcScanner.SendingData = true;
            }
                
        }


        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            //checkBox2.Checked = false;//
            DoMeizure.Open(Settings.Default.PortCubiscan, 9600, 8); //@#Settings.Default.PortCubiscan
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }
    }
}
