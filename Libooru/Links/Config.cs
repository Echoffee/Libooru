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

        public List<string> PicFiles { get; set; }

        public Config(Core core)
        {
            this.core = core;
        }

        public void GetConfig()
        {

            var path = AppFolderPath;
            var filePath = path + @"/config.json";
            if (!File.Exists(filePath))
            {
                this.Data = new ConfigDataSet();
                using (var w = new StreamWriter(filePath))
                    w.Write(JsonConvert.SerializeObject(this.Data, Formatting.Indented));
                core.FirstLaunch();
            }else{
            var f = File.OpenRead(filePath);
            string s;
            using (StreamReader r = new StreamReader(f))
            {
                s = r.ReadToEnd();
            }
            this.Data = JsonConvert.DeserializeObject<ConfigDataSet>(s);
            }
        }

        public List<string> GetPictures(int index = 0, int count = 20)
        {
            var path = AppFolderPath;
            var filePath = path + @"/list.lb";
            this.PicFiles = new List<string>();
            var f = File.OpenRead(filePath);
            string s;
            var counter = -index;
            using (StreamReader r = new StreamReader(f))
            {
                
                s = r.ReadLine();
                while (s != string.Empty && counter < count)
                {
                    if (counter >= 0)
                        PicFiles.Add(s);
                    s = r.ReadLine();
                    counter++;
                }
            }
            return PicFiles;
        }

        public void SavePictureList(IList<string> list)
        {
            var path = AppFolderPath;
            var filePath = path + @"/list.lb";

            string s;
            using (var w = new StreamWriter(filePath, false))
            {
                foreach (var item in list)
                {
                    w.WriteLine(item);
                }
            }
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
            core.SoftUpdate();
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
