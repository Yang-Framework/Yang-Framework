using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Yang_Framework.Defines.Data.Database;

namespace Yang_Framework.Data.Database.MySQL
{
    public class MySQL
    {
        string _host, _username, _password, _database, _connectionstring;
        MySqlConnection _connection;
        int _port;

        public MySQL(string Host, int Port, string Username, string Password, string Database)
        {
            _host             = Host;
            _port             = Port;
            _username         = Username;
            _password         = Password;
            _database         = Database;
            _connectionstring = "Server=" + _host + ";User Id=" + _username + ";Port=" + _port + ";Password=" + _password + ";Database=" + _database + ";Allow Zero Datetime=True;Min Pool Size = 25;Max Pool Size=150";
        }

        public bool connect()
        {
            _connection = new MySqlConnection(_connectionstring);
            try
            {
                _connection.Open();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool disconnect()
        {
            if (Convert.ToString(_connection.State) == "Open")
            {
                _connection.Close();
                return true;
            } 
            else
            {
                return false;
            }
        }

        public void fetch_assoc(SQLResult result)
        {
            
        }
    }
}
