// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless.Tests;

public class ObjectToClayJsonConverterTests
{
    [Fact]
    public void Read_ReturnOK()
    {
        const string json = "{\"id\":1,\"name\":\"furion\"}";
        var obj = JsonSerializer.Deserialize<object>(json);
        Assert.NotNull(obj);
        Assert.True(obj is JsonElement);

        var jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerOptions.Default);
        jsonSerializerOptions.Converters.Add(new ObjectToClayJsonConverter());

        var obj2 = JsonSerializer.Deserialize<object>(json, jsonSerializerOptions)!;
        Assert.NotNull(obj2);
        Assert.True(obj2 is Clay);

        const string json2 = "[{\"id\":1,\"name\":\"furion\"},{\"id\":1,\"name\":\"furion\"}]";
        var obj3 = JsonSerializer.Deserialize<object>(json2, jsonSerializerOptions)!;
        Assert.NotNull(obj3);
        Assert.True(obj3 is Clay);

        const string json3 = "\"furion\"";
        dynamic obj4 = JsonSerializer.Deserialize<object>(json3, jsonSerializerOptions)!;
        Assert.NotNull(obj4);
        Assert.True(obj4 is JsonElement);
    }

    [Fact]
    public void Write_ReturnOK()
    {
        var obj = new { id = 1, name = "furion" };
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", JsonSerializer.Serialize(obj));

        var jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerOptions.Default);
        jsonSerializerOptions.Converters.Add(new ObjectToClayJsonConverter());

        var json = JsonSerializer.Serialize(obj, jsonSerializerOptions);
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", json);
    }
}