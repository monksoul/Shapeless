// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless;

/// <summary>
///     粘土对象的基本类型
/// </summary>
/// <remarks>用于区分是单一对象还是集合（数组）形式。</remarks>
public enum ClayType
{
    /// <summary>
    ///     单一对象
    /// </summary>
    /// <remarks>缺省值。</remarks>
    Object = 0,

    /// <summary>
    ///     集合（数组）形式
    /// </summary>
    Array
}