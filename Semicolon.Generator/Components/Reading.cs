using PlotStax.Gen.Client;
using PlotStax.Gen.Client.Attributes;

using Semicolon.Generator.Data;

namespace Semicolon.Generator.Components;

[PsComponent(CSharpComponentType.Library)]
[PsComponentInWebApi<WebApiProject>(WebApiProject.Semicolon_Main)]
interface Reading
{
    [PsService(ExposureSetting = SubComponentExposureSetting.AlwaysExternalWebApi)]
    [PsServiceReferencesService<Storage.DataAccess>]
    [PsServiceAutoGenMethodsForNamespace(AutoGenMethod.RetrieveByKey)] // restrict the generated methods for reading only.  See also the second parameter of this attribute and other auto-generation attributes for controlling what methods to generate.
    interface BlogReader
    {
        // For more info about these specific pagination methods, see Storage.cs and the builder
        // code in Solution.cs.
        Task<PagedResult<Blog>> GetBlogs(int page, BlogTopic topics);
        Task<PagedResult<Blog>> GetBlogsCustomPageSize(int page, int itemsPerPage, BlogTopic topics);
        Task<List<Comment>> GetComments(int blogId, int beforeCommentId);
        Task<List<Comment>> GetCommentsCustomPageSize(int blogId, int beforeCommentId, int itemsPerPage);
    }
}
