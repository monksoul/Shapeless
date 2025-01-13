// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless;

/// <summary>
///     流变对象模块帮助类
/// </summary>
internal static class Helpers
{
    /// <summary>
    ///     将 <see cref="JsonNode" /> 转换为目标类型
    /// </summary>
    /// <param name="jsonNode">
    ///     <see cref="JsonNode" />
    /// </param>
    /// <param name="resultType">转换的目标类型</param>
    /// <param name="jsonSerializerOptions">
    ///     <see cref="JsonSerializerOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="object" />
    /// </returns>
    internal static object? DeserializeNode(JsonNode? jsonNode, Type resultType,
        JsonSerializerOptions? jsonSerializerOptions = null) =>
        jsonNode.As(resultType,
            jsonSerializerOptions ??
            new JsonSerializerOptions(JsonSerializerOptions.Default) { Converters = { new ClayJsonConverter() } });
}