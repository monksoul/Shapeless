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

        // 删除项
        clay.Remove(4); // 或使用 clay.Delete(4)

        // 删除末项
        clay.Pop();

        // 输出字符串
        Console.WriteLine(clay); // 或使用 clay.ToString();

        return clay;
    }

    [HttpGet]
    public Clay Usage()
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
        Console.WriteLine($"{clay.sayHello()}\r\n{clay:UZ}");

        return clay;
    }
}