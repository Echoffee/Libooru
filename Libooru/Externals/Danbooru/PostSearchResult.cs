using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libooru.Externals.Danbooru
{

    public class PostSearchResult
    {
        public long? id { get; set; }
        public DateTime created_at { get; set; }
        public long? uploader_id { get; set; }
        public long? score { get; set; }
        public string source { get; set; }
        public string md5 { get; set; }
        public object last_comment_bumped_at { get; set; }
        public string rating { get; set; }
        public int? image_width { get; set; }
        public int? image_height { get; set; }
        public string tag_string { get; set; }
        public bool is_note_locked { get; set; }
        public long? fav_count { get; set; }
        public string file_ext { get; set; }
        public object last_noted_at { get; set; }
        public bool is_rating_locked { get; set; }
        public long? parent_id { get; set; }
        public bool has_children { get; set; }
        public long? approver_id { get; set; }
        public int? tag_count_general { get; set; }
        public int? tag_count_artist { get; set; }
        public int? tag_count_character { get; set; }
        public int? tag_count_copyright { get; set; }
        public long? file_size { get; set; }
        public bool is_status_locked { get; set; }
        public string fav_string { get; set; }
        public string pool_string { get; set; }
        public long? up_score { get; set; }
        public long? down_score { get; set; }
        public bool is_pending { get; set; }
        public bool is_flagged { get; set; }
        public bool is_deleted { get; set; }
        public int? tag_count { get; set; }
        public DateTime updated_at { get; set; }
        public bool is_banned { get; set; }
        public long? pixiv_id { get; set; }
        public object last_comment_at { get; set; }
        public bool have_active_children { get; set; }
        public int? bit_flags { get; set; }
        public string uploader_name { get; set; }
        public bool has_large { get; set; }
        public string tag_string_artist { get; set; }
        public string tag_string_character { get; set; }
        public string tag_string_copyright { get; set; }
        public string tag_string_general { get; set; }
        public bool has_visible_children { get; set; }
        public string file_url { get; set; }
        public string large_file_url { get; set; }
        public string preview_file_url { get; set; }
    }
}
