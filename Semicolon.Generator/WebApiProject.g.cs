// The "Initializer" console app can be used to regenerate this file as needed to update the
// available Web API projects.

using PlotStax.Gen.Client.Attributes;

namespace Semicolon.Generator;

/// <summary>
/// Provides a symbolic representation of the available PlotStax WebApi projects while avoiding any
/// reference from Generator to the AppHost.
/// </summary>
public enum WebApiProject
{
    [PsWebApiProjectPath("Semicolon.Main\\Semicolon.Main.csproj")]
    Semicolon_Main
}
