// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless.Tests;

public class ClayTypeTests
{
    [Fact]
    public void Definition_ReturnOK()
    {
        var names = Enum.GetNames<ClayType>();
        Assert.Equal(2, names.Length);

        string[] strings =
        [
            nameof(ClayType.Object), nameof(ClayType.Array)
        ];
        Assert.True(strings.SequenceEqual(names));
    }
}