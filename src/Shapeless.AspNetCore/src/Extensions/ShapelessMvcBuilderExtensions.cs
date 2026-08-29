// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///     流变对象模块 <see cref="IMvcBuilder" /> 扩展类
/// </summary>
public static class ShapelessMvcBuilderExtensions
{
    /// <summary>
    ///     添加 <see cref="Clay" /> 配置
    /// </summary>
    /// <param name="builder">
    ///     <see cref="IMvcBuilder" />
    /// </param>
    /// <returns>
    ///     <see cref="IMvcBuilder" />
    /// </returns>
    public static IMvcBuilder AddClayOptions(this IMvcBuilder builder)
    {
        builder.Services.AddClayOptions();

        return builder;
    }

    /// <summary>
    ///     添加 <see cref="Clay" /> 配置
    /// </summary>
    /// <param name="builder">
    ///     <see cref="IMvcBuilder" />
    /// </param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="IMvcBuilder" />
    /// </returns>
    public static IMvcBuilder AddClayOptions(this IMvcBuilder builder, Action<ClayOptions> configure)
    {
        builder.Services.AddClayOptions(configure);

        return builder;
    }
}