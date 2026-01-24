// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Microsoft.AspNetCore.Mvc;

/// <summary>
///     <see cref="Controller" /> 扩展类
/// </summary>
public static class ControllerExtensions
{
    /// <summary>
    ///     创建一个 <see cref="ViewResult" /> 对象，并将视图模型设置为 <see cref="Clay" /> 类型
    /// </summary>
    /// <param name="controller">
    ///     <see cref="Controller" />
    /// </param>
    /// <param name="model">视图模型</param>
    /// <param name="clayOptions">
    ///     <see cref="ClayOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="ViewResult" />
    /// </returns>
    public static ViewResult ViewClay(this Controller controller, object? model, ClayOptions? clayOptions = null) =>
        controller.View(Clay.Parse(model, clayOptions));

    /// <summary>
    ///     创建一个 <see cref="ViewResult" /> 对象，并将视图模型设置为 <see cref="Clay" /> 类型
    /// </summary>
    /// <param name="controller">
    ///     <see cref="Controller" />
    /// </param>
    /// <param name="viewName">视图名称</param>
    /// <param name="model">视图模型</param>
    /// <param name="clayOptions">
    ///     <see cref="ClayOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="ViewResult" />
    /// </returns>
    public static ViewResult ViewClay(this Controller controller, string? viewName, object? model,
        ClayOptions? clayOptions = null) =>
        controller.View(viewName, Clay.Parse(model, clayOptions));
}