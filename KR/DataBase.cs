using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Drawing;

namespace KR
{
    internal class DataBase
    {
        SqlConnection sqlconnection = new SqlConnection(@"Data Source = DESKTOP-AEAMUNC; Initial Catalog = mydb; Integrated Security = True");
   
        public void OpenConnection()
        {
            if(sqlconnection.State == System.Data.ConnectionState.Closed) { 
                sqlconnection.Open();
            }
        }
        public void CloseConnection()
        {
            if (sqlconnection.State == System.Data.ConnectionState.Open)
            {
                sqlconnection.Close();
            }
        }

        public SqlConnection getConnection()
        {
            return sqlconnection;
        }
    }
}
