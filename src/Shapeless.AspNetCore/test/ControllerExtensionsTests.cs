// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless.AspNetCore.Tests;

public class ControllerExtensionsTests
{
    [Fact]
    public void ViewClay_ReturnOK()
    {
        var controller = new TestController();
        var viewResult = controller.ViewClay("""{"id":1,"name":"Furion"}""");
        Assert.NotNull(viewResult);
        Assert.True(viewResult.Model is Clay);

        var viewResult2 = controller.ViewClay("home", """{"id":1,"name":"Furion"}""");
        Assert.NotNull(viewResult2);
        Assert.True(viewResult2.Model is Clay);
    }
}

public class TestController : Controller
{
}