using System;
using System.Windows.Forms;
using System.Configuration;
using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Cargoscan
{
    static class StartProgram
    {
       public static readonly ILog log = LogManager.GetLogger(typeof(StartProgram));
        public static readonly string queueName = ConfigurationManager.AppSettings["queueName"];
        public static readonly string userNameMq = ConfigurationManager.AppSettings["UserNameMQ"];
        public static readonly string passwordMq = ConfigurationManager.AppSettings["PasswordMQ"];

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Program pr = new Program();
            log.Info("Start programm");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

            log.Info("Stop programm");
        }

    }
}
