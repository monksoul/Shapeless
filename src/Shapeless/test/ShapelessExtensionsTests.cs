// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

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

        dynamic clay3 = Clay.Parse(new { id = 1, name = "furion" });
        var clay4 = clay3.ToClay();
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", clay4.ToJsonString());

        var clay5 = clay3.ToClay(null);
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", clay5.ToJsonString());

        var clay6 = clay3.ToClay(ClayOptions.Flexible);
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", clay6.ToJsonString());

        var clay7 = clay3.ToClay(new Action<ClayOptions>(u => u.AllowIndexOutOfRange = true));
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", clay7.ToJsonString());
    }

    [Fact]
    public async Task PipeAsync_Invalid_Parameters()
    {
        var task = Task.FromResult(
            Clay.Parse(new { id = 1, name = "furion", children = new { id = 1, name = "child" } }));
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await task!.PipeAsync(u => u.name));

        Assert.Equal(
            "An unexpected error occurred during the transformation. Please verify the implementation of the transformation function.",
            exception.Message);
        Assert.Equal(
            "Transformation must return a non-null Clay object. The provided function either returned null or an incompatible type.",
            exception.InnerException?.Message);
    }

    [Fact]
    public async Task PipeAsync_ReturnOK()
    {
        var task = Task.FromResult(
            Clay.Parse(new { id = 1, name = "furion", children = new { id = 1, name = "child" } }));
        var clay = await task!.PipeAsync(u => u.children);
        Assert.Equal("{\"id\":1,\"name\":\"child\"}", clay?.ToJsonString());
    }

    [Fact]
    public async Task PipeTryAsync_ReturnOK()
    {
        var task = Task.FromResult(
            Clay.Parse(new { id = 1, name = "furion", children = new { id = 1, name = "child" } }));
        var clay = await task!.PipeTryAsync(u => u.child).PipeAsync(u => u.children);
        Assert.Equal("{\"id\":1,\"name\":\"child\"}", clay?.ToJsonString());
    }
}