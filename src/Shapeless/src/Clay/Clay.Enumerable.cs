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
        ? AsEnumerateObject().Select(u => new KeyValuePair<object, dynamic?>(u.Key, u.Value))
        : AsEnumerateArray().Select(u => new KeyValuePair<object, dynamic?>(u.Key, u.Value));

    /// <summary>
    ///     获取单一对象的迭代器
    /// </summary>
    /// <returns>
    ///     <see cref="IEnumerable{T}" />
    /// </returns>
    public IEnumerable<KeyValuePair<string, dynamic?>> AsEnumerateObject()
    {
        // 检查是否是集合或数组实例调用
        ThrowIfMethodCalledOnArrayCollection(nameof(AsEnumerateObject));

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
    public IEnumerable<KeyValuePair<int, dynamic?>> AsEnumerateArray()
    {
        // 检查是否是单一对象实例调用
        ThrowIfMethodCalledOnSingleObject(nameof(AsEnumerateArray));

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
    /// <param name="action">自定义委托</param>
    public void ForEach(Action<dynamic> action)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(action);

        ForEach((_, item) => action(item));
    }

    /// <summary>
    ///     遍历 <see cref="Clay" />
    /// </summary>
    /// <param name="action">自定义委托</param>
    public void ForEach(Action<object, dynamic> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        // 逐条遍历
        foreach (var (identifier, item) in AsEnumerable())
        {
            action(identifier, item);
        }
    }

    /// <summary>
    ///     遍历 <see cref="Clay " /> 并返回映射后的 <typeparamref name="T" /> 集合
    /// </summary>
    /// <param name="func">自定义委托</param>
    /// <typeparam name="T">目标结果类型</typeparam>
    /// <returns>
    ///     <see cref="IEnumerable{T}" />
    /// </returns>
    public IEnumerable<T> Map<T>(Func<dynamic, T> func)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(func);

        return Map<T>((_, item) => func(item));
    }

    /// <summary>
    ///     遍历 <see cref="Clay " /> 并返回映射后的 <typeparamref name="T" /> 集合
    /// </summary>
    /// <param name="func">自定义委托</param>
    /// <typeparam name="T">目标结果类型</typeparam>
    /// <returns>
    ///     <see cref="IEnumerable{T}" />
    /// </returns>
    public IEnumerable<T> Map<T>(Func<object, dynamic, T> func)
    {
        ArgumentNullException.ThrowIfNull(func);

        // 逐条遍历
        foreach (var (identifier, item) in AsEnumerable())
        {
            yield return func(identifier, item);
        }
    }

    /// <summary>
    ///     根据条件过滤并返回新的 <see cref="Clay" />
    /// </summary>
    /// <param name="predicate">自定义条件委托</param>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    public Clay Filter(Func<dynamic, bool> predicate)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(predicate);

        return Filter((_, item) => predicate(item));
    }

    /// <summary>
    ///     根据条件过滤并返回新的 <see cref="Clay" />
    /// </summary>
    /// <param name="predicate">自定义条件委托</param>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    public Clay Filter(Func<object, dynamic, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        // 根据条件过滤
        var keyValuePairs = AsEnumerable().Where(u => predicate(u.Key, u.Value));

        return Parse(IsObject ? keyValuePairs.ToDictionary() : keyValuePairs.Select(u => u.Value), Options);
    }
}