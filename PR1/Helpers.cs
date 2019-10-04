using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;
using YamlDotNet.Serialization;

namespace PR1
{
    public static class Helpers
    {
        public static FetchedDataType GetFetchedDataType(JObject rawObject)
        {
            var mime_type = rawObject["mime_type"];
            var data = rawObject["data"];
            FetchedDataType returnData = null;

            if (mime_type == null)
            {
                return returnData = GetObjectArrayToFetchedDataType(data.ToString());
            }
            else
            if (mime_type.ToString() == "application/xml")
            {
                return returnData = GetXMLToFetchedDataType(data.ToString());
            }
            else
            if (mime_type.ToString() == "text/csv")
            {
                return returnData = GetCSVToFetchedDataType(data.ToString());
            }
            else
            if (mime_type.ToString() == "application/x-yaml")
            {
                return returnData = GetYamlToFetchedDataType(data.ToString());
            }
            return null;
        }

        private static FetchedDataType GetObjectArrayToFetchedDataType(string rawString)
        {
            FetchedDataType returnData = new FetchedDataType();
            var array = (JArray)JsonConvert.DeserializeObject(rawString);
            foreach (var item in array.Children<JObject>())
            {
                var tempItem = new List<string>();
                foreach (var value in item)
                {
                    if (!returnData.columns.Contains(value.Key))
                    {
                        returnData.columns.Add(value.Key);
                    }
                    tempItem.Add(value.Value.ToString());
                }
                returnData.data.Add(tempItem);
            }

            return returnData;
        }

        private static FetchedDataType GetXMLToFetchedDataType(string rawString)
        {
            FetchedDataType returnData = new FetchedDataType();
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(rawString);
            var nodes = xml.SelectSingleNode("dataset");
            foreach (XmlNode record in nodes.ChildNodes)
            {
                var tempItem = new List<string>();
                foreach (XmlNode value in record.ChildNodes)
                {
                    if (!returnData.columns.Contains(value.Name))
                    {
                        returnData.columns.Add(value.Name);
                    }
                    tempItem.Add(value.InnerText);
                }
                returnData.data.Add(tempItem);
            }


            return returnData;
        }

        private static FetchedDataType GetCSVToFetchedDataType(string rawString)
        {
            FetchedDataType returnData = new FetchedDataType();
            var array = new List<string>(rawString.Split('\n'));
            var temp = array[0].Split(',');
            var temp2 = array[0];

            foreach (var column in array[0].Split(','))
            {
                returnData.columns.Add(column);

            }
            array.RemoveAt(0);

            foreach (var record in array)
            {
                var tempItem = new List<string>();
                foreach (var i in record.Split(','))
                {
                    tempItem.Add(i);

                }
                returnData.data.Add(tempItem);
            }

            return returnData;
        }
        private static FetchedDataType GetYamlToFetchedDataType(string rawString)
        {
            FetchedDataType returnData = new FetchedDataType();
            var deserializer = new DeserializerBuilder().Build();
            var result = deserializer.Deserialize<List<object>>(rawString);
            for (int i = 0; i < result.Count; i++)
            {
                var json = JsonConvert.SerializeObject(result[i]);
                var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                returnData.columns = new List<string>(dictionary.Keys);
                returnData.data.Add(new List<string>(dictionary.Values));
            }
            return returnData;
        }

        public static List<string> GetColumn(List<FetchedDataType> list, string columnName) {
            List<string> response = new List<string>();
            list.ForEach(table => {
                if(table.columns.Contains(columnName)){
                    var index = table.columns.IndexOf(columnName);
                    table.data.ForEach(record => {
                        if(record.Count > index){
                        response.Add(record[index]);
                        } 
                    });
                }
            });
            if(response.Count == 0){
                response.Add("No such column");
            }
            return response;
        }
    }
}