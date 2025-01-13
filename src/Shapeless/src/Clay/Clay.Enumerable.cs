// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless;

/// <summary>
///     流变对象
/// </summary>
public partial class Clay
{
    /// <summary>
    ///     元素数量
    /// </summary>
    public int Count => IsObject ? JsonCanvas.AsObject().Count : JsonCanvas.AsArray().Count;

    /// <summary>
    ///     元素数量
    /// </summary>
    public int Length => Count;

    /// <summary>
    ///     是否为空元素
    /// </summary>
    public bool IsEmpty => Count == 0;

    /// <summary>
    ///     获取键或索引集合
    /// </summary>
    public IEnumerable<object> Indexes => AsEnumerable().Select(u => u.Key);

    /// <summary>
    ///     获取键或索引集合
    /// </summary>
    public IEnumerable<object> Keys => Indexes;

    /// <summary>
    ///     获取值集合
    /// </summary>
    public IEnumerable<dynamic?> Values => AsEnumerable().Select(u => u.Value);

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    ///     返回循环访问元素的枚举数
    /// </summary>
    /// <returns>
    ///     <see cref="IEnumerator{T}" />
    /// </returns>
    public IEnumerator<KeyValuePair<object, dynamic?>> GetEnumerator() => AsEnumerable().GetEnumerator();

    /// <summary>
    ///     返回类型化为 <see cref="IEnumerable{T}" /> 的输入
    /// </summary>
    /// <returns>
    ///     <see cref="IEnumerable{T}" />
    /// </returns>
    public IEnumerable<KeyValuePair<object, dynamic?>> AsEnumerable() => IsObject
        ? AsEnumerableObject().Select(u => new KeyValuePair<object, dynamic?>(u.Key, u.Value))
        : AsEnumerableArray().Select(u => new KeyValuePair<object, dynamic?>(u.Key, u.Value));

    /// <summary>
    ///     枚举 <see cref="JsonCanvas" /> 作为对象时的键值对
    /// </summary>
    /// <returns>
    ///     <see cref="IEnumerable{T}" />
    /// </returns>
    public IEnumerable<KeyValuePair<string, dynamic?>> AsEnumerableObject()
    {
        // 检查是否是集合（数组）实例调用
        ThrowIfMethodCalledOnArrayCollection(nameof(AsEnumerableObject));

        // 获取循环访问 JsonObject 的枚举数
        using var enumerator = JsonCanvas.AsObject().GetEnumerator();

        // 遍历 JsonObject 每个键值对
        while (enumerator.MoveNext())
        {
            // 获取当前的键值对
            var current = enumerator.Current;

            yield return new KeyValuePair<string, dynamic?>(current.Key, DeserializeNode(current.Value, Options));
        }
    }

    /// <summary>
    ///     枚举 <see cref="JsonCanvas" /> 作为数组时的元素
    /// </summary>
    /// <returns>
    ///     <see cref="IEnumerable{T}" />
    /// </returns>
    public IEnumerable<KeyValuePair<int, dynamic?>> AsEnumerableArray()
    {
        // 检查是否是单一对象实例调用
        ThrowIfMethodCalledOnSingleObject(nameof(AsEnumerableArray));

        // 获取循环访问 JsonArray 的枚举数
        using var enumerator = JsonCanvas.AsArray().GetEnumerator();

        // 定义索引变量用于记录数组中元素的位置
        var index = 0;

        // 遍历 JsonArray 每个元素
        while (enumerator.MoveNext())
        {
            // 获取当前的元素
            var current = enumerator.Current;

            yield return new KeyValuePair<int, dynamic?>(index++, DeserializeNode(current, Options));
        }
    }
}