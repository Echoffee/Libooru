using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;

namespace Libooru.Workers.Iqdb
{
    public class IqdbQueryResult
    {
        public string Content { get; set; }

        public IqdbMatch BestMatch { get; set; }

        public IqdbQueryResult(string result)
        {
            this.Content = result;
        }

        public void Compute()
        {
            if (string.IsNullOrEmpty(Content))
                return;

            var document = new HtmlDocument();
            document.LoadHtml(Content);

            BestMatch = new IqdbMatch();
            var urlNode = document.DocumentNode.SelectSingleNode("/html/body/div/div[@id='pages']/div[2]/table/tr[2]/td/a");
            var id = urlNode.Attributes.Single(x => x.Name == "href").Value.Split('/').Last();
            var sizeNode = document.DocumentNode.SelectSingleNode("/html/body/div/div[@id='pages']/div[2]/table/tr[3]/td");
            var size = sizeNode.InnerHtml.Split(' ')[0].Split('×');
            var accNode = document.DocumentNode.SelectSingleNode("/html/body/div/div[@id='pages']/div[2]/table/tr[4]/td");
            var acc = accNode.InnerHtml.Replace("% similarity", "");
            BestMatch.url = "https://danbooru.donmai.us/posts/" + id + ".json";
            BestMatch.width = int.Parse(size[0]);
            BestMatch.height = int.Parse(size[1]);
            BestMatch.accuracy = int.Parse(acc);
        }
    }
}
