namespace Shapeless.Samples.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class GetStartController
{
    [HttpGet]
    public Clay SingleObject()
    {
        dynamic clay = new Clay();

        // 属性或索引方式设置值
        clay.Id = 1;
        clay["Name"] = "Shapeless";
        clay.IsDynamic = true;

        // 设置匿名对象
        clay.sub = new
        {
            HomePage = new[] { "https://furion.net", "https://baiqian.com" }
        };
        // 继续添加数组内容
        clay.sub.HomePage[2] = "https://baiqian.ltd";
        clay.sub.HomePage.Add("https://百签.com"); // 使用 Add 方法

        // 嵌套设置流变对象
        clay.extend = new Clay();
        clay.extend.username = "MonkSoul";

        // 支持输出字符串格式化：U（取消中文 Unicode 编码）
        Console.WriteLine(
            $"{clay.Id} {clay.Name} {clay.IsDynamic} {clay.sub:U} {clay.extend} {clay.extend.username}");

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
        Console.WriteLine($"{clay.sayHello()}\r\n{clay.ToJsonString()}");

        return clay;
    }
}