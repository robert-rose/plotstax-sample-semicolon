using PlotStax.Gen.Client;
using PlotStax.Gen.Client.Attributes;

using Semicolon.Generator.Data;

namespace Semicolon.Generator.Components;

[PsComponent(CSharpComponentType.Library)] // this will give us a C# component called "Semicolon.Authoring"
[PsComponentInWebApi<WebApiProject>(WebApiProject.Semicolon_Main)]
interface Authoring
{
    [PsService(ExposureSetting = SubComponentExposureSetting.AlwaysExternalWebApi)] // this will give us endpoints for a scoped service in the component called "Blog"
    [PsServiceReferencesService<Storage.DataAccess>] // this will build a Contract layer for DataAccess and inject a proxy for using it in the Blog service
    [PsServiceAutoGenMethodsForNamespace(AutoGenMethod.All)] // this will generate CRUD methods for all record definitions in the Generator namespace (see folder "Data").
    interface Blog
    {
        // By default, a method signature that precisely matches the signature of a method on a
        // dependency of the service causes the generated code to automatically map those calls.
        // In this layer the content for these methods is generated to call the matching method in
        // Storage, while in the Storage layer the content is generated for an EF query.
        //
        // This mapping code generation behavior can be controlled as needed using
        // "AutoMethodContent" at various scopes in the builders.
        //
        // For more info about these specific pagination methods, see Storage.cs and the builder
        // code in Solution.cs.
        Task<PagedResult<Blog>> GetBlogs(int page, BlogTopic topics);
        Task<PagedResult<Blog>> GetBlogsCustomPageSize(int page, int itemsPerPage, BlogTopic topics);
        Task<List<Comment>> GetComments(int blogId, int beforeCommentId);
        Task<List<Comment>> GetCommentsCustomPageSize(int blogId, int beforeCommentId, int itemsPerPage);
        [PsMethod(HttpVerb = ServiceMethodHttpVerb.Get)] // if the verb is left unspecified, a method like this with a single string parameter will POST the parameter for highest capacity and fidelity but we don't need that for a last name
        Task<PagedResult<User>> GetUsers(int page, string lastName);
        [PsMethod(HttpVerb = ServiceMethodHttpVerb.Get)]
        Task<PagedResult<User>> GetUsersCustomPageSize(int page, int itemsPerPage, string lastName);
    }
}
