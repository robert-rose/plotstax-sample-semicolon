using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Semicolon.Storage.SetupHelpers.Base;

namespace Semicolon.Storage.SetupHelpers;

internal class DependenciesHelper : BaseDependenciesHelper
{
    public DependenciesHelper(WebApplicationBuilder builder) : base(builder) { }

    public override void AddDbContext()
    {
        _builder.Services.AddDbContext<Repository.StorageDbContext>(
            options => options.UseInMemoryDatabase("BlogDb"));
    }
}
