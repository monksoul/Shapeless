// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless;

/// <summary>
///     <see cref="Clay" /> 对象事件数据
/// </summary>
public abstract class ClayEventArgs : EventArgs
{
    /// <summary>
    ///     <inheritdoc cref="ClayEventArgs" />
    /// </summary>
    /// <param name="keyOrIndex">键或索引</param>
    internal ClayEventArgs(object keyOrIndex) => KeyOrIndex = keyOrIndex;

    /// <summary>
    ///     键或索引
    /// </summary>
    public virtual object KeyOrIndex { get; }
}