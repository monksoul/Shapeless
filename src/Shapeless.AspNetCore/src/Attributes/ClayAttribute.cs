// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless;

/// <summary>
///     流变对象模型绑定特性
/// </summary>
/// <remarks>示例代码：<c>[Clay] dynamic input</c>。</remarks>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class ClayAttribute : Attribute;