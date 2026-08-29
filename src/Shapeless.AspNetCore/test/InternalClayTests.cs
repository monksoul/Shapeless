// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless.AspNetCore.Tests;

public class InternalClayTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var internalClay = CreateClay();
        Assert.NotNull(internalClay);
    }

    [Fact]
    public void Create_ReturnOK()
    {
        var internalClay = CreateClay();
        var clay = internalClay.Create();
        Assert.Equal("{}", clay.ToJsonString());

        var array = internalClay.Create(ClayType.Array);
        Assert.Equal("[]", array.ToJsonString());
    }

    [Fact]
    public void EmptyObject_ReturnOK()
    {
        var internalClay = CreateClay();
        var clay = internalClay.EmptyObject();
        Assert.Equal("{}", clay.ToJsonString());
    }

    [Fact]
    public void EmptyArray_ReturnOK()
    {
        var internalClay = CreateClay();
        var clay = internalClay.EmptyArray();
        Assert.Equal("[]", clay.ToJsonString());
    }

    [Fact]
    public void Parse_ReturnOK()
    {
        var internalClay = CreateClay();
        var clay = internalClay.Parse(new { id = 1, name = "furion" });
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", clay.ToJsonString());

        var utf8JsonReader2 = new Utf8JsonReader("{\"id\":1,\"name\":\"furion\"}"u8.ToArray(), true, default);
        var clay21 = internalClay.Parse(ref utf8JsonReader2);
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", clay21.ToJsonString());
    }

    [Fact]
    public void ParseFromFile_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "json.txt");
        var internalClay = CreateClay();
        var clay = internalClay.ParseFromFile(filePath);
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", clay.ToJsonString());
    }

    [Fact]
    public void GetOptions_ReturnOK()
    {
        var internalClay = CreateClay();
        var clayOptions = internalClay.GetOptions();
        Assert.NotNull(clayOptions);
        Assert.False(clayOptions.KeyValueJsonToObject);

        var clayOptions2 = internalClay.GetOptions(new ClayOptions { KeyValueJsonToObject = true });
        Assert.NotNull(clayOptions2);
        Assert.True(clayOptions2.KeyValueJsonToObject);
    }

    private static InternalClay CreateClay()
    {
        var services = new ServiceCollection();
        services.AddClayOptions();

        using var serviceProvider = services.BuildServiceProvider();
        var clayOptions = serviceProvider.GetRequiredService<IOptionsMonitor<ClayOptions>>();

        var clay = new InternalClay(clayOptions);

        return clay;
    }
}