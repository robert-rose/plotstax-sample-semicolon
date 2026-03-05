using PlotStax.Gen.Client;

var initializer = new Initializer<Projects.Semicolon_AppHost>("Semicolon");
initializer
    .UseAllAvailableProjectsAsWebApis()
    .WriteFiles();