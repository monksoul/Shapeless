// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless;

/// <summary>
///     帕斯卡（大驼峰）命名策略
/// </summary>
public sealed partial class PascalCaseNamingPolicy : JsonNamingPolicy
{
    /// <summary>
    ///     用于分割字符串中的单词的正则表达式
    /// </summary>
    internal static readonly Regex _splitter = WordBoundaryRegex();

    /// <inheritdoc />
    public override string ConvertName(string name)
    {
        // 空检查
        if (string.IsNullOrEmpty(name))
        {
            return name;
        }

        // 对于全部大写的单词（如 'URL', 'ID'），保持不变
        if (name.All(char.IsUpper) && !name.Any(char.IsDigit))
        {
            return name;
        }

        // 初始化 StringBuilder 实例
        var result = new StringBuilder();

        // 将字符串按非字母数字字符、大小写字母变化处分割成多个部分
        var parts = _splitter.Split(name);

        // 遍历多个部分逐个处理
        foreach (var part in parts)
        {
            // 空检查
            if (string.IsNullOrWhiteSpace(part))
            {
                continue;
            }

            // 如果是连续的大写字母，假设是缩写，保持不变
            if (part.Length > 1 && part.All(char.IsUpper))
            {
                result.Append(part);
            }
            else
            {
                // 对每个部分的第一个字母进行大写转换，其余小写
                result.Append(char.ToUpper(part[0]));

                if (part.Length > 1)
                {
                    result.Append(part[1..].ToLower());
                }
            }
        }

        return result.ToString();
    }

    /// <summary>
    ///     单词边界正则表达式
    /// </summary>
    /// <returns>
    ///     <see cref="Regex" />
    /// </returns>
    [GeneratedRegex(
        @"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])|(?<=\d)(?=\D)|(?<=\D)(?=\d)")]
    private static partial Regex WordBoundaryRegex();
}