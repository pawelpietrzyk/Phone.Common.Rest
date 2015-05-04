using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Common.Rest
{
    public class Serializer
    {
        public static void Serialize(object obj, Stream outputStream)
        {
            DataContractJsonSerializer json = new DataContractJsonSerializer(obj.GetType());
            json.WriteObject(outputStream, obj);
        }
        public static string SerializeToString(object obj)
        {
            string content = String.Empty;
            if (obj != null)
            {
                MemoryStream stream = new MemoryStream();
                Serializer.Serialize(obj, stream);
                stream.Position = 0;
                StreamReader reader = new StreamReader(stream);
                content = reader.ReadToEnd();
                reader.Close();
            }
            return content;
        }
        public static object Deserialize(Stream inputStream, Type type)
        {
            DataContractJsonSerializer json = new DataContractJsonSerializer(type);
            return json.ReadObject(inputStream);
        }
        public static object Deserialize(String content, Type type)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            MemoryStream memory = new MemoryStream(bytes);
            return Serializer.Deserialize(memory, type);
        }
    }
}