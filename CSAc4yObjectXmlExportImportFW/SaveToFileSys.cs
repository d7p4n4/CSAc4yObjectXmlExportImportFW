using CSAc4yObjectDBCap;
using CSAc4yObjectObjectService.Object;
using CSAc4yUtilityContainer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace CSAc4yObjectXmlExportImport
{

    public class SaveToFileSys
    {

        private SqlConnection _sqlConnection { get; set; }
        public string sqlConnectionString;
        public string TemplateName;
        public string outPath;
        public string outPathProcess;
        public string outPathSuccess;
        public string outPathError;

        public SaveToFileSys() { }

        public SaveToFileSys(string newSqlConnectionString, string newTemp, string newOut, string newProc, string newSucc, string newErr)
        {
            sqlConnectionString = newSqlConnectionString;
            TemplateName = newTemp;
            outPath = newOut;
            outPathProcess = newProc;
            outPathSuccess = newSucc;
            outPathError = newErr;

            _sqlConnection = new SqlConnection(sqlConnectionString);
            _sqlConnection.Open();
        }

        public SaveToFileSys(string newSqlConn, string newTemp, string newOut)
        {
            sqlConnectionString = newSqlConn;
            TemplateName = newTemp;
            outPath = newOut;

            _sqlConnection = new SqlConnection(sqlConnectionString);
            _sqlConnection.Open();
        }
        
        public SaveToFileSys(string newSqlConn, string newOut)
        {
            sqlConnectionString = newSqlConn;
            outPath = newOut;

            _sqlConnection = new SqlConnection(sqlConnectionString);
            _sqlConnection.Open();
        }

        public SaveToFileSys(SqlConnection sqlConnection)
        {
            _sqlConnection = sqlConnection;
        }

        public void WriteOutAc4yObject()
        {
            StringToPascalCase stringToPascalCase = new StringToPascalCase();

            ListInstanceByNameResponse listInstanceByNameResponse =
                new Ac4yObjectObjectService(_sqlConnection).ListInstanceByName(
                    new ListInstanceByNameRequest() { TemplateName = TemplateName }
                );

            foreach (var element in listInstanceByNameResponse.Ac4yObjectList)
            {
                string xml = serialize(element, typeof(Ac4yObject));
                string templateSimpledId = stringToPascalCase.Convert(element.TemplatePublicHumanId).ToUpper();

                WriteOut(xml, element.SimpledHumanId + "@" + templateSimpledId + "@Ac4yObject", outPath);
            }

        }

        public List<string> WriteOutAc4yObjectNameList()
        {
            StringToPascalCase stringToPascalCase = new StringToPascalCase();
            List<string> names = new List<string>();

            ListInstanceByNameResponse listInstanceByNameResponse =
                new Ac4yObjectObjectService(_sqlConnection).ListInstanceByName(
                    new ListInstanceByNameRequest() { TemplateName = TemplateName }
                );

            foreach (var element in listInstanceByNameResponse.Ac4yObjectList)
            {
                string xml = serialize(element, typeof(Ac4yObject));
                string templateSimpledId = stringToPascalCase.Convert(element.TemplatePublicHumanId).ToUpper();
                string name = element.SimpledHumanId + "@" + templateSimpledId + "@Ac4yObject";
                names.Add(name);

                WriteOut(xml, name, outPath);
            }

            return names;
        }

        public void ExportAllInstances(string outputPath)
        {

            if (String.IsNullOrEmpty(outputPath))
                throw new Exception("OUTPUTPATH nem lehet üres!");

            ListInstanceResponse listInstanceResponse =
                new Ac4yObjectObjectService(_sqlConnection).ListInstance(
                    new ListInstanceRequest() { }
                );

            if (listInstanceResponse.Result.Fail())
                throw new Exception(listInstanceResponse.Result.Message);

            foreach (var element in listInstanceResponse.Ac4yObjectList)
            {
                string xml = serialize(element, typeof(Ac4yObject));
                //string templateSimpledId = new StringToPascalCase().Convert(element.TemplatePublicHumanId).ToUpper();

                WriteOut(xml, element.SimpledHumanId + "@" + element.TemplateSimpledHumanId + "@Ac4yObject", outputPath);
            }

        } // ExportAllInstances

        public void ExportInstanceOfTemplate(string template)
        {
            
            ListInstanceResponse listInstanceResponse =
                new Ac4yObjectObjectService(_sqlConnection).ListInstance(
                    new ListInstanceRequest() { }
                );

            foreach (var element in listInstanceResponse.Ac4yObjectList)
            {
                string xml = serialize(element, typeof(Ac4yObject));
                string templateSimpledId = new StringToPascalCase().Convert(element.TemplatePublicHumanId).ToUpper();

                WriteOut(xml, element.SimpledHumanId + "@" + templateSimpledId + "@Ac4yObject", outPath);
            }

        } // ExportInstanceOfTemplate

        public void WriteOut(string text, string fileName, string outputPath)
        {

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            File.WriteAllText(outputPath + fileName + ".xml", text);

        } // WriteOut

        public string serialize(Object taroltEljaras, Type anyType)
        {
            XmlSerializer serializer = new XmlSerializer(anyType);
            var xml = "";

            using (var writer = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(writer))
                {
                    serializer.Serialize(writer, taroltEljaras);
                    xml = writer.ToString(); // Your XML
                }
            }
            //System.IO.File.WriteAllText(path, xml);

            return xml;
        }

        public void Load()
        {
            if (!Directory.Exists(outPathProcess))
                Directory.CreateDirectory(outPathProcess);

            if (!Directory.Exists(outPathSuccess))
                Directory.CreateDirectory(outPathSuccess);

            if (!Directory.Exists(outPathError))
                Directory.CreateDirectory(outPathError);

            Ac4yObjectObjectService ac4YObjectObjectService = new Ac4yObjectObjectService(_sqlConnection);

            try
            {
                string[] files =
                    Directory.GetFiles(outPath, "*.xml", SearchOption.TopDirectoryOnly);

                Console.WriteLine(files.Length);

                foreach (var _file in files)
                {
                    string _filename = Path.GetFileNameWithoutExtension(_file);
                    Console.WriteLine(_filename);
                    System.IO.File.Move(outPath + _filename + ".xml", outPathProcess + _filename + ".xml");


                    string xml = readIn(_filename, outPathProcess);

                    Ac4yObject tabla = (Ac4yObject)Deserialize(xml, typeof(Ac4yObject));

                    SetByGuidAndNamesResponse response = ac4YObjectObjectService.SetByGuidAndNames(
                        new SetByGuidAndNamesRequest() { TemplateName = tabla.TemplateHumanId, Name = tabla.HumanId, Guid = tabla.GUID }
                        );

                    if (response.Result.Success())
                    {
                        System.IO.File.Move(outPathProcess + _filename + ".xml", outPathSuccess + _filename + ".xml");

                    }
                    else
                    {
                        System.IO.File.Move(outPathProcess + _filename + ".xml", outPathError + _filename + ".xml");

                    }
                }
            }
            catch (Exception _exception)
            {
                Console.WriteLine(_exception.Message);
            }
        }

        public Object Deserialize(string xml, Type anyType)
        {
            Object result = null;

            XmlSerializer serializer = new XmlSerializer(anyType);
            using (TextReader reader = new StringReader(xml))
            {
                result = serializer.Deserialize(reader);
            }

            return result;
        }

        public string readIn(string fileName, string path)
        {

            string textFile = path + fileName + ".xml";

            string text = File.ReadAllText(textFile);

            return text;


        }

    } // SaveToFileSys

} // CSAc4yObjectXmlExportImport