using Libooru.Externals.Danbooru;
using Libooru.Links;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
            var r = client.DownloadData(CreateRequest(path));
            var content = Encoding.UTF8.GetString(r);
            var l = JsonConvert.DeserializeObject<PostSearchResult[]>(content);
            return l.First();
        }

        public string CreateRequest(string path)
        {
            var result = "https://danbooru.donmai.us/posts.json?tags=md5:" + GetMd5FromFile(path);
            if (!string.IsNullOrEmpty(core.config.Data.Externals.Danbooru.ApiKey) && !string.IsNullOrEmpty(core.config.Data.Externals.Danbooru.Login))
            {
                result += "&login=" + core.config.Data.Externals.Danbooru.Login + "&api_key=" + core.config.Data.Externals.Danbooru.ApiKey;
            }

            return result;
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
