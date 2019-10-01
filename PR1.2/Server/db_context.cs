using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace ConsoleApp1
{
    class DbContext
    {
        public JObject data {set; get;}

        public DbContext() {
            using (StreamReader r = new StreamReader("../../../db_context.json"))
            {
                string json = r.ReadToEnd();
                data = JObject.Parse(json);
            }
        }

        public List<string> GetColumn(string columnName) {
            JObject table = (JObject)data["tables"][0];
            List<string> columns = table["columns"].ToObject<List<string>>();
            int index = columns.IndexOf(columnName);
            List<string> response = new List<string>();

            if(index < 0)
            {
                return response;
            }

            JToken children = table["data"];
            
            foreach(var child in children.Children())
            {
                response.Add(child.ToObject<List<string>>()[index]);
            }
            return response;
        }

    }

}