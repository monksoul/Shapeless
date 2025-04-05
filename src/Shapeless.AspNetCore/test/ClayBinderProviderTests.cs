// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless.AspNetCore.Tests;

public class ClayBinderProviderTests(ITestOutputHelper output)
{
    [Fact]
    public async Task GetBinder_ReturnOK()
    {
        const int port = 6754;
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers().AddJsonOptions(options => { }).AddClayOptions(options =>
        {
            options.KeyValueJsonToObject = true;
        });
        builder.Services.AddHttpClient();

        await using var app = builder.Build();

        app.MapPost("/test", async (HttpContext context, Clay clay) =>
        {
            await context.Response.WriteAsync(clay.ToJsonString());
        });

        app.MapControllers();

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri($"http://localhost:{port}/test"));
        httpRequestMessage.Content =
            new StringContent("{\"id\":1,\"name\":\"furion\"}", Encoding.UTF8,
                new MediaTypeHeaderValue("application/json"));

        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
        var content = await httpResponseMessage.Content.ReadAsStringAsync();
        output.WriteLine(content);
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task PostClayParameter_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(ClayController).Assembly)
            .AddClayOptions();
        builder.Services.AddHttpClient();

        await using var app = builder.Build();
        app.MapControllers();

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpRequestMessage =
            new HttpRequestMessage(HttpMethod.Post, new Uri($"http://localhost:{port}/clay/post1"));
        httpRequestMessage.Content =
            new StringContent("""{"id":1,"name":"Furion"}""", new MediaTypeHeaderValue("application/json"));

        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("""{"id":1,"name":"Furion"}""", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task PostDynamicParameter_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(ClayController).Assembly)
            .AddClayOptions();
        builder.Services.AddHttpClient();

        await using var app = builder.Build();
        app.MapControllers();

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpRequestMessage =
            new HttpRequestMessage(HttpMethod.Post, new Uri($"http://localhost:{port}/clay/post2"));
        httpRequestMessage.Content =
            new StringContent("""{"id":1,"name":"Furion"}""", new MediaTypeHeaderValue("application/json"));

        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("""{"id":1,"name":"Furion"}""", str);

        await app.StopAsync();
    }
}