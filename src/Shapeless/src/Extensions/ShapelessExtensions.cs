// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless.Extensions;

/// <summary>
///     流变对象模块扩展类
/// </summary>
public static class ShapelessExtensions
{
    /// <summary>
    ///     将对象转换为 <see cref="Clay" /> 实例
    /// </summary>
    /// <param name="obj">
    ///     <see cref="object" />
    /// </param>
    /// <param name="options">
    ///     <see cref="ClayOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    public static Clay ToClay(this object? obj, ClayOptions? options = null) => Clay.Parse(obj, options);

    /// <summary>
    ///     将对象转换为 <see cref="Clay" /> 实例
    /// </summary>
    /// <param name="obj">
    ///     <see cref="object" />
    /// </param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    public static Clay ToClay(this object? obj, Action<ClayOptions> configure) => Clay.Parse(obj, configure);

    /// <summary>
    ///     将 <see cref="Clay" /> 实例通过转换管道传递并返回新的 <see cref="Clay" />（失败时抛出异常）
    /// </summary>
    /// <param name="clayTask">
    ///     <see cref="Task{TResult}" />
    /// </param>
    /// <param name="transformer">转换函数</param>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<Clay?> PipeAsync(this Task<Clay?> clayTask, Func<dynamic, dynamic?> transformer)
    {
        var clay = await clayTask;
        return clay?.Pipe(transformer);
    }

    /// <summary>
    ///     尝试将 <see cref="Clay" /> 实例通过转换管道传递，失败时返回原始对象
    /// </summary>
    /// <param name="clayTask">
    ///     <see cref="Task{TResult}" />
    /// </param>
    /// <param name="transformer">returns</param>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    public static async Task<Clay?> PipeTryAsync(this Task<Clay?> clayTask, Func<dynamic, dynamic?> transformer)
    {
        var clay = await clayTask;
        return clay?.PipeTry(transformer);
    }
}