using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using NLog;

namespace FFXIVClassic.Common
{
    /*
    class SqlCommand
    {
        public static Logger Log = LogManager.GetCurrentClassLogger();

        public SqlCommand()
        {

        }

        public SqlCommand(string cmdText)
        {
            try
            {
                MySqlCommand.MySqlCommand("");
            }
        }
        public SqlCommand(string cmdText, MySqlConnection connection);
        public SqlCommand(string cmdText, MySqlConnection connection, MySqlTransaction transaction);

        ~SqlCommand()
        {

        }

        public int CacheAge { get; set; }
        
        public string CommandText { get; set; }
        public int CommandTimeout { get; set; }
        
        public CommandType CommandType { get; set; }

        public MySqlConnection Connection { get; set; }
        
        public bool DesignTimeVisible { get; set; }
        public bool EnableCaching { get; set; }
        
        public bool IsPrepared { get; }
        
        public long LastInsertedId { get; }

        public MySqlParameterCollection Parameters { get; }
        
        public MySqlTransaction Transaction { get; set; }
        public UpdateRowSource UpdatedRowSource { get; set; }
        protected DbConnection DbConnection { get; set; }
        protected DbParameterCollection DbParameterCollection { get; }
        protected DbTransaction DbTransaction { get; set; }

        public IAsyncResult BeginExecuteNonQuery();
        public IAsyncResult BeginExecuteNonQuery(AsyncCallback callback, object stateObject);
        public IAsyncResult BeginExecuteReader();
        public IAsyncResult BeginExecuteReader(CommandBehavior behavior);
        public void Cancel();
        public SqlCommand Clone();
        public MySqlParameter CreateParameter();
        public void Dispose();
        public int EndExecuteNonQuery(IAsyncResult asyncResult);
        public MySqlDataReader EndExecuteReader(IAsyncResult result);
        public int ExecuteNonQuery();
        public MySqlDataReader ExecuteReader();
        public MySqlDataReader ExecuteReader(CommandBehavior behavior);
        public object ExecuteScalar();
        public void Prepare();
        protected DbParameter CreateDbParameter();
        protected void Dispose(bool disposing);
        protected DbDataReader ExecuteDbDataReader(CommandBehavior behavior);
    }
    */
}
