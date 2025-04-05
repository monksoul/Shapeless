// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless.AspNetCore.Tests;

public class ClayAttributeTests
{
    [Fact]
    public void AttributeUsage_Definition()
    {
        var attributeUsageAttribute =
            typeof(ClayAttribute).GetCustomAttribute<AttributeUsageAttribute>();

        Assert.NotNull(attributeUsageAttribute);
        Assert.Equal(AttributeTargets.Parameter, attributeUsageAttribute.ValidOn);
        Assert.False(attributeUsageAttribute.AllowMultiple);
        Assert.True(attributeUsageAttribute.Inherited);
    }

    [Fact]
    public void New_ReturnOK()
    {
        var attribute = new ClayAttribute();
        Assert.NotNull(attribute);
    }
}