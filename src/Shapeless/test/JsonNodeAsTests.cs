// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless.Tests;

public class JsonNodeAsTests
{
    [Fact]
    public void As_Invalid_Parameter()
    {
        var jsonObject = new JsonObject();
        Assert.Throws<ArgumentNullException>(() => jsonObject.As(null!));
    }

    [Fact]
    public void As_ReturnOK()
    {
        var options = ClayOptions.Default;

        JsonNode? jsonNode = null;
        Assert.Null(jsonNode.As(typeof(object)));

        var jsonNode1 = JsonNode.Parse("""{"id":1,"name":"furion"}""");
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", jsonNode1.As(typeof(string)));

        var jsonNode2 = jsonNode1.As(typeof(JsonNode)) as JsonNode;
        Assert.NotNull(jsonNode2);
        Assert.Equal(1, jsonNode2["id"]?.GetValue<int>());
        Assert.Equal("furion", jsonNode2["name"]?.GetValue<string>());

        var jsonNode3 = jsonNode1.As(typeof(JsonObject)) as JsonObject;
        Assert.NotNull(jsonNode3);
        Assert.Equal(1, jsonNode3["id"]?.GetValue<int>());
        Assert.Equal("furion", jsonNode3["name"]?.GetValue<string>());

        var jsonDocument = jsonNode1.As(typeof(JsonDocument)) as JsonDocument;
        Assert.NotNull(jsonDocument);
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", jsonDocument.RootElement.GetRawText());
        jsonDocument.Dispose();

        var xElement = jsonNode1.As(typeof(XElement));
        Assert.NotNull(xElement);
        Assert.Equal(
            "<root type=\"object\">\r\n  <id type=\"number\">1</id>\r\n  <name type=\"string\">furion</name>\r\n</root>",
            xElement.ToString());

        // 注意这里需要配置序列化
        var objectModel = jsonNode1.As(typeof(ObjectModel),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) as ObjectModel;
        Assert.NotNull(objectModel);
        Assert.Equal(1, objectModel.Id);
        Assert.Equal("furion", objectModel.Name);

        var object1 = jsonNode1.As(typeof(object), options.JsonSerializerOptions);
        Assert.NotNull(object1);
        Assert.True(object1 is Clay);
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", ((Clay)object1).ToJsonString());

        var object2 = jsonNode1.As(typeof(JsonElement));
        Assert.NotNull(object2);
        Assert.True(object2 is JsonElement);
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", ((JsonElement)object2).GetRawText());

        var dictionary = jsonNode1.As(typeof(Dictionary<string, object>));
        Assert.NotNull(dictionary);
        Assert.True(dictionary is Dictionary<string, object>);
        var dictionary2 = (Dictionary<string, object>)dictionary;
        Assert.Equal("1", dictionary2["id"].ToString());
        Assert.Equal("furion", dictionary2["name"].ToString());

        var jsonValue = JsonNode.Parse("10");
        Assert.Equal(10, jsonValue.As(typeof(int)));
        Assert.Equal("10", jsonValue.As(typeof(string)));

        var jsonValue2 = JsonNode.Parse("true");
        Assert.Equal(true, jsonValue2.As(typeof(bool)));

        var jsonValue3 = JsonNode.Parse("false");
        Assert.Equal(false, jsonValue3.As(typeof(bool)));
        Assert.Equal("false", jsonValue3.As(typeof(string)));

        var jsonValue4 = JsonNode.Parse("10.1");
        Assert.Equal(10.1, jsonValue4.As(typeof(double)));

        var jsonArray = JsonNode.Parse("""["monksoul","百小僧","furion"]""");
        var stringArray = jsonArray.As(typeof(string[])) as string[];
        Assert.NotNull(stringArray);
        Assert.Equal(["monksoul", "百小僧", "furion"], stringArray);

        var objectModel2 = jsonNode1.As(typeof(ObjectModel),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) as ObjectModel;
        Assert.NotNull(objectModel2);
        Assert.Equal(1, objectModel2.Id);
        Assert.Equal("furion", objectModel2.Name);

        var clay = jsonNode1.As(typeof(Clay), options.JsonSerializerOptions) as Clay;
        Assert.NotNull(clay);
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", clay.ToJsonString());

        var jsonValue5 = JsonNode.Parse("\"True\"");
        Assert.Equal(true, jsonValue5.As(typeof(bool)));
        var jsonValue6 = JsonNode.Parse("\"False\"");
        Assert.Equal(false, jsonValue6.As(typeof(bool)));

        var jsonValue7 = JsonNode.Parse("\"2025-01-14T00:00:00\"");
        Assert.Equal(2025, (jsonValue7.As(typeof(DateTime)) as DateTime?)?.Year);
    }
}

file class ObjectModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
}