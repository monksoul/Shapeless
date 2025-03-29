// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless.Core.Extensions;

/// <summary>
///     <see cref="string" /> 拓展类
/// </summary>
internal static class StringExtensions
{
    /// <summary>
    ///     解析符合键值对格式的字符串为键值对列表
    /// </summary>
    /// <param name="keyValueString">键值对格式的字符串</param>
    /// <param name="separators">分隔符字符数组</param>
    /// <param name="trimChar">要删除的前导字符</param>
    /// <returns>
    ///     <see cref="List{T}" />
    /// </returns>
    internal static List<KeyValuePair<string, string?>> ParseFormatKeyValueString(this string keyValueString,
        char[]? separators = null, char? trimChar = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(keyValueString);

        // 空检查
        if (string.IsNullOrWhiteSpace(keyValueString))
        {
            return [];
        }

        // 默认隔符为 &
        separators ??= ['&'];

        var pairs = (trimChar is null ? keyValueString : keyValueString.TrimStart(trimChar.Value)).Split(separators);
        return (from pair in pairs
            select pair.Split('=', 2) // 限制只分割一次
            into keyValue
            where keyValue.Length == 2
            select new KeyValuePair<string, string?>(keyValue[0].Trim(), keyValue[1])).ToList();
    }
}