using CodeCapital.WordPress.Core.Models;
using CodeCapital.WordPress.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Core.Repositories
{
    public class MenuRepository
    {
        private const string EmptyStringFix = ""; // it doesn't work without that
        private readonly WordPressDbContext _context;

        public MenuRepository(WordPressDbContext context) => _context = context;

        //ToDo this is not doing categories from WordPress, categoryId is in _menu_item_object_id
        public Task<List<MenuItem>> GetMenuAsync(int menuId)
        {
            // Note the empty string at the end, it doesn't work without that
            var items = _context.Set<MenuItem>().FromSqlRaw(
                $@" SELECT 
                        CAST(`p`.`ID` AS signed) AS `MenuId`, 
                        `p`.`post_title` AS `CustomTitle`, 
                        `postMetaUrl`.`meta_value` AS `CustomUrl`,
                        `targetPost`.`post_title` AS `PostTitle`, 
                        `targetPost`.`post_name` AS `PostName`,
                        `targetPost`.`post_type` AS `PostType`,
                        CAST(`targetPost`.`ID` AS signed) AS `PostId`,
                        CAST(`targetPost`.`post_parent` AS signed) AS `ParentPostId`,
                        CAST(`postMetaParent`.`meta_value` AS signed) AS `ParentMenuId`
                  FROM `{_context.TablePrefix}posts` AS `p`
                  LEFT JOIN `{_context.TablePrefix}term_relationships` AS `tr` ON `p`.`ID` = `tr`.`object_id`
                  LEFT JOIN `{_context.TablePrefix}term_taxonomy` AS `tt` ON `tr`.`term_taxonomy_id` = `tt`.`term_taxonomy_id`
                  LEFT JOIN `{_context.TablePrefix}postmeta` AS `pm` ON `p`.`ID` = `pm`.`post_id`
                  LEFT JOIN `{_context.TablePrefix}posts` AS `targetPost` ON `pm`.`meta_value` = `targetPost`.`ID` AND `pm`.`meta_key` = '_menu_item_object_id'
                  LEFT JOIN `{_context.TablePrefix}postmeta` AS `postMetaUrl` ON `p`.`ID` = `postMetaUrl`.`post_id` AND '_menu_item_url' = `postMetaUrl`.`meta_key`
                  LEFT JOIN `{_context.TablePrefix}postmeta` AS `postMetaParent` ON `p`.`ID` = `postMetaParent`.`post_id` AND `postMetaParent`.`meta_key` = '_menu_item_menu_item_parent'
                  WHERE `tt`.`term_id` = {menuId} AND `pm`.`meta_key` = '_menu_item_object_id' AND `targetPost`.`post_title` IS NOT NULL 
                  ORDER BY `p`.`menu_order`").AsNoTracking().ToListAsync();

            return items;
        }

        //public async Task<List<MenuItem>> GetMenuAsync(int menuId)
        //{
        //    var items = await (from p in _context.Posts
        //                       join tr in _context.TermRelationships on p.ID equals tr.object_id into tr1
        //                       from tr in tr1.DefaultIfEmpty()

        //                       join tt in _context.TermTaxonomies on tr.term_taxonomy_id equals tt.term_taxonomy_id into tt1
        //                       from tt in tt1.DefaultIfEmpty()

        //                       join pm in _context.PostMetas on p.ID equals pm.post_id into pm1
        //                       from pm in pm1.DefaultIfEmpty()

        //                       join targetPost in _context.Posts
        //                           on new { Value = pm.meta_value, Key = pm.meta_key }
        //                           // this is causing MySqlException: Illegal mix of collations (utf8mb4_unicode_ci,IMPLICIT) and (utf8mb4_general_ci,IMPLICIT) for operation '='
        //                           equals new { Value = targetPost.ID.ToString(), Key = "_menu_item_object_id" } into targetPost1
        //                       from targetPost in targetPost1.DefaultIfEmpty()

        //                       join postMetaUrl in _context.PostMetas
        //                            on new { Value = p.ID, Key = "_menu_item_url" }
        //                            equals new { Value = postMetaUrl.post_id, Key = postMetaUrl.meta_key } into postMetaUrl1
        //                       from postMetaUrl in postMetaUrl1.DefaultIfEmpty()

        //                       where tt.term_id == menuId
        //                             && pm.meta_key == "_menu_item_object_id"

        //                       orderby p.menu_order
        //                       select new MenuItem
        //                       {
        //                           MenuId = p.ID,
        //                           CustomTitle = p.post_title,
        //                           CustomUrl = postMetaUrl.meta_value,
        //                           PostTitle = targetPost.post_title,
        //                           PostName = targetPost.post_name,
        //                           PostType = targetPost.post_type,
        //                           PostId = targetPost.ID,
        //                           ParentId = targetPost.post_parent
        //                       }).ToListAsync();

        //    return items;
        //}
    }
}
