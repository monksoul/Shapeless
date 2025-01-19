// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless;

/// <summary>
///     流变对象上下文
/// </summary>
/// <remarks>用于动态调用自定义委托时提供上下文 <see cref="Clay" /> 实例。无需外部手动初始化。</remarks>
public sealed class ClayContext
{
    /// <summary>
    ///     <inheritdoc cref="ClayContext" />
    /// </summary>
    /// <param name="current">上下文 <see cref="Clay" /> 实例</param>
    internal ClayContext(Clay current)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(current);

        Current = current;
    }

    /// <summary>
    ///     上下文 <see cref="Clay" /> 实例
    /// </summary>
    public dynamic Current { get; }
}