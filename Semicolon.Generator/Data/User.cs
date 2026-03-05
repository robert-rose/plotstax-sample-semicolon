using PlotStax.Gen.Client;
using PlotStax.Gen.Client.Attributes;

namespace Semicolon.Generator.Data;

[PsRecord]
interface User
{
    int Id { get; }

    [PsProperty(Length = 100)]
    string FirstName { get; }

    [PsProperty(Length = 100)]
    string LastName { get; }

    [PsProperty(Length = 256)]
    string Email { get; }

    [PsProperty(Length = 100)]
    // By assigning a field to a profile, we control precisely where it's available and not available.
    // See comments below for more info.
    [PsPropertyInDataProfile<ServerOnly>]
    string CreatedFromIpAddress { get; }
}

[PsDataProfile]
// This means the field is known to the Authoring component internally but will not be exposed
// on its contract:
[PsDataProfileComponent<Components.Authoring>(DataProfileComponentUse.Internal)]
// This means the field is known to the Storage component and also exposed on its contract so
// that a caller can pass the value for it.  However, the Web API for Storage itself doesn't
// need to be exposed to anything other than the other components so the field
// "CreatedFromIpAddress" defined above can be entirely internal to the system.
[PsDataProfileComponent<Components.Storage>(DataProfileComponentUse.Both)]
interface ServerOnly
{
}
