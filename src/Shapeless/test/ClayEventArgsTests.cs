// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless.Tests;

public class ClayEventArgsTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(typeof(ClayEventArgs).IsSubclassOf(typeof(EventArgs)));

        var eventArgs = new ClayEventArgs("Name", false);
        Assert.Equal("Name", eventArgs.Identifier);
        Assert.False(eventArgs.IsFound);

        var eventArgs2 = new ClayEventArgs(0, true);
        Assert.Equal(0, eventArgs2.Identifier);
        Assert.True(eventArgs2.IsFound);
    }
}