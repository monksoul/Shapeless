# Shapeless

[![license](https://img.shields.io/badge/license-MIT-orange?cacheSeconds=10800)](https://gitee.com/dotnetchina/Shapeless/blob/master/LICENSE) [![nuget](https://img.shields.io/nuget/v/Shapeless.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Shapeless) [![dotNET China](https://img.shields.io/badge/organization-dotNET%20China-yellow?cacheSeconds=10800)](https://gitee.com/dotnetchina)

Shapeless is a high-performance C# open-source library that offers a flexible manipulation experience akin to JavaScript
JSON, supporting dynamic addition, deletion, lookup, and modification along with Linq and Lambda expression queries. It
significantly simplifies the construction and manipulation of runtime objects while maintaining simplicity and robust
performance characteristics.

## Features

## Installation

```powershell
dotnet add package Shapeless
```

## Getting Started

We have many examples on our [homepage](https://furion.net/docs/shapeless/). Here's your first one to get you started:

```cs
dynamic clay = Clay.Parse("{\"id\":1,\"name\":\"furion\"}");
clay.id = 100;
clay.name = "shapeless";
clay.author = "百小僧";
clay.homepage = new[] { "https://furion.net/", "https://baiqian.com" };

Console.WriteLine(clay.ToJsonString());
```

[More Documentation](https://furion.net/docs/shapeless/)

## Documentation

You can find the Shapeless documentation on our [homepage](https://furion.net/docs/shapeless/).

## Contributing

The main purpose of this repository is to continue developing the core of Shapeless, making it faster and easier to use.
The development of Shapeless is publicly hosted on [Gitee](https://gitee.com/dotnetchina/Shapeless), and we appreciate
community contributions for bug fixes and improvements.

## License

Shapeless is released under the [MIT](./LICENSE) open source license.

[![](./assets/baiqian.svg)](https://baiqian.com)