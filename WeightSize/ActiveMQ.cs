
using Apache.NMS;
using System.Threading;
using System;



namespace Cargoscan
{
    class ActiveMQ
    {
        public const string DESTINATION = "queue://Meashure";
        public const string URI = "activemq:tcp://localhost:61616";

        public void Initialize()
        {


            try
            {
                IConnectionFactory connectionFactory = new NMSConnectionFactory(URI);
                IConnection _connection = connectionFactory.CreateConnection();
                _connection.Start();
                ISession _session = _connection.CreateSession();
                IDestination dest = _session.GetDestination(DESTINATION);
                using (IMessageConsumer consumer = _session.CreateConsumer(dest))
                {
                    Console.WriteLine("Listener started.");
                    Console.WriteLine("Listener created.rn");
                    IMessage message;
                    while (true)
                    {
                        message = consumer.Receive();
                        if (message != null)
                        {
                            ITextMessage textMessage = message as ITextMessage;
                            if (!string.IsNullOrEmpty(textMessage.Text))
                            {
                                Console.WriteLine(textMessage.Text);
                                //Chat pMesg = JsonConvert.DeserializeObject<Chat>(textMessage.Text);
                                //mainWindow.UpdateCollection(pMesg);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Press <ENTER> to exit.");
                Console.Read();
            }
        }

        public void IntializeActiveMQ()
        {
            Thread t1 = new Thread(new ThreadStart(Initialize));
            t1.Start();
            //listener.Initialize();
        }

        public class BaseClass
        {
            public const string URI = "activemq:tcp://localhost:61616";
            public IConnectionFactory connectionFactory;
            public IConnection _connection;
            public ISession _session;

            public BaseClass()
            {
                connectionFactory = new NMSConnectionFactory(URI);
                if (_connection == null)
                {
                    _connection = connectionFactory.CreateConnection();
                    _connection.Start();
                    _session = _connection.CreateSession();
                }
            }
        }

        public class Publisher : BaseClass
        {
            public const string DESTINATION = "queue://Meashure";
            public Publisher()
            {
            }

            public string SendMessage(string message)
            {
                string result = string.Empty;
                try
                {
                    IDestination destination = _session.GetDestination(DESTINATION);
                    using (IMessageProducer producer = _session.CreateProducer(destination))
                    {
                        var textMessage = producer.CreateTextMessage(message);
                        producer.Send(textMessage);
                    }
                    result = "Message sent successfully.";
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                    Console.WriteLine(ex.ToString());
                }
                return result;
            }

        }


    }

    
}
