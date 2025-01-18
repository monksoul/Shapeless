// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless.Tests;

public class ClayEnumerableTests(ITestOutputHelper output)
{
    [Fact]
    public void Count_ReturnOK()
    {
        dynamic clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        Assert.Equal(2, clay.Count);

        var clay2 = Clay.Parse("[1,2,3]");
        Assert.Equal(3, clay2.Count);
    }

    [Fact]
    public void Length_ReturnOK()
    {
        dynamic clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        Assert.Equal(2, clay.Length);

        var clay2 = Clay.Parse("[1,2,3]");
        Assert.Equal(3, clay2.Length);
    }

    [Fact]
    public void IsEmpty_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        Assert.False(clay.IsEmpty);
        clay.Clear();
        Assert.True(clay.IsEmpty);

        var clay2 = Clay.Parse("[1,2,3]");
        Assert.False(clay2.IsEmpty);
        clay2.Clear();
        Assert.True(clay2.IsEmpty);
    }

    [Fact]
    public void Keys_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        Assert.Equal(["id", "name"], clay.Keys);

        var clay2 = Clay.Parse("[1,2,3]");
        Assert.Equal([0, 1, 2], clay2.Keys);
    }

    [Fact]
    public void Values_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        Assert.Equal([1, "furion"], clay.Values);

        var clay2 = Clay.Parse("[1,2,3]");
        Assert.Equal([1, 2, 3], clay2.Values);
    }

    [Fact]
    public void GetEnumerator_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        using var objectEnumerator = clay.GetEnumerator();

        var list = new List<KeyValuePair<object, dynamic?>>();
        while (objectEnumerator.MoveNext())
        {
            list.Add(objectEnumerator.Current);
        }

        Assert.Equal(2, list.Count);
        Assert.Equal("id", list[0].Key);
        Assert.Equal(1, list[0].Value);
        Assert.Equal("name", list[1].Key);
        Assert.Equal("furion", list[1].Value);

        var clay2 = Clay.Parse("[1,2,3]");
        using var arrayEnumerator = clay2.GetEnumerator();

        var list2 = new List<KeyValuePair<object, dynamic?>>();
        while (arrayEnumerator.MoveNext())
        {
            list2.Add(arrayEnumerator.Current);
        }

        Assert.Equal(3, list2.Count);
        Assert.Equal(0, list2[0].Key);
        Assert.Equal(1, list2[0].Value);
        Assert.Equal(1, list2[1].Key);
        Assert.Equal(2, list2[1].Value);
        Assert.Equal(2, list2[2].Key);
        Assert.Equal(3, list2[2].Value);

        Assert.Equal(["id", "name"], clay.Select(u => u.Key).ToList());
        Assert.Equal([0, 1, 2], clay2.Select(u => u.Key).ToList());

        dynamic clay3 = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        foreach (KeyValuePair<object, dynamic?> item in clay3)
        {
            output.WriteLine(item.Key.ToString());
        }

        dynamic clay4 = Clay.Parse("[1,2,3]");
        foreach (KeyValuePair<object, dynamic?> item in clay4)
        {
            output.WriteLine(item.Key.ToString());
        }
    }

    [Fact]
    public void AsEnumerable_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        var keyValuePairs = clay.AsEnumerable();
        Assert.Equal(["id", "name"], keyValuePairs.Select(u => u.Key).ToList());

        var clay2 = Clay.Parse("[1,2,3]");
        var keyValuePairs2 = clay2.AsEnumerable();
        Assert.Equal([1, 2, 3], keyValuePairs2.Select(u => u.Value).ToList());
    }

    [Fact]
    public void AsEnumerableObject_Invalid_Parameters()
    {
        var clay = Clay.Parse("[1,2,3]");
        var exception = Assert.Throws<NotSupportedException>(() => clay.AsEnumerableObject().ToList());
        Assert.Equal("`AsEnumerableObject` method can only be used for single object operations.", exception.Message);
    }

    [Fact]
    public void AsEnumerableObject_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        foreach (var item in clay.AsEnumerableObject())
        {
            output.WriteLine(item.Key);
        }
    }

    [Fact]
    public void AsEnumerableArray_Invalid_Parameters()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        var exception = Assert.Throws<NotSupportedException>(() => clay.AsEnumerableArray().ToList());
        Assert.Equal("`AsEnumerableArray` method can only be used for array or collection operations.",
            exception.Message);
    }

    [Fact]
    public void AsEnumerableArray_ReturnOK()
    {
        var clay = Clay.Parse("[1,2,3]");
        foreach (var item in clay.AsEnumerableArray())
        {
            output.WriteLine(item.Key.ToString());
        }
    }

    [Fact]
    public void ForEach_Invalid_Parameters()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        Assert.Throws<ArgumentNullException>(() => clay.ForEach((Action<dynamic>)null!));
        Assert.Throws<ArgumentNullException>(() => clay.ForEach((Action<object, dynamic>)null!));
    }

    [Fact]
    public void ForEach_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        clay.ForEach(item =>
        {
            output.WriteLine($"Value:{item}");
        });

        clay.ForEach((key, item) =>
        {
            output.WriteLine($"Key: {key}, Value:{item}");
        });
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", clay.ToJsonString());

        var array = Clay.Parse("[1,2,3]");
        array.ForEach(item =>
        {
            output.WriteLine($"Value:{item}");
        });

        array.ForEach((index, item) =>
        {
            output.WriteLine($"Index: {index}, Value:{item}");
        });

        Assert.Equal("[1,2,3]", array.ToJsonString());
    }
}