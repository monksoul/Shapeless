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

        // 通过索引或 Add/Push 方法添加数组项
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
    ///     创建集合/数组
    /// </summary>
    /// <returns></returns>
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

        // 反转集合/数组
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
    /// 从 JSON 字符串创建
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
    ///     解析 C# 对象
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    ///     更多使用例子
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    ///     遍历操作
    /// </summary>
    [HttpGet]
    public void Foreach()
    {
        // TODO: ForEach 方法例子

        // ===================== 单一对象 =====================

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

        // ===================== 集合/数组 =====================

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

    /// <summary>
    ///     Lambda 和 Linq 操作
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public object LambdaAndLinq()
    {
        // ===================== 单一对象 =====================

        dynamic clay = Clay.Parse("""{"id":1,"name":"shapeless"}""");

        // 转换为 Clay 类型后遍历
        var list1 = ((Clay)clay).Where(u => (string)u.Key == "id").OrderBy(u => u.Key).ToList();

        // 转换为 IEnumerable<KeyValuePair<object, dynamic?>> 类型后遍历
        var list2 = ((IEnumerable<KeyValuePair<object, dynamic?>>)clay).Where(u => (string)u.Key == "id")
            .OrderBy(u => u.Key).FirstOrDefault();

        // 声明为 Clay 类型再遍历（推荐方式，能够提供 IDE 智能提示）
        Clay clayList = clay;
        var list3 = clayList.Where(u => (string)u.Key == "id").OrderBy(u => u.Key).ToList();
        // AsEnumerableObject 返回了 string 类型的 Key
        var list31 = clayList.AsEnumerableObject().Where(u => u.Key == "id").ToList();

        // 声明为 IEnumerable<KeyValuePair<object, dynamic?>> 类型再遍历
        IEnumerable<KeyValuePair<object, dynamic?>> clayList2 = clay;
        var list4 = clayList2.Where(u => (string)u.Key == "id").OrderBy(u => u.Key).ToList();

        // Linq 查询
        var query1 = from item in clayList
            where (string)item.Key == "id"
            orderby item.Key
            select item;

        // Linq 查询，AsEnumerableObject 返回了 string 类型的 Key
        var query2 = from item in clayList.AsEnumerableObject()
            where item.Key == "id"
            orderby item.Key
            select item;

        var list5 = query1.ToList();
        var list6 = query2.ToList();

        // ===================== 集合/数组 =====================

        dynamic array = Clay.Parse("""[1,2,true,false,"Furion",{"id":1,"name":"shapeless"},null]""");

        // 转换为 Clay 类型后遍历
        var arrayList1 = ((Clay)array).Where(u => (int)u.Key > 2).OrderBy(u => u.Key).ToList();

        // 转换为 IEnumerable<KeyValuePair<object, dynamic?>> 类型后遍历
        var arrayList2 = ((IEnumerable<KeyValuePair<object, dynamic?>>)array).Where(u => (int)u.Key > 2)
            .OrderBy(u => u.Key).FirstOrDefault();

        // 声明为 Clay 类型再遍历（推荐方式，能够提供 IDE 智能提示）
        Clay clayArray = array;
        var arrayList3 = clayArray.Where(u => (int)u.Key > 2).OrderBy(u => u.Key).ToList();
        // AsEnumerableArray 返回了 int 类型的索引
        var arrayList31 = clayArray.AsEnumerableArray().Where(u => u.Key > 2).ToList();

        // 声明为 IEnumerable<KeyValuePair<object, dynamic?>> 类型再遍历
        IEnumerable<KeyValuePair<object, dynamic?>> clayArray2 = array;
        var arrayList4 = clayArray2.Where(u => (int)u.Key > 2).OrderBy(u => u.Key).ToList();

        // Linq 查询
        var arrayQuery1 = from item in clayArray
            where (int)item.Key > 2
            orderby item.Key
            select item;

        // Linq 查询，AsEnumerableArray 返回了 int 类型的索引
        var arrayQuery2 = from item in clayArray.AsEnumerableArray()
            where item.Key > 2
            orderby item.Key
            select item;

        var arrayList5 = arrayQuery1.ToList();
        var arrayList6 = arrayQuery2.ToList();

        return new
        {
            list1, list2, list3, list31, list4, list5, list6, arrayList1, arrayList2, arrayList3, arrayList31,
            arrayList4,
            arrayList5, arrayList6
        };
    }

    /// <summary>
    ///     事件监听
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

        clay.Delete("author");

        // ===================== 集合/数组 =====================

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

        array.Delete(3);
    }

    /// <summary>
    ///     类型转换
    /// </summary>
    [HttpGet]
    public void TypeConvert()
    {
        dynamic clay =
            Clay.Parse("""{"id":1,"name":"shapeless","date":"2025-01-14T00:00:00","isTrue":true}"""); // ISO 8601 时间格式

        // ===================== 属性类型转换 =====================

        // 通过 属性<T> 方式
        var date = clay.date<DateTime>();
        var isTrue = clay.isTrue<bool>();
        var date11 = clay.date<DateTime>(new JsonSerializerOptions()); // 支持传递序列化参数

        Console.WriteLine(date);
        Console.WriteLine(isTrue);
        Console.WriteLine(date11);

        // 通过 属性(Type) 方式
        var date2 = clay.date(typeof(DateTime)) as DateTime?;
        var isTrue2 = clay.isTrue(typeof(bool)) as bool?;
        var date21 =
            clay.date(typeof(DateTime),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) as DateTime?; // 支持传递序列化参数

        Console.WriteLine(date2);
        Console.WriteLine(isTrue2);
        Console.WriteLine(date21);

        // 通过 Get<T> 方式
        var date3 = clay.Get<DateTime>("date");
        var isTrue3 = clay.Get<bool>("isTrue");
        var date31 =
            clay.Get<DateTime>("date", new JsonSerializerOptions { PropertyNameCaseInsensitive = true }); // 支持传递序列化参数

        Console.WriteLine(date3);
        Console.WriteLine(isTrue3);
        Console.WriteLine(date31);

        // 通过 Get(Type) 方式
        var date4 = clay.Get("date", typeof(DateTime)) as DateTime?;
        var isTrue4 = clay.Get("isTrue", typeof(bool)) as bool?;
        var date41 =
            clay.Get("date", typeof(DateTime),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) as DateTime?; // 支持传递序列化参数

        Console.WriteLine(date4);
        Console.WriteLine(isTrue4);
        Console.WriteLine(date41);

        // 通过 属性(Func<string?, object?>) 方式
        var date5 = clay.date(new Func<string?, object?>(u => Convert.ToDateTime(u)));
        Console.WriteLine(date5);

        // ===================== 流变对象转换 =====================

        // 声明方式自动转换
        ClayModel clayModel = clay;
        Console.WriteLine($"{clayModel.Id} {clayModel.Name} {clayModel.Date} {clayModel.IsTrue}");

        // 强制转换
        var clayModel2 = (ClayModel)clay;
        Console.WriteLine($"{clayModel2.Id} {clayModel2.Name} {clayModel2.Date} {clayModel2.IsTrue}");

        // 通过 As<T> 方式
        var clayModel3 = clay.As<ClayModel>();
        var clayModel31 =
            clay.As<ClayModel>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true }); // 支持传递序列化参数
        Console.WriteLine($"{clayModel3.Id} {clayModel3.Name} {clayModel3.Date} {clayModel3.IsTrue}");
        Console.WriteLine($"{clayModel31.Id} {clayModel31.Name} {clayModel31.Date} {clayModel31.IsTrue}");

        // 通过 As(Type) 方式
        var clayModel4 = clay.As(typeof(ClayModel)) as ClayModel;
        var clayModel41 =
            clay.As(typeof(ClayModel),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) as ClayModel; // 支持传递序列化参数
        Console.WriteLine($"{clayModel4?.Id} {clayModel4?.Name} {clayModel4?.Date} {clayModel4?.IsTrue}");
        Console.WriteLine($"{clayModel41?.Id} {clayModel41?.Name} {clayModel4?.Date} {clayModel41?.IsTrue}");

        // 通过反序列化方式
        var clayModel5 = JsonSerializer.Deserialize<ClayModel>(clay.ToString(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Console.WriteLine($"{clayModel5.Id} {clayModel5.Name} {clayModel5.Date} {clayModel5.IsTrue}");

        // 转换为 XElement 对象
        var xElement = clay.As<XElement>();
        Console.WriteLine(xElement);
    }

    /// <summary>
    ///     格式化输出
    /// </summary>
    [HttpGet]
    public void FormatOutput()
    {
        dynamic clay = Clay.Parse("""{"id":1,"name":"shapeless","author":"百小僧"}""");

        // 默认格式化输出（中文 Unicode 编码）
        Console.WriteLine(clay);
        Console.WriteLine(clay.ToString());

        // 支持格式化符输出（Z：压缩输出）
        Console.WriteLine($"{clay:Z}");
        Console.WriteLine(clay.ToString("Z"));

        // 支持格式化符输出（U：取消中文 Unicode 编码）
        Console.WriteLine($"{clay:U}");
        Console.WriteLine(clay.ToString("U"));

        // 支持格式化符输出（C：小驼峰键命名）
        Console.WriteLine($"{clay:C}");
        Console.WriteLine(clay.ToString("C"));

        // 支持格式化符输出（P：帕斯卡（大驼峰）键命名）
        Console.WriteLine($"{clay:P}");
        Console.WriteLine(clay.ToString("P"));

        // 组合使用（Z：压缩； U：取消中文 Unicode 编码； P：帕斯卡（大驼峰）键命名）
        Console.WriteLine($"{clay:ZUP}");
        Console.WriteLine(clay.ToString("ZUP"));

        // 输出标准 JSON 字符串
        Console.WriteLine(clay.ToJsonString());
        Console.WriteLine(clay.ToJsonString(new JsonSerializerOptions())); // 支持传入序列化参数

        // 输出 XML 字符串
        Console.WriteLine(clay.ToXmlString());
        Console.WriteLine(clay.ToXmlString(new XmlWriterSettings { Indent = true })); // 支持传入 XML 写入参数
    }
}