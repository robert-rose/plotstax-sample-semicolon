using PlotStax.Gen.Client.Attributes;

namespace Semicolon.Generator.Data;

[PsRecord]
interface Comment
{
    int Id { get; }
    string Content { get; }
    DateTimeOffset Created { get; }
    int UserId { get; }
    int BlogId { get; }
}