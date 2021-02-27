using CodeCapital.WordPress.Core.Models;
using CodeCapital.WordPress.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Core.Repositories
{
    public class MetadataRepository
    {
        private const string EmptyStringFix = ""; // it doesn't work without that
        private readonly WordPressDbContext _context;
        private readonly WordPressSettings _wordPressSettings;

        //private const string ExcludedMetaKeys = @"'_yoast_wpseo_content_score','_edit_lock','_edit_last','_revision-control','_wp_old_slug','wp-smpro-smush-data','_expiration-date-status'";

        public MetadataRepository(WordPressDbContext context, IOptions<WordPressSettings> wordPressSettings)
        {
            _context = context;
            _wordPressSettings = wordPressSettings.Value;
        }

        public Task<List<PostMeta>> GetPlainAsync(int postId)
            => _context.PostMetas.Where(w => w.PostId == postId).ToListAsync();

        // Maybe we can merge it with GetAsync(List<int> ids)
        /// <summary>
        /// This includes Featured image metadata through _thumbnail_id
        /// </summary>
        /// <returns></returns>
        public Task<List<PostMeta>> GetAsync()
        {
            var (whereCondition1, whereCondition2) = GetConditions();

            return _context.PostMetas.FromSqlRaw($@"
                        SELECT meta_id, post_id, meta_key, meta_value
                        FROM {_context.TablePrefix}postmeta 
                        {whereCondition1}
                        UNION
                        SELECT pm2.meta_id, pm.post_id, pm2.meta_key, pm2.meta_value
                        FROM {_context.TablePrefix}postmeta pm
                        JOIN {_context.TablePrefix}postmeta pm1 ON pm.post_id = pm1.post_id
                            AND pm1.meta_key = '_thumbnail_id'
                        JOIN {_context.TablePrefix}postmeta pm2 ON pm1.meta_value = pm2.post_id
                        {whereCondition2}").AsNoTracking().ToListAsync();

            (string, string) GetConditions()
            {
                if (string.IsNullOrWhiteSpace(_wordPressSettings.AllowedMetaKeys)) return ("", "");

                return ($@"WHERE meta_key IN ({_wordPressSettings.SqlFormattedAllowedMetaKeys})",
                        $@"WHERE pm2.meta_key IN ({_wordPressSettings.SqlFormattedAllowedMetaKeys})");
            }
        }

        /// <summary>
        /// This includes Featured image metadata through _thumbnail_id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<List<PostMeta>> GetAsync(int id) => GetAsync(new List<int> { id });

        /// <summary>
        /// This includes Featured image metadata through _thumbnail_id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task<List<PostMeta>> GetAsync(List<int> ids)
        {
            if (ids.Count == 0) return Task.FromResult(new List<PostMeta>());

            var (andCondition1, andCondition2) = GetConditions();

            var idsString = string.Join(",", ids);

            return _context.PostMetas.FromSqlRaw($@"
                        SELECT meta_id, post_id, meta_key, meta_value
                        FROM {_context.TablePrefix}postmeta 
                        WHERE {andCondition1} post_id IN ({idsString})
                        UNION
                        SELECT pm2.meta_id, pm.post_id, pm2.meta_key, pm2.meta_value
                        FROM {_context.TablePrefix}postmeta pm
                        JOIN {_context.TablePrefix}postmeta pm1 ON pm.post_id = pm1.post_id
                            AND pm1.meta_key = '_thumbnail_id'
                        JOIN {_context.TablePrefix}postmeta pm2 ON pm1.meta_value = pm2.post_id
                        WHERE {andCondition2} pm.post_id IN ({idsString})").AsNoTracking().ToListAsync();

            (string, string) GetConditions()
            {
                if (string.IsNullOrWhiteSpace(_wordPressSettings.AllowedMetaKeys)) return ("", "");

                return ($@"meta_key IN ({_wordPressSettings.SqlFormattedAllowedMetaKeys}) AND ",
                        $@"pm2.meta_key IN ({_wordPressSettings.SqlFormattedAllowedMetaKeys}) AND ");
            }
        }

        //private static string GetMediumImage(string meta)
        //{
        //    string url = null;
        //    var ht = (Hashtable)new Conversive.PHPSerializationLibrary.Serializer().Deserialize(meta);

        //    if (ht != null && ht.ContainsKey("sizes") && ht.ContainsKey("file"))
        //    {
        //        var size = (Hashtable)ht["sizes"];
        //        var file = ht["file"].ToString();
        //        string folder = null;

        //        if (!string.IsNullOrWhiteSpace(file) && file.LastIndexOf('/') > 0)
        //        {
        //            folder = file.Substring(0, file.LastIndexOf('/') + 1);
        //        }

        //        if (size != null && !string.IsNullOrWhiteSpace(folder) && size.ContainsKey("medium"))
        //        {
        //            var medium = (Hashtable)size["medium"];

        //            if (medium != null && medium.ContainsKey("file"))
        //            {
        //                url = folder + medium["file"];
        //            }
        //        }
        //    }

        //    return url;

        //}

    }
}

//a:6:
//    {s:5:"width";i:1140;s:6:"height";i:312;s:4:"file";s:21:"2015/03/beach1140.jpg";s:5:"sizes";a:4:
//        {
//            s:9:"thumbnail";a:5:{s:4:"file";s:21:"beach1140-150x150.jpg";s:5:"width";i:150;s:6:"height";i:150;s:9:"mime-type";s:10:"image/jpeg";s:10:"wp_smushit";s:26:"ERROR: posting to Smush.it";}
//            s:6:"medium";a:5:{s:4:"file";s:20:"beach1140-300x82.jpg";s:5:"width";i:300;s:6:"height";i:82;s:9:"mime-type";s:10:"image/jpeg";s:10:"wp_smushit";s:26:"ERROR: posting to Smush.it";}
//            s:5:"large";a:5:{s:4:"file";s:22:"beach1140-1024x280.jpg";s:5:"width";i:1024;s:6:"height";i:280;s:9:"mime-type";s:10:"image/jpeg";s:10:"wp_smushit";s:26:"ERROR: posting to Smush.it";}
//            s:14:"post-thumbnail";a:5:{s:4:"file";s:21:"beach1140-825x312.jpg";s:5:"width";i:825;s:6:"height";i:312;s:9:"mime-type";s:10:"image/jpeg";s:10:"wp_smushit";s:26:"ERROR: posting to Smush.it";}
//        }
//    s:10:"image_meta";
//    a:11:{s:8:"aperture";i:0;s:6:"credit";s:0:"";s:6:"camera";s:0:"";s:7:"caption";s:0:"";s:17:"created_timestamp";i:0;s:9:"copyright";s:0:"";s:12:"focal_length";i:0;s:3:"iso";i:0;s:13:"shutter_speed";i:0;s:5:"title";s:0:"";s:11:"orientation";i:0;}
//    s:10:"wp_smushit";
//    s:26:"ERROR: posting to Smush.it";
//    }
//Result \"((thumbnail|medium|large|post-thumbnail)|(([^\"]*\.jpe?g)))\"