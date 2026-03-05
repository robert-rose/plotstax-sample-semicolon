using PlotStax.Gen.Client.Attributes;

namespace Semicolon.Generator.Data;

[PsRecord]
interface Blog
{
    int Id { get; }
    int UserId { get; }

    [PsProperty(Length = 100)]
    string Title { get; }
    BlogTopic Topics { get; }
    string Content { get; }
    DateTimeOffset Created { get; }
}

[PsPatchRecord<Blog>]
interface BlogPatchSmall
{
    string Title { get; }
    BlogTopic Topics { get; }
}

[PsPatchRecord<Blog>]
interface BlogPatchLarge
{
    string Content { get; }
}
