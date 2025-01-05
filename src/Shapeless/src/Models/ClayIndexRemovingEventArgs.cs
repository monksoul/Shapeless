// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless;

/// <summary>
///     键或索引移除之前事件数据
/// </summary>
public sealed class ClayIndexRemovingEventArgs : ClayEventArgs
{
    /// <summary>
    ///     <inheritdoc cref="ClayIndexRemovingEventArgs" />
    /// </summary>
    /// <param name="keyOrIndex">键或索引</param>
    internal ClayIndexRemovingEventArgs(object keyOrIndex)
        : base(keyOrIndex)
    {
    }
}