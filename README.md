# Shapeless

[![license](https://img.shields.io/badge/license-MIT-orange?cacheSeconds=10800)](https://gitee.com/dotnetchina/Shapeless/blob/master/LICENSE) [![nuget](https://img.shields.io/nuget/v/Shapeless.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Shapeless) [![dotNET China](https://img.shields.io/badge/organization-dotNET%20China-yellow?cacheSeconds=10800)](https://gitee.com/dotnetchina)

Shapeless is a high-performance C# open-source library that offers a flexible manipulation experience akin to JavaScript
JSON, supporting dynamic addition, deletion, lookup, and modification along with Linq and Lambda expression queries. It
significantly simplifies the construction and manipulation of runtime objects while maintaining simplicity and robust
performance characteristics.

## Features

- **Flexible `JSON` Manipulation**: Offers functionalities for adding, deleting, querying, and modifying similar to
  `JavaScript`, with compatibility for `Linq` and `Lambda` expressions.
- **`Web` Development Friendly**: Seamlessly integrates with `ASP.NET Core WebAPI` and `MVC`, simplifying `HTTP` request
  handling and `API` development processes.
- **Dynamic Data Processing**: Supports dynamic object construction and type conversion, featuring an efficient built-in
  data validation mechanism.
- **Serialization Support**: Provides fast `JSON` serialization and deserialization capabilities, suitable for data
  exchange and storage needs.
- **Reactive Listening**: Supports monitoring changes in values and removal of keys within `JSON` objects, automatically
  triggering event notifications.
- **Architectural Design**: The design is flexible, making it easy to use and extend.
- **Cross-platform Without Dependencies**: Supports cross-platform execution without requiring external dependencies.
- **High-quality Code Assurance**: Adheres to high-standard coding norms, boasting unit test and integration test
  coverage as high as `98%`.
- **`.NET 8+` Compatibility**: Can be deployed and used in environments running `.NET 8` and higher versions.

## Installation

```powershell
dotnet add package Shapeless
```

## Getting Started

We have many examples on our [homepage](https://furion.net/docs/shapeless/). Here's your first one to get you started:

```cs
dynamic clay = Clay.Parse("""{"id":1,"name":"shapeless"}""");

// Add Property
clay.author = "百小僧";
clay["company"] = "百签科技";
clay.homepage = new[] { "https://furion.net/", "https://baiqian.com" };
clay.number = 10;

// Add Method
clay.sayHello = (Func<string>)(() => $"Hello, {clay.name}!");
clay.Increment = new Action(() => { clay.number++; });

// Call Method
Console.WriteLine(clay.number); // number: 10
clay.Increment();
Console.WriteLine(clay.number); // number: 11

// Print JSON
Console.WriteLine($"{clay.sayHello()}\r\n{clay.ToJsonString()}");
```

After running the above code, the console outputs the following content:

```
10
11
Hello, shapeless!
{"id":1,"name":"shapeless","author":"百小僧","company":"百签科技","homepage":["https://furion.net/","https://baiqian.com"],"number":11}
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