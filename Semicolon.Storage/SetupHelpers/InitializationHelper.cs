using Microsoft.Extensions.DependencyInjection;
using Semicolon.Storage.SetupHelpers.Base;
using Semicolon.Storage.Repository;
using Semicolon.Storage.Services.Interfaces;

namespace Semicolon.Storage.SetupHelpers;

internal class InitializationHelper : BaseInitializationHelper
{
    public InitializationHelper(IServiceProvider services) : base(services) { }

    public override void Initialize()
    {
        base.Initialize();

        using var serviceScope = _services.CreateScope();

        var context = serviceScope.ServiceProvider.GetRequiredService<StorageDbContext>();

        if (!context.Database.EnsureCreated())
        {
            return; //DB already exists
        }

        // this atypical use of the service methods is just to set up some test data in dev Users, Blogs, and Comments:
        var svc = serviceScope.ServiceProvider.GetRequiredService<IDataAccess>();

        var now = DateTimeOffset.UtcNow;
        string[] firstNames = ["Maria", "Nushi", "Mohammed", "Jose", "Wei", "Yan", "David", "Annie"];
        string[] lastNames = ["Wang", "Smith", "Ivanov", "Garcia", "Silva", "Nguyen", "Deng", "Hansen"];
        var allUsers = new List<Data.User>();
        var allBlogs = new List<Data.Blog>();
        const int blogsPerUser = 3;
        const int maxCommentsPerBlog = 42;

        foreach (var f in firstNames)
        {
            foreach (var l in lastNames)
            {
                var user = AddTestUser(svc, f, l, $"{f}.{l}@whatever.com");
                allUsers.Add(user);

                for (var i = 0; i < blogsPerUser; i++)
                {
                    allBlogs.Add(AddTestBlog(svc, user, now));
                }
            }
        }

        foreach (var b in allBlogs)
        {
            var commentCount = Random.Shared.Next(3, maxCommentsPerBlog);

            for (var i = 0; i < commentCount; i++)
            {
                var u = allUsers[Random.Shared.Next(0, allUsers.Count)];
                AddTestComment(svc, "Comment on blog.  Lorem ipsum.", now, u.Id, b.Id);
            }
        }
    }

    private Data.User AddTestUser(IDataAccess svc, string firstName, string lastName, string email)
    {
        var result = svc.AddUser(new Data.User(0, firstName, lastName, email, "1.2.3.4"), null).GetAwaiter().GetResult();
        return result.Item;
    }

    private Data.Blog AddTestBlog(IDataAccess svc, Data.User user, DateTimeOffset created)
    {
        var topics = (Data.BlogTopic)Random.Shared.Next(1, 16);
        var title = $"{user.FirstName}'s Blog about {topics}";
        var content = "Lorem ipsum.";
        var result = svc.AddBlog(new Data.Blog(0, user.Id, title, topics, content, created), null).GetAwaiter().GetResult();
        return result.Item;
    }

    private Data.Comment AddTestComment(IDataAccess svc, string content, DateTimeOffset created, int userId, int blogId)
    {
        var result = svc.AddComment(new Data.Comment(0, content, created, userId, blogId), null).GetAwaiter().GetResult();
        return result.Item;
    }
}
