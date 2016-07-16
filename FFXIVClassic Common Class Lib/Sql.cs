using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using NLog;
using System.Data;
using System.Data.Common;

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
