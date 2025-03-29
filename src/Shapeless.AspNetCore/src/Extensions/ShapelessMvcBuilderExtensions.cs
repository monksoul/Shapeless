// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///     流变对象模块 <see cref="IMvcBuilder" /> 拓展类
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
    public static IMvcBuilder AddClayOptions(this IMvcBuilder builder) => builder.AddClayOptions(_ => { });

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
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        // 配置 JsonOptions 选项，添加 ClayJsonConverter 转换器
        builder.Services.Configure<JsonOptions>(options =>
        {
            if (!options.JsonSerializerOptions.Converters.OfType<ClayJsonConverter>().Any())
            {
                options.JsonSerializerOptions.Converters.Add(new ClayJsonConverter());
            }
        });

        // 配置 ClayOptions 选项服务
        builder.Services.Configure(configure);

        // 添加 Clay 模型绑定提供器
        builder.Services.Configure<MvcOptions>(options =>
        {
            if (!options.ModelBinderProviders.OfType<ClayBinderProvider>().Any())
            {
                options.ModelBinderProviders.Insert(0, new ClayBinderProvider());
            }
        });

        return builder;
    }
}