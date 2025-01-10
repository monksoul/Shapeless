# Shapeless

[![license](https://img.shields.io/badge/license-MIT-orange?cacheSeconds=10800)](https://gitee.com/dotnetchina/Shapeless/blob/master/LICENSE) [![nuget](https://img.shields.io/nuget/v/Shapeless.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Shapeless) [![dotNET China](https://img.shields.io/badge/organization-dotNET%20China-yellow?cacheSeconds=10800)](https://gitee.com/dotnetchina)

Shapeless 是一个高性能的 C# 开源库，提供类似 JavaScript JSON 的灵活操作体验，支持动态的增删查改和 Linq、Lambda
表达式查询，极大地简化了运行时对象的构建与操作，同时保持了简洁性和强大的性能表现。

## 特性

- **灵活 `JSON` 操作**：提供类似 `JavaScript`  的增删查改功能，兼容 `Linq` 和 `Lambda` 表达式。
- **`Web` 开发友好**：无缝集成 `ASP.NET Core WebAPI` 和 `MVC`，简化 `HTTP` 请求处理和 `API` 开发流程。
- **动态数据处理**：支持动态对象构建与类型转换，内置高效数据校验机制。
- **序列化支持**：提供快速的 `JSON` 序列化和反序列化功能，适用于数据交换和存储需求。
- **响应式监听**：支持监听 `JSON` 对象中值的变化和键的移除，自动触发事件通知。
- **架构设计**：架构设计灵活，易于使用与扩展。
- **跨平台无依赖**：支持跨平台运行，无需外部依赖。
- **高质量代码保障**：遵循高标准编码规范，拥有高达 `98%` 的单元测试与集成测试覆盖率。
- **`.NET 8+` 兼容性**：可在 `.NET 8` 及更高版本环境中部署使用。

## 安装

```powershell
dotnet add package Shapeless
```

## 快速入门

我们在[主页](https://furion.net/docs/shapeless/)上有不少例子，这是让您入门的第一个：

```cs
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
```

运行上述代码后，控制台输出如下内容：

```
10
11
Hello, shapeless!
{"id":1,"name":"shapeless","author":"百小僧","company":"百签科技","homepage":["https://furion.net/","https://baiqian.com"],"number":11}
```

[更多文档](https://furion.net/docs/shapeless/)

## 文档

您可以在[主页](https://furion.net/docs/shapeless/)找到 Shapeless 文档。

## 贡献

该存储库的主要目的是继续发展 Shapeless 核心，使其更快、更易于使用。Shapeless
的开发在 [Gitee](https://gitee.com/dotnetchina/Shapeless) 上公开进行，我们感谢社区贡献错误修复和改进。

## 许可证

Shapeless 采用 [MIT](./LICENSE) 开源许可证。

[![](./assets/baiqian.svg)](https://baiqian.com)