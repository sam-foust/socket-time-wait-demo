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
    var client = new HttpClient();


    var standardTime = await Connect(ct, false, url);
    Console.WriteLine($"{ct} calls Keep-Alive {standardTime}");

   

    var socketsHandler = new SocketsHttpHandler
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
        var start = DateTime.Now;
        client.DefaultRequestHeaders.ConnectionClose = closeConnection;
        for (var i = 0; i < count; i++)
        {
            var resp = await client.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            await Task.Delay(TimeSpan.FromMilliseconds(2));
        }
        var end = DateTime.Now;
        return end - start;
    }
}