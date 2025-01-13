// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless;

/// <summary>
///     流变对象事件委托
/// </summary>
public delegate void ClayEventHandler(dynamic sender, ClayEventArgs args);

/// <summary>
///     流变对象
/// </summary>
public partial class Clay
{
    /// <summary>
    ///     数据变更之前事件
    /// </summary>
    public event ClayEventHandler? Changing;

    /// <summary>
    ///     数据变更之后事件
    /// </summary>
    public event ClayEventHandler? Changed;

    /// <summary>
    ///     移除数据之前事件
    /// </summary>
    public event ClayEventHandler? Removing;

    /// <summary>
    ///     移除数据之后事件
    /// </summary>
    public event ClayEventHandler? Removed;

    /// <summary>
    ///     触发数据变更之前事件
    /// </summary>
    /// <param name="identifier">标识符，可以是键（字符串）或索引（整数）</param>
    internal void OnChanging(object identifier) => TryInvoke(Changing, identifier);

    /// <summary>
    ///     触发数据变更之后事件
    /// </summary>
    /// <param name="identifier">标识符，可以是键（字符串）或索引（整数）</param>
    internal void OnChanged(object identifier) => TryInvoke(Changed, identifier);

    /// <summary>
    ///     触发移除数据之前事件
    /// </summary>
    /// <param name="identifier">标识符，可以是键（字符串）或索引（整数）</param>
    internal void OnRemoving(object identifier) => TryInvoke(Removing, identifier);

    /// <summary>
    ///     触发移除数据之后事件
    /// </summary>
    /// <param name="identifier">标识符，可以是键（字符串）或索引（整数）</param>
    internal void OnRemoved(object identifier) => TryInvoke(Removed, identifier);

    /// <summary>
    ///     尝试执行事件处理程序
    /// </summary>
    /// <param name="handler">
    ///     <see cref="ClayEventHandler" />
    /// </param>
    /// <param name="identifier">标识符，可以是键（字符串）或索引（整数）</param>
    internal void TryInvoke(ClayEventHandler? handler, object identifier)
    {
        // 空检查
        if (handler is null)
        {
            return;
        }

        try
        {
            handler(this, new ClayEventArgs(identifier, Contains(identifier)));
        }
        catch (Exception)
        {
            // ignored
        }
    }
}