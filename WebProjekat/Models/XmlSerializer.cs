using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace WebProjekat.Models
{
    public class XmlSerializer<T>
    {
        static string path = AppDomain.CurrentDomain.BaseDirectory + "App_Data/";

        public void Serialize(List<T> lista, string listName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
            TextWriter textWriter = new StreamWriter(path + listName);
            serializer.Serialize(textWriter, lista);
            textWriter.Close();
        }

        public List<T> Deserialize(List<T> lista, string listName)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(List<T>));
            try
            {
                using (TextReader reader = new StreamReader(path + listName))
                {
                    object obj = deserializer.Deserialize(reader);
                    return (List<T>)obj;
                }
            }
            catch
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
                TextWriter textWriter = new StreamWriter(path + listName);
                serializer.Serialize(textWriter, lista);
                textWriter.Close();

                using (TextReader reader = new StreamReader(path + listName))
                {
                    object obj = deserializer.Deserialize(reader);
                    return (List<T>)obj;
                }

            }
        }
    }
}