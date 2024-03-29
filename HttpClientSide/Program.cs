/*
 * 可以使用以下命令查看端口占用情况
 * netstat -na | find "7016"
 * 
 * 或開啟 PowerShell 執行以下指令來持續觀察（每秒更新一次）
 * while ($true) { cls; Write-Output "Current: $(Get-Date)"; netstat -na | Select-String "7016"; Start-Sleep -Seconds 1 }
 */
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHttpClient();
        services.AddHostedService<Startup>();
    });
var host = builder.Build();
host.Run();

class Startup(IHttpClientFactory httpClientFactory, HttpClient httpClient) : IHostedService
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly HttpClient _httpClient = httpClient;
    const string url = "https://localhost:7016/WeatherForecast";
    const int count = 100;
    private static HttpClient _staticHttpClient = new HttpClient();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Version 1: new a HttpClient instance every time
        //TestHttpClientVersion1(url);
        // Version 2: use using statement to dispose HttpClient instance
        //TestHttpClientVersion2(url);
        // Version 3: use static HttpClient and reuse it
        //TestHttpClientVersion3(url);
        // Version 4: inject HttpClient instance and reuse it
        //TestHttpClientVersion4(_httpClient,url);
        // Version 5: inject HttpClientFactory and create HttpClient instance by it
        //TestHttpClientVersion5(_httpClientFactory, url);

        return Task.CompletedTask;

        void TestHttpClientVersion1(string url)
        {
            for (var i = 0; i < count; i++)
            {
                var httpClient = new HttpClient();
                httpClient.GetAsync(url).GetAwaiter().GetResult();
                Console.WriteLine(i);
                Thread.Sleep(0_100);
            }
        }

        void TestHttpClientVersion2(string url)
        {
            for (var i = 0; i < count; i++)
            {
                using var httpClient = new HttpClient();
                httpClient.GetAsync(url).GetAwaiter().GetResult();
                Console.WriteLine(i);
                Thread.Sleep(0_100);
            }
        }

        void TestHttpClientVersion3(string url)
        {
            for (var i = 0; i < count; i++)
            {
                _staticHttpClient.GetAsync(url).GetAwaiter().GetResult();
                Console.WriteLine(i);
                Thread.Sleep(0_100);
            }
        }

        void TestHttpClientVersion4(HttpClient httpClient, string url)
        {
            for (var i = 0; i < count; i++)
            {
                httpClient.GetAsync(url).GetAwaiter().GetResult().Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Console.WriteLine(i);
                Thread.Sleep(200_000);
            }
        }

        void TestHttpClientVersion5(IHttpClientFactory httpClientFactory, string url)
        {
            for (var i = 0; i < count; i++)
            {
                httpClientFactory.CreateClient()
                    .GetAsync(url).GetAwaiter().GetResult()
                    .Content
                    .ReadAsStringAsync().GetAwaiter().GetResult();
                Console.WriteLine(i);
                Thread.Sleep(0_100);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
