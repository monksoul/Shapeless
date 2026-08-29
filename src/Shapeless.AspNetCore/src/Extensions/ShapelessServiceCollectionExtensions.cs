// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///     流变对象模块 <see cref="IServiceCollection" /> 扩展类
/// </summary>
public static class ShapelessServiceCollectionExtensions
{
    /// <summary>
    ///     添加 <see cref="Clay" /> 配置
    /// </summary>
    /// <param name="services">
    ///     <see cref="IServiceCollection" />
    /// </param>
    /// <returns>
    ///     <see cref="IServiceCollection" />
    /// </returns>
    public static IServiceCollection AddClayOptions(this IServiceCollection services) =>
        services.AddClayOptions(_ => { });

    /// <summary>
    ///     添加 <see cref="Clay" /> 配置
    /// </summary>
    /// <param name="services">
    ///     <see cref="IServiceCollection" />
    /// </param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="IServiceCollection" />
    /// </returns>
    public static IServiceCollection AddClayOptions(this IServiceCollection services, Action<ClayOptions> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        // 注册流变对象服务
        services.TryAddSingleton<IClay, InternalClay>();

        // 配置 JsonOptions 选项，添加 ClayJsonConverter 和 ObjectToClayJsonConverter 转换器
        services.Configure<JsonOptions>(options => options.JsonSerializerOptions.AddClayConverters());

        // 配置 ClayOptions 选项服务
        services.Configure(configure);

        // 添加 Clay 模型绑定提供器
        services.Configure<MvcOptions>(options =>
        {
            if (!options.ModelBinderProviders.OfType<ClayBinderProvider>().Any())
            {
                options.ModelBinderProviders.Insert(0, new ClayBinderProvider());
            }
        });

        return services;
    }
}