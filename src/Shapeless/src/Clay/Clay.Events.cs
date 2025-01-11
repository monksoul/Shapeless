// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless;

/// <summary>
///     流变对象
/// </summary>
public partial class Clay
{
    /// <summary>
    ///     值变更之前事件
    /// </summary>
    public event EventHandler<ClayValueChangingEventArgs>? ValueChanging;

    /// <summary>
    ///     值变更之后事件
    /// </summary>
    public event EventHandler<ClayValueChangedEventArgs>? ValueChanged;

    /// <summary>
    ///     键或索引移除之前事件
    /// </summary>
    public event EventHandler<ClayIndexRemovingEventArgs>? IndexRemoving;

    /// <summary>
    ///     键或索引移除之后事件
    /// </summary>
    public event EventHandler<ClayIndexRemovedEventArgs>? IndexRemoved;

    /// <summary>
    ///     触发值变更之前事件
    /// </summary>
    /// <param name="keyOrIndex">键或索引</param>
    internal void OnValueChanging(object keyOrIndex) =>
        ValueChanging?.TryInvoke(this, new ClayValueChangingEventArgs(keyOrIndex));

    /// <summary>
    ///     触发值变更之后事件
    /// </summary>
    /// <param name="keyOrIndex">键或索引</param>
    internal void OnValueChanged(object keyOrIndex) =>
        ValueChanged?.TryInvoke(this, new ClayValueChangedEventArgs(keyOrIndex));

    /// <summary>
    ///     触发键或索引移除之前事件
    /// </summary>
    /// <param name="keyOrIndex">键或索引</param>
    internal void OnIndexRemoving(object keyOrIndex) =>
        IndexRemoving?.TryInvoke(this, new ClayIndexRemovingEventArgs(keyOrIndex));

    /// <summary>
    ///     触发键或索引移除之后事件
    /// </summary>
    /// <param name="keyOrIndex">键或索引</param>
    internal void OnIndexRemoved(object keyOrIndex) =>
        IndexRemoved?.TryInvoke(this, new ClayIndexRemovedEventArgs(keyOrIndex));
}