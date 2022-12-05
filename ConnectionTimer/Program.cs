// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Net;


//wiki sites are hosted on the same server (localed in NV) and have identical code base
await RunTest(1000, "https://wiki2.admin.samfoust.com/");

Console.WriteLine();
await RunTest(1000, "http://wiki.admin.samfoust.com/");


///Local web api starter apps hosted in docker
Console.WriteLine();
await RunTest(1000, "https://localhost:7266/Hook");

Console.WriteLine();
await RunTest(1000, "http://localhost:49153/Test");

Console.WriteLine();
Console.WriteLine("Press a key to exit...");
Console.ReadKey();


async Task RunTest(int ct, string url)
{
    Console.WriteLine($"Running test on {url}");
    //force client to not reuse connections
    HttpClient client = new();

    TimeSpan standardTime = await Connect(ct, false, url);
    Console.WriteLine($"{ct} calls Keep-Alive {standardTime}");

    SocketsHttpHandler socketsHandler = new()
    {
        PooledConnectionLifetime = TimeSpan.FromMilliseconds(1),
        PooledConnectionIdleTimeout = TimeSpan.FromMilliseconds(1),
        MaxConnectionsPerServer = 10
    };

    client = new HttpClient(socketsHandler);
    var standardTimeClose = await Connect(ct, true, url);
    Console.WriteLine($"{ct} calls force ConnectionClose {standardTimeClose}");

    async Task<TimeSpan> Connect(int count, bool closeConnection, string url)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        client.DefaultRequestHeaders.ConnectionClose = closeConnection;

        for (int i = 0; i < count; i++)
        {
            HttpResponseMessage resp = await client.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            await Task.Delay(TimeSpan.FromMilliseconds(2));
        }

        // NOTE: I assume the logic won't be broken :)
        return stopwatch.Elapsed;
    }
}