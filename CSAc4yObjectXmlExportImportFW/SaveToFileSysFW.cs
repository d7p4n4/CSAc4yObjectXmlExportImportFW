using CSAc4yObjectDBCap;
using CSAc4yObjectObjectService.Object;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace CSAc4yObjectXmlExportImportFW
{
    public class SaveToFileSysFW
    {
        
        public SqlConnection sqlConnection;
        public string sqlConnectionString;
        public string TemplateName;
        public string outPath;
        public string outPathProcess;
        public string outPathSuccess;
        public string outPathError;

        public SaveToFileSysFW() { }

        public SaveToFileSysFW(string newSqlConnectionString, string newTemp, string newOut, string newProc, string newSucc, string newErr)
        {
            sqlConnectionString = newSqlConnectionString;
            TemplateName = newTemp;
            outPath = newOut;
            outPathProcess = newProc;
            outPathSuccess = newSucc;
            outPathError = newErr;

            sqlConnection = new SqlConnection(sqlConnectionString);
            sqlConnection.Open();
        }

        public SaveToFileSysFW(string newSqlConn, string newTemp, string newOut)
        {
            sqlConnectionString = newSqlConn;
            TemplateName = newTemp;
            outPath = newOut;

            sqlConnection = new SqlConnection(sqlConnectionString);
            sqlConnection.Open();
        }
        
        public SaveToFileSysFW(string newSqlConn, string newOut)
        {
            sqlConnectionString = newSqlConn;
            outPath = newOut;

            sqlConnection = new SqlConnection(sqlConnectionString);
            sqlConnection.Open();
        }

        public void WriteOutAc4yObject()
        {
            StringToPascalCase stringToPascalCase = new StringToPascalCase();

            ListInstanceByNameResponse listInstanceByNameResponse =
                new Ac4yObjectObjectService(sqlConnection).ListInstanceByName(
                    new ListInstanceByNameRequest() { TemplateName = TemplateName }
                );

            foreach (var element in listInstanceByNameResponse.Ac4yObjectList)
            {
                string xml = serialize(element, typeof(Ac4yObject));
                string templateSimpledId = stringToPascalCase.Convert(element.TemplatePublicHumanId).ToUpper();

                writeOut(xml, element.SimpledHumanId + "@" + templateSimpledId + "@Ac4yObject", outPath);
            }

        }

        public List<string> WriteOutAc4yObjectNameList()
        {
            StringToPascalCase stringToPascalCase = new StringToPascalCase();
            List<string> names = new List<string>();

            ListInstanceByNameResponse listInstanceByNameResponse =
                new Ac4yObjectObjectService(sqlConnection).ListInstanceByName(
                    new ListInstanceByNameRequest() { TemplateName = TemplateName }
                );

            foreach (var element in listInstanceByNameResponse.Ac4yObjectList)
            {
                string xml = serialize(element, typeof(Ac4yObject));
                string templateSimpledId = stringToPascalCase.Convert(element.TemplatePublicHumanId).ToUpper();
                string name = element.SimpledHumanId + "@" + templateSimpledId + "@Ac4yObject";
                names.Add(name);

                writeOut(xml, name, outPath);
            }

            return names;
        }

        public void WriteOutAc4yObjectAll()
        {
            StringToPascalCase stringToPascalCase = new StringToPascalCase();

            ListInstanceResponse listInstanceResponse =
                new Ac4yObjectObjectService(sqlConnection).ListInstance(
                    new ListInstanceRequest() { }
                );

            foreach (var element in listInstanceResponse.Ac4yObjectList)
            {
                string xml = serialize(element, typeof(Ac4yObject));
                string templateSimpledId = stringToPascalCase.Convert(element.TemplatePublicHumanId).ToUpper();

                writeOut(xml, element.SimpledHumanId + "@" + templateSimpledId + "@Ac4yObject", outPath);
            }

        }

        public void writeOut(string text, string fileName, string outputPath)
        {
            File.WriteAllText(outputPath + fileName + ".xml", text);

        }

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

            Ac4yObjectObjectService ac4YObjectObjectService = new Ac4yObjectObjectService(sqlConnection);

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
    }
}
