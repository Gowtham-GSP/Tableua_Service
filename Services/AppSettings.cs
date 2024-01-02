using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TABLEAU_SERVICE.Services
{
    public class AppSetting
    {
        public int PollIntervalInSeconds { get; set; }
        public FileProcess[] FileProcess { get; set; }
        public int CreatingFileInSpecifiedMinutes { get; set; }
        public string ResultedDataSeparatedBySymbol { get; set; }
        public string SP_GET_TABLEAUDATA { get; set; }
    }

    public class Configurations
    {
        public const string SectionName = "Configuration";
        public AppSetting Appsettings { get; set; }
        public ConnectionString connectionString { get; set; }
        public string FileOutputPath { get; set; }
    }
    public class ConnectionString
    {
        public string DBConnectionString { get; set; }
    }

    public class FileProcess
    {
        public string FileColumnNames { get; set; }
     
    }












}
