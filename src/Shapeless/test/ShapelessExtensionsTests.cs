// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

using Shapeless.Extensions;

namespace Shapeless.Tests;

public class ShapelessExtensionsTests
{
    [Fact]
    public void ToClay_ReturnOK()
    {
        var obj = new { id = 1, name = "furion" };
        var clay = obj.ToClay();
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", clay.ToJsonString());

        object[] obj2 = [1, 2, 3];
        var clay2 = obj2.ToClay(u => u.AllowIndexOutOfRange = true);
        Assert.Equal("[1,2,3]", clay2.ToJsonString());
    }
}