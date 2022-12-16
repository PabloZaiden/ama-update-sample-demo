using AMAUpdateSample.Utils;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .AddObjectStore(Constants.AzureManagedAppsStoreName)
    .Build();

host.Run();
