using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace Api.FunctionApp.DataContext

{
    public class DapperContext
    {
        public IDbConnection CreateConnection()
        {
            var connectionString = Environment.GetEnvironmentVariable("MySqlConnectionString");
            //return new SqlConnection("Data Source=mysql://mangado:$Agp860720@mydemoserver-mangado1.mysql.database.azure.com/c_test_db");
            //return new SqlConnection("Data Source=localhost;Uid=root;Pwd=root;Initial Catalog=epsa;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            return new MySqlConnection(connectionString);
        }
    }
}