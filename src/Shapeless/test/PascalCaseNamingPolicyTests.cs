// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless.Tests;

public class PascalCaseNamingPolicyTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var pascalCaseNamingPolicy = new PascalCaseNamingPolicy();
        Assert.NotNull(pascalCaseNamingPolicy);
        Assert.NotNull(PascalCaseNamingPolicy._splitter);
    }

    [Theory]
    [InlineData("hello", "Hello")]
    [InlineData("world", "World")]
    [InlineData("helloWorld", "HelloWorld")]
    [InlineData("hello_world", "Hello_world")]
    [InlineData("hello-world", "Hello-world")]
    [InlineData("hello world", "Hello world")]
    [InlineData("hello.world", "Hello.world")]
    [InlineData("helloWorld123", "HelloWorld123")]
    [InlineData("hello123world", "Hello123World")]
    [InlineData("123helloWorld", "123HelloWorld")]
    [InlineData("hello123", "Hello123")]
    [InlineData("hello World!", "HelloWorld!")]
    [InlineData("hello_world!!", "Hello_world!!")]
    [InlineData("HELLO", "HELLO")]
    [InlineData("URL", "URL")]
    [InlineData("htmlParser", "HtmlParser")]
    [InlineData("HTMLParser", "HTMLParser")]
    [InlineData("JSONData", "JSONData")]
    [InlineData("id", "Id")]
    [InlineData("a", "A")]
    [InlineData("A", "A")]
    [InlineData("__leadingUnderscores", "__leadingUnderscores")]
    [InlineData("trailingUnderscores__", "TrailingUnderscores__")]
    [InlineData("---multiple---hyphens---", "---multiple---hyphens---")]
    [InlineData("...multiple...dots...", "...multiple...dots...")]
    public void ConvertName_ReturnOK(string input, string expected)
    {
        var pascalCaseNamingPolicy = new PascalCaseNamingPolicy();
        var result = pascalCaseNamingPolicy.ConvertName(input);
        Assert.Equal(expected, result);
    }
}