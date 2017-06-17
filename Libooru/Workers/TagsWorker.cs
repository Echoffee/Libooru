using Libooru.Links;
using Libooru.Workers.TagsItems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libooru.Links.ConfigData;
using LiteDB;
using Libooru.Models;
using System.Linq.Expressions;
using Libooru.Externals.Danbooru;

namespace Libooru.Workers
{
    public class TagsWorker
    {
        public Core core { get; set; }

        public string TagsFolderPath { get; set; }

        public long tagNumber { get; set; }

        public LiteCollection<PictureTag> tagCollection { get; internal set; }

        private string[] tagFileExtensions = { ".tag" };

        public TagsWorker(Core core)
        {
            this.core = core;
        }

        public void ScanForTags()
        {
            core.SetStatus("Retrieving tags...");
            tagNumber = GetTagFilesNum(TagsFolderPath);
            core.SetStatus("");
        }

        public long GetTagFilesNum(string path)
        {
            var result = 0L;
            var dInfo = new DirectoryInfo(path);
            foreach (var d in dInfo.GetDirectories())
            {
                result += GetTagFilesNum(d.FullName);
            }
            foreach (var f in dInfo.GetFiles())
            {
                if (tagFileExtensions.Contains(f.Extension))
                    result++;
            }
            return result;
        }

        internal void AddTag(string item, int id_pic)
        {
            var tagResults = tagCollection.Find(x => x.Name.Equals(item)).ToList();
            PictureTag t;
            if (tagResults.Count < 1)
            {
                t = new PictureTag();
                t.Name = item;
                t.PictureIDs = new List<int>();
                tagCollection.Insert(t);
				var r = core.taggerWorker.SearchForTag(item);
				if (r != null)
					t.Type = TagSearchResult.GetCategory(r.category);
            }
            else
                t = tagResults.First();

            if (!t.PictureIDs.Contains(id_pic))
                t.PictureIDs.Add(id_pic);

            tagCollection.Update(t);
        }

        public IList<PictureTag> GetTagsForPicture(int id)
        {
            var result = tagCollection.FindAll();
            var result2 = new List<PictureTag>();
            foreach (var item in result)
            {
                if (item.PictureIDs.Contains(id))
                    result2.Add(item);
            }

            return result2;
        }



        /*public SearchResults SearchTags(string tags)
        {
            var result = new SearchResults(tags);
            var query = tags.Split(' ');
            var l = new List<string>();
            foreach (var tag in query)
            {
                var s = SearchTag(tag);
                l.AddRange(s.results);
            }
            result.results = l.Distinct().ToList();
            return result;
        }

        public SearchResults SearchTag(string tag)
        {
            var result = new SearchResults(tag);
            var dInfo = new DirectoryInfo(TagsFolderPath);
            var fList = new List<string>();
            foreach (var folder in dInfo.GetDirectories())
            {
                var s = folder.FullName + @"/" + tag + ".tag";
                if (File.Exists(s))
                    fList.Add(s);
            }

            var l = new List<string>();
            foreach (var file in fList)
            {
                string content;
                using (var r = new StreamReader(File.OpenRead(file)))
                {
                    content = r.ReadToEnd();
                }
                var tf = JsonConvert.DeserializeObject<TagFile>(content);
                l.AddRange(tf.Files);
            }
            result.results = l.Distinct().ToList();
            return result;
        }*/
    }
}
