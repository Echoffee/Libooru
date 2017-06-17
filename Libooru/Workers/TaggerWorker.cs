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

		private WebClient client;

		private HttpClient httpClient;

        public TaggerWorker(Core core)
        {
            this.core = core;
			client = new WebClient();
			httpClient = new HttpClient();
        }

		/// <summary>
		/// DANBOORU ONLY - Get Danbooru data from a given picture by MD5.
		/// </summary>
		/// <param name="path">Path of the picture</param>
		/// <returns></returns>
		[System.Obsolete]
        public PostSearchResult SearchForMd5(string path)
        {
            var r = client.DownloadData(CreateMD5Request(path));
            var content = Encoding.UTF8.GetString(r);
            var l = JsonConvert.DeserializeObject<PostSearchResult[]>(content);
            return l.First();
        }

		/// <summary>
		/// DANBOORU ONLY - Get Danbooru data from a given id post.
		/// </summary>
		/// <param name="id">Danbooru post Id.</param>
		/// <returns></returns>
        public PostSearchResult Search(string id)
        {
            //var r = client.DownloadString(t + "?login=" + core.config.Data.Externals.Danbooru.Login + "&api_key=" + core.config.Data.Externals.Danbooru.ApiKey);
            var r = client.DownloadString(CreatePostIdRequest(id));
            //var content = Encoding.UTF8.GetString(r);
            var l = JsonConvert.DeserializeObject<PostSearchResult>(r);
            return l;
        }

		/// <summary>
		/// DANBOORU ONLY - Get Danbooru data from a given tag name.
		/// </summary>
		/// <param name="tag">Tag name to look for.</param>
		/// <returns></returns>
		public TagSearchResult SearchForTag(string tag)
		{
			//var r = client.DownloadString(t + "?login=" + core.config.Data.Externals.Danbooru.Login + "&api_key=" + core.config.Data.Externals.Danbooru.ApiKey);
			var r = client.DownloadString(CreateTagSearchNameRequest(tag));
			//var content = Encoding.UTF8.GetString(r);
			var l = JsonConvert.DeserializeObject<TagSearchResult[]>(r);
			if (l.Length < 1)
				return null;

			return l[0];
		}

		/// <summary>
		/// DANBOORU ONLY - Get Danbooru data from a given MD5 hash.
		/// </summary>
		/// <param name="md5">MD5</param>
		/// <returns></returns>
		[System.Obsolete]
		public string CreateMD5Request(string md5)
		{
			//var result = "https://danbooru.donmai.us/posts.json?tags=md5:" + GetMd5FromFile(path);
			var result = "https://danbooru.donmai.us/posts/" + md5.Replace("https://danbooru.donmai.us/posts/", "");
			if (!string.IsNullOrEmpty(core.config.Data.Externals.Danbooru.ApiKey) && !string.IsNullOrEmpty(core.config.Data.Externals.Danbooru.Login))
			{
				result += "&login=" + core.config.Data.Externals.Danbooru.Login + "&api_key=" + core.config.Data.Externals.Danbooru.ApiKey;
			}

			return result;
		}

		/// <summary>
		/// DANBOORU ONLY - Create a GET request for tag informations from tag names.
		/// </summary>
		/// <param name="tag">Tags to look for.</param>
		/// <returns></returns>
		public string CreateTagSearchNameRequest(string tag)
		{
			var result = "https://danbooru.donmai.us/tags.json?search[name]=" + tag;
			if (!string.IsNullOrEmpty(core.config.Data.Externals.Danbooru.ApiKey) && !string.IsNullOrEmpty(core.config.Data.Externals.Danbooru.Login))
				result += "&login=" + core.config.Data.Externals.Danbooru.Login + "&api_key=" + core.config.Data.Externals.Danbooru.ApiKey;

			return result;
		}

		/// <summary>
		/// DANBOORU ONLY - Create a GET request for post informations from post id.
		/// </summary>
		/// <param name="id">Id of the post to look for.</param>
		/// <returns></returns>
		public string CreatePostIdRequest(string id)
        {
            var result = "https://danbooru.donmai.us/posts/" + id + ".json";
            if (!string.IsNullOrEmpty(core.config.Data.Externals.Danbooru.ApiKey) && !string.IsNullOrEmpty(core.config.Data.Externals.Danbooru.Login))
            {
                result += "?login=" + core.config.Data.Externals.Danbooru.Login + "&api_key=" + core.config.Data.Externals.Danbooru.ApiKey;
            }

            return result;
        }

		/// <summary>
		/// DANBOORU ONLY - (All-in-one) Tag a picture using IQDB's reverse search and Danbooru's post id.
		/// </summary>
		/// <param name="id">Id of the Picture object to tag.</param>
		/// <returns>True if tagging was successful, False otherwise.</returns>
        public bool TagPicture(int id)
        {
			core.SetProgress(0);
            var r = QueryDanbooruIQDB(id);
			core.SetProgress(10);
            if (!r.Compute())
				return false;

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

			return true;
        }

		/// <summary>
		/// DANBOORU ONLY - Get Danbooru posts from a given picture using IQDB's reverse search. 
		/// </summary>
		/// <param name="id">Id of the Picture object to look for.</param>
		/// <returns>IqdbQueryResult object.</returns>
        public IqdbQueryResult QueryDanbooruIQDB(int id)
        {
            var p = core.picturesWroker.GetPicture(id);
            var f = new ByteArrayContent(p.Thumbnail);
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(f, "file", "picture");
                var response = httpClient.PostAsync("http://danbooru.iqdb.org/", formData).Result;
                if (!response.IsSuccessStatusCode)
                    return null;

                var result = response.Content.ReadAsStringAsync().Result;
                var queryresult = new IqdbQueryResult(result);
                return queryresult;
            }
        }

		/// <summary>
		/// Compute MD5 hash from a given file.
		/// </summary>
		/// <param name="path">Path of the file to compute.</param>
		/// <returns>Hash in hexadecimal form.</returns>
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
