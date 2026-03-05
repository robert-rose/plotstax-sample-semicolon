using PlotStax.Gen.Client;
using Semicolon.Generator.Components;
using Semicolon.Generator.Data;

namespace Semicolon.Generator;

public static class Solution
{
    public static SolutionBuilder GetSolutionBuilder()
    {
        var slnBuilder = new SolutionBuilder("Semicolon")
            .WithToken("SAMPLES_ONLY_TOKEN") // this free-use token works with small numbers of structures
            .WithOptions(o => {
                o.AppHostDirectory = "Semicolon.AppHost";
                o.AppHostProjectFileName = "Semicolon.AppHost.csproj";
                o.AppHostAssemblyName = "Semicolon.AppHost";
            });

        ModifyServices(slnBuilder);

        return slnBuilder;
    }

    public static void ModifyServices(SolutionBuilder slnBuilder)
    {
        var dataSvcBuilder = slnBuilder.ModifyService<Storage.DataAccess>();

        // The following builder configurations demonstrate 3 different ways of setting up
        // paginated queries.

        // Approach #1 - Front end filtering with EF, a simple implementation that's only for small
        // numbers of rows.

        dataSvcBuilder.ModifyMethod(nameof(Storage.DataAccess.GetBlogs))
            .WithSelect<Blog>()
            .ConditionOnParameter(b => b.Topics, Condition.SupersetOfFlags)
            .OrderByDescending(b => b.Created)
            .WithFixedItemsPerPage(15);

        dataSvcBuilder.ModifyMethod(nameof(Storage.DataAccess.GetBlogsCustomPageSize))
            .WithSelect<Blog>()
            .ConditionOnParameter(b => b.Topics, Condition.SupersetOfFlags)
            .OrderByDescending(b => b.Created)
            .WithMaxItemsPerPage(200);

        // Approach #2 - Caller managed start and end point filtering.  Performs well on large
        // numbers of rows when DB is properly indexed but has disadvantage that pages can only be
        // viewed in sequence and information on total count or total page count is not available.

        dataSvcBuilder.ModifyMethod(nameof(Storage.DataAccess.GetComments))
            .WithSelect<Comment>()
            .ConditionOnParameter(c => c.BlogId, Condition.Equal)
            .ConditionOnParameter(c => c.Id, Condition.LessThan, "beforeCommentId") //assumes we want newest first
            .OrderByDescending(c => c.Id)
            .Take(10);

        dataSvcBuilder.ModifyMethod(nameof(Storage.DataAccess.GetCommentsCustomPageSize))
            .WithSelect<Comment>()
            .ConditionOnParameter(b => b.BlogId, Condition.Equal)
            .ConditionOnParameter(b => b.Id, Condition.LessThan, "beforeCommentId")
            .OrderByDescending(b => b.Id)
            .TakeParameter("itemsPerPage")
            .WithMaxItemsPerPage(200);

        // Approach #3 - Managed key data-point filtering.  Similar to approach #2 but involves
        // briefly caching the whole set of key data points for identifying the first record of
        // each page.  It also avoids making the caller responsible for passing in the key data
        // points for the page.  As with the first approach, it also allows any page to be queried
        // ad-hoc and every call returns a PagedResult with a total record count and total page
        // count.
        // The cache of key data points is built as needed by an optimal TSQL query.
        // Performs well on large numbers of rows when DB is properly indexed.

        dataSvcBuilder.ModifyMethod(nameof(Storage.DataAccess.GetUsers))
            .WithSelect<User>()
            .ConditionOnParameter(u => u.LastName, Condition.PatternLeft)
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .WithFixedItemsPerPage(20)
            .WithOptimization(120);

        dataSvcBuilder.ModifyMethod(nameof(Storage.DataAccess.GetUsersCustomPageSize))
            .WithSelect<User>()
            .ConditionOnParameter(u => u.LastName, Condition.PatternLeft)
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .WithMaxItemsPerPage(200)
            .WithOptimization(120);

        // In more complex scenarios, the configuration of the queries through the builders as it's
        // done above might not provide the level of sophistication needed in the query, for
        // example if additional joins are needed.  In those scenarios dev-managed code can be used
        // to override with custom code as needed.  For example, in the third approach, virtual
        // methods are isolated for both the TSQL key data-point filter and the Queryable for EF so
        // that these parts can be modified as needed while still taking advantage of the code
        // generated to manage the pagination.
        //
        // If additional joins are used that always match a single record (and therefore do not
        // affect the row count), it'd be simpler to just return a transient data type and use the
        // dev-managed method to modify the Queryable instead of replacing it.  This can be done
        // with any select query by changing the method return type to a type that's different from
        // the type specified in "WithSelect".  See the description of the record "BlogListItem" for
        // an example of this.
    }
}
