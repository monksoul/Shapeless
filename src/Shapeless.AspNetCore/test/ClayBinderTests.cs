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
            await ClayBinder.TryReadAndConvertBodyToClayAsync(null!, clayOptions, false, CancellationToken.None));
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
            await ClayBinder.TryReadAndConvertBodyToClayAsync(memoryStream, clayOptions, false, CancellationToken.None);
        Assert.True(canParse);
        Assert.NotNull(model);
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", model.ToJsonString());

        using var memoryStream2 = new MemoryStream([]);
        var (canParse2, model2) =
            await ClayBinder.TryReadAndConvertBodyToClayAsync(memoryStream2, clayOptions, false,
                CancellationToken.None);
        Assert.False(canParse2);
        Assert.Null(model2);

        using var memoryStream3 = new MemoryStream(
            "IsDeviceEnable=0&Thresholdvalue=1&DeviceState=0&SurplusParams=&DeviceName=2%E5%B1%82%E8%8C%B6%E6%A5%BC%E7%83%AD%E6%B0%B4%E7%94%A8%E6%B0%B4%E9%87%8F6789&Concentrator=476103385&ProtocolId=888085307&BuildId=524328523&DeviceId=489414407&DeviceCode=6789"u8
                .ToArray());
        var (canParse3, model3) =
            await ClayBinder.TryReadAndConvertBodyToClayAsync(memoryStream3, clayOptions, true,
                CancellationToken.None);
        Assert.True(canParse3);
        Assert.NotNull(model3);
        Assert.Equal(
            "{\"IsDeviceEnable\":\"0\",\"Thresholdvalue\":\"1\",\"DeviceState\":\"0\",\"SurplusParams\":\"\",\"DeviceName\":\"2层茶楼热水用水量6789\",\"Concentrator\":\"476103385\",\"ProtocolId\":\"888085307\",\"BuildId\":\"524328523\",\"DeviceId\":\"489414407\",\"DeviceCode\":\"6789\"}",
            model3.ToJsonString());
    }
}