using TABLEAU_SERVICE.Interface;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TABLEAU_SERVICE.Services
{
  public class DBHelper : IDBHelper
    {
        private readonly IOptions<Configurations> _configuration;
        private readonly ILogger<DBHelper> _logger;

        public DBHelper(IOptions<Configurations> configuration, ILogger<DBHelper> logger)
        {
            _logger = logger;
            _configuration = configuration;
        }


        public DataTable GetEMPDataFromDB()
        {
            _logger.LogInformation("Inside the GetEMPDataFromDB method");

            DataTable dataTable = new DataTable();
            try 
            {
                string connectionString = _configuration.Value.connectionString.DBConnectionString;

                using(SqlConnection sqlConnection = new SqlConnection( connectionString ))
                {
                    sqlConnection.Open();

                    using (SqlCommand sqlCommand = new SqlCommand((_configuration.Value.Appsettings.SP_GET_TABLEAUDATA), sqlConnection))
                    {
                        sqlCommand.CommandTimeout = 0;
                        sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@LastUpdated", DateTime.Now.AddMinutes(-(_configuration.Value.Appsettings.CreatingFileInSpecifiedMinutes)));
                        var result = sqlCommand.ExecuteReaderAsync().Result;
                        dataTable.Load( result );
                    }
                    if(dataTable.Rows.Count > 0)
                    {
                        _logger.LogInformation("Tableau data got from table");
                    }
                    else
                    {
                        _logger.LogInformation("no more data's from Tableau table");
                    }
                }
            }
            catch (Exception ex)
            { 
              _logger.LogInformation(ex,"Error retrieving data from Database");
            }

            _logger.LogInformation("outside the GetEMPDataFromDB method");
            return dataTable;
        }

    }


}
