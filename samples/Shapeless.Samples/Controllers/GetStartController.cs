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

        // 属性方式（添加）设置值
        clay.Id = 1;
        clay.Name = "Shapeless";

        // 索引方式（添加）设置值
        clay["IsDynamic"] = true;
        clay["IsArray"] = false;

        // 设置对象或匿名对象
        clay.Author = new
        {
            Nickname = "MonkSoul",
            HomePage = new[] { "https://furion.net", "https://baiqian.com" }
        };

        // 属性或索引或它们组合方式（修改）设置值
        clay.Author.Nickname = "百小僧";
        clay["Author"].Age = 30;
        clay["Author"]["Gender"] = "男";
        clay.Author["E-Mail"] = "monksoul@outlook.com";

        // 使用索引或 Add/Push 方法添加数组项
        clay.Author.HomePage[2] = "https://baiqian.ltd"; // 使用索引方式

        var homePage = clay.Author.HomePage; // 简化 clay.Author.HomePage[2] 操作
        homePage[homePage.Length] = "https://chinadot.net"; // 使用数组长度作为索引
        homePage.Add("https://百签.com"); // 或使用 homePage.Push("https://百签.com");

        // 设置嵌套流变对象
        clay.extend = new Clay();
        clay.extend.username = "MonkSoul";
        clay.extend.gitee = "https://gitee.com/monksoul";

        // 删除（移除）属性
        clay.Remove("IsArray"); // 或使用 clay.Delete("IsArray")

        // 访问并修改属性
        clay.Id += 1;

        // 输出字符串
        Console.WriteLine(clay); // 或使用 clay.ToString();

        // 输出字符串（U：取消中文 Unicode 编码）
        Console.WriteLine($"{clay:U}"); // 或使用 clay.ToString("U");

        // 调用 ToJsonString 方法并设置 JsonSerializerOptions，指定 Encoder 属性
        Console.WriteLine(clay.ToJsonString(new JsonSerializerOptions
            { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));

        // 输出字符串（C：输出小驼峰键命名；P：输出帕斯卡（大驼峰）键命名）
        Console.WriteLine($"{clay:UC}"); // 或使用 clay.ToString("UC");
        Console.WriteLine($"{clay:UP}"); // 或使用 clay.ToString("UP");

        // 调用 ToJsonString 方法并设置 JsonSerializerOptions，指定 WriteIndented 属性
        Console.WriteLine(clay.ToJsonString(new JsonSerializerOptions
        {
            WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));

        // 调用 ToJsonString 方法并设置 JsonSerializerOptions，指定 WriteIndented 属性
        Console.WriteLine(clay.ToJsonString(new JsonSerializerOptions
        {
            WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = new PascalCaseNamingPolicy()
        }));

        // 输出字符串（Z：压缩（取消格式化））
        Console.WriteLine($"{clay:Z}"); // 或使用 clay.ToString("Z");

        // 调用 ToJsonString 方法并设置 JsonSerializerOptions，指定 WriteIndented 属性
        Console.WriteLine(clay.ToJsonString(new JsonSerializerOptions
            { WriteIndented = false }));

        // 组合使用格式化符
        Console.WriteLine($"{clay:ZUC}");

        return clay;
    }

    /// <summary>
    ///     创建集合或数组
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public Clay ArrayCollection()
    {
        // 创建空的集合或数组
        dynamic clay = new Clay(ClayType.Array); // 或使用 Clay.EmptyArray(); 或 new Clay.Array();

        // 追加项
        clay.Add(1); // 或使用 clay.Push(1);
        clay.Add(true);
        clay.Add("Furion");
        clay.Add(false);

        // 追加对象或匿名对象
        clay.Add(new { id = 1, name = "Furion" });

        // 追加流变对象
        clay.Add(Clay.Parse("{\"id\":2,\"name\":\"shapeless\"}"));

        // 批量追加项
        clay.AddRange(new object[] { 2, 3, "will be deleted" });

        // 修改指定索引项
        clay[0] += 1; // 或使用 clay.Set(0, 2);
        // clay[^1] = "Last";  // 或使用 clay.Set(^1, 2);

        // 在索引为 1 处插入
        clay.Insert(1, "Insert");

        // 在索引为 2 处批量插入
        clay.InsertRange(2, new object[] { "Furion", "Sundial", "Jaina", "TimeCrontab", "HttpAgent" });

        // 删除项
        clay.Remove(4); // 或使用 clay.Delete(4)

        // 删除指定索引范围项
        // clay.Remove(1, 4); // 或使用 clay.Delete(1, 4);
        // clay.Remove(1..^4); // clay.Delete(1..^4);

        // 删除末项
        clay.Pop();

        // 输出字符串
        Console.WriteLine(clay); // 或使用 clay.ToString();

        // 反转集合或数组
        var array = clay.Reverse();

        // 输出字符串
        Console.WriteLine(array); // 或使用 array.ToString();

        // 截取数组
        var parts = array[2..^4]; // 或使用 array.Slice(2, 4);

        // 输出字符串
        Console.WriteLine(parts); // 或使用 parts.ToString();

        return clay;
    }

    /// <summary>
    ///     从 JSON 字符串创建
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public Clay ParseJson()
    {
        // 从 JSON 对象字符串创建
        dynamic clay = Clay.Parse("""{"id":1,"name":"Furion"}""");

        // 添加新属性
        clay.age = 30;

        // 访问属性
        var id = clay.id; // 1
        var name = clay["name"]; // "Furion"
        var age = clay.age; // 30

        // 输出字符串
        Console.WriteLine(clay);

        // 从 JSON 数组字符串创建
        dynamic array = Clay.Parse("[1,2,3,true,\"Furion\"]");

        // 追加新项
        array.Add(false);
        array.Add(clay);

        // 访问项
        var first = array[0]; // 1
        var second = array[1]; // 2
        var last = array[^1]; // "{\"id\":1,\"name\":\"Furion\",\"age\":30}"

        // 输出字符串
        Console.WriteLine(array);

        // 从任意 JSON 字面量字符串创建
        dynamic scalar = Clay.Parse("true");

        // 访问值
        var value = scalar.data;

        // 输出字符串
        Console.WriteLine(scalar);

        // 自定义包装字面量的键名
        dynamic wrapper = Clay.Parse("true", new ClayOptions
        {
            ScalarValueKey = "value"
        });

        // 访问值
        var data = wrapper.value;

        // 输出字符串
        Console.WriteLine(wrapper);

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

        // 访问值
        var id0 = dicObject.id; // 1
        var name0 = dicObject.name; // "Furion"

        // 输出字符串
        Console.WriteLine(dicObject);

        return Clay.Parse(new { clay, array, any = scalar, custom = wrapper, dicObject });
    }

    /// <summary>
    ///     从 C# 对象创建
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public Clay ParseObject()
    {
        // 从具体类型对象创建
        dynamic clay1 = Clay.Parse(new YourModel { Id = 1, Name = "Shapeless" });

        // 从匿名对象创建
        dynamic clay2 = Clay.Parse(new { id = 1, name = "Furion" });

        // 从字典对象创建
        dynamic clay3 = Clay.Parse(new Dictionary<string, object> { { "id", 1 }, { "name", "Furion" } });

        // 从键值对集合或数组创建
        dynamic clay4 = Clay.Parse(new[]
        {
            new KeyValuePair<string, object?>("id", 1), new KeyValuePair<string, object?>("name", "furion")
        }.ToDictionary());

        // 从集合或数组（具体类型或匿名类型）创建
        dynamic clay5 = Clay.Parse(new List<YourModel>
            { new() { Id = 1, Name = "Furion" }, new() { Id = 2, Name = "Shapeless" } });

        // 从字节数组（JSON 字符串）创建
        dynamic clay6 = Clay.Parse("{\"id\":1,\"name\":\"furion\"}"u8.ToArray());

        // 从 Stream 流（JSON 字符串）创建
        using var memoryStream = new MemoryStream("{\"id\":1,\"name\":\"furion\"}"u8.ToArray());
        dynamic clay7 = Clay.Parse(memoryStream);

        // 从 Utf8JsonReader 中创建
        var utf8JsonReader = new Utf8JsonReader("{\"id\":1,\"name\":\"furion\"}"u8.ToArray(), true, default);
        dynamic clay8 = Clay.Parse(ref utf8JsonReader);

        // 从 JsonElement 中创建
        using var jsonDocument = JsonDocument.Parse("{\"id\":1,\"name\":\"Furion\"}");
        dynamic clay9 = Clay.Parse(jsonDocument.RootElement);

        // 从流变对象自身创建
        dynamic clay10 = Clay.Parse(new Clay
        {
            ["id"] = 1,
            ["name"] = "Shapeless"
        });

        // 从任意字面量创建
        dynamic clay11 = Clay.Parse(true);

        // 自定义字面量键名
        dynamic clay12 = Clay.Parse(true, new ClayOptions { ScalarValueKey = "value" });

        // 使用自定义 JsonConverter 创建，如 DataTable 转换为流变对象
        var dataTable = new DataTable();
        dataTable.Columns.Add("id", typeof(int));
        dataTable.Columns.Add("name", typeof(string));
        dataTable.Rows.Add(1, "Furion");
        dataTable.Rows.Add(2, "百小僧");

        dynamic clay13 = Clay.Parse(dataTable,
            new ClayOptions().Configure(options =>
                options.JsonSerializerOptions.Converters.Add(new DataTableJsonConverter())));

        // 打印 JSON
        Console.WriteLine(
            $"{Clay.Parse(new { clay1, clay2, clay3, clay4, clay5, clay6, clay7, clay8, clay9, clay10, clay11, clay12, clay13 }):U}");

        return Clay.Parse(new
            { clay1, clay2, clay3, clay4, clay5, clay6, clay7, clay8, clay9, clay10, clay11, clay12, clay13 });
    }

    /// <summary>
    ///     遍历流变对象
    /// </summary>
    [HttpGet]
    public void Foreach()
    {
        // ===================== 单一对象 =====================

        dynamic clay = Clay.Parse("""{"id":1,"name":"shapeless"}""");

        // 遍历键值（object 类型键）
        foreach (KeyValuePair<object, dynamic?> item in clay) // 或使用 clay.AsEnumerable()
        {
            Console.WriteLine($"Key: {item.Key} Value: {item.Value}");
        }

        // 遍历键值（string 类型键）
        foreach (KeyValuePair<string, dynamic?> item in clay.AsEnumerableObject())
        {
            Console.WriteLine($"Key: {item.Key} Value: {item.Value}");
        }

        // 遍历键
        foreach (var key in clay.Keys)
        {
            Console.WriteLine($"Key: {key}");
        }

        // 遍历值
        foreach (var value in clay.Values)
        {
            Console.WriteLine($"Value: {value}");
        }

        // 使用枚举器方式遍历
        using IEnumerator<KeyValuePair<object, dynamic?>> objectEnumerator = clay.GetEnumerator();

        var listObject = new List<KeyValuePair<object, dynamic?>>();
        while (objectEnumerator.MoveNext())
        {
            listObject.Add(objectEnumerator.Current);
        }

        Debug.Assert(listObject.Count == 2);

        // 使用 ForEach 方法遍历值
        clay.ForEach(new Action<dynamic?>(value => { Console.WriteLine($"Value: {value}"); }));

        // 使用 ForEach 方法遍历键值
        clay.ForEach(new Action<object, dynamic?>((key, value) =>
        {
            Console.WriteLine($"Key: {key} Value: {value}");
        }));

        // 享受友好的代码智能完成编程体验
        foreach (var (key, value) in (Clay)clay)
        {
            Console.WriteLine($"Key: {key} Value: {value}");
        }

        // ===================== 集合或数组 =====================

        dynamic array = Clay.Parse("""[1,2,true,false,"Furion",{"id":1,"name":"shapeless"},null]""");

        // 遍历项（object 类型索引）
        foreach (KeyValuePair<object, dynamic?> item in array) // 或使用 clay.AsEnumerable()
        {
            Console.WriteLine($"Index: {item.Key} Value: {item.Value}");
        }

        // 遍历项（int 类型索引）
        foreach (KeyValuePair<int, dynamic?> item in array.AsEnumerableArray())
        {
            Console.WriteLine($"Index: {item.Key} Value: {item.Value}");
        }

        // 遍历索引
        foreach (var index in array.Keys)
        {
            Console.WriteLine($"Index: {index}");
        }

        // 遍历值
        foreach (var value in array.Values)
        {
            Console.WriteLine($"Value: {value}");
        }

        // 使用枚举器方式遍历
        using IEnumerator<KeyValuePair<object, dynamic?>> arrayEnumerator = array.GetEnumerator();

        var listArray = new List<KeyValuePair<object, dynamic?>>();
        while (arrayEnumerator.MoveNext())
        {
            listArray.Add(objectEnumerator.Current);
        }

        Debug.Assert(listArray.Count == 7);

        // 使用 ForEach 方法遍历值
        array.ForEach(new Action<dynamic?>(value => { Console.WriteLine($"Value: {value}"); }));

        // 使用 ForEach 方法遍历索引与值
        array.ForEach(new Action<object, dynamic?>((index, value) =>
        {
            Console.WriteLine($"Index: {index} Value: {value}");
        }));

        // 享受友好的代码智能完成编程体验
        foreach (var (index, value) in (Clay)array)
        {
            Console.WriteLine($"Index: {index} Value: {value}");
        }
    }

    /// <summary>
    ///     单一对象 Lambda 和 Linq 操作
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public object SingleObjectLambdaAndLinq()
    {
        // ===================== 单一对象 =====================

        dynamic clay = Clay.Parse("""{"id":1,"name":"shapeless"}""");

        // Lambda 查询键值（object 类型键）
        var list1 = ((Clay)clay).Where(u => (string)u.Key == "id").OrderBy(u => u.Key).ToList();
        var list2 = ((IEnumerable<KeyValuePair<object, dynamic?>>)clay).Where(u => (string)u.Key == "id")
            .OrderBy(u => u.Key).ToList();

        // Lambda 查询键值（string 类型键）
        var list3 = ((Clay)clay).AsEnumerableObject().Where(u => u.Key == "id").OrderBy(u => u.Key).ToList();

        // Linq 查询键值（object 类型键）
        var query1 = from item in (Clay)clay
            where (string)item.Key == "id"
            orderby item.Key
            select item;

        var list4 = query1.ToList();

        // Linq 查询键值（string 类型键）
        var query2 = from item in ((Clay)clay).AsEnumerableObject()
            where item.Key == "id"
            orderby item.Key
            select item;

        var list5 = query2.ToList();

        // 将 dynamic 对象显式转换回 Clay 类型，简化类型转换操作
        Clay clayObject = clay; // 或使用 var clayObject = (Clay)clay;

        // Lambda 查询键值（object 类型键）
        var list6 = clayObject.Where(u => (string)u.Key == "id").OrderBy(u => u.Key).ToList();

        // Lambda 查询键值（string 类型键）
        var list7 = clayObject.AsEnumerableObject().Where(u => u.Key == "id").OrderBy(u => u.Key).ToList();

        // Linq 查询键值（object 类型键）
        var query3 = from item in clayObject
            where (string)item.Key == "id"
            orderby item.Key
            select item;

        var list8 = query3.ToList();

        // Linq 查询键值（string 类型键）
        var query4 = from item in clayObject.AsEnumerableObject()
            where item.Key == "id"
            orderby item.Key
            select item;

        var list9 = query4.ToList();

        return new { list1, list2, list3, list4, list5, list6, list7, list8, list9 };
    }

    /// <summary>
    ///     集合或数组 Lambda 和 Linq 操作
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public object ArrayCollectionLambdaAndLinq()
    {
        dynamic clay = Clay.Parse("""[1,2,true,false,"Furion",{"id":1,"name":"shapeless"},null]""");

        // Lambda 查询索引与值（object 类型索引）
        var list1 = ((Clay)clay).Where(u => (int)u.Key > 2).OrderBy(u => u.Key).ToList();
        var list2 = ((IEnumerable<KeyValuePair<object, dynamic?>>)clay).Where(u => (int)u.Key > 2)
            .OrderBy(u => u.Key).ToList();

        // Lambda 查询索引与值（int 类型索引）
        var list3 = ((Clay)clay).AsEnumerableArray().Where(u => u.Key > 2).OrderBy(u => u.Key).ToList();

        // Linq 查询索引与值（object 类型索引）
        var query1 = from item in (Clay)clay
            where (int)item.Key > 2
            orderby item.Key
            select item;

        var list4 = query1.ToList();

        // Linq 查询索引与值（int 类型索引）
        var query2 = from item in ((Clay)clay).AsEnumerableArray()
            where item.Key > 2
            orderby item.Key
            select item;

        var list5 = query2.ToList();

        // 将 dynamic 对象显式转换回 Clay 类型，简化类型转换操作
        Clay clayArray = clay; // 或使用 var clayArray = (Clay)clay;

        // Lambda 查询索引与值（object 类型索引）
        var list6 = clayArray.Where(u => (int)u.Key > 2).OrderBy(u => u.Key).ToList();

        // Lambda 查询索引与值（int 类型索引）
        var list7 = clayArray.AsEnumerableArray().Where(u => u.Key > 2).OrderBy(u => u.Key).ToList();

        // Linq 查询索引与值（object 类型索引）
        var query3 = from item in clayArray
            where (int)item.Key > 2
            orderby item.Key
            select item;

        var list8 = query3.ToList();

        // Linq 查询索引与值（int 类型索引）
        var query4 = from item in clayArray.AsEnumerableArray()
            where item.Key > 2
            orderby item.Key
            select item;

        var list9 = query4.ToList();

        return new { list1, list2, list3, list4, list5, list6, list7, list8, list9 };
    }

    /// <summary>
    ///     流变对象的类型转换
    /// </summary>
    [HttpGet]
    public void TypeConvert()
    {
        dynamic clay = Clay.Parse("""{"id":1,"name":"shapeless","date":"2025-01-14T00:00:00","isTrue":true}""");

        // ===================== 单一对象属性及集合或数组元素的类型转换 =====================

        // AutoConvertToDateTime

        // 使用属性名作为方法名，并指定目标类型为泛型参数进行调用
        var id = clay.id; // id 为 int 类型
        var name = clay.name; // name 为 string 类型

        var date = clay.date<DateTime>();
        var isTrue = clay.isTrue<bool>();

        // 支持传入 JSON 序列化选项
        var date2 = clay.date<DateTime>(new JsonSerializerOptions());

        Console.WriteLine(date);
        Console.WriteLine(isTrue);
        Console.WriteLine(date2);

        // 使用属性名作为方法名，并指定目标类型为方法参数进行调用
        var date3 = clay.date(typeof(DateTime)) as DateTime?;
        var isTrue2 = clay.isTrue(typeof(bool)) as bool?;

        // 支持传入 JSON 序列化选项
        var date4 = clay.date(typeof(DateTime), new JsonSerializerOptions()) as DateTime?;

        Console.WriteLine(date3);
        Console.WriteLine(isTrue2);
        Console.WriteLine(date4);

        // 使用流变对象 Get<T>(identifier) 方法
        var date5 = clay.Get<DateTime>("date");
        var isTrue3 = clay.Get<bool>("isTrue");

        // 支持传入 JSON 序列化选项
        var date6 = clay.Get<DateTime>("date", new JsonSerializerOptions());

        Console.WriteLine(date5);
        Console.WriteLine(isTrue3);
        Console.WriteLine(date6);

        // 使用流变对象 Get(type, identifier) 方法
        var date7 = clay.Get("date", typeof(DateTime)) as DateTime?;
        var isTrue4 = clay.Get("isTrue", typeof(bool)) as bool?;

        // 支持传入 JSON 序列化选项
        var date8 = clay.Get("date", typeof(DateTime), new JsonSerializerOptions()) as DateTime?;

        Console.WriteLine(date7);
        Console.WriteLine(isTrue4);
        Console.WriteLine(date8);

        // 使用属性名作为方法名，并传递 Func<string?, object?> 委托为方法参数进行调用
        var date9 = clay.date(new Func<string?, object?>(u => Convert.ToDateTime(u)));
        Console.WriteLine(date9);

        dynamic array = Clay.Parse("[1,\"2025-01-14T00:00:00\",true]");

        var date10 = array.Get<DateTime>(1); // 索引为 1
        var isTrue5 = array.Get<bool>(2); // 索引为 2

        Console.WriteLine(date10);
        Console.WriteLine(isTrue5);

        // 配置 ClayOptions 以自动解析时间格式字符串为 `DateTime` 类型
        dynamic clay2 = Clay.Parse("""{"id":1,"name":"shapeless","date":"2025-01-14T00:00:00","isTrue":true}""",
            new ClayOptions
            {
                AutoConvertToDateTime = true
            });

        // 直接访问 date 属性，它已被自动转换为 DateTime 类型
        var date11 = clay2.date;
        Console.WriteLine(date11);

        // ===================== 流变对象的类型转换 =====================

        // 使用声明方式（隐式转换）自动转换
        ClayModel clayModel = clay;
        Console.WriteLine($"{clayModel.Id} {clayModel.Name} {clayModel.Date} {clayModel.IsTrue}");

        // 使用强制转换（显示转换）
        var clayModel2 = (ClayModel)clay;
        Console.WriteLine($"{clayModel2.Id} {clayModel2.Name} {clayModel2.Date} {clayModel2.IsTrue}");

        // 使用流变对象 As<T>() 方法
        var clayModel3 = clay.As<ClayModel>();

        // 支持传入 JSON 序列化选项
        var clayModel31 =
            clay.As<ClayModel>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Console.WriteLine($"{clayModel3.Id} {clayModel3.Name} {clayModel3.Date} {clayModel3.IsTrue}");
        Console.WriteLine($"{clayModel31.Id} {clayModel31.Name} {clayModel31.Date} {clayModel31.IsTrue}");

        // 使用流变对象 As(Type) 方法
        var clayModel4 = clay.As(typeof(ClayModel)) as ClayModel;

        // 支持传入 JSON 序列化选项
        var clayModel41 =
            clay.As(typeof(ClayModel),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) as ClayModel;

        Console.WriteLine($"{clayModel4?.Id} {clayModel4?.Name} {clayModel4?.Date} {clayModel4?.IsTrue}");
        Console.WriteLine($"{clayModel41?.Id} {clayModel41?.Name} {clayModel4?.Date} {clayModel41?.IsTrue}");

        // 使用 JsonSerializer 反序列化方法
        var clayModel5 = JsonSerializer.Deserialize<ClayModel>(clay.ToString(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Console.WriteLine($"{clayModel5.Id} {clayModel5.Name} {clayModel5.Date} {clayModel5.IsTrue}");

        // 使用流变对象实例作为方法调用
        var clayModel6 = clay(typeof(ClayModel)) as ClayModel;

        // 支持传入 JSON 序列化选项
        var clayModel7 =
            clay(typeof(ClayModel), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) as ClayModel;

        Console.WriteLine($"{clayModel6?.Id} {clayModel6?.Name} {clayModel6?.Date} {clayModel6?.IsTrue}");
        Console.WriteLine($"{clayModel7?.Id} {clayModel7?.Name} {clayModel7?.Date} {clayModel7?.IsTrue}");

        // 转换为 XElement 类型对象
        // 隐式转换
        XElement xElement = clay;
        // 显式转换
        var xElement2 = (XElement)clay;
        // As<T>() 方法
        var xElement3 = clay.As<XElement>();
        Console.WriteLine(xElement);
        Console.WriteLine(xElement2);
        Console.WriteLine(xElement3);
    }

    /// <summary>
    ///     数据变更事件监听
    /// </summary>
    [HttpGet]
    public void Events()
    {
        // ===================== 单一对象 =====================

        dynamic clay = Clay.Parse("""{"id":1,"name":"shapeless"}""");

        // 数据变更之前
        ((Clay)clay).Changing += (sender, args) =>
        {
            Console.WriteLine(args.IsFound
                ? $"变更之前 (键：{args.Identifier}，值：{sender[args.Identifier]})"
                : $"变更之前 (键: {args.Identifier}) 不存在");
        };

        // 数据变更之后
        ((Clay)clay).Changed += (sender, args) =>
        {
            Console.WriteLine($"变更之后 (键：{args.Identifier}，值：{sender[args.Identifier]})");
        };

        // 移除数据之前
        ((Clay)clay).Removing += (sender, args) =>
        {
            Console.WriteLine(args.IsFound
                ? $"移除之前 (键：{args.Identifier}，值：{sender[args.Identifier]})"
                : $"移除之前 (键: {args.Identifier}) 不存在");
        };

        // 移除数据之后
        ((Clay)clay).Removed += (sender, args) => { Console.WriteLine($"移除之后 (键: {args.Identifier}) 不存在"); };

        clay.id = 2;
        clay.name = "Shapeless";
        clay.author = "百小僧";

        clay.Remove("author");

        // ===================== 集合或数组 =====================

        dynamic array = Clay.Parse("[1,2,10.3,true,false]");

        // 数据变更之前
        ((Clay)array).Changing += (sender, args) =>
        {
            Console.WriteLine(args.IsFound
                ? $"变更之前 (索引：{args.Identifier}，值：{sender[args.Identifier]})"
                : $"变更之前 (索引: {args.Identifier}) 不存在");
        };

        // 数据变更之后
        ((Clay)array).Changed += (sender, args) =>
        {
            Console.WriteLine($"变更之后 (索引：{args.Identifier}，值：{sender[args.Identifier]})");
        };

        // 移除数据之前
        ((Clay)array).Removing += (sender, args) =>
        {
            Console.WriteLine(args.IsFound
                ? $"移除之前 (索引：{args.Identifier}，值：{sender[args.Identifier]})"
                : $"移除之前 (索引: {args.Identifier}) 不存在");
        };

        // 移除数据之后
        ((Clay)array).Removed += (sender, args) => { Console.WriteLine($"移除之后 (索引: {args.Identifier}) 不存在"); };

        array.Add("Furion");
        array.Insert(0, "One");

        array.Remove(3);
    }

    /// <summary>
    ///     输出多种格式
    /// </summary>
    [HttpGet]
    public void FormatOutput()
    {
        dynamic clay = Clay.Parse("""{"id":1,"name":"shapeless","author":"百小僧"}""");

        // 默认格式化输出（中文 Unicode 编码）
        Console.WriteLine(clay);
        Console.WriteLine(clay.ToString());

        // 支持格式化符输出（U：取消中文 Unicode 编码）
        Console.WriteLine($"{clay:U}");
        Console.WriteLine(clay.ToString("U"));

        // 支持格式化符输出（Z：压缩输出）
        Console.WriteLine($"{clay:Z}");
        Console.WriteLine(clay.ToString("Z"));

        // 支持组合使用
        Console.WriteLine($"{clay:UZ}");
        Console.WriteLine(clay.ToString("UZ"));

        // 支持格式化符输出（C：小驼峰键命名）
        Console.WriteLine($"{clay:C}");
        Console.WriteLine(clay.ToString("C"));

        // 支持格式化符输出（P：帕斯卡（大驼峰）键命名）
        Console.WriteLine($"{clay:P}");
        Console.WriteLine(clay.ToString("P"));

        // 组合使用（Z：压缩； U：取消中文 Unicode 编码； P：帕斯卡（大驼峰）键命名）
        Console.WriteLine($"{clay:ZUC}");
        Console.WriteLine(clay.ToString("ZUC"));

        Console.WriteLine($"{clay:ZUP}");
        Console.WriteLine(clay.ToString("ZUP"));

        // 输出标准 JSON 字符串
        Console.WriteLine(clay.ToJsonString());
        // 支持传入 JSON 序列化选项
        Console.WriteLine(clay.ToJsonString(new JsonSerializerOptions
        {
            WriteIndented = true, // 格式化 JSON
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // 取消中文 Unicode 编码
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase // 小驼峰键命名策略
        }));

        // 使用流变对象实例作为方法调用
        Console.WriteLine(clay());
        // 支持传入 JSON 序列化选项
        Console.WriteLine(clay(new JsonSerializerOptions
        {
            WriteIndented = true, // 格式化 JSON
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // 取消中文 Unicode 编码
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase // 小驼峰键命名策略
        }));

        // 输出 XML 字符串
        Console.WriteLine(clay.ToXmlString());
        // 支持传入 XML 写入选项
        Console.WriteLine(clay.ToXmlString(new XmlWriterSettings { Indent = true }));
    }

    /// <summary>
    ///     动态添加委托方法
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public Clay DynamicAddMethod()
    {
        dynamic clay = Clay.Parse("""{"id":1,"name":"shapeless"}""");

        // 添加 Func 委托
        clay.sayHello = (Func<string>)(() => $"Hello, {clay.name}!");

        // 添加 Action 委托
        clay.addProp = new Action<string, object?>((key, value) => { clay[key] = value; });
        clay.Increment = new Action(() => clay.id++);

        // 调用 sayHello 方法
        Console.WriteLine(clay.sayHello());

        Console.WriteLine($"调用 Increment 方法之前的 id 值：{clay.id}");
        // 调用 Increment 方法
        clay.Increment();
        Console.WriteLine($"调用 Increment 方法之后的 id 值：{clay.id}");

        // 调用 addProp 方法
        clay.addProp("age", 30);
        clay["addProp"]("address", "广东省中山市"); // 同样支持索引方式

        Console.WriteLine($"{clay:U}");

        // ClayContext 使用
        clay.sayHello2 =
            (Func<ClayContext, string>)(ctx =>
                $"Hello, {ctx.Current.name}!"); // 或使用 new Func<ClayContext, string>(...) 

        // 调用 sayHello2 方法，无需传入首个 ClayContext 实例
        Console.WriteLine(clay.sayHello2());

        // 更多参数示例
        clay.sayHello3 =
            (Func<ClayContext, string, string>)((ctx, arg) =>
                $"Hello, {ctx.Current.name} {arg}!"); // 或使用 new Func<ClayContext, string>(...) 

        // 调用 sayHello3 方法，无需传入首个 ClayContext 实例
        Console.WriteLine(clay.sayHello3("新年快乐"));

        return clay;
    }
}