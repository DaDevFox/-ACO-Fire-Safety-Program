using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography.X509Certificates;

namespace BuildingSafetyACO.Data
{
    public class Database
    {
        public string baseDirectoryPath { get; private set; }
        //public DirectoryInfo baseDirectory { get; private set; }

        public Dictionary<string, FileDB> database = new Dictionary<string, Database.FileDB>();

        public PlanDB PlanRegistry { get; set; }

        public void Initialize()
        {
            PlanRegistry.ReadAll();
        }

        public Database()
        {
            baseDirectoryPath = "D:/School/ISRDE/FireSafety/Data";
            PlanRegistry = new PlanDB(baseDirectoryPath);
        }

        public abstract class FileDB
        {
            public abstract string id { get; }
            public abstract string directory { get; }

            public virtual Dictionary<string, object> data { get; } = new Dictionary<string, object>();

            public void Load()
            {

            }

            /// <summary>
            /// Read all files from directory
            /// </summary>
            public virtual void ReadAll()
            {

            }

            /// <summary>
            /// Dispose all objects of type T in <c>data</c> dictionary
            /// </summary>
            public virtual void Dispose()
            {

            }
        }

        public abstract class FileDB<T> : FileDB where T : class
        {
            /// <summary>
            /// EXPENSIVE: Generates a boxed data dictionary (per-call) based on unboxed <c>Data</c> dictionary
            /// </summary>
            public sealed override Dictionary<string, object> data
            {
                get
                {
                    if (Data.Count == 0)
                        return null;
                    Dictionary<string, object> result = new Dictionary<string, object>();

                    foreach (KeyValuePair<string, T> pair in Data)
                        result.Add(pair.Key, pair.Value);
                    return result;
                }
            }

            public virtual Dictionary<string, T> Data { get; } = new Dictionary<string, T>();

            public sealed override void ReadAll()
            {
                string[] fileNames = Directory.GetFiles(directory);
                string[] fileContents = new string[fileNames.Length];
                for (int i = 0; i < fileNames.Length; i++)
                {
                    fileContents[i] = File.ReadAllText(fileNames[i]);
                    if (JObject.Parse(fileContents[i]) != null)
                        Data.Add(fileNames[i], Read(JObject.Parse(fileContents[i])));
                }
            }

            /// <summary>
            /// Creates a <c>T</c> object from a given JSON object <c>source</c>
            /// </summary>
            /// <param name="source"></param>
            /// <returns></returns>
            public virtual T Read(JObject source)
            {
                Type instanceType = typeof(T);
                T instance = Activator.CreateInstance(instanceType) as T;

                if (instance == null)
                    throw new ArgumentException("Type parameter T cannot be instantiated");

                foreach(FieldInfo field in instanceType.GetFields())
                    if(!field.IsStatic && !field.IsPrivate)
                        if (source.GetValue(field.Name) != null)
                            field.SetValue(instance, source.GetValue(field.Name).Value());

                return instance;
            }
        }

        public class PlanDB : FileDB<Plan>
        {

            public override string id => "plans";

            public override string directory => Directory;
            public string Directory;

            public PlanDB(string directory) => Directory = directory;


            //public override Plan Read(JObject source)
            //{
            //    Console.WriteLine(source.ToString());
            //    return null;
            //}
        }

        public class OrganizationDB : FileDB<Organization>
        {
            public override Dictionary<string, Organization> Data => throw new NotImplementedException();

            public override string id => throw new NotImplementedException();

            public override string directory => throw new NotImplementedException();

            public override Organization Read(JObject source)
            {
                throw new NotImplementedException();
            }
        }
    }
}
