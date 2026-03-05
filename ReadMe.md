# Semicolon: Services for a Simple Blog System
## Solution Summary

"Semicolon" is a fictitious blog management system.  The underlying services are three simple components.  One is a repository manager for the blog data and the other two selectively expose some of the repository functionality as needed:

* "Storage" component has CRUD methods for saving and loading blog data.
* "Authoring" exposes all CRUD functionality, and provides a layer for blog-authoring specific logic.
* "Reading" exposes only the ability to see data, and provides a layer for blog-reading specific logic.

## Dev Machine Dependencies / Prerequisites

* .NET 10 and .NET Aspire SDK version 13 or higher using Visual Studio 2026. The free Community Edition suffices.
* Some basic familiarity with .NET Aspire, C#, and .NET will help in understanding what PlotStax does.
* NPM needs to be installed, but only if you add the UI component.

## Before Running the Generator

Initially, there are a very small number of files.  Before running anything, it might be informative to look through the code and the descriptions below.  It might also help to put this solution in a local Git repo so that file differences can be reviewed and committed after each automated code change.

### Semicolon.AppHost and Semicolon.ServiceDefaults
These are just the default projects of a new "Aspire Empty App" solution.

### Semicolon.Main
This is just an empty "ASP.NET Core Web API" project that's enlisted in orchestration in Semicolon.AppHost.

### Semicolon.Generator
This console app controls the code generation.  It defines a hierarchy of builders that instruct the PlotStax service to generate our application architecture, wiring up all communication so we don't have to write that code.

Builders can be defined with construction method calls chained off a root
"SolutionBuilder" or by using an attributed interface approach.  Both techniques work but the
interface approach lays out the application more clearly.  For example, in the file under "Components" called "Authoring.cs" the interface defines an application component called "Authoring" in which its nested interface defines a scoped service inside Authoring called "Blog".  The methods defined on Blog allow concise definitions for the service methods that Authoring should expose without the need to call any builders.  In the attributed interface approach, the convention of starting interfaces with an "I" is ignored because these don't become application interfaces.  We're just using the syntax to describe builders.

The interface attribute names all begin with "Ps" to make it easy to see what's available.  An interface with a "PsComponent" attribute on it can only have additional attributes applied that also begin with "PsComponent", such as "PsComponentInWebApi" to specify where the component should run and "PsComponentReferences" to specify the component's dependencies.  Similarly, an interface with "PsService" on it can only have additional attributes applied that begin with "PsService".  This naming convention is used for all types of attributes to make them easy to find.

Whether builders are defined with interfaces or directly with chained method calls, PlotStax will use the builders to generate the real application interfaces for the service contract, its internal implementation, its setup of dependency injection, as well as end points and client libraries depending on the configured exposure and usage.

To learn more about the PlotStax client API used by the Generator, see the [PlotStax API Documentation.](https://plotcourse.com/ps/apidocs)

### Semicolon.Initializer
The main purpose of the Initializer is to generate the enum defined in "WebApiProject.g.cs" in the Generator project based on the Web API projects enlisted in orchestration.

This allows the Generator to be run without a dependency on any other project in the solution yet
still contain a symbolic representation of the ASP.NET Core Web API projects available for running
components.  Keeping the Generator free of any project dependencies allows it to be used as needed regardless of whether the other projects build.

### Directory under the solution directory called "Semicolon.Storage\SetupHelpers"
This folder wouldn't normally exist prior to running the Generator but for the purpose of this sample these folders have code that will set up an in-memory database and populate some data as part of the complete program after the Generator is run.

## Run the Generator
Run the console app Semicolon.Generator either by switching it to the startup project and running or using "Debug" on the project context menu.

When it runs it will show a warning about setting up a database connection string.  This can be ignored since there's some code in this sample to just use an in-memory database.  However, there are a couple limitations on the types of queries handled by an in-memory database:

1) None of the methods for deleting data can be used.
1) The optimized pagination methods "GetUsers" and "GetUsersCustomPageSize" cannot be used because they rely on relational data.  To run these methods requires setting up the connection string to a real database.  This [Microsoft Learn page shows how to configure a connection string.](https://learn.microsoft.com/en-us/ef/core/miscellaneous/connection-strings?tabs=dotnet-core-cli)

All other methods should work.

## After Running the Generator
A couple of the services are configured for exposure outside the Web API so after running the Generator it's possible to switch the startup project to the Semicolon.AppHost and directly run some service methods through the generated Web API endpoints.  By default in dev, Scalar is used as the interface built from the OpenAPI but this can be changed.

### UI Layers
We could add the following declaration in a new file under the Generator project's "Components" folder and rerun the Generator.  When you run the new AppHost, a new entry should appear in the dashboard for the UI along with a sub-entry for an npm install.  Because the attribute "PsUiComponentReferences" specifies "Authoring" this UI project will be equipped with the data types and proxy models for calling the server dependency "Authoring" using strongly typed methods.  No need to write the "fetch" code.

```C#
using PlotStax.Gen.Client.Attributes;

namespace Semicolon.Generator.Components;

[PsUiComponent]
[PsUiComponentReferences<Authoring>]
public interface AuthoringUi
{
}
```

PlotStax also offers quick scaffolding of additional sub-components in UI components.  These are described in detail in the [online API documnentation.](https://plotcourse.com/ps/apidocs/release-notes/february-2026)

### Scaling
The sample uses one ASP.NET Core Web API to run all components.  Because components are cleanly abstracted from the WebAPI and because communication code between components is generated, we can move components to different web APIs without having to change dev-managed code such as internal service implementations and UI code.  Suppose we want the component "Authoring" to run in its own dedicated ASP.NET Core Web API.  To do that:

1) Add a new "ASP.NET Core Web API" project named "Semicolon.AuthoringWebApi"
1) Enlist the new project in orchestration.
1) Run the Initializer console app.  This will give the Generator a new symbol to reference the Web API project.
1) Under the Generator, change the configuration of Authoring to specify the new project by changing its "PsComponentInWebApi" attribute (replace Semicolon_Main with Semicolon_AuthoringWebApi).
1) Re-run the Generator.

We're now ready to deploy the work done in Authoring to its own dedicated resource (container, VM, or server).  Before changing the resource where a component runs, it's important to consider the impact on the overall architecture due to pre-existing component relationships.  Any dependencies of a component that's moved may need increased exposure as this example demonstrates.  After the change to run the Authoring component in its own resource, the service it references in Storage automatically has new endpoints exposed that are required to support calls from Authoring.  So it might be desirable to also move Storage to its own resource (such as "StorageWebApi") and that resource could run without any public exposure while the Main resource could have public exposure with only the Reading endpoints on it.

### Data Differences Between Layers
In the Generator under the Data folder, the code for "User" indicates that it has an internal field called "CreatedFromIpAddress".  The comments in that code describe how to control the scope of use of a field.  Since this field is unknown to anything outside the components "Authoring" and "Storage" any outside callers to create a new User record can't provide anything for this field, so the component Authoring would need to handle that data.  For example, we could add dev-managed code to set this field on any new User record:

1) Open the internal service implementation for the Authoring component's Blog service here:

	[Semicolon Root]\Semicolon.Authoring\Services\Blog.cs

1) Update the "using" section to this:

	```C#
	using Microsoft.AspNetCore.Http;
	using Semicolon.Authoring.Services.Interfaces;
	using Semicolon.Authoring.Data;
	using Semicolon.Shared.Data.Wrappers;
	```

1) Override the AddUser method and set the internal field by adding this:

	```C#
	public override Task<Result<User>> AddUser(User user, HttpContext httpContext)
	{
		user.CreatedFromIpAddress = httpContext?.Request?.Headers?["X-Forwarded-For"].FirstOrDefault()
			?? httpContext?.Connection?.RemoteIpAddress?.ToString()
			?? "";
		return base.AddUser(user, httpContext);
	}
	```

In some cases, mappers can also be used to handle data field differences more globally for multiple methods in a component but that's beyond the scope of this sample.

### Transient Records

Applications often need transient records for communication of data that does not correspond 1:1 with a main record data type.  Transient records might be used to send sets of data to services that include multiple other entities or to return results of calculations and/or data joined from multiple sources.  When PlotStax generates EF DbContext definitions it will always ignore transient records since we wouldn't want them to be stored.

As an example of a transient record, in the places where we show paginated lists of blog titles to users the "Blog" data type doesn't include the name of the author and does include a potentially large amount of data we don't need by including the field "Content".

To solve this:

1) Add a transient record for what we really want in our paginated lists, i.e. include author name and exclude the full blog text.  In the Blog.cs file under the Generator's "Data" folder, we could add:

	```C#
	[PsTransientRecord]
	interface BlogListItem
	{
		int BlogId { get; }
		string Title { get; }
		string AuthorName { get; }
		DateTimeOffset Created { get; }
	}
	```

1) Do a find/replace only in the Generator project.  Replace ``PagedResult<Blog>`` with ``PagedResult<BlogListItem>``.  This should just match six locations under Components.

1) Re-run the Generator.

1) In regenerating the code for the Storage component PlotStax will detect the discrepancy between the service method return type (BlogListItem) and the queryable result type (Blog) and will generate a couple abstract methods that need to be implemented by dev-managed code to resolve this.  To do that, open the dev-managed portion of the service defined here:

    [Semicolon Root]\Semicolon.Storage\Services\DataAccess.cs

1) Add these methods to generate instances of the new transient record while still leveraging all the code generated for paginated queries:

	```C#
	protected override IQueryable<Data.BlogListItem> GetBlogs_Transduce(IQueryable<Data.Blog> baseQuery)
	{
		var query = from b in baseQuery
			join u in _storageDbContext.User
			on b.UserId equals u.Id
			into matches
			from m in matches.DefaultIfEmpty()
			select new Data.BlogListItem(b.Id, b.Title, $"{m.FirstName} {m.LastName}", b.Created);
		return query;
	}

	protected override IQueryable<Data.BlogListItem> GetBlogsCustomPageSize_Transduce(IQueryable<Data.Blog> baseQuery)
	{
		return GetBlogs_Transduce(baseQuery);
	}
	```

### More to Explore
This small sample doesn't cover everything you can do with PlotStax, for example, defining Broadcasts and Internal Singletons, two other types of sub-components that can be generated.  Broadcasts allow event-driven communication from dependencies to all dependents by publishing Messages or changes to Observables.  Internal Singletons can subscribe and publish to broadcasts and do other kinds of tasks internal to a component.  The online [PlotStax API Documentation](https://plotcourse.com/ps/apidocs) provides more details.
