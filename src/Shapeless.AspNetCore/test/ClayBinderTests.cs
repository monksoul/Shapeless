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

        using var memoryStream3 = new MemoryStream(
            "IsDeviceEnable=0&Thresholdvalue=1&DeviceState=0&SurplusParams=&DeviceName=2%E5%B1%82%E8%8C%B6%E6%A5%BC%E7%83%AD%E6%B0%B4%E7%94%A8%E6%B0%B4%E9%87%8F6789&Concentrator=476103385&ProtocolId=888085307&BuildId=524328523&DeviceId=489414407&DeviceCode=6789"u8
                .ToArray());
        var (canParse3, model3) =
            await ClayBinder.TryReadAndConvertBodyToClayAsync(memoryStream3, clayOptions, CancellationToken.None);
        Assert.True(canParse3);
        Assert.NotNull(model3);
        Assert.Equal(
            "{\"IsDeviceEnable\":\"0\",\"Thresholdvalue\":\"1\",\"DeviceState\":\"0\",\"SurplusParams\":\"\",\"DeviceName\":\"2层茶楼热水用水量6789\",\"Concentrator\":\"476103385\",\"ProtocolId\":\"888085307\",\"BuildId\":\"524328523\",\"DeviceId\":\"489414407\",\"DeviceCode\":\"6789\"}",
            model3.ToJsonString());

        using var memoryStream4 = new MemoryStream(
            "Id=222824160&GroupId=524328523&Address=%E5%8C%97%E4%BA%AC%E6%B0%B8%E5%85%B4%E8%8A%B1%E5%9B%AD%E5%BE%A1%E7%91%9E%E9%85%92%E5%BA%97%E7%AE%A1%E7%90%86%E6%9C%89%E9%99%90%E5%85%AC%E5%8F%B8%E8%83%BD%E6%BA%90%E7%AE%A1%E7%90%86%E5%B9%B3%E5%8F%B0%2F%E6%B0%B8%E5%85%B4%E8%8A%B1%E5%9B%AD%E5%BE%A1%E6%9C%97%E9%85%92%E5%BA%97%2F%E7%94%A8%E6%B0%B4%E8%AE%A1%E9%87%8F%2F%E5%A4%96%E7%A7%9F%E5%8C%BA%E5%9F%9F%2F%E7%83%AD%E6%B0%B4%2F2%E5%B1%82%2F2%E5%B1%82%E8%8C%B6%E6%A5%BC%E7%94%A8%E6%B0%B4%E9%87%8F&HouseholderName=%E5%A4%8F%E5%88%A9%E8%8A%B3&HouseholderNo=%E6%9D%BE%E6%9C%88%E8%8C%B6%E6%A5%BC%2F%E7%83%AD%E6%B0%B4%2F1183&HouseholderType=20063&Telephone=13800138000%2C13800138000&MethodId=568747067&CreateActor=%E7%B3%BB%E7%BB%9F%E7%AE%A1%E7%90%86%E5%91%98&CreateTime=2025-01-02+15%3A11%3A07&IsRemainingSMSSend=1"u8
                .ToArray());
        var (canParse4, model4) =
            await ClayBinder.TryReadAndConvertBodyToClayAsync(memoryStream4, clayOptions, CancellationToken.None);
        Assert.True(canParse4);
        Assert.NotNull(model4);
        Assert.Equal(
            "{\"Id\":\"222824160\",\"GroupId\":\"524328523\",\"Address\":\"北京永兴花园御瑞酒店管理有限公司能源管理平台/永兴花园御朗酒店/用水计量/外租区域/热水/2层/2层茶楼用水量\",\"HouseholderName\":\"夏利芳\",\"HouseholderNo\":\"松月茶楼/热水/1183\",\"HouseholderType\":\"20063\",\"Telephone\":\"13800138000,13800138000\",\"MethodId\":\"568747067\",\"CreateActor\":\"系统管理员\",\"CreateTime\":\"2025-01-02 15:11:07\",\"IsRemainingSMSSend\":\"1\"}",
            model4.ToJsonString());

        using var memoryStream5 = new MemoryStream(
            "datalist%5B0%5D%5BNumber%5D=1&datalist%5B0%5D%5BF_AvgEnergy%5D=2025&datalist%5B0%5D%5BF_TotalNumber%5D=1&datalist%5B0%5D%5BF_TotalEnergy%5D=7.63&datalist%5B1%5D%5BNumber%5D=2&datalist%5B1%5D%5BF_AvgEnergy%5D=2025&datalist%5B1%5D%5BF_TotalNumber%5D=2&datalist%5B1%5D%5BF_TotalEnergy%5D=7.63"u8
                .ToArray());
        var (canParse5, model5) =
            await ClayBinder.TryReadAndConvertBodyToClayAsync(memoryStream5, clayOptions, CancellationToken.None);
        Assert.True(canParse5);
        Assert.NotNull(model5);
        Assert.Equal(
            "{\"datalist\":[{\"Number\":\"1\",\"F_AvgEnergy\":\"2025\",\"F_TotalNumber\":\"1\",\"F_TotalEnergy\":\"7.63\"},{\"Number\":\"2\",\"F_AvgEnergy\":\"2025\",\"F_TotalNumber\":\"2\",\"F_TotalEnergy\":\"7.63\"}]}",
            model5.ToJsonString());
    }
}