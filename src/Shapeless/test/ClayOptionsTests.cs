// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless.Tests;

public class ClayOptionsTests
{
    [Fact]
    public void Static_ReturnOK()
    {
        Assert.NotNull(ClayOptions.Default);
        var defaultValue = ClayOptions.Default;
        var defaultValue2 = ClayOptions.Default;
        Assert.Equal(defaultValue, defaultValue);
        Assert.NotEqual(defaultValue.GetHashCode(), defaultValue2.GetHashCode());

        Assert.NotNull(ClayOptions.Default.JsonSerializerOptions);
        Assert.True(ClayOptions.Default.JsonSerializerOptions.PropertyNameCaseInsensitive);
        Assert.Null(ClayOptions.Default.JsonSerializerOptions.PropertyNamingPolicy);
        Assert.Equal(JsonNumberHandling.AllowReadingFromString,
            ClayOptions.Default.JsonSerializerOptions.NumberHandling);
        Assert.Equal(JavaScriptEncoder.UnsafeRelaxedJsonEscaping, ClayOptions.Default.JsonSerializerOptions.Encoder);
        Assert.True(ClayOptions.Default.JsonSerializerOptions.AllowTrailingCommas);
    }

    [Fact]
    public void New_ReturnOK()
    {
        var clayOptions = new ClayOptions();
        Assert.Equal("data", clayOptions.ScalarValueKey);
        Assert.False(clayOptions.AllowMissingProperty);
        Assert.False(clayOptions.AllowIndexOutOfRange);
        Assert.False(clayOptions.AutoCreateNestedObjects);
        Assert.False(clayOptions.AutoCreateNestedArrays);
        Assert.False(clayOptions.AutoExpandArrayWithNulls);
        Assert.False(clayOptions.ValidateAfterConversion);
        Assert.False(clayOptions.AutoConvertToDateTime);
        Assert.False(clayOptions.PropertyNameCaseInsensitive);

        Assert.NotNull(clayOptions.JsonSerializerOptions);
        Assert.True(clayOptions.JsonSerializerOptions.PropertyNameCaseInsensitive);
        Assert.Null(clayOptions.JsonSerializerOptions.PropertyNamingPolicy);
        Assert.Equal(JsonNumberHandling.AllowReadingFromString, clayOptions.JsonSerializerOptions.NumberHandling);
        Assert.Equal(JavaScriptEncoder.UnsafeRelaxedJsonEscaping, clayOptions.JsonSerializerOptions.Encoder);
    }
}