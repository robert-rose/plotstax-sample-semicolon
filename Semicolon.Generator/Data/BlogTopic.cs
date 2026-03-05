using PlotStax.Gen.Client.Attributes;

namespace Semicolon.Generator.Data;

[PsEnum]
[Flags]
enum BlogTopic
{
    None = 0,
    CSharp = 1,
    DotNet = 2,
    Databases = 4,
    PlotStax = 8
}
