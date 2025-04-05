// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless.AspNetCore.Tests;

[ApiController]
[Route("[controller]/[action]")]
public class ClayController : ControllerBase
{
    [HttpPost]
    public Clay Post1(Clay clay) => clay;

    [HttpPost]
    public Clay Post2([Clay] dynamic clay) => clay;
}