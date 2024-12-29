// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless;

/// <summary>
///     <c>UTF-8</c> 格式的 <see cref="StringWriter" />
/// </summary>
internal sealed class Utf8StringWriter : StringWriter
{
    /// <inheritdoc />
    public override Encoding Encoding => Encoding.UTF8;
}