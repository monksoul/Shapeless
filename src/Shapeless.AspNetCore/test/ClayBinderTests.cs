// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless.AspNetCore.Tests;

public class ClayBinderTests
{
    [Fact]
    public async Task TryReadAndConvertBodyToClayAsync_Invalid_Parameters()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddControllers().AddClayOptions(options =>
        {
            options.KeyValueJsonToObject = true;
        });

        await using var app = builder.Build();
        var clayOptions = app.Services.GetRequiredService<IOptions<ClayOptions>>().Value;

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await ClayBinder.TryReadAndConvertBodyToClayAsync(null!, clayOptions, CancellationToken.None));
    }

    [Fact]
    public async Task TryReadAndConvertBodyToClayAsync_ReturnOK()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddControllers().AddClayOptions(options =>
        {
            options.KeyValueJsonToObject = true;
        });

        await using var app = builder.Build();
        var clayOptions = app.Services.GetRequiredService<IOptions<ClayOptions>>().Value;

        var utf8Bytes = "{\"id\":1,\"name\":\"furion\"}"u8.ToArray();
        using var memoryStream = new MemoryStream(utf8Bytes);

        var (canParse, model) =
            await ClayBinder.TryReadAndConvertBodyToClayAsync(memoryStream, clayOptions, CancellationToken.None);
        Assert.True(canParse);
        Assert.NotNull(model);
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", model.ToJsonString());

        using var memoryStream2 = new MemoryStream([]);
        var (canParse2, model2) =
            await ClayBinder.TryReadAndConvertBodyToClayAsync(memoryStream2, clayOptions, CancellationToken.None);
        Assert.False(canParse2);
        Assert.Null(model2);
    }
}