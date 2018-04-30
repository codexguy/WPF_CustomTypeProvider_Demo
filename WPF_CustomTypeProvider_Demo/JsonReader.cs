/***********************************************************************
Copyright 2018 CodeX Enterprises LLC

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

Major Changes:
04/2018    1.0     Initial release (Joel Champagne)
***********************************************************************/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CodexMicroORM.Core;
using CodexMicroORM.Core.Services;
using Newtonsoft.Json.Linq;

/// <summary>
/// The differences in implementation come from the fact CodexMicroORM already has native JSON support - we need to write this explicitly for our custom example.
/// We omit most validation checks for brevity.
/// </summary>

namespace WPF_CustomTypeProvider_Demo.NoInherit
{
    public class JsonReader
    {
        public static EntitySet<T> LoadData<T>() where T : class, new()
        {
            string json = File.ReadAllText(@"sampledata.json");

            EntitySet<T> populated = new EntitySet<T>();
            populated.PopulateFromSerializationText(json, new JsonSerializationSettings() { SerializationType = SerializationType.ObjectWithSchemaType1AndRows, DataRootName = "People" });
            return populated;
        }
    }
}

namespace WPF_CustomTypeProvider_Demo.Inherit
{
    public class JsonReader
    {
        public static ObservableCollection<T> LoadData<T>() where T : TypeProviderBase, new()
        {
            string json = File.ReadAllText(@"sampledata.json");

            ObservableCollection<T> populated = new ObservableCollection<T>();
            Dictionary<string, Type> schema = new Dictionary<string, Type>();

            var root = JObject.Parse(json);

            // Read schema
            foreach (var propInfo in root.GetValue("Schema").ToArray())
            {
                if (propInfo is JObject jo)
                {
                    if (jo.Count == 2 && jo.First is JProperty fp && jo.Last is JProperty lp)
                    {
                        var t = Type.GetType($"System.{lp.Value.ToString()}");

                        // Assume that any property might be omitted/missing which means everything should be considered nullable
                        if (t.IsValueType && !(t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        {
                            t = typeof(Nullable<>).MakeGenericType(t);
                        }

                        schema[fp.Value.ToString()] = t;
                    }
                }
            }

            // Read data
            foreach (var itemInfo in root.GetValue("People").ToArray())
            {
                var item = new T();

                List<(string name, string value)> values = new List<(string name, string value)>();

                foreach (var prop in itemInfo.Values())
                {
                    var propName = ((JProperty)prop.Parent).Name;
                    values.Add((propName, prop.Value<object>().ToString()));
                }

                // We start from the schema: omission of data should not imply omission of the property itself!
                foreach (var propInfo in schema)
                {
                    item.SetPropertyValue(propInfo.Key, propInfo.Value, (from a in values where a.name == propInfo.Key select a.value).FirstOrDefault());
                }

                populated.Add(item);
            }

            return populated;
        }
    }
}
