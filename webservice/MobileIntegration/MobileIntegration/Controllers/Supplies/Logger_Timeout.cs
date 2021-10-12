using log4net;
using log4net.Config;

namespace MobileIntegration.Controllers.Supplies
{
    public class Logger_Timeout
    {
        private static ILog log = LogManager.GetLogger("LOGGER_TIMEOUT");

        public static ILog Log
        {
            get { return log; }
        }

        public static void InitLogger()
        {
            XmlConfigurator.Configure();
        }
    }
}