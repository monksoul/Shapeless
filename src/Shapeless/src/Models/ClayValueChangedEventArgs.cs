// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless;

/// <summary>
///     值变更之后事件数据
/// </summary>
public sealed class ClayValueChangedEventArgs : EventArgs
{
    /// <summary>
    ///     <inheritdoc cref="ClayValueChangedEventArgs" />
    /// </summary>
    /// <param name="keyOrIndex">键或索引</param>
    internal ClayValueChangedEventArgs(object keyOrIndex) => KeyOrIndex = keyOrIndex;

    /// <summary>
    ///     键或索引
    /// </summary>
    public object KeyOrIndex { get; }
}