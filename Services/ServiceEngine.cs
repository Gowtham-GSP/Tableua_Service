using TABLEAU_SERVICE.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TABLEAU_SERVICE.Services
{
    public class ServiceEngine : IServiceEngine
    {
        #region

        private readonly ILogger<ServiceEngine> _logger;
        private readonly IOptions<Configurations> _options;
        private readonly IDBHelper _dbHelper;

        private System.Timers.Timer pollTimer { get; set; }

        private int Interval
        {
            get
            {
                return (1000 * _options.Value.Appsettings.PollIntervalInSeconds);
            }
        }

        #endregion

        public ServiceEngine(ILogger<ServiceEngine> logger, IOptions<Configurations> options, IDBHelper dbHelper)
        {
            _logger = logger;
            _options = options;
            _dbHelper = dbHelper;

            this.pollTimer = new System.Timers.Timer();
            _logger.LogInformation("Tableau Service pollTimer interval {0} milliseconds", Interval);
            this.pollTimer.Interval = Interval;
            this.pollTimer.AutoReset = true;
            this.pollTimer.Elapsed += new ElapsedEventHandler(PollTimer_Elapsed);
        }


        private void PollTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.pollTimer.Stop();
            try
            {
                _logger.LogInformation("Inside PollTimer_Elapsed method");

                DataTable dataTable = _dbHelper.GetEMPDataFromDB();
                if (dataTable != null)
                {
                    SaveDataToFile(dataTable);
                }
                else
                {
                    _logger.LogError("DataTable returned null.");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred in PollTimer_Elapsed : {ex}");
            }
            _logger.LogInformation("Outside PollTimer_Elapsed method");

            this.pollTimer.Start();
        }
 

        public void SaveDataToFile(DataTable dataTable)
        {
            try
            {
                _logger.LogInformation("Inside the SaveDataToFile method");

                string outputPath = _options.Value.FileOutputPath;

                if (outputPath != null)
                {
                    string folderName = DateTime.Now.ToString("yyyy-MM-dd");
                    string folderPath = Path.Combine(outputPath, folderName);

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                        _logger.LogInformation("Folder created in that particular path");
                    }

                    string fileName = "TABLEAU_DATA  " + DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss") + ".txt";
                    string filePath = Path.Combine(folderPath, fileName);

                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        foreach (DataColumn col in dataTable.Columns)
                        {
                            writer.Write($"{col.ColumnName}{_options.Value.Appsettings.ResultedDataSeparatedBySymbol} ");
                        }
                        writer.WriteLine();


                        foreach (DataRow row in dataTable.Rows)
                        { 
                            foreach (DataColumn col in dataTable.Columns)
                            {
                                writer.Write($"{row[col]}{_options.Value.Appsettings.ResultedDataSeparatedBySymbol} ");
                            }
                            writer.WriteLine();
                        }
                    }

                    _logger.LogInformation($"Data saved to file : {filePath}, {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to save data to file : {ex}");
            }
            _logger.LogInformation("Outside SaveDataToFile method");
        }



        public void Dispose()
        {
            this.pollTimer?.Dispose();
            _logger.LogInformation("Service Disposed");
        }

        public void Start()
        {
            this.pollTimer.Start();
            _logger.LogInformation("PollTimer Started");
        }

        public void Stop()
        {
            this.pollTimer.Stop();
            _logger.LogInformation("PollTimer Stopped");

        }
    }
}
