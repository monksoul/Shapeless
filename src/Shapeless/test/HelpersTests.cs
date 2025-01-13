// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless.Tests;

public class HelpersTests
{
    [Fact]
    public void DeserializeNode_Invalid_Parameter()
    {
        var jsonObject = new JsonObject();
        Assert.Throws<ArgumentNullException>(() => Helpers.DeserializeNode(jsonObject, null!));
    }

    [Fact]
    public void DeserializeNode_ReturnOK()
    {
        JsonNode? jsonNode = null;
        Assert.Null(Helpers.DeserializeNode(jsonNode, typeof(object)));

        var jsonNode1 = JsonNode.Parse("""{"id":1,"name":"furion"}""");
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", Helpers.DeserializeNode(jsonNode1, typeof(string)));

        var jsonNode2 = Helpers.DeserializeNode(jsonNode1, typeof(JsonNode)) as JsonNode;
        Assert.NotNull(jsonNode2);
        Assert.Equal(1, jsonNode2["id"]?.GetValue<int>());
        Assert.Equal("furion", jsonNode2["name"]?.GetValue<string>());

        var jsonNode3 = Helpers.DeserializeNode(jsonNode1, typeof(JsonObject)) as JsonObject;
        Assert.NotNull(jsonNode3);
        Assert.Equal(1, jsonNode3["id"]?.GetValue<int>());
        Assert.Equal("furion", jsonNode3["name"]?.GetValue<string>());

        var jsonDocument = Helpers.DeserializeNode(jsonNode1, typeof(JsonDocument)) as JsonDocument;
        Assert.NotNull(jsonDocument);
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", jsonDocument.RootElement.GetRawText());
        jsonDocument.Dispose();

        var xElement = Helpers.DeserializeNode(jsonNode1, typeof(XElement));
        Assert.NotNull(xElement);
        Assert.Equal(
            "<root type=\"object\">\r\n  <id type=\"number\">1</id>\r\n  <name type=\"string\">furion</name>\r\n</root>",
            xElement.ToString());

        // 注意这里需要配置序列化
        var objectModel =
            Helpers.DeserializeNode(jsonNode1, typeof(ObjectModel),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) as
                ObjectModel;
        Assert.NotNull(objectModel);
        Assert.Equal(1, objectModel.Id);
        Assert.Equal("furion", objectModel.Name);

        var object1 = Helpers.DeserializeNode(jsonNode1, typeof(object));
        Assert.NotNull(object1);
        Assert.True(object1 is JsonElement);
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", ((JsonElement)object1).GetRawText());

        var object2 = Helpers.DeserializeNode(jsonNode1, typeof(JsonElement));
        Assert.NotNull(object2);
        Assert.True(object2 is JsonElement);
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", ((JsonElement)object2).GetRawText());

        var dictionary = Helpers.DeserializeNode(jsonNode1, typeof(Dictionary<string, object>));
        Assert.NotNull(dictionary);
        Assert.True(dictionary is Dictionary<string, object>);
        var dictionary2 = (Dictionary<string, object>)dictionary;
        Assert.Equal("1", dictionary2["id"].ToString());
        Assert.Equal("furion", dictionary2["name"].ToString());

        var jsonValue = JsonNode.Parse("10");
        Assert.Equal(10, Helpers.DeserializeNode(jsonValue, typeof(int)));
        Assert.Equal("10", Helpers.DeserializeNode(jsonValue, typeof(string)));

        var jsonValue2 = JsonNode.Parse("true");
        Assert.Equal(true, Helpers.DeserializeNode(jsonValue2, typeof(bool)));

        var jsonValue3 = JsonNode.Parse("false");
        Assert.Equal(false, Helpers.DeserializeNode(jsonValue3, typeof(bool)));
        Assert.Equal("false", Helpers.DeserializeNode(jsonValue3, typeof(string)));

        var jsonValue4 = JsonNode.Parse("10.1");
        Assert.Equal(10.1, Helpers.DeserializeNode(jsonValue4, typeof(double)));

        var jsonArray = JsonNode.Parse("""["monksoul","百小僧","furion"]""");
        var stringArray = Helpers.DeserializeNode(jsonArray, typeof(string[])) as string[];
        Assert.NotNull(stringArray);
        Assert.Equal(["monksoul", "百小僧", "furion"], stringArray);

        var objectModel2 =
            Helpers.DeserializeNode(jsonNode1, typeof(ObjectModel),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) as ObjectModel;
        Assert.NotNull(objectModel2);
        Assert.Equal(1, objectModel2.Id);
        Assert.Equal("furion", objectModel2.Name);

        var clay = Helpers.DeserializeNode(jsonNode1, typeof(Clay)) as Clay;
        Assert.NotNull(clay);
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", clay.ToJsonString());
    }
}

file class ObjectModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
}