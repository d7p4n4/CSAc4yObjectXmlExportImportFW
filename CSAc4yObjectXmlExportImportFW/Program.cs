
using System;
using System.Collections.Generic;
using System.Configuration;
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
        private static readonly log4net.ILog _naplo = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        public static string connectionString = ConfigurationManager.AppSettings["connectionString"];
        //public static SqlConnection sqlConn = new SqlConnection(ConfigurationManager.AppSettings["conneectionString"]);
        //public static SqlConnection sqlConnXML = new SqlConnection(ConfigurationManager.AppSettings["connectionStringXML"]);
        public static string TemplateName = ConfigurationManager.AppSettings["TemplateName"];
        public static string outPath = ConfigurationManager.AppSettings["outputPath"];
        public static string defaultPath = ConfigurationManager.AppSettings["DefaultPath"];
        public static string outPathProcess = defaultPath + ConfigurationManager.AppSettings["PathProcess"];
        public static string outPathSuccess = defaultPath + ConfigurationManager.AppSettings["PathSuccess"];
        public static string outPathError = defaultPath + ConfigurationManager.AppSettings["PathError"];

        static void Main(string[] args)
        {
            try
            {
                //GetXmls getXmls = new GetXmls();
                SaveToFileSysFW saveToFileSysFW = new SaveToFileSysFW(connectionString, TemplateName, outPath, outPathProcess, outPathSuccess, outPathError);

                saveToFileSysFW.Load();
                saveToFileSysFW.WriteOutAc4yObject();
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
                _naplo.Error(exception.Message);
            }

        }

    }
}
