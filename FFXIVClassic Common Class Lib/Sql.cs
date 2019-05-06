using NLog;

namespace FFXIVClassic.Common
{
    // todo:
    // havent decided whether it's worth wrapping every sql class
    // so i'll just leave it with logger for now
    public class Sql
    {
        public static Logger Log = LogManager.GetCurrentClassLogger();
    }
}
