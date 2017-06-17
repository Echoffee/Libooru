using Libooru.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libooru.Externals.Danbooru
{
	public class TagSearchResult
	{
		public long? id { get; set; }
		public string name { get; set; }
		public long? post_count { get; set; }
		public string related_tags { get; set; }
		public string related_tags_updated_at { get; set; }
		public int category { get; set; }
		public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }
		public bool is_locked { get; set; }

		public static TagType GetCategory(int c)
		{
			switch(c){
				default:
				case 0: return TagType.General;
				case 1: return TagType.Artist;
				case 3: return TagType.Copyright;
				case 4: return TagType.Character;
				case 2: return TagType.Meta;
			}

		}

	}
}
