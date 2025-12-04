// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless.Tests;

public class ClayJsonConverterTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var converter = new ClayJsonConverter();
        Assert.NotNull(converter);
        Assert.Null(converter.Options);
    }

    [Fact]
    public void Read_Invalid_Parameters()
    {
        const string json = "{\"id\":1,\"name\":\"furion\"}";
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Clay>(json));
    }

    [Fact]
    public void Read_ReturnOK()
    {
        const string json = "{\"id\":1,\"name\":\"furion\"}";
        var jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerOptions.Default);
        jsonSerializerOptions.Converters.Add(new ClayJsonConverter());

        dynamic clay = JsonSerializer.Deserialize<Clay>(json, jsonSerializerOptions)!;
        Assert.NotNull(clay);
        Assert.Equal(1, clay.id);
        Assert.Equal("furion", clay.name);

        const string json2 = "\"furion\"";
        dynamic clay2 = JsonSerializer.Deserialize<Clay>(json2, jsonSerializerOptions)!;
        Assert.NotNull(clay2);
        Assert.Equal("furion", clay2.value);
    }

    [Fact]
    public void Write_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");

        Assert.Equal("[{\"Key\":\"id\",\"Value\":1},{\"Key\":\"name\",\"Value\":\"furion\"}]",
            JsonSerializer.Serialize(clay));

        var jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerOptions.Default)
        {
            PropertyNameCaseInsensitive = true
        };
        jsonSerializerOptions.Converters.Add(new ClayJsonConverter());

        var json = JsonSerializer.Serialize(clay, jsonSerializerOptions);
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", json);
    }
}