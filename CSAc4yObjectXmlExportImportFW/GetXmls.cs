using CSAc4yObjectObjectService.Object;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CSAc4yObjectXmlExportImportFW
{
    class GetXmls
    {
        public List<SerializationObject> GetXmlsMethod(SqlConnection sqlConnection, SqlConnection sqlConnectionXML, string templateName)
        {
            sqlConnection.Open();
            StringToPascalCase stringToPascalCase = new StringToPascalCase();

            List<SerializationObject> xmls = new List<SerializationObject>();
            List<Xmls> guids = new List<Xmls>();

            ListInstanceByNameResponse listInstanceByNameResponse =
                new Ac4yObjectObjectService(sqlConnection).ListInstanceByName(
                    new ListInstanceByNameRequest() { TemplateName = templateName }
                );

            string sql = "select GUID, serialization from Ac4yXMLObjects;";
            
            using (SqlConnection connection = sqlConnectionXML)
            {
                SqlCommand command = new SqlCommand(
                    sql, connection);
                    
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    Console.WriteLine(reader.FieldCount);
                    while (reader.Read())
                    {
                        Xmls xmlsObj = new Xmls() { serialization = reader.GetValue(1).ToString(), guid = reader.GetValue(0).ToString() };
                        guids.Add(xmlsObj);
                    }
                }
            }
            foreach (var tabla in listInstanceByNameResponse.Ac4yObjectList)
            {
                foreach (var xml in guids)
                {
                    if (xml.guid.Equals(tabla.GUID))
                    {
                        string templateSimpledId = stringToPascalCase.Convert(templateName).ToUpper();
                        SerializationObject serializationObject = new SerializationObject() {
                            fileName = tabla.SimpledHumanId + "@" + templateSimpledId,
                            xml = xml.serialization
                        };
                        xmls.Add(serializationObject);
                        break;
                    }
                }
            }

            return xmls;

        }
    }
}
