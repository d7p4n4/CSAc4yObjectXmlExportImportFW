using Ac4yDBCap;
using Ac4yObjectService.Object;
using Ac4yObjectService.Txt;
using CSAc4yClass.Class;
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
        /*
        public string sqlConnectionString;
        public string TemplateName;
        public string outPath;
        public string outPathProcess;
        public string outPathSuccess;
        public string outPathError;
*/
/*
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
*/
        public SaveToFileSys(SqlConnection sqlConnection)
        {
            _sqlConnection = sqlConnection;
        }
        /*
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

                WriteOut(xml, element.SimpledHumanId + "@" + templateSimpledId + "@Ac4yObject", outPath);
            }

        }
        */
        /*
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
        */

        public void ExportAc4yObjectList(List<Ac4yObject> ac4yObjectList, string outputPath)
        {

            foreach (var element in ac4yObjectList)
            {

                WriteOut(
                    new Ac4yUtility().GetAsXml(element)
                    , element.SimpledHumanId + "@" + element.TemplateSimpledHumanId + "@Ac4yObject"
                    , outputPath
                );

            }

        } // ExportAc4yObjectList

        public void ExportAllInstances(string outputPath)
        {

            if (String.IsNullOrEmpty(outputPath))
                throw new Exception("OUTPUTPATH nem lehet üres!");

            ListInstanceResponse response =
                new Ac4yObjectObjectService(_sqlConnection).ListInstance(
                    new ListInstanceRequest() { }
                );

            if (response.Result.Fail())
                throw new Exception(response.Result.Message);

            ExportAc4yObjectList(response.Ac4yObjectList, outputPath);

        } // ExportAllInstances

        public void ExportInstanceOfTemplate(string template, string outputPath)
        {

            if (String.IsNullOrEmpty(outputPath))
                throw new Exception("outputPath nem lehet üres!");

            if (String.IsNullOrEmpty(template))
                throw new Exception("template nem lehet üres!");

            ListInstanceByNameResponse response =
                new Ac4yObjectObjectService(_sqlConnection).ListInstanceByName(
                    new ListInstanceByNameRequest() { TemplateName=template }
                );

            ExportAc4yObjectList(response.Ac4yObjectList, outputPath);

        } // ExportInstanceOfTemplate

        public void WriteOut(string text, string fileName, string outputPath)
        {

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            File.WriteAllText(outputPath + fileName + ".xml", text);

        } // WriteOut
        /*
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
        */
        
        public void Load(
                        string outPathProcess
                        ,string outPathSuccess
                        ,string outPathError
                        ,string outputPath
                )
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
                    Directory.GetFiles(outputPath, "*.xml", SearchOption.TopDirectoryOnly);

                Console.WriteLine(files.Length);

                foreach (var _file in files)
                {
                    string _filename = Path.GetFileNameWithoutExtension(_file);
                    Console.WriteLine(_filename);
                    System.IO.File.Move(outputPath + _filename + ".xml", outPathProcess + _filename + ".xml");


                    string xml = ReadIn(_filename, outPathProcess);

                    //Ac4yObject tabla = (Ac4yObject)Deserialize(xml, typeof(Ac4yObject));
                    Ac4yObject tabla = (Ac4yObject)new Ac4yUtility().Xml2Object(xml, typeof(Ac4yObject));

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

        } // Load

        public void LoadAc4yClass(
                        string inputPath
                        , string processingPath
                        , string successPath
                        , string errorPath
                )
        {
            if (!Directory.Exists(inputPath+processingPath))
                Directory.CreateDirectory(inputPath+processingPath);

            if (!Directory.Exists(inputPath+ successPath))
                Directory.CreateDirectory(inputPath+ successPath);

            if (!Directory.Exists(inputPath+ errorPath))
                Directory.CreateDirectory(inputPath+ errorPath);

            Ac4yObjectObjectService ac4YObjectObjectService = new Ac4yObjectObjectService(_sqlConnection);

            try
            {
                string[] files =
                    Directory.GetFiles(inputPath, "*.xml", SearchOption.TopDirectoryOnly);

                Console.WriteLine(files.Length);

                foreach (var file in files)
                {
                    string filename = Path.GetFileNameWithoutExtension(file);
                    //Console.WriteLine(filename);
                    System.IO.File.Move(inputPath + filename + ".xml", inputPath+ processingPath + filename + ".xml");


                    string xml = ReadIn(filename, inputPath + processingPath);

                    //Ac4yObject tabla = (Ac4yObject)Deserialize(xml, typeof(Ac4yObject));
                    //Ac4yObject tabla = (Ac4yObject)new Ac4yUtility().Xml2Object(xml, typeof(Ac4yObject));
                    Ac4yClass ac4yClass = (Ac4yClass)new Ac4yUtility().Xml2Object(xml, typeof(Ac4yClass));

                    SetByNamesResponse setByNamesResponse = new Ac4yObjectObjectService(_sqlConnection).SetByNames(
                            new SetByNamesRequest() {
                                TemplateName = "Ac4yClass"
                                , Name = ac4yClass.Name
                            }
                        );

                    if (setByNamesResponse.Result.Success())
                    {
                        System.IO.File.Move(inputPath + processingPath + filename + ".xml", inputPath + successPath + filename + ".xml");

                    }
                    else
                    {
                        System.IO.File.Move(inputPath + processingPath + filename + ".xml", inputPath + errorPath + filename + ".xml");

                    }

                    if (!setByNamesResponse.Result.Success())
                        throw new Exception("Az Ac4yClass objektum kiírása sikertelen!");

                    SetByGuidResponse setByGuidResponse  =
                        new Ac4yTxtObjectService(_sqlConnection).SetByGuid(
                                new SetByGuidRequest()
                                {
                                    Guid = setByNamesResponse.Ac4yObject.GUID
                                    ,
                                    Txt = xml
                                }
                            );

                    if (!setByGuidResponse.Result.Success())
                        throw new Exception("Az Ac4yClass xml kiírása sikertelen!");

                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

        } // LoadAc4yClass

        /*
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
        */
        public string ReadIn(string fileName, string path)
        {

            string textFile = path + fileName + ".xml";

            string text = File.ReadAllText(textFile);

            return text;


        }

    } // SaveToFileSys

} // CSAc4yObjectXmlExportImport