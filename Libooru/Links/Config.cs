using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.IO;
using Libooru.Links.ConfigData;

namespace Libooru.Links
{
    public class Config
    {
        public Core core { get; set; }

        public string AppFolderPath { get; set; }

        public ConfigDataSet Data { get; set; }

        public Config(Core core)
        {
            this.core = core;
        }

        public void GetConfig()
        {

            var path = AppFolderPath;
            var filePath = path + @"/config.json";

            var f = File.OpenRead(filePath);
            string s;
            using (StreamReader r = new StreamReader(f))
            {
                s = r.ReadToEnd();
            }
            this.Data = JsonConvert.DeserializeObject<ConfigDataSet>(s);
        }

        public void ApplyChanges()
        {
            var path = AppFolderPath;
            var filePath = path + @"/config.json";

            var f = File.Open(filePath, FileMode.Create);
            var ns = JsonConvert.SerializeObject(Data, Formatting.Indented);
            using (StreamWriter w = new StreamWriter(f))
            {
                w.Write(ns);
            }

            GetConfig();
            core.Update();
        }

        public void SetLibooruEnv()
        {
            var path = AppFolderPath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var filePath = path + @"/config.json";
            if (!File.Exists(filePath))
            {
                var nf = File.Create(filePath);
                var ns = JsonConvert.SerializeObject(new ConfigDataSet(), Formatting.Indented);
                using (StreamWriter w = new StreamWriter(nf))
                {
                    w.Write(ns);
                }
            }

            var tagFolderPath = path + @"/tags";
            if (!Directory.Exists(tagFolderPath))
            {
                Directory.CreateDirectory(tagFolderPath);
            }

            var thumbnailsFolderPath = path + @"/thumbnails";
            if (!Directory.Exists(thumbnailsFolderPath))
            {
                Directory.CreateDirectory(thumbnailsFolderPath);
            }
        }
    }

    
}
