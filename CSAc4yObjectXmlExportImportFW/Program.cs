
using CSAc4yObjectXmlExportImport;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace CSAc4yObjectXmlExportImportFW
{

    class Program
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string APPSETTINGS_CONNECTIONPARAMETER = "CONNECTIONPARAMETER";
        private const string APPSETTINGS_TEMPLATE = "TEMPLATE";
        private const string APPSETTINGS_OUTPUTPATH = "OUTPUTPATH";

        public static string connectionString = ConfigurationManager.AppSettings["connectionString"];
        //public static SqlConnection sqlConn = new SqlConnection(ConfigurationManager.AppSettings["conneectionString"]);
        //public static SqlConnection sqlConnXML = new SqlConnection(ConfigurationManager.AppSettings["connectionStringXML"]);
        public static string TemplateName = ConfigurationManager.AppSettings["TemplateName"];
        public static string outPath = ConfigurationManager.AppSettings["outputPath"];
        public static string defaultPath = ConfigurationManager.AppSettings["DefaultPath"];
        public static string outPathProcess = defaultPath + ConfigurationManager.AppSettings["PathProcess"];
        public static string outPathSuccess = defaultPath + ConfigurationManager.AppSettings["PathSuccess"];
        public static string outPathError = defaultPath + ConfigurationManager.AppSettings["PathError"];

        public SqlConnection SqlConnection { get; set; }
        public Configuration Config { get; set; }

        public Program(Configuration config)
        {

            Config = config;

            try
            {

                SqlConnection =
                    new SqlConnection(
                        config.ConnectionStrings.ConnectionStrings[APPSETTINGS_CONNECTIONPARAMETER].ConnectionString
                    );

                SqlConnection.Open();

                if (!SqlConnection.State.Equals(ConnectionState.Open))
                    throw new Exception("Nem kapcsolódik az adatbázishoz!");

            }
            catch (Exception exception)
            {

                log.Error(exception.Message);
                log.Error(exception.StackTrace);

            }

        } // Program

        static void Main(string[] args)
        {
            try
            {

                Program program = new Program(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None));

                new SaveToFileSys(program.SqlConnection).ExportAllInstances(
                        program.Config.AppSettings.Settings[APPSETTINGS_OUTPUTPATH].Value
                    );
                /*
                new SaveToFileSys(program.SqlConnection).ExportInstanceOfTemplate(
                        program.Config.AppSettings.Settings[APPSETTINGS_TEMPLATE].Value
                    );
                */
                //GetXmls getXmls = new GetXmls();
///                SaveToFileSys saveToFileSys = new SaveToFileSys(connectionString, TemplateName, outPath, outPathProcess, outPathSuccess, outPathError);

///                saveToFileSys.Load();
///                saveToFileSys.WriteOutAc4yObject();
                
                //saveToFileSysFW.WriteOutAc4yObjectHome();
                //sqlConnXML.Open();
                /*
                List<SerializationObject> xmls = getXmls.GetXmlsMethod(sqlConn, sqlConnXML, TemplateName);
                foreach(var xml in xmls)
                {
                    saveToFileSysFW.writeOut(xml.xml, xml.fileName, outPath);
                }
                */
            }
            catch(Exception exception)
            {
                log.Error(exception.Message);
                log.Error(exception.StackTrace);
            }

        }

    }
}
