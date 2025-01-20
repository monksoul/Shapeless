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
    ///     获取键或元素的数量
    /// </summary>
    public int Count => IsObject ? JsonCanvas.AsObject().Count : JsonCanvas.AsArray().Count;

    /// <summary>
    ///     获取键或元素的数量
    /// </summary>
    /// <remarks>同 <see cref="Count" />。在某些上下文中，<see cref="Length" /> 可能更常用于数组，<see cref="Count" /> 更常用于集合。</remarks>
    public int Length => Count;

    /// <summary>
    ///     判断是否未定义键、为空集合或为空数组
    /// </summary>
    public bool IsEmpty => Count == 0;

    /// <summary>
    ///     获取键或索引的列表
    /// </summary>
    public IEnumerable<object> Keys => AsEnumerable().Select(u => u.Key);

    /// <summary>
    ///     获取值或元素的列表
    /// </summary>
    public IEnumerable<dynamic?> Values => AsEnumerable().Select(u => u.Value);

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    ///     获取循环访问元素的枚举数
    /// </summary>
    /// <returns>
    ///     <see cref="IEnumerator{T}" />
    /// </returns>
    public IEnumerator<KeyValuePair<object, dynamic?>> GetEnumerator() => AsEnumerable().GetEnumerator();

    /// <summary>
    ///     获取单一对象或集合或数组的迭代器
    /// </summary>
    /// <returns>
    ///     <see cref="IEnumerable{T}" />
    /// </returns>
    public IEnumerable<KeyValuePair<object, dynamic?>> AsEnumerable() => IsObject
        ? AsEnumerableObject().Select(u => new KeyValuePair<object, dynamic?>(u.Key, u.Value))
        : AsEnumerableArray().Select(u => new KeyValuePair<object, dynamic?>(u.Key, u.Value));

    /// <summary>
    ///     获取单一对象的迭代器
    /// </summary>
    /// <returns>
    ///     <see cref="IEnumerable{T}" />
    /// </returns>
    public IEnumerable<KeyValuePair<string, dynamic?>> AsEnumerableObject()
    {
        // 检查是否是集合或数组实例调用
        ThrowIfMethodCalledOnArrayCollection(nameof(AsEnumerableObject));

        // 获取循环访问 JsonObject 的枚举数
        using var enumerator = JsonCanvas.AsObject().GetEnumerator();

        // 遍历 JsonObject 键值对
        while (enumerator.MoveNext())
        {
            // 获取当前的键值对
            var current = enumerator.Current;

            yield return new KeyValuePair<string, dynamic?>(current.Key, DeserializeNode(current.Value, Options));
        }
    }

    /// <summary>
    ///     获取集合或数组的迭代器
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

        // 遍历 JsonArray 项
        while (enumerator.MoveNext())
        {
            // 获取当前的元素
            var current = enumerator.Current;

            yield return new KeyValuePair<int, dynamic?>(index++, DeserializeNode(current, Options));
        }
    }

    /// <summary>
    ///     遍历 <see cref="Clay" />
    /// </summary>
    public void ForEach(Action<dynamic> action)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(action);

        ForEach((_, item) => action(item));
    }

    /// <summary>
    ///     遍历 <see cref="Clay" />
    /// </summary>
    public void ForEach(Action<object, dynamic> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        // 逐条遍历
        foreach (var (identifier, item) in AsEnumerable())
        {
            action(identifier, item);
        }
    }
}