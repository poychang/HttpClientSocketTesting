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
    private static HttpClient _staticHttpClient = new HttpClient();
    private readonly HttpClient _httpClient = httpClient;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    private static Instance _instance = new Instance(new HttpClient());

    const string url = "https://localhost:7016/WeatherForecast";
    const int count = 10;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Scenario 1: new a HttpClient instance every time
        //TestHttpClientScenario1(url);
        // Scenario 2: use using statement to dispose HttpClient instance
        //TestHttpClientScenario2(url);
        // Scenario 3: use static HttpClient and reuse it
        //TestHttpClientScenario3(url);
        // Scenario 4: inject HttpClient instance and reuse it
        //TestHttpClientScenario4(_httpClient, url);
        // Scenario 5: inject HttpClientFactory and create HttpClient instance by it
        //TestHttpClientScenario5(_httpClientFactory, url);
        // Scenario 6: use static instance with new HttpClient instance
        //TestHttpClientScenario6(url);

        return Task.CompletedTask;

        void TestHttpClientScenario1(string url)
        {
            for (var i = 0; i < count; i++)
            {
                var httpClient = new HttpClient();
                httpClient.GetAsync(url).GetAwaiter().GetResult();
                Console.WriteLine(i);
                Thread.Sleep(0_100);
            }
        }

        void TestHttpClientScenario2(string url)
        {
            for (var i = 0; i < count; i++)
            {
                using var httpClient = new HttpClient();
                httpClient.GetAsync(url).GetAwaiter().GetResult();
                Console.WriteLine(i);
                Thread.Sleep(0_100);
            }
        }

        void TestHttpClientScenario3(string url)
        {
            for (var i = 0; i < count; i++)
            {
                _staticHttpClient.GetAsync(url).GetAwaiter().GetResult();
                Console.WriteLine(i);
                Thread.Sleep(0_100);
            }
        }

        void TestHttpClientScenario4(HttpClient httpClient, string url)
        {
            for (var i = 0; i < count; i++)
            {
                httpClient.GetAsync(url).GetAwaiter().GetResult().Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Console.WriteLine(i);
                Thread.Sleep(0_100);
            }
        }

        void TestHttpClientScenario5(IHttpClientFactory httpClientFactory, string url)
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

        void TestHttpClientScenario6(string url)
        {

            for (var i = 0; i < count; i++)
            {
                _instance.Client.GetAsync(url).GetAwaiter().GetResult();
                Console.WriteLine(i);
                Thread.Sleep(0_100);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

public class Instance(HttpClient client)
{
    public HttpClient Client { get; } = client;
}