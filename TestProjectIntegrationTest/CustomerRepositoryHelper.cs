using BusinessLayer;
using DataLayer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProjectIntegrationTest
{
    public class CustomerRepositoryHelper
    {
        private string connectionString;

        public CustomerRepositoryHelper(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public int StatusBike(int bikeId)
        {
            try
            {
                string sql = "SELECT status FROM bike WHERE id=@id";
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = sql;
                    command.Parameters.AddWithValue("@id", bikeId);
                    int status=(int)command.ExecuteScalar();
                    return status;
                }
            }
            catch (Exception ex) { throw new CustomerRepositoryException("DeleteCustomer", ex); }
        }
    }
}
