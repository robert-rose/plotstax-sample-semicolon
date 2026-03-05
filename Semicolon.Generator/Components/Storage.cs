using PlotStax.Gen.Client;
using PlotStax.Gen.Client.Attributes;
using Semicolon.Generator.Data;

namespace Semicolon.Generator.Components;

[PsComponent(CSharpComponentType.RepositoryManagerEF)] // this will give us a C# component with a database context
[PsComponentInWebApi<WebApiProject>(WebApiProject.Semicolon_Main)]
interface Storage
{
    [PsService]
    [PsServiceAutoGenMethodsForNamespace(AutoGenMethod.All)] // because of the component type, this will give us EF implementations for all methods.
    interface DataAccess
    {
        // Method definitions do not need to be auto-generated to benefit from automatic wire-up
        //  in proxies, dependencies, endpoints, etc., even if the method implementation needs to
        //  be defined in the dev-managed code.  For example:
        Task<int> DoWhateverCustomerOperations(int someSetting);

        // These methods are used to demonstrate different ways of setting up paginated queries.
        // These definitions direct construction of method builders.  The pagination settings are
        // further configured and explained in the Solution.cs file.
        Task<PagedResult<Blog>> GetBlogs(int page, BlogTopic topics);
        Task<PagedResult<Blog>> GetBlogsCustomPageSize(int page, int itemsPerPage, BlogTopic topics);
        Task<List<Comment>> GetComments(int blogId, int beforeCommentId);
        Task<List<Comment>> GetCommentsCustomPageSize(int blogId, int beforeCommentId, int itemsPerPage);
        Task<PagedResult<User>> GetUsers(int page, string lastName);
        Task<PagedResult<User>> GetUsersCustomPageSize(int page, int itemsPerPage, string lastName);
    }
}
