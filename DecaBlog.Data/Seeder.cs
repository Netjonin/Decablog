using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DecaBlog.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace DecaBlog.Data
{
    public class Seeder
    {
        private readonly DecaBlogDbContext _context;
        private readonly UserManager<User> _userMgr;
        private readonly RoleManager<IdentityRole> _roleMgr;

        public Seeder(DecaBlogDbContext context, UserManager<User> userMgr, RoleManager<IdentityRole> roleMgr)
        {
            _context = context;
            _userMgr = userMgr;
            _roleMgr = roleMgr;
        }

        public async Task SeedMe(string envName)
        {
            _context.Database.EnsureCreated();
            var roles = new string[] { "Decadev", "Editor", "Admin" };
            if (!_roleMgr.Roles.Any())
            {
                foreach (var role in roles)
                    await _roleMgr.CreateAsync(new IdentityRole(role));
            }

            var path_userSeed = "/app/Seeds.json";
            var path_squadSeed = "/app/SquadSeeds.json";
            var path_stackSeed = "/app/StackSeeds.json";
            var path_articleSeed = "/app/articleSeed.json";
            var path_articleTopicSeed = "/app/articleTopicSeed.json";
            var path_categorySeed = "/app/categorySeed.json";
            if (envName.Equals("Development"))
            {
                path_userSeed = @"../DecaBlog.Data/Seeds.json";
                path_squadSeed = @"../DecaBlog.Data/SquadSeeds.json";
                path_stackSeed = @"../DecaBlog.Data/StackSeeds.json";
                path_articleSeed = @"../DecaBlog.Data/articleSeed.json";
                path_articleTopicSeed = @"../DecaBlog.Data/articleTopicSeed.json";
                path_categorySeed = @"../DecaBlog.Data/categorySeed.json";
            }

            var squadData = File.ReadAllText(path_squadSeed);
            var squads = JsonConvert.DeserializeObject<List<Squad>>(squadData);
            var stackData = File.ReadAllText(path_stackSeed);
            var stacks = JsonConvert.DeserializeObject<List<Stack>>(stackData);
            var userData = File.ReadAllText(path_userSeed);
            var users = JsonConvert.DeserializeObject<List<User>>(userData);
            var articleTopicData = File.ReadAllText(path_articleTopicSeed);
            var articleTopics = JsonConvert.DeserializeObject<List<ArticleTopic>>(articleTopicData);
            var articleData = File.ReadAllText(path_articleSeed);
            var articles = JsonConvert.DeserializeObject<List<Article>>(articleData);
            var categoryData = File.ReadAllText(path_categorySeed);
            var categories = JsonConvert.DeserializeObject<List<Category>>(categoryData);

            if (!_context.Stacks.Any())
                _context.Stacks.AddRange(stacks);
            if (!_context.Squads.Any())
                _context.Squads.AddRange(squads);
            _context.SaveChanges();
            if (!_userMgr.Users.Any())
            {
                int count = 0;
                var stackCount = 0;
                var squadCount = 0;
                var role = roles[2];
                foreach (var user in users)
                {
                    user.UserName = user.Email;
                    await _userMgr.CreateAsync(user, "P@ssw0rd");
                    if (count > 2)
                        role = roles[1];
                    else if (count > 5)
                        role = roles[0];
                    if (count > 2)
                    {
                        var userStack = new UserStack { Stack = stacks[stackCount] };
                        user.UserStacks.Add(userStack);
                        var userSquad = new UserSquad { Squad = squads[squadCount] };
                        user.UserSquads.Add(userSquad);
                        stackCount++;
                        squadCount++;
                        if (squadCount > squads.Count - 1)
                            squadCount = 0;
                        if (stackCount > stacks.Count - 1)
                            stackCount = 0;
                    }
                    await _userMgr.AddToRoleAsync(user, role);
                    count++;
                }
            }
            if (!_context.Articles.Any())
                _context.Articles.AddRange(articles);
            if (!_context.ArticleTopics.Any())
                _context.ArticleTopics.AddRange(articleTopics);
            if (!_context.Categories.Any())
                _context.Categories.AddRange(categories);
            _context.SaveChanges();
        }
    }
}
