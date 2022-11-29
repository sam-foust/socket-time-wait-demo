// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Net;


var ips = await Dns.GetHostAddressesAsync("www.admin.samfoust.com");
var ip = ips[0].MapToIPv4().ToString();

Console.WriteLine("Default system netstat");
netstat(ip);


//force client to not reuse connections
var socketsHandler = new SocketsHttpHandler
{
    PooledConnectionLifetime = TimeSpan.FromMilliseconds(1),
    PooledConnectionIdleTimeout = TimeSpan.FromMilliseconds(1),
    MaxConnectionsPerServer = 10
};

var client = new HttpClient(socketsHandler);



await Connect(5,false);

await Connect(5,false);

await Connect(5,true);



Console.WriteLine("Press a key to exit...");
Console.ReadKey();


async Task Connect(int count, bool closeConnection)
{
    client.DefaultRequestHeaders.ConnectionClose = closeConnection;
    Console.WriteLine($"Connecting {count} times, ConnectionClose: {closeConnection}");
    for (var i = 0; i < count; i++)
    {
        try
        {

            await client.GetAsync("http://www.admin.samfoust.com");
            //delay just to make sure connections are not reused
            await Task.Delay(TimeSpan.FromMilliseconds(2));
        }
        catch (Exception e)
        {

            throw;
        }

    }
    netstat(ip);
}
void netstat(string ip)
{
    Process cmd = new Process();

    cmd.StartInfo.FileName = "cmd.exe";
    cmd.StartInfo.RedirectStandardInput = true;
    cmd.StartInfo.RedirectStandardOutput = true;
    cmd.StartInfo.CreateNoWindow = true;
    cmd.StartInfo.UseShellExecute = false;

    cmd.Start();

    cmd.StandardInput.WriteLine($"netstat -ano | findstr {ip}");
    cmd.StandardInput.Flush();
    cmd.StandardInput.Close();
    var output = cmd.StandardOutput.ReadToEnd();
    var count = output.Split(' ').Count(c => c == "TIME_WAIT");
    Console.WriteLine($"System has {count} sockets in TIME_WAIT for ip: {ip} ");
    Console.WriteLine();
}