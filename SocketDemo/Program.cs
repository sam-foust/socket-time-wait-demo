// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Net;


IPAddress[] ips = await Dns.GetHostAddressesAsync("www.admin.samfoust.com");
string ip = ips[0].MapToIPv4().ToString();

Console.WriteLine("Default system netstat");
netstat(ip);

//force client to not reuse connections
SocketsHttpHandler socketsHandler = new()
{
    PooledConnectionLifetime = TimeSpan.FromMilliseconds(1),
    PooledConnectionIdleTimeout = TimeSpan.FromMilliseconds(1),
    MaxConnectionsPerServer = 10
};

HttpClient client = new(socketsHandler);

await Connect(5,false);
await Connect(5,false);
await Connect(5,true);

Console.WriteLine("Press a key to exit...");
Console.ReadKey();

async Task Connect(int count, bool closeConnection)
{
    client.DefaultRequestHeaders.ConnectionClose = closeConnection;

    Console.WriteLine($"Connecting {count} times, ConnectionClose: {closeConnection}");

    for (int i = 0; i < count; i++)
    {
        HttpResponseMessage resp = await client.GetAsync("http://www.admin.samfoust.com");
        resp.EnsureSuccessStatusCode();
        //delay just to make sure connections are not reused
        await Task.Delay(TimeSpan.FromMilliseconds(2));

    }
    netstat(ip);
}

void netstat(string ip)
{
    Process cmd = new();

    cmd.StartInfo.FileName = "cmd.exe";
    cmd.StartInfo.RedirectStandardInput = true;
    cmd.StartInfo.RedirectStandardOutput = true;
    cmd.StartInfo.CreateNoWindow = true;
    cmd.StartInfo.UseShellExecute = false;

    cmd.Start();

    cmd.StandardInput.WriteLine($"netstat -ano | findstr {ip}");
    cmd.StandardInput.Flush();
    cmd.StandardInput.Close();
    string output = cmd.StandardOutput.ReadToEnd();
    int count = output.Split(' ').Count(c => c == "TIME_WAIT");
    Console.WriteLine($"System has {count} sockets in TIME_WAIT for ip: {ip} ");
    Console.WriteLine();
}