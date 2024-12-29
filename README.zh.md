# Shapeless

[![license](https://img.shields.io/badge/license-MIT-orange?cacheSeconds=10800)](https://gitee.com/dotnetchina/Shapeless/blob/master/LICENSE) [![nuget](https://img.shields.io/nuget/v/Shapeless.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Shapeless) [![dotNET China](https://img.shields.io/badge/organization-dotNET%20China-yellow?cacheSeconds=10800)](https://gitee.com/dotnetchina)

Shapeless 是一个高性能的 C# 开源库，提供类似 JavaScript JSON 的灵活操作体验，支持动态的增删查改和 Linq、Lambda 表达式查询，极大地简化了运行时对象的构建与操作，同时保持了简洁性和强大的性能表现。

## 特性

## 安装

```powershell
dotnet add package Shapeless
```

## 快速入门

我们在[主页](https://furion.net/docs/shapeless/)上有不少例子，这是让您入门的第一个：

```cs
dynamic clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
clay.id = 100;
clay.name = "shapeless";
clay.author = "百小僧";
clay.homepage = new[] { "https://furion.net/", "https://baiqian.com" };

Console.WriteLine(clay.ToJsonString());
```

[更多文档](https://furion.net/docs/shapeless/)

## 文档

您可以在[主页](https://furion.net/docs/shapeless/)找到 Shapeless 文档。

## 贡献

该存储库的主要目的是继续发展 Shapeless 核心，使其更快、更易于使用。Shapeless
的开发在 [Gitee](https://gitee.com/dotnetchina/Shapeless) 上公开进行，我们感谢社区贡献错误修复和改进。

## 许可证

HttpAgent 采用 [MIT](./LICENSE) 开源许可证。

[![](./assets/baiqian.svg)](https://baiqian.com)