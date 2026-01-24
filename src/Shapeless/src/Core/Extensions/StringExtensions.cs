// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless.Core.Extensions;

/// <summary>
///     <see cref="string" /> 扩展类
/// </summary>
internal static partial class StringExtensions
{
    /// <summary>
    ///     验证字符串是否是 <c>application/x-www-form-urlencoded</c> 格式
    /// </summary>
    /// <param name="output">字符串</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool IsUrlEncodedFormFormat(this string output)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(output);

        return UrlEncodedFormFormatRegex().IsMatch(output);
    }

    /// <summary>
    ///     将 <c>application/x-www-form-urlencoded</c> 格式的字符串解析为 <see cref="JsonObject" />
    /// </summary>
    /// <param name="formData">URL 编码的表单数据字符串</param>
    /// <returns>
    ///     <see cref="JsonObject" />
    /// </returns>
    internal static JsonObject? ParseUrlEncodedFormToJsonObject(this string? formData)
    {
        // 尝试移除开头的 ?
        formData = formData?.TrimStart('?');

        // 空检查
        if (string.IsNullOrWhiteSpace(formData))
        {
            return null;
        }

        // 初始化 JsonObject 实例
        var root = new JsonObject();

        // 按 & 分割每个键值对
        foreach (var part in formData.Split('&'))
        {
            // 查找第一个 =
            var eqIndex = part.IndexOf('=');

            // 键名为空或不存在 = 则跳过
            if (eqIndex <= 0)
            {
                continue;
            }

            // URL 解码键和值
            var key = WebUtility.UrlDecode(part[..eqIndex]);
            var value = WebUtility.UrlDecode(part[(eqIndex + 1)..]);

            // 将键名（如 user[0][name]）拆分为 token：["user", "0", "name"]
            var tokens = key.Replace("]", "").Split('[');
            var current = root;
            var i = 0;

            // 逐层构建嵌套结构
            while (i < tokens.Length)
            {
                // 空检查
                var token = tokens[i];
                if (string.IsNullOrEmpty(token))
                {
                    i++;
                    continue;
                }

                // 检查是否是最后一项
                if (i == tokens.Length - 1)
                {
                    current[token] = value;
                    break;
                }

                // 下一项为数组索引，按数组处理
                var nextToken = tokens[i + 1];
                if (int.TryParse(nextToken, out var index) && index >= 0)
                {
                    // 确保当前 token 是一个 JsonArray
                    if (current[token] is not JsonArray array)
                    {
                        array = [];
                        current[token] = array;
                    }

                    // 扩容数组
                    while (array.Count <= index)
                    {
                        array.Add(new JsonObject());
                    }

                    // 进入该数组元素并跳过数组名和索引
                    current = (JsonObject)array[index]!;
                    i += 2;
                }
                else
                {
                    // 下一个 token 是普通属性名则视为对象名
                    if (current[token] is not JsonObject child)
                    {
                        child = new JsonObject();
                        current[token] = child;
                    }

                    current = child;
                    i++;
                }
            }
        }

        return root;
    }

    [GeneratedRegex(
        "^(?:(?:[a-zA-Z0-9-._~]|%[0-9A-Fa-f]{2})+=(?:[a-zA-Z0-9-._~+]|%[0-9A-Fa-f]{2})*)(?:&(?:[a-zA-Z0-9-._~]|%[0-9A-Fa-f]{2})+=(?:[a-zA-Z0-9-._~+]|%[0-9A-Fa-f]{2})*)*$",
        RegexOptions.IgnorePatternWhitespace)]
    private static partial Regex UrlEncodedFormFormatRegex();
}