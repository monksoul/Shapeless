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

        return IsObject ? AreObjectEqual(this, other) : AreArrayEqual(this, other);
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

    /// <inheritdoc />
    public override int GetHashCode()
    {
        // 初始化 HashCode 实例
        var hash = new HashCode();

        if (IsObject)
        {
            // 预处理键值对（排序）
            var sortedEntries = AsEnumerateObject().OrderBy(kvp => kvp.Key, StringComparer.Ordinal);

            // 遍历键值对集合
            foreach (var (key, value) in sortedEntries)
            {
                // 递归计算键和值的哈希码
                hash.Add(key?.GetHashCode() ?? 0);
                hash.Add(value?.GetHashCode() ?? 0);
            }
        }
        else
        {
            // 遍历集合或数组集合
            foreach (var value in AsEnumerateArray())
            {
                // 递归计算元素的哈希码
                hash.Add(value?.GetHashCode() ?? 0);
            }
        }

        return hash.ToHashCode();
    }

    /// <summary>
    ///     检查两个单一对象实例是否相等
    /// </summary>
    /// <param name="clay1">
    ///     <see cref="Clay" />
    /// </param>
    /// <param name="clay2">
    ///     <see cref="Clay" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool AreObjectEqual(Clay clay1, Clay clay2) =>
        clay1.Count == clay2.Count && clay1.All((dynamic? item) =>
            clay2.HasProperty(item?.Key) && object.Equals(item?.Value, clay2[item?.Key]));

    /// <summary>
    ///     检查两个集合或数组实例是否相等
    /// </summary>
    /// <param name="clay1">
    ///     <see cref="Clay" />
    /// </param>
    /// <param name="clay2">
    ///     <see cref="Clay" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool AreArrayEqual(Clay clay1, Clay clay2)
    {
        // 检查集合或数组长度是否相等
        if (clay1.Count != clay2.Count)
        {
            return false;
        }

        // 遍历检查每一项是否相等
        for (var i = 0; i < clay1.Count; i++)
        {
            if (!Equals(clay1[i], clay2[i]))
            {
                return false;
            }
        }

        return true;
    }
}