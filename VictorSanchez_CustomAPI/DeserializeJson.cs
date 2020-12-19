﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace VictorSanchez_CustomAPI
{
    public static class DeserializeJson
    {
        public static DadzJokeResponse DeserializeJSON(string json)
        {
            using (MemoryStream DeSerializememoryStream = new MemoryStream())
            {

                //initialize DataContractJsonSerializer object and pass Student class type to it
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(DadzJokeResponse));

                //user stream writer to write JSON string data to memory stream
                StreamWriter writer = new StreamWriter(DeSerializememoryStream);
                writer.Write(json);
                writer.Flush();

                DeSerializememoryStream.Position = 0;
                //get the Desrialized data in object of type Student
                DadzJokeResponse SerializedObject = (DadzJokeResponse)serializer.ReadObject(DeSerializememoryStream);
                return SerializedObject;
            }
        }
    }
}
