// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

using Binder = Microsoft.CSharp.RuntimeBinder.Binder;

namespace Shapeless;

/// <summary>
///     流变对象
/// </summary>
public sealed partial class Clay : DynamicObject, IEnumerable<KeyValuePair<object, object?>>
{
    /// <summary>
    ///     获取 <see cref="InvokeMemberBinder" /> 类型的 <c>TypeArguments</c> 属性访问器
    /// </summary>
    /// <remarks>实际上获取的是内部类型 <c>CSharpInvokeMemberBinder</c> 的 <c>TypeArguments</c> 属性访问器。</remarks>
    internal static readonly Lazy<Func<object, object?>> _getCSharpInvokeMemberBinderTypeArguments = new(() =>
    {
        // 获取内部的 CSharpInvokeMemberBinder 类型
        var csharpInvokeMemberBinderType =
            typeof(Binder).Assembly.GetType("Microsoft.CSharp.RuntimeBinder.CSharpInvokeMemberBinder")!;

        // 获取 TypeArguments 属性对象
        var typeArgumentsProperty =
            csharpInvokeMemberBinderType.GetProperty("TypeArguments", BindingFlags.Public | BindingFlags.Instance)!;

        // 创建 TypeArguments 属性访问器
        return csharpInvokeMemberBinderType.CreatePropertyGetter(typeArgumentsProperty);
    });

    /// <summary>
    ///     <inheritdoc cref="Clay" />
    /// </summary>
    /// <param name="jsonNode">
    ///     <see cref="JsonNode" />
    /// </param>
    /// <param name="options">
    ///     <see cref="ClayOptions" />
    /// </param>
    /// <exception cref="NotSupportedException"></exception>
    internal Clay(JsonNode? jsonNode, ClayOptions? options = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(jsonNode, "obj");

        // 初始化 ClayOptions
        Options = options ?? ClayOptions.Default;

        // 处理非对象和非数组类型的 JSON 节点
        var jsonCanvas = jsonNode.GetValueKind() is JsonValueKind.Object or JsonValueKind.Array
            ? jsonNode
            : JsonNode.Parse($"{{\"{Options.ScalarValueKey}\":{jsonNode.ToJsonString()}}}",
                new JsonNodeOptions { PropertyNameCaseInsensitive = Options.PropertyNameCaseInsensitive })!;

        JsonCanvas = jsonCanvas;

        IsObject = JsonCanvas is JsonObject;
        IsArray = JsonCanvas is JsonArray;
    }

    /// <summary>
    ///     <inheritdoc cref="ClayOptions" />
    /// </summary>
    internal ClayOptions Options { get; private set; }

    /// <summary>
    ///     JSON 格式的画布
    /// </summary>
    /// <remarks>用于作为 <see cref="Clay" /> 的核心数据容器。</remarks>
    internal JsonNode JsonCanvas { get; private set; }

    /// <summary>
    ///     根据键或索引获取值
    /// </summary>
    /// <param name="index">键或索引</param>
    /// <returns>
    ///     <see cref="object" />
    /// </returns>
    internal object? GetValue(object index)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(index);

        return DeserializeNode(FindNode(index), Options);
    }

    /// <summary>
    ///     根据键或索引设置值
    /// </summary>
    /// <param name="index">键或索引</param>
    /// <param name="value">值</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal bool SetValue(object index, object? value)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(index);

        return IsObject ? SetNodeInObject(index, value) : SetNodeInArray(index, value);
    }

    /// <summary>
    ///     根据键或索引删除值
    /// </summary>
    /// <param name="index">键或索引</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal bool RemoveValue(object index)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(index);

        return IsObject ? RemoveNodeFromObject(index) : RemoveNodeFromArray(index);
    }

    /// <summary>
    ///     根据键或索引查找 <see cref="JsonNode" /> 节点
    /// </summary>
    /// <param name="index">键或索引</param>
    /// <returns>
    ///     <see cref="JsonNode" />
    /// </returns>
    internal JsonNode? FindNode(object index)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(index);

        return IsObject ? GetNodeFromObject(index) : GetNodeFromArray(index);
    }

    /// <summary>
    ///     根据键获取 <see cref="JsonNode" /> 节点
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>
    ///     <see cref="JsonNode" />
    /// </returns>
    /// <exception cref="KeyNotFoundException"></exception>
    internal JsonNode? GetNodeFromObject(object key)
    {
        // 将键转换为字符串类型
        var stringKey = key.ToString()!;

        // 处理嵌套带空传播字符 ? 的索引键
        var indexKey = ProcessNestedNullPropagationIndexKey(stringKey);

        // 将 JsonCanvas 转换为 JsonObject 实例
        var jsonObject = JsonCanvas.AsObject();

        // 根据键获取 JSON 节点
        if (jsonObject.TryGetPropertyValue(indexKey, out var jsonNode))
        {
            return jsonNode;
        }

        // 检查是否允许访问不存在的属性
        if (!Options.AllowMissingProperty)
        {
            throw new KeyNotFoundException($"The property `{indexKey}` was not found in the Clay.");
        }

        // 检查是否需要处理嵌套带空传播字符 ? 的索引键
        if (indexKey == stringKey || !Options.AutoCreateNestedObjects)
        {
            return null;
        }

        SetValue(indexKey, new Clay(Options));
        return FindNode(indexKey);
    }

    /// <summary>
    ///     根据索引获取 <see cref="JsonNode" /> 节点
    /// </summary>
    /// <param name="index">索引</param>
    /// <returns>
    ///     <see cref="JsonNode" />
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal JsonNode? GetNodeFromArray(object index)
    {
        // 检查数组索引合法性
        EnsureLegalArrayIndex(index, out var intIndex);

        // 将 JsonCanvas 转换为 JsonArray 实例
        var jsonArray = JsonCanvas.AsArray();

        // 获取 JsonArray 长度
        var count = jsonArray.Count;

        // 检查索引小于数组长度
        if (intIndex < count)
        {
            return jsonArray[intIndex];
        }

        // 检查是否允许访问越界的数组索引
        if (!Options.AllowIndexOutOfRange)
        {
            ThrowIfOutOfRange(intIndex, count);
        }

        // 检查是否自动创建嵌套的数组实例
        if (Options is not { AutoCreateNestedArrays: true, AutoExpandArrayWithNulls: true })
        {
            return null;
        }

        SetValue(intIndex, new Clay(ClayType.Array, Options));
        return FindNode(intIndex);
    }

    /// <summary>
    ///     根据键设置 <see cref="JsonNode" /> 节点
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">属性值</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal bool SetNodeInObject(object key, object? value)
    {
        // 将键转换为字符串类型
        var stringKey = key.ToString()!;

        // 处理嵌套带空传播字符 ? 的索引键
        var indexKey = ProcessNestedNullPropagationIndexKey(stringKey);

        // 将 JsonCanvas 转换为 JsonObject 实例
        var jsonObject = JsonCanvas.AsObject();

        jsonObject[indexKey] = SerializeToNode(value, Options);

        return true;
    }

    /// <summary>
    ///     根据索引设置 <see cref="JsonNode" /> 节点
    /// </summary>
    /// <param name="index">索引</param>
    /// <param name="value">元素值</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal bool SetNodeInArray(object index, object? value)
    {
        // 检查数组索引合法性
        EnsureLegalArrayIndex(index, out var intIndex);

        // 将 JsonCanvas 转换为 JsonArray 实例
        var jsonArray = JsonCanvas.AsArray();

        // 获取 JsonArray 长度
        var count = jsonArray.Count;

        // 检查索引小于数组长度
        if (intIndex < count)
        {
            jsonArray[intIndex] = SerializeToNode(value, Options);
        }
        // 检查索引是否等于长度，如果是则追加
        else if (intIndex == count)
        {
            jsonArray.Add(SerializeToNode(value, Options));
        }
        // 检查是否允许访问越界的数组，如果是则采用补位方式
        else if (Options.AllowIndexOutOfRange)
        {
            // 检查是否需要进行补位操作
            if (!Options.AutoExpandArrayWithNulls)
            {
                return false;
            }

            // 补位操作
            while (jsonArray.Count < intIndex)
            {
                jsonArray.Add(null);
            }

            jsonArray.Add(SerializeToNode(value, Options));
        }
        else
        {
            ThrowIfOutOfRange(intIndex, count);
        }

        return true;
    }

    /// <summary>
    ///     根据键删除 <see cref="JsonNode" /> 节点
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal bool RemoveNodeFromObject(object key)
    {
        // 将键转换为字符串类型
        var stringKey = key.ToString()!;

        // 处理嵌套带空传播字符 ? 的索引键
        var indexKey = ProcessNestedNullPropagationIndexKey(stringKey);

        // 将 JsonCanvas 转换为 JsonObject 实例
        var jsonObject = JsonCanvas.AsObject();

        // 检查键是否定义
        if (jsonObject.ContainsKey(indexKey))
        {
            return jsonObject.Remove(indexKey);
        }

        // 检查是否允许访问不存在的属性
        if (!Options.AllowMissingProperty)
        {
            throw new KeyNotFoundException($"The property `{indexKey}` was not found in the Clay.");
        }

        return false;
    }

    /// <summary>
    ///     根据索引删除 <see cref="JsonNode" /> 节点
    /// </summary>
    /// <param name="index">索引</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal bool RemoveNodeFromArray(object index)
    {
        // 检查数组索引合法性
        EnsureLegalArrayIndex(index, out var intIndex);

        // 将 JsonCanvas 转换为 JsonArray 实例
        var jsonArray = JsonCanvas.AsArray();

        // 获取 JsonArray 长度
        var count = jsonArray.Count;

        // 检查索引小于数组长度
        if (intIndex < count)
        {
            jsonArray.RemoveAt(intIndex);
            return true;
        }

        // 检查是否允许访问越界的数组索引
        if (!Options.AllowIndexOutOfRange)
        {
            ThrowIfOutOfRange(intIndex, count);
        }

        return false;
    }

    /// <summary>
    ///     将对象序列化成 <see cref="JsonNode" /> 实例
    /// </summary>
    /// <param name="obj">
    ///     <see cref="object" />
    /// </param>
    /// <param name="options">
    ///     <see cref="ClayOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="JsonNode" />
    /// </returns>
    internal static JsonNode? SerializeToNode(object? obj, ClayOptions? options = null) =>
        obj switch
        {
            null => null,
            JsonNode jsonNode => jsonNode.DeepClone(),
            Clay clay => clay.DeepClone(options).JsonCanvas,
            _ => JsonSerializer.SerializeToNode(obj, options?.JsonSerializerOptions)
        };

    /// <summary>
    ///     将 <see cref="JsonNode" /> 转换为对象实例
    /// </summary>
    /// <param name="jsonNode">
    ///     <see cref="JsonNode" />
    /// </param>
    /// <param name="options">
    ///     <see cref="ClayOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="object" />
    /// </returns>
    internal static object? DeserializeNode(JsonNode? jsonNode, ClayOptions? options = null) =>
        jsonNode?.GetValueKind() switch
        {
            JsonValueKind.String when options?.AutoConvertToDateTime == true &&
                                      DateTime.TryParse(jsonNode.GetValue<string>(), out var dateTime) => dateTime,
            JsonValueKind.String => jsonNode.GetValue<string>(),
            JsonValueKind.Number => jsonNode.GetNumericValue(),
            JsonValueKind.True or JsonValueKind.False => jsonNode.GetValue<bool>(),
            // TODO: 避免每次获取会触发 new 操作，建议未来版本进行缓存优化
            JsonValueKind.Object or JsonValueKind.Array => new Clay(jsonNode, options),
            _ => null
        };

    /// <summary>
    ///     重建 <see cref="Clay" /> 实例
    /// </summary>
    /// <param name="options">
    ///     <see cref="ClayOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    internal Clay Rebuilt(ClayOptions? options = null)
    {
        // 初始化 ClayOptions 实例
        var clayOptions = options ?? ClayOptions.Default;

        // 如果新旧选项对于属性名称大小写不敏感的设置相同，则无需重建 JsonCanvas；否则重建。
        if (clayOptions.PropertyNameCaseInsensitive != Options.PropertyNameCaseInsensitive)
        {
            JsonCanvas = JsonNode.Parse(JsonCanvas.ToJsonString(),
                new JsonNodeOptions { PropertyNameCaseInsensitive = clayOptions.PropertyNameCaseInsensitive })!;
        }

        Options = clayOptions;

        return this;
    }

    /// <summary>
    ///     处理嵌套带空传播字符 <c>?</c> 的索引键
    /// </summary>
    /// <param name="indexKey">索引键</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal string ProcessNestedNullPropagationIndexKey(string indexKey) =>
        !Options.AutoCreateNestedObjects ? indexKey : indexKey.TrimEnd('?');

    /// <summary>
    ///     枚举 <see cref="JsonCanvas" /> 作为对象时的键值对
    /// </summary>
    /// <returns>
    ///     <see cref="IEnumerable{T}" />
    /// </returns>
    internal IEnumerable<KeyValuePair<string, dynamic?>> EnumerateObject()
    {
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
    internal IEnumerable<KeyValuePair<int, dynamic?>> EnumerateArray()
    {
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

    /// <summary>
    ///     抛出越界的数组索引异常
    /// </summary>
    /// <param name="index">索引</param>
    /// <param name="count">数组长度</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [DoesNotReturn]
    internal static void ThrowIfOutOfRange(int index, int count)
    {
        // 构建数组越界的错误细节
        var errorDetails = count switch
        {
            0 => "The array is empty, so no indices are valid.",
            1 => "The array contains a single element at index 0.",
            _ => $"The allowed index range for the array is 0 to {count - 1}."
        };

        throw new ArgumentOutOfRangeException(nameof(index), $"Index `{index}` is out of range. {errorDetails}");
    }

    /// <summary>
    ///     检查数组索引合法性
    /// </summary>
    /// <param name="index"><see cref="object" /> 类型索引</param>
    /// <param name="intIndex">整数索引</param>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal static void EnsureLegalArrayIndex(object index, out int intIndex)
    {
        // 将索引转换为字符串类型
        var stringIndex = index.ToString();

        // 尝试将字符串索引转换为整数索引
        if (!int.TryParse(stringIndex, out intIndex))
        {
            throw new InvalidOperationException($"The provided index `{stringIndex}` is not a valid array index.");
        }

        // 检查索引是否小于 0
        if (intIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index),
                "Negative indices are not allowed. Index must be greater than or equal to 0.");
        }
    }

    /// <summary>
    ///     如果当前实例是单一对象且尝试调用不支持的操作，则抛出异常
    /// </summary>
    /// <param name="method">方法名</param>
    /// <exception cref="NotSupportedException"></exception>
    internal void ThrowIfMethodCalledOnSingleObject(string method)
    {
        // 检查是否是单一对象
        if (IsObject)
        {
            throw new NotSupportedException(
                $"`{method}` method can only be used for array or collection operations.");
        }
    }
}