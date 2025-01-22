// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless.Tests;

public class ClayOverrideTests
{
    [Fact]
    public void TryGetMember_ReturnOK()
    {
        dynamic clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");

        Assert.Equal(1, clay.id);
        Assert.Equal("furion", clay.name);
        Assert.Equal(1, clay["id"]);
        Assert.Equal("furion", clay["name"]);

        dynamic clay2 = Clay.Parse("{\"id\":1,\"name\":\"furion\"}",
            new ClayOptions { PropertyNameCaseInsensitive = true });
        Assert.Equal(1, clay2.Id);
        Assert.Equal("furion", clay2.Name);
        Assert.Equal(1, clay2["Id"]);
        Assert.Equal("furion", clay2["Name"]);

        dynamic clay3 = Clay.Parse("{\"id\":1,\"name\":\"furion\",\"child\":{\"id\":10,\"name\":\"百小僧\"}}");
        Assert.Equal(10, clay3.child.id);
        Assert.Equal("百小僧", clay3.child.name);
        Assert.Equal(10, clay3["child"].id);
        Assert.Equal("百小僧", clay3["child"].name);
        Assert.Equal(10, clay3["child"]["id"]);
        Assert.Equal("百小僧", clay3["child"]["name"]);
        Assert.Equal(10, clay3.child["id"]);
        Assert.Equal("百小僧", clay3.child["name"]);

        dynamic clay4 = Clay.Parse("{\"id\":1,\"name\":\"furion\",\"child\":{\"id\":10,\"name\":\"百小僧\"}}",
            new ClayOptions { PropertyNameCaseInsensitive = true });
        Assert.Equal(10, clay4.Child.Id);
        Assert.Equal("百小僧", clay4.Child.Name);
        Assert.Equal(10, clay4["Child"].Id);
        Assert.Equal("百小僧", clay4["Child"].Name);
        Assert.Equal(10, clay4["Child"]["Id"]);
        Assert.Equal("百小僧", clay4["Child"]["Name"]);
        Assert.Equal(10, clay4.Child["Id"]);
        Assert.Equal("百小僧", clay4.Child["Name"]);

        dynamic clay5 = new Clay();
        clay5[""] = "Furion";
        clay5.百小僧 = "monksoul";
        clay5["!@#$%^&*()_+/{}\\;<>?/:'`-"] = "特殊符号";
        clay5.a = "A";
        clay5.@int = 10;
        Assert.Equal("Furion", clay5[""]);
        Assert.Equal("monksoul", clay5.百小僧);
        Assert.Equal("monksoul", clay5["百小僧"]);
        Assert.Equal("特殊符号", clay5["!@#$%^&*()_+/{}\\;<>?/:'`-"]);
        Assert.Equal("A", clay5["a"]);
        Assert.Equal("A", clay5.a);
        Assert.Equal("A", clay5['a']);
        Assert.Equal(10, clay5.@int);
        Assert.Equal(10, clay5["int"]);
        Assert.Throws<KeyNotFoundException>(() => clay5["@int"]);

        dynamic clay6 = new Clay(new ClayOptions { PropertyNameCaseInsensitive = true });
        clay6.Clay = clay;
        Assert.Equal(1, clay6.Clay.id);
        Assert.Equal("furion", clay6.Clay.name);
        Assert.Equal(1, clay6.clay.id);
        Assert.Equal("furion", clay6.clay.name);
        Assert.Equal(1, clay6.clay.Id);
        Assert.Equal("furion", clay6.clay.Name);

        Assert.Equal("furion", clay.name);
        Assert.Throws<KeyNotFoundException>(() => clay.Name);

        dynamic clay7 = new Clay(new ClayOptions { AllowMissingProperty = true });
        // 空传播
        Assert.Null(clay7.none?.test);

        dynamic clay8 = new Clay(new ClayOptions { AllowMissingProperty = true, AutoCreateNestedObjects = true });
        Assert.Null(clay8["none?"].test);
        Assert.Null(clay8.nested?.nested?.nested);
        Assert.Null(clay8["nested?"]["nested?"].nested);

        // 测试递归
        dynamic clay9 = new Clay();
        clay9.id = 1;
        clay9.name = "Furion";
        clay9.nested = clay9;
        Assert.Equal("{\"id\":1,\"name\":\"Furion\",\"nested\":{\"id\":1,\"name\":\"Furion\"}}", clay9.ToJsonString());

        dynamic clay10 = new Clay.Array(new ClayOptions { AllowIndexOutOfRange = true });
        Assert.Null(clay10[0]?[0]?[0]);

        dynamic clay11 = new Clay.Array(new ClayOptions { AllowIndexOutOfRange = true, AutoCreateNestedArrays = true });
        Assert.Null(clay11[0]?[0]?[0]);
        Assert.Null(clay11["0?"]["0?"][0]);
        Assert.Equal("[[[]]]", clay11.ToJsonString());
        clay11["0?"]["0?"][0] = 20;
        Assert.Equal("[[[20]]]", clay11.ToJsonString());
        clay11["0?"]["0?"][3] = 30; // 无效
        Assert.Equal("[[[20]]]", clay11.ToJsonString());

        dynamic clay12 = new Clay.Array(new ClayOptions
        {
            AllowIndexOutOfRange = true, AutoCreateNestedArrays = true, AutoExpandArrayWithNulls = true
        });
        Assert.Null(clay12[0]?[0]?[0]);
        Assert.Null(clay12["0?"]["0?"][0]);
        Assert.Equal("[[[]]]", clay12.ToJsonString());
        clay12["0?"]["0?"][0] = 20;
        Assert.Equal("[[[20]]]", clay12.ToJsonString());
        clay12["0?"]["0?"][3] = 30; // 无效
        Assert.Equal("[[[20,null,null,30]]]", clay12.ToJsonString());
    }

    [Fact]
    public void TrySetMember_ReturnOK()
    {
        dynamic clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        clay.id = 10;
        clay.Id = 100;
        Assert.Equal(10, clay.id);
        Assert.Equal(100, clay.Id);

        clay["id"] = 11;
        clay["Id"] = 101;
        Assert.Equal(11, clay.id);
        Assert.Equal(101, clay.Id);

        dynamic clay2 = Clay.Parse("{\"id\":1,\"name\":\"furion\"}",
            new ClayOptions { PropertyNameCaseInsensitive = true });
        clay2.id = 10;
        clay2.Id = 100;
        Assert.Equal(100, clay2.id);
        Assert.Equal(100, clay2.Id);

        clay2["id"] = 11;
        clay2["Id"] = 101;
        Assert.Equal(101, clay2.id);
        Assert.Equal(101, clay2.Id);

        dynamic clay3 = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        clay3.child = new { Id = 10, Name = "furion" };
        clay3.nested = clay;
        Assert.Equal(10, clay3.child.Id);
        Assert.Equal("furion", clay3.child.Name);
        Assert.Equal(101, clay3.nested.Id);
        Assert.Equal("furion", clay3.nested.name);

        clay3.child.Age = 30;
        Assert.Equal(30, clay3.child.Age);

        clay3["child"].Age = 31;
        Assert.Equal(31, clay3.child.Age);

        clay3["child"]["Age"] = 32;
        Assert.Equal(32, clay3.child.Age);

        clay3.undefined = new { test = "furion" };
        Assert.Equal("furion", clay3.undefined.test);

        dynamic errorClay = new Clay();
        Assert.Throws<KeyNotFoundException>(() => errorClay["null?"].test = "furion");

        dynamic clay4 = new Clay(new ClayOptions { AllowMissingProperty = true, AutoCreateNestedObjects = true });
        clay4["null?"].test = "furion";
        Assert.Equal("furion", clay4.@null.test);

        clay4["nested?"]["nested?"]["nested?"] = "furion";
        Assert.Equal("furion", clay4.nested.nested.nested);

        dynamic clay5 = new Clay();
        clay5[""] = "Furion";
        clay5.百小僧 = "monksoul";
        clay5["!@#$%^&*()_+/{}\\;<>?/:'`-"] = "特殊符号";
        clay5.a = "A";
        clay5.@int = 10;
        Assert.Equal("Furion", clay5[""]);
        Assert.Equal("monksoul", clay5.百小僧);
        Assert.Equal("monksoul", clay5["百小僧"]);
        Assert.Equal("特殊符号", clay5["!@#$%^&*()_+/{}\\;<>?/:'`-"]);
        Assert.Equal("A", clay5["a"]);
        Assert.Equal("A", clay5.a);
        Assert.Equal("A", clay5['a']);
        Assert.Equal(10, clay5.@int);
        Assert.Equal(10, clay5["int"]);
        Assert.Throws<KeyNotFoundException>(() => clay5["@int"]);

        dynamic clay6 = new Clay();
        clay6.json = "{\"id\":1,\"name\":\"furion\",\"arr\":[1,2,3]}";
        Assert.Equal("{\"id\":1,\"name\":\"furion\",\"arr\":[1,2,3]}", clay6.json);

        dynamic clay7 = new Clay();
        clay7.@enum = EnumModel.One;
        Assert.Equal(1, clay7.@enum);
        Assert.Equal(EnumModel.One, clay7.@enum<EnumModel>());

        dynamic clay8 = new Clay();
        dynamic expandoObject = new ExpandoObject();
        expandoObject.id = 1;
        expandoObject.name = "furion";
        clay8.expando = expandoObject;
        Assert.Equal(1, clay8.expando.id);
        Assert.Equal("furion", clay8.expando.name);
    }

    [Fact]
    public void TryGetIndex_ReturnOK()
    {
        dynamic clay = Clay.Parse("[1,2,3]");
        Assert.Equal(1, clay[0]);
        Assert.Equal(2, clay[1]);
        Assert.Equal(3, clay[2]);

        Assert.Throws<ArgumentOutOfRangeException>(() => clay[3]);

        dynamic clay2 = new Clay(ClayType.Array, new ClayOptions { AllowIndexOutOfRange = true });
        Assert.Null(clay2[0]);
        Assert.Null(clay2[1]);
        // 空传播
        Assert.Null(clay2[2]?[0]);

        dynamic clay3 = new Clay(ClayType.Array,
            new ClayOptions { AllowIndexOutOfRange = true, AutoExpandArrayWithNulls = true });
        Assert.Null(clay3[0]);
        Assert.Null(clay3[1]);

        dynamic clay4 = new Clay(ClayType.Array);
        Assert.Throws<ArgumentOutOfRangeException>(() => clay4[0][0]);
        clay4[0] = 0;
        Assert.Throws<RuntimeBinderException>(() => clay4[0][0]);
        clay4[0] = Array.Empty<object>();
        Assert.Throws<ArgumentOutOfRangeException>(() => clay4[0][0]);

        dynamic clay5 = new Clay(ClayType.Array, new ClayOptions { AllowIndexOutOfRange = true });
        Assert.Throws<RuntimeBinderException>(() => clay5[0][0]);
        clay5[0] = 0;
        Assert.Throws<RuntimeBinderException>(() => clay5[0][0]);
        clay5[0] = Array.Empty<object>();
        Assert.Null(clay5[0][0]);

        dynamic clay6 = new Clay(ClayType.Array,
            new ClayOptions { AllowIndexOutOfRange = true, AutoExpandArrayWithNulls = true });
        Assert.Throws<RuntimeBinderException>(() => clay6[0][0]);
        clay6[0] = 0;
        Assert.Throws<RuntimeBinderException>(() => clay6[0][0]);
        clay6[0] = Array.Empty<object>();
        Assert.Null(clay6[0][0]);

        dynamic clay7 = new Clay(ClayType.Array,
            new ClayOptions { AllowIndexOutOfRange = true, AutoCreateNestedArrays = true });
        Assert.Null(clay7["0?"][0]);

        clay7[0] = 0;
        Assert.Throws<RuntimeBinderException>(() => clay7[0][0]);
        clay7[0] = Array.Empty<object>();
        // 以下配置无效
        clay7[0](new ClayOptions
        {
            AllowIndexOutOfRange = true, AutoExpandArrayWithNulls = true, AutoCreateNestedArrays = false
        });
        Assert.Null(clay7[0][0]);

        var clay71 = clay7[0];
        // 以下配置有效
        clay71(new ClayOptions
        {
            AllowIndexOutOfRange = true, AutoExpandArrayWithNulls = true, AutoCreateNestedArrays = false
        });
        Assert.Null(clay71[1]);
    }

    [Fact]
    public void TrySetIndex_ReturnOK()
    {
        dynamic clay = new Clay(ClayType.Array);
        clay[0] = 1;
        clay[1] = new { id = 1, name = "furion" };
        clay[2] = true;

        Assert.Equal("[1,{\"id\":1,\"name\":\"furion\"},true]", clay.ToJsonString());

        clay[1] = 3;
        Assert.Equal("[1,3,true]", clay.ToJsonString());

        dynamic clay2 = new Clay(ClayType.Array);
        Assert.Throws<ArgumentOutOfRangeException>(() => clay2[1]);

        dynamic clay3 = new Clay(ClayType.Array, new ClayOptions { AllowIndexOutOfRange = true });
        clay3[1] = 10;
        Assert.Equal("[]", clay3.ToJsonString());

        dynamic clay4 = new Clay(ClayType.Array,
            new ClayOptions { AllowIndexOutOfRange = true, AutoExpandArrayWithNulls = true });
        clay4[1] = 10;
        Assert.Equal("[null,10]", clay4.ToJsonString());
        clay4[2] = Array.Empty<object>();
        Assert.Equal("[null,10,[]]", clay4.ToJsonString());

        dynamic clay5 = new Clay(ClayType.Array,
            new ClayOptions { AllowIndexOutOfRange = true, AutoExpandArrayWithNulls = true });
        Assert.Throws<RuntimeBinderException>(() => clay5[0][0] = 10);

        dynamic clay6 = new Clay(ClayType.Array,
            new ClayOptions
            {
                AllowIndexOutOfRange = true, AutoExpandArrayWithNulls = true, AutoCreateNestedArrays = true
            });
        clay6["0?"][0] = 10;
        Assert.Equal("[[10]]", clay6.ToJsonString());
        clay6["0?"]["2?"][2] = 10;
        Assert.Equal("[[10,null,[null,null,10]]]", clay6.ToJsonString());
    }

    [Fact]
    public void TryInvoke_ReturnOK()
    {
        dynamic clay = Clay.Parse("{\"id\":1,\"name\":\"furion\",\"author\":\"百小僧\",\"a\":\"A\"}");

        Assert.Equal("{\"id\":1,\"name\":\"furion\",\"author\":\"百小僧\",\"a\":\"A\"}", clay());

        Assert.Equal("{\"id\":1,\"name\":\"furion\",\"author\":\"\\u767E\\u5C0F\\u50E7\",\"a\":\"A\"}",
            clay(new JsonSerializerOptions()));

        Assert.Equal(1, clay("id"));
        Assert.Equal("A", clay('a'));

        dynamic clay2 = Clay.Parse("[1,2,3]");

        Assert.Equal(2, clay2(1));

        Assert.Equal(new[] { 1, 2, 3 }, clay2(typeof(int[])));
        Assert.Equal(new[] { 1, 2, 3 }, clay2(typeof(int[]), new JsonSerializerOptions()));

        Assert.Equal(2, clay2[^2]);

        Assert.Equal("[1,2]", clay2(..^1).ToJsonString());

        // 更改选项
        Assert.Throws<KeyNotFoundException>(() => clay.Id);
        clay(new ClayOptions { PropertyNameCaseInsensitive = true });
        Assert.Equal(1, clay.Id);

        // 更改选项（委托方式）
        clay(new Action<ClayOptions>(options => options.PropertyNameCaseInsensitive = false));
        Assert.Throws<KeyNotFoundException>(() => clay.Id);

        Assert.Throws<RuntimeBinderException>(() => clay(1, 2));
        Assert.Throws<RuntimeBinderException>(() => clay(Array.Empty<object>()));
    }

    [Fact]
    public void TryInvokeMember_ReturnOK()
    {
        dynamic clay = new Clay();
        clay.id = 1;
        clay.name = "furion";
        clay.datetime = "2024-12-26T00:00:00"; // ISO 8601
        clay.datetime_nostandard = "2024-12-26 00:00:00";

        Assert.True(clay.id());
        Assert.False(clay.Id());

        Assert.True(clay.id(typeof(decimal)) is decimal);
        Assert.True(clay.datetime(typeof(DateTime)) is DateTime);
        Assert.True(clay.datetime(typeof(DateTime), new JsonSerializerOptions()) is DateTime);
        Assert.True(clay.datetime(typeof(DateTime), null) is DateTime);

        Assert.True(clay.id<decimal>() is decimal);
        Assert.False(clay.datetime is DateTime);
        Assert.True(clay.datetime<DateTime>() is DateTime);
        Assert.True(clay.datetime<DateTime>(new JsonSerializerOptions()) is DateTime);
        Assert.True(clay.datetime<DateTime>(null) is DateTime);

        var floatId = (float)clay.id;
        Assert.Equal(1, floatId);

        Assert.Throws<RuntimeBinderException>(() => clay.name(1));
        Assert.Throws<RuntimeBinderException>(() => clay.name<string>(1));
        Assert.Throws<RuntimeBinderException>(() => clay.id(typeof(decimal), 1));

        var dateTime = clay.datetime_nostandard(new Func<string?, object?>(u => Convert.ToDateTime(u))) as DateTime?;
        Assert.Equal(2024, dateTime?.Year);
    }

    [Fact]
    public void TryInvokeMember_WithDelegate_ReturnOK()
    {
        dynamic clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");

        clay.sayHello = (Func<string>)(() => $"Hello, {clay.name}!");
        clay.addProp = new Action<string, object?>((key, value) =>
        {
            clay[key] = value;
        });
        clay.Increment = new Action(() => clay.id++);
        clay.wrapper = new Func<string, string>(u => $"__{u}__");
        clay.callThis = new Func<ClayContext, string>(u => $"Hello, {u.Current.name}!");

        var name = clay.sayHello();
        Assert.Equal("Hello, furion!", name);

        clay.addProp("age", 30);
        Assert.Equal("{\"id\":1,\"name\":\"furion\",\"age\":30}", clay.ToJsonString());

        Assert.Equal(1, clay.id);
        clay.Increment();
        Assert.Equal(2, clay.id);

        var wrapper = clay.wrapper("str");
        Assert.Equal("__str__", wrapper);

        var name2 = clay.callThis();
        Assert.Equal("Hello, furion!", name2);
    }

    [Fact]
    public void TryConvert_ReturnOK()
    {
        dynamic clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");

        ClayModel clayModel = clay;
        Assert.Equal(1, clayModel.Id);
        Assert.Equal("furion", clayModel.Name);

        var clayModel2 = (ClayModel)clay;
        Assert.Equal(1, clayModel2.Id);
        Assert.Equal("furion", clayModel2.Name);

        clay(new ClayOptions { ValidateAfterConversion = true });

        Assert.Throws<ValidationException>(() =>
        {
            ClayModel _ = clay;
        });
    }

    [Fact]
    public void DynamicInvokeDelegate_Invalid_Parameters()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
        Assert.Throws<ArgumentNullException>(() => clay.DynamicInvokeDelegate(null, null, out _));
    }

    [Fact]
    public void DynamicInvokeDelegate_ReturnOK()
    {
        var clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");

        Assert.True(clay.DynamicInvokeDelegate(new Action(() => { }), null, out _));
        Assert.True(clay.DynamicInvokeDelegate(new Action<string>(str => { }), ["str"], out _));
        Assert.True(clay.DynamicInvokeDelegate(new Func<string>(() => "Furion"), null, out var result1));
        Assert.Equal("Furion", result1);
        Assert.True(
            clay.DynamicInvokeDelegate(new Func<ClayContext, string>(ctx => ctx.Current.name), [], out var result2));
        Assert.Equal("furion", result2);
    }
}

public class ClayModel
{
    [Range(2, 10)] public int Id { get; set; }

    [Required] [MinLength(3)] public string? Name { get; set; }
}

public enum EnumModel
{
    One = 1
}