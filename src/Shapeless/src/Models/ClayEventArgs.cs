// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless;

/// <summary>
///     <see cref="Clay" /> 对象事件数据
/// </summary>
public sealed class ClayEventArgs : EventArgs
{
    /// <summary>
    ///     <inheritdoc cref="ClayEventArgs" />
    /// </summary>
    /// <param name="identifier">标识符，可以是键（字符串）或索引（整数）或索引运算符（Index）或范围运算符（Range）</param>
    /// <param name="isFound">指示标识符是否存在</param>
    internal ClayEventArgs(object identifier, bool isFound)
    {
        Identifier = identifier;
        IsFound = isFound;
    }

    /// <summary>
    ///     标识符，可以是键（字符串）或索引（整数）或索引运算符（Index）或范围运算符（Range）
    /// </summary>
    public object Identifier { get; }

    /// <summary>
    ///     指示标识符是否存在
    /// </summary>
    public bool IsFound { get; }
}