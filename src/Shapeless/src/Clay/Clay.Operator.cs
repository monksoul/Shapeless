// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless;

/// <summary>
///     流变对象
/// </summary>
public partial class Clay
{
    /// <inheritdoc />
    public bool Equals(Clay? other)
    {
        // 检查是否是相同的实例
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        // 空检查及基础类型检查
        if (other is null || Type != other.Type)
        {
            return false;
        }

        return JsonNode.DeepEquals(JsonCanvas, other.JsonCanvas);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || Equals(obj as Clay);

    /// <summary>
    ///     重载 == 运算符
    /// </summary>
    /// <param name="left">
    ///     <see cref="Clay" />
    /// </param>
    /// <param name="right">
    ///     <see cref="Clay" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public static bool operator ==(Clay? left, Clay? right) => Equals(left, right);

    /// <summary>
    ///     重载 != 运算符
    /// </summary>
    /// <param name="left">
    ///     <see cref="Clay" />
    /// </param>
    /// <param name="right">
    ///     <see cref="Clay" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public static bool operator !=(Clay? left, Clay? right) => !(left == right);

    /// <summary>
    ///     支持 <see cref="Clay" /> 类型隐式转换为 <see cref="string" />
    /// </summary>
    /// <param name="clay">
    ///     <see cref="Clay" />
    /// </param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public static implicit operator string(in Clay clay) => clay.JsonCanvas.ToJsonString();

    /// <summary>
    ///     支持 <see cref="Clay" /> 类型隐式转换为 <see cref="Dictionary{TKey,TValue}" />
    /// </summary>
    /// <param name="clay">
    ///     <see cref="Clay" />
    /// </param>
    /// <returns>
    ///     <see cref="Dictionary{TKey,TValue}" />
    /// </returns>
    public static implicit operator Dictionary<string, object?>(in Clay clay) =>
        clay.As<Dictionary<string, object?>>()!;

    /// <summary>
    ///     支持 <see cref="string" /> 类型隐式转换为 <see cref="Clay" />
    /// </summary>
    /// <param name="str">
    ///     <see cref="string" />
    /// </param>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    public static implicit operator Clay(string? str) => Parse(str);

    /// <summary>
    ///     支持 <see cref="Stream" /> 类型隐式转换为 <see cref="Clay" />
    /// </summary>
    /// <param name="stream">
    ///     <see cref="Stream" />
    /// </param>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    public static implicit operator Clay(Stream? stream) => Parse(stream);

    /// <summary>
    ///     支持 <see cref="byte" />[] 类型隐式转换为 <see cref="Clay" />
    /// </summary>
    /// <param name="bytes">
    ///     <see cref="byte" />[]
    /// </param>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    public static implicit operator Clay(byte[]? bytes) => Parse(bytes);

    /// <summary>
    ///     支持 <see cref="JsonNode" /> 类型隐式转换为 <see cref="Clay" />
    /// </summary>
    /// <param name="jsonNode">
    ///     <see cref="JsonNode" />
    /// </param>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    public static implicit operator Clay(JsonNode? jsonNode) => Parse(jsonNode);

    /// <summary>
    ///     支持 <see cref="Dictionary{TKey,TValue}" /> 类型隐式转换为 <see cref="Clay" />
    /// </summary>
    /// <param name="dic">
    ///     <see cref="Dictionary{TKey,TValue}" />
    /// </param>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    public static implicit operator Clay(Dictionary<string, object?>? dic) => Parse(dic);

    /// <summary>
    ///     支持 <see cref="ExpandoObject" /> 类型隐式转换为 <see cref="Clay" />
    /// </summary>
    /// <param name="expandoObject">
    ///     <see cref="ExpandoObject" />
    /// </param>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    public static implicit operator Clay(ExpandoObject? expandoObject) => Parse(expandoObject);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        // 初始化 HashCode 实例
        var hashCode = new HashCode();

        // 递归计算 JsonCanvas 哈希值
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        ComputeHash(JsonCanvas, ref hashCode);

        return hashCode.ToHashCode();

        // 递归计算 JsonNode 哈希值
        static void ComputeHash(JsonNode? jsonNode, ref HashCode hash)
        {
            // 空检查
            if (jsonNode is null)
            {
                hash.Add(0);
                return;
            }

            // 根据 JSON 值的种类分别计算哈希值
            switch (jsonNode.GetValueKind())
            {
                // 对象
                case JsonValueKind.Object:
                    // 预处理键值对（按键名排序）
                    var sortedProperties = jsonNode.AsObject().OrderBy(p => p.Key, StringComparer.Ordinal);

                    // 遍历所有键值对，递归计算哈希
                    foreach (var (key, value) in sortedProperties)
                    {
                        hash.Add(key);
                        ComputeHash(value, ref hash);
                    }

                    break;
                // 数组
                case JsonValueKind.Array:
                    // 遍历数组每一项，按顺序递归计算
                    foreach (var item in jsonNode.AsArray())
                    {
                        ComputeHash(item, ref hash);
                    }

                    break;
                // 字符串
                case JsonValueKind.String:
                    hash.Add(jsonNode.GetValue<string>());
                    break;
                // 数值
                case JsonValueKind.Number:
                    hash.Add(jsonNode.GetValue<decimal>());
                    break;
                // True
                case JsonValueKind.True:
                    hash.Add(true);
                    break;
                // False
                case JsonValueKind.False:
                    hash.Add(false);
                    break;
                // 其他类型
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                default:
                    hash.Add(0);
                    break;
            }
        }
    }
}