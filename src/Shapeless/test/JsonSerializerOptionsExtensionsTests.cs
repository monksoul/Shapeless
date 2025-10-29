// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless.Tests;

public class JsonSerializerOptionsExtensionsTests
{
    [Fact]
    public void AddClayConverters_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => JsonSerializerOptionsExtensions.AddClayConverters(null!));

    [Fact]
    public void AddClayConverters_ReturnOK()
    {
        var options = new JsonSerializerOptions();
        options.AddClayConverters();

        Assert.Single(options.Converters.OfType<ClayJsonConverter>());
        Assert.Single(options.Converters.OfType<ObjectToClayJsonConverter>());

        options.AddClayConverters();
        options.AddClayConverters();
        Assert.Single(options.Converters.OfType<ClayJsonConverter>());
        Assert.Single(options.Converters.OfType<ObjectToClayJsonConverter>());
    }
}