namespace Shapeless.Samples.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class GetStartController
{
    /// <summary>
    ///     创建单一对象
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public Clay SingleObject()
    {
        // 创建空的单一对象
        dynamic clay = new Clay(); // 或使用 Clay.EmptyObject(); 或 new Clay.Object();

        // 属性或索引方式设置值
        clay.Id = 1;
        clay["Name"] = "Shapeless";
        clay.IsDynamic = true;
        clay.IsArray = false;

        // 设置匿名对象
        clay.sub = new
        {
            HomePage = new[] { "https://furion.net", "https://baiqian.com" }
        };
        // 继续添加数组内容
        clay.sub.HomePage[2] = "https://baiqian.ltd"; // 使用索引方式
        clay.sub.HomePage.Add("https://百签.com"); // 使用 Add 方法

        // 嵌套设置流变对象
        clay.extend = new Clay();
        clay.extend.username = "MonkSoul";

        // 删除 IsArray 属性
        clay.Remove("IsArray"); // 或使用 clay.Delete("IsArray")

        // 支持输出字符串格式化：U（取消中文 Unicode 编码）
        Console.WriteLine($"{clay:U}"); // 或使用 clay.ToString("U");

        // C：输出小驼峰键命名；P：输出帕斯卡（大驼峰）键命名
        Console.WriteLine($"{clay:UC}"); // 或使用 clay.ToString("UC");
        Console.WriteLine($"{clay:UP}"); // 或使用 clay.ToString("UP");

        return clay;
    }

    [HttpGet]
    public Clay ArrayCollection()
    {
        // 创建空的集合/数组
        dynamic clay = new Clay(ClayType.Array); // 或使用 Clay.EmptyArray(); 或 new Clay.Array();

        // 追加项
        clay.Add(1); // 或使用 clay.Push(1);
        clay.Add(true);
        clay.Add("Furion");
        clay.Add(false);

        // 追加对象
        clay.Add(new { id = 1, name = "Furion" });

        // 追加流变对象
        clay.Add(Clay.Parse("{\"id\":2,\"name\":\"shapeless\"}"));

        // 批量追加项
        clay.AddRange(new object[] { 2, 3, "will be deleted" });

        // 修改项
        clay[0] += 1; // 或使用 clay.Set(0, 2);

        // 在索引为 1 处插入
        clay.Insert(1, "Insert");

        // 在索引为 2 处批量插入
        clay.InsertRange(2, new object[] { "Furion", "Sundial", "Jaina", "TimeCrontab", "HttpAgent" });

        // 删除项
        clay.Remove(4); // 或使用 clay.Delete(4)

        // 删除末项
        clay.Pop();

        // 输出字符串
        Console.WriteLine(clay); // 或使用 clay.ToString();

        return clay;
    }

    [HttpGet]
    public Clay ParseJson()
    {
        // 从 JSON 对象字符串创建
        dynamic clay = Clay.Parse("""{"id":1,"name":"Furion"}""");
        var id = clay.id; // 1
        var name = clay["name"]; // "Furion"
        clay.age = 30;

        Console.WriteLine(clay);

        // 从 JSON 数组字符串创建
        dynamic array = Clay.Parse("[1,2,3,true,\"Furion\"]");
        array.Add(false);
        array.Add(clay);

        Console.WriteLine(array);

        // 从任意 JSON 字面量字符串创建
        dynamic any = Clay.Parse("true");
        Console.WriteLine(any);

        // 自定义包装字面量的键名
        dynamic custom = Clay.Parse("true", new ClayOptions
        {
            ScalarValueKey = "value"
        });
        Console.WriteLine(custom);

        // 从 JSON 字典格式字符串创建
        dynamic dicObject = Clay.Parse("""
                                       [
                                         {
                                           "key": "id",
                                           "value": 1
                                         },
                                         {
                                           "key": "name",
                                           "value": "Furion"
                                         }
                                       ]
                                       """, useObjectForDictionaryJson: true);
        Console.WriteLine(dicObject);

        return Clay.Parse(new { clay, array, any, custom, dicObject });
    }

    [HttpGet]
    public Clay ParseObject()
    {
        // 从现有的对象创建
        dynamic clay1 = Clay.Parse(new Model { Id = 1, Name = "Shapeless" });

        // 从匿名对象创建
        dynamic clay2 = Clay.Parse(new { id = 1, name = "Furion" });

        // 从字典对象创建
        dynamic clay3 = Clay.Parse(new Dictionary<string, object> { { "id", 1 }, { "name", "Furion" } });

        // 从集合/数组创建
        dynamic clay4 = Clay.Parse(new List<Model>
            { new() { Id = 1, Name = "Furion" }, new() { Id = 2, Name = "Shapeless" } });

        // 从 Byte[] 中创建
        dynamic clay5 = Clay.Parse("{\"id\":1,\"name\":\"furion\"}"u8.ToArray());

        // 从 Stream 中创建
        using var memoryStream = new MemoryStream("{\"id\":1,\"name\":\"furion\"}"u8.ToArray());
        dynamic clay6 = Clay.Parse(memoryStream);

        // 从 Utf8JsonReader 中创建
        var utf8JsonReader = new Utf8JsonReader("{\"id\":1,\"name\":\"furion\"}"u8.ToArray(), true, default);
        var clay7 = Clay.Parse(ref utf8JsonReader);

        // 从 Clay 中创建
        dynamic dyn = new Clay();
        dyn.Id = 1;
        dyn.name = "Shapeless";
        var clay8 = Clay.Parse(dyn);

        // 从任意对象（支持序列化的类型）中创建
        var clay9 = Clay.Parse(true);

        // 通过自定义 JsonConverter 创建，如 DataTable 转换为流变对象
        var dataTable = new DataTable();
        dataTable.Columns.Add("id", typeof(int));
        dataTable.Columns.Add("name", typeof(string));
        dataTable.Rows.Add(1, "Furion");
        dataTable.Rows.Add(2, "百小僧");

        var clay10 = Clay.Parse(dataTable,
            new ClayOptions().Configure(options =>
                options.JsonSerializerOptions.Converters.Add(new DataTableJsonConverter())));

        // 打印 JSON
        Console.WriteLine(
            $"{Clay.Parse(new { clay1, clay2, clay3, clay4, clay5, clay6, clay7, clay8, clay9, clay10 }):U}");

        return Clay.Parse(new { clay1, clay2, clay3, clay4, clay5, clay6, clay7, clay8, clay9, clay10 });
    }

    [HttpGet]
    public Clay MoreUsage()
    {
        dynamic clay = Clay.Parse("""{"id":1,"name":"shapeless"}""");

        // 添加属性
        clay.author = "百小僧";
        clay["company"] = "百签科技";
        clay.homepage = new[] { "https://furion.net/", "https://baiqian.com" };
        clay.number = 10;

        // 添加方法
        clay.sayHello = (Func<string>)(() => $"Hello, {clay.name}!");
        clay.Increment = new Action(() => { clay.number++; });

        // 调用方法
        Console.WriteLine(clay.number); // number: 10
        clay.Increment();
        Console.WriteLine(clay.number); // number: 11

        // 打印 JSON
        Console.WriteLine($"{clay.sayHello()}\r\n{clay:U}");

        return clay;
    }

    [HttpGet]
    public void Foreach()
    {
        // ===================== 遍历单一对象 =====================

        dynamic clay = Clay.Parse("""{"id":1,"name":"shapeless"}""");

        // 遍历项（Key 为 Object）
        foreach (KeyValuePair<object, dynamic?> item in clay) // 或使用 clay.AsEnumerable()
            Console.WriteLine($"Key: {item.Key} Value: {item.Value}");

        // 遍历项（Key 为 String）
        foreach (KeyValuePair<string, dynamic?> item in clay.AsEnumerableObject())
            Console.WriteLine($"Key: {item.Key} Value: {item.Value}");

        // 遍历键
        foreach (var key in clay.Keys) // 或使用 clay.Indexes
            Console.WriteLine($"Key: {key}");

        // 遍历值
        foreach (var value in clay.Values) Console.WriteLine($"Value: {value}");

        // 游标方式
        using IEnumerator<KeyValuePair<object, dynamic?>> objectEnumerator = clay.GetEnumerator();

        var listObject = new List<KeyValuePair<object, dynamic?>>();
        while (objectEnumerator.MoveNext()) listObject.Add(objectEnumerator.Current);

        Debug.Assert(listObject.Count == 2);

        // ===================== 遍历集合/数组 =====================

        dynamic array = Clay.Parse("""[1,2,true,false,"Furion",{"id":1,"name":"shapeless"},null]""");

        // 遍历项（Key 为 Object）
        foreach (KeyValuePair<object, dynamic?> item in array) // 或使用 clay.AsEnumerable()
            Console.WriteLine($"Index: {item.Key} Value: {item.Value}");

        // 遍历项（Key 为 Int）
        foreach (KeyValuePair<int, dynamic?> item in array.AsEnumerableArray())
            Console.WriteLine($"Index: {item.Key} Value: {item.Value}");

        // 遍历索引
        foreach (var index in array.Keys) // 或使用 clay.Indexes
            Console.WriteLine($"Index: {index}");

        // 遍历值
        foreach (var value in array.Values) Console.WriteLine($"Value: {value}");

        // 游标方式
        using IEnumerator<KeyValuePair<object, dynamic?>> arrayEnumerator = array.GetEnumerator();

        var listArray = new List<KeyValuePair<object, dynamic?>>();
        while (arrayEnumerator.MoveNext()) listArray.Add(objectEnumerator.Current);

        Debug.Assert(listArray.Count == 7);
    }
}