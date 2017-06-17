using Libooru.Externals.Danbooru;
using Libooru.Links;
using Libooru.Workers.Iqdb;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Libooru.Workers
{
    public class TaggerWorker
    {

        public Core core { get; set; }

        public TaggerWorker(Core core)
        {
            this.core = core;
        }

        public PostSearchResult SearchForMd5(string path)
        {
            var client = new WebClient();
            var r = client.DownloadData(CreateMD5Request(path));
            var content = Encoding.UTF8.GetString(r);
            var l = JsonConvert.DeserializeObject<PostSearchResult[]>(content);
            return l.First();
        }

        public PostSearchResult Search(string id)
        {
            var client = new WebClient();
            //var r = client.DownloadString(t + "?login=" + core.config.Data.Externals.Danbooru.Login + "&api_key=" + core.config.Data.Externals.Danbooru.ApiKey);
            var r = client.DownloadString(CreatePostIdRequest(id));
            //var content = Encoding.UTF8.GetString(r);
            var l = JsonConvert.DeserializeObject<PostSearchResult>(r);
            return l;
        }

		public TagSearchResult SearchForTag(string tag)
		{
			var client = new WebClient();
			//var r = client.DownloadString(t + "?login=" + core.config.Data.Externals.Danbooru.Login + "&api_key=" + core.config.Data.Externals.Danbooru.ApiKey);
			var r = client.DownloadString(CreateTagSearchNameRequest(tag));
			//var content = Encoding.UTF8.GetString(r);
			var l = JsonConvert.DeserializeObject<TagSearchResult[]>(r);
			if (l.Length < 1)
				return null;

			return l[0];
		}

		public string CreateMD5Request(string path)
		{
			//var result = "https://danbooru.donmai.us/posts.json?tags=md5:" + GetMd5FromFile(path);
			var result = "https://danbooru.donmai.us/posts/" + path.Replace("https://danbooru.donmai.us/posts/", "");
			if (!string.IsNullOrEmpty(core.config.Data.Externals.Danbooru.ApiKey) && !string.IsNullOrEmpty(core.config.Data.Externals.Danbooru.Login))
			{
				result += "&login=" + core.config.Data.Externals.Danbooru.Login + "&api_key=" + core.config.Data.Externals.Danbooru.ApiKey;
			}

			return result;
		}

		public string CreateTagSearchNameRequest(string tag)
		{
			var result = "https://danbooru.donmai.us/tags.json?search[name]=" + tag;
			if (!string.IsNullOrEmpty(core.config.Data.Externals.Danbooru.ApiKey) && !string.IsNullOrEmpty(core.config.Data.Externals.Danbooru.Login))
				result += "&login=" + core.config.Data.Externals.Danbooru.Login + "&api_key=" + core.config.Data.Externals.Danbooru.ApiKey;

			return result;
		}

		public string CreatePostIdRequest(string id)
        {
            var result = "https://danbooru.donmai.us/posts/" + id + ".json";
            if (!string.IsNullOrEmpty(core.config.Data.Externals.Danbooru.ApiKey) && !string.IsNullOrEmpty(core.config.Data.Externals.Danbooru.Login))
            {
                result += "?login=" + core.config.Data.Externals.Danbooru.Login + "&api_key=" + core.config.Data.Externals.Danbooru.ApiKey;
            }

            return result;
        }

        public void TagPicture(int id)
        {
			core.SetProgress(0);
            var r = QueryDanbooruIQDB(id);
			core.SetProgress(10);
            r.Compute();
			core.SetProgress(20);
			var pr = Search(r.BestMatch.id);
			core.SetProgress(30);
			var tag_string = pr.tag_string.Split(' ');
			float tick = 70 / tag_string.Length;
			float p = 30f;
            foreach (var item in tag_string)
            {
                core.tagsWorker.AddTag(item, id);
				p += tick;
				core.SetProgress((int)p);
			}
        }

        public IqdbQueryResult QueryDanbooruIQDB(int id)
        {
            var p = core.picturesWroker.GetPicture(id);
            var f = new ByteArrayContent(p.Thumbnail);
            using (var client = new HttpClient())
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(f, "file", "picture");
                var response = client.PostAsync("http://danbooru.iqdb.org/", formData).Result;
                if (!response.IsSuccessStatusCode)
                    return null;

                var result = response.Content.ReadAsStringAsync().Result;
                var queryresult = new IqdbQueryResult(result);
                return queryresult;
            }
        }

        public string GetMd5FromFile(string path)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", ""); ;
                }
            }
        }
    }
}
