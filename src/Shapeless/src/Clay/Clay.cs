// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless;

/// <summary>
///     流变对象
/// </summary>
public partial class Clay : DynamicObject, IEnumerable<object?>, IFormattable, IEquatable<Clay>
{
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

        // 创建 JsonNode 选项
        var (jsonNodeOptions, jsonDocumentOptions) = CreateJsonNodeOptions(Options);

        // 处理非对象和非数组类型的 JSON 节点
        var jsonCanvas = jsonNode.GetValueKind() is JsonValueKind.Object or JsonValueKind.Array
            ? jsonNode
            : JsonNode.Parse($"{{\"{Options.ScalarValueKey}\":{jsonNode.ToJsonString()}}}", jsonNodeOptions,
                jsonDocumentOptions)!;

        JsonCanvas = jsonCanvas;

        IsObject = JsonCanvas is JsonObject;
        IsArray = JsonCanvas is JsonArray;
        Type = IsObject ? ClayType.Object : ClayType.Array;
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
    ///     单一对象自定义委托字典
    /// </summary>
    internal IDictionary<string, Delegate?> DelegateMap { get; } = new Dictionary<string, Delegate?>();

    /// <summary>
    ///     根据标识符获取值
    /// </summary>
    /// <param name="identifier">标识符，可以是键（字符串）或索引（整数）或索引运算符（Index）或范围运算符（Range）</param>
    /// <returns>
    ///     <see cref="object" />
    /// </returns>
    internal object? GetValue(object identifier)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(identifier);

        return TryGetDelegate(identifier, out var @delegate)
            ? @delegate
            : DeserializeNode(FindNode(identifier), Options);
    }

    /// <summary>
    ///     根据标识符设置值
    /// </summary>
    /// <param name="identifier">标识符，可以是键（字符串）或索引（整数）或索引运算符（Index）或范围运算符（Range）</param>
    /// <param name="value">值</param>
    /// <param name="insert">是否作为在指定位置插入</param>
    internal void SetValue(object identifier, object? value, bool insert = false)
    {
        // 确保当前实例不在只读模式下
        EnsureNotReadOnlyBeforeModify();

        // 空检查
        ArgumentNullException.ThrowIfNull(identifier);

        // 检查是否是单一对象
        if (IsObject)
        {
            SetNodeInObject(identifier, value);
        }
        else
        {
            SetNodeInArray(identifier, value, insert);
        }
    }

    /// <summary>
    ///     根据标识符移除值
    /// </summary>
    /// <param name="identifier">标识符，可以是键（字符串）或索引（整数）或索引运算符（Index）或范围运算符（Range）</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal bool RemoveValue(object identifier)
    {
        // 确保当前实例不在只读模式下
        EnsureNotReadOnlyBeforeModify();

        // 空检查
        ArgumentNullException.ThrowIfNull(identifier);

        // 根据标识符移除值并获取结果
        return IsObject
            ? RemoveNodeFromObject(identifier)
            : RemoveNodeFromArray(identifier);
    }

    /// <summary>
    ///     根据键获取 <see cref="JsonNode" /> 节点
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>
    ///     <see cref="JsonNode" />
    /// </returns>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="KeyNotFoundException"></exception>
    internal JsonNode? GetNodeFromObject(object key)
    {
        // 检查键是否是不受支持的类型
        ThrowIfUnsupportedKeyType(key);

        // 处理嵌套带空传播字符 ? 的标识符
        var identifier =
            ProcessNestedNullPropagationIdentifier(key, Options.AutoCreateNestedObjects, out var isUnchanged);

        // 将 JsonCanvas 转换为 JsonObject 实例
        var jsonObject = JsonCanvas.AsObject();

        // 根据键获取 JSON 节点
        if (jsonObject.TryGetPropertyValue(identifier, out var jsonNode))
        {
            return jsonNode;
        }

        // 检查是否允许访问不存在的属性
        if (!Options.AllowMissingProperty)
        {
            throw new KeyNotFoundException($"The property `{identifier}` was not found in the Clay.");
        }

        // 检查是否需要处理嵌套带空传播字符 ? 的标识符
        if (isUnchanged || !Options.AutoCreateNestedObjects)
        {
            return null;
        }

        SetValue(identifier, new Clay(Options));
        return FindNode(identifier);
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
        // 将 JsonCanvas 转换为 JsonArray 实例
        var jsonArray = JsonCanvas.AsArray();

        // 检查是否是 Range 实例
        if (index is Range range)
        {
            return new JsonArray(jsonArray.Select(u => u?.DeepClone()).ToArray()[range]);
        }

        // 检查是否是 Index 实例
        var arrayIndex = index is Index idx
            ? idx.IsFromEnd ? jsonArray.Count - idx.Value : idx.Value
            : index;

        // 处理嵌套带空传播字符 ? 的标识符
        var identifier =
            ProcessNestedNullPropagationIdentifier(arrayIndex, Options.AutoCreateNestedArrays, out var isUnchanged);

        // 检查数组索引合法性
        EnsureLegalArrayIndex(identifier, out var intIndex);

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

        // 检查是否需要处理嵌套带空传播字符 ? 的标识符
        if (isUnchanged || !Options.AutoCreateNestedArrays)
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
    /// <exception cref="NotSupportedException"></exception>
    internal void SetNodeInObject(object key, object? value)
    {
        // 检查键是否是不受支持的类型
        ThrowIfUnsupportedKeyType(key);

        // 处理嵌套带空传播字符 ? 的标识符
        var identifier = ProcessNestedNullPropagationIdentifier(key, Options.AutoCreateNestedObjects, out _);

        // 将 JsonCanvas 转换为 JsonObject 实例
        var jsonObject = JsonCanvas.AsObject();

        // 处理值是一个 Delegate 委托类型
        if (value is Delegate @delegate)
        {
            // 移除可能存在的同名属性
            jsonObject.Remove(identifier);
            DelegateMap[identifier] = @delegate;
        }
        else
        {
            // 移除可能存在的同名委托属性
            DelegateMap.Remove(identifier);

            // 触发数据变更之前事件
            OnChanging(identifier);

            jsonObject[identifier] = SerializeToNode(value, Options);

            // 触发数据变更之后事件
            OnChanged(identifier);
        }
    }

    /// <summary>
    ///     根据索引设置 <see cref="JsonNode" /> 节点
    /// </summary>
    /// <param name="index">索引</param>
    /// <param name="value">元素值</param>
    /// <param name="insert">是否作为在指定位置插入</param>
    /// <exception cref="NotSupportedException"></exception>
    internal void SetNodeInArray(object index, object? value, bool insert = false)
    {
        // 检查是否是 Range 实例
        if (index is Range)
        {
            throw new NotSupportedException(
                $"Setting values using a System.Range `{index}` is not supported in the Clay.");
        }

        // 将 JsonCanvas 转换为 JsonArray 实例
        var jsonArray = JsonCanvas.AsArray();

        // 检查是否是 Index 实例
        var arrayIndex = index is Index idx
            ? idx.IsFromEnd ? jsonArray.Count - idx.Value : idx.Value
            : index;

        // 处理嵌套带空传播字符 ? 的标识符
        var identifier =
            ProcessNestedNullPropagationIdentifier(arrayIndex, Options.AutoCreateNestedArrays, out _);

        // 检查数组索引合法性
        EnsureLegalArrayIndex(identifier, out var intIndex);

        // 获取 JsonArray 长度
        var count = jsonArray.Count;

        // 触发数据变更之前事件
        OnChanging(intIndex);

        // 检查索引小于数组长度
        if (intIndex < count)
        {
            // 将值序列化成 JsonNode 实例
            var jsonNodeValue = SerializeToNode(value, Options);

            // 替换指定位置的值
            if (!insert)
            {
                jsonArray[intIndex] = jsonNodeValue;
            }
            // 在指定位置插入
            else
            {
                jsonArray.Insert(intIndex, jsonNodeValue);
            }

            // 触发数据变更之后事件
            OnChanged(intIndex);
        }
        // 检查索引是否等于长度，如果是则追加
        else if (intIndex == count)
        {
            jsonArray.Add(SerializeToNode(value, Options));

            // 触发数据变更之后事件
            OnChanged(intIndex);
        }
        // 检查是否允许访问越界的数组，如果是则采用补位方式
        else if (Options.AllowIndexOutOfRange)
        {
            // 检查是否需要进行补位操作
            if (!Options.AutoExpandArrayWithNulls)
            {
                return;
            }

            // 补位操作
            while (jsonArray.Count < intIndex)
            {
                // 对数组进行 null 值补位
                ExpandArrayWithNulls(jsonArray);
            }

            jsonArray.Add(SerializeToNode(value, Options));

            // 触发数据变更之后事件
            OnChanged(intIndex);
        }
        else
        {
            ThrowIfOutOfRange(intIndex, count);
        }
    }

    /// <summary>
    ///     根据键删除 <see cref="JsonNode" /> 节点
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    /// <exception cref="NotSupportedException"></exception>
    internal bool RemoveNodeFromObject(object key)
    {
        // 检查键是否是不受支持的类型
        ThrowIfUnsupportedKeyType(key);

        // 处理嵌套带空传播字符 ? 的标识符
        var identifier = ProcessNestedNullPropagationIdentifier(key, Options.AutoCreateNestedObjects, out _);

        // 将 JsonCanvas 转换为 JsonObject 实例
        var jsonObject = JsonCanvas.AsObject();

        // 移除可能存在的同名委托属性
        if (DelegateMap.Remove(identifier))
        {
            return true;
        }

        // 触发移除数据之前事件
        OnRemoving(identifier);

        // 移除键
        if (jsonObject.Remove(identifier))
        {
            // 触发移除数据之后事件
            OnRemoved(identifier);

            return true;
        }

        // 检查是否允许访问不存在的属性
        if (!Options.AllowMissingProperty)
        {
            throw new KeyNotFoundException($"The property `{identifier}` was not found in the Clay.");
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
        // 检查是否是 Range 实例
        if (index is Range range)
        {
            return RemoveNodeFromArrayByRange(range);
        }

        // 将 JsonCanvas 转换为 JsonArray 实例
        var jsonArray = JsonCanvas.AsArray();

        // 检查是否是 Index 实例
        var arrayIndex = index is Index idx
            ? idx.IsFromEnd ? jsonArray.Count - idx.Value : idx.Value
            : index;

        // 处理嵌套带空传播字符 ? 的标识符
        var identifier =
            ProcessNestedNullPropagationIdentifier(arrayIndex, Options.AutoCreateNestedArrays, out _);

        // 检查数组索引合法性
        EnsureLegalArrayIndex(identifier, out var intIndex);

        // 获取 JsonArray 长度
        var count = jsonArray.Count;

        // 触发移除数据之前事件
        OnRemoving(intIndex);

        // 检查索引小于数组长度
        if (intIndex < count)
        {
            jsonArray.RemoveAt(intIndex);

            // 触发移除数据之后事件
            OnRemoved(intIndex);

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
    ///     根据 <see cref="Range" /> 删除 <see cref="JsonNode" /> 节点
    /// </summary>
    /// <param name="range">
    ///     <see cref="Range" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal bool RemoveNodeFromArrayByRange(Range range)
    {
        // 计算范围对象的开始偏移量和长度
        var (offset, length) = range.GetOffsetAndLength(JsonCanvas.AsArray().Count);

        // 移除指定范围内的元素
        for (var i = 0; i < length; i++)
        {
            RemoveNodeFromArray(offset);
        }

        return true;
    }

    /// <summary>
    ///     尝试根据键获取委托
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="delegate">
    ///     <see cref="Delegate" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal bool TryGetDelegate(object key, out Delegate? @delegate)
    {
        // 检查是否是单一对象
        if (!IsObject)
        {
            @delegate = null;
            return false;
        }

        // 处理嵌套带空传播字符 ? 的标识符
        var identifier = ProcessNestedNullPropagationIdentifier(key, Options.AutoCreateNestedObjects, out _);

        return DelegateMap.TryGetValue(identifier, out @delegate);
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
            // 该操作不会复制自定义委托方法
            Clay clay => clay.DeepClone(options).JsonCanvas,
            // 排除 ExpandoObject 委托属性
            ExpandoObject expandoObject => SerializeToNode(
                expandoObject.Where(kvp => kvp.Value is not Delegate).ToDictionary(u => u.Key, u => u.Value), options),
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
            JsonValueKind.String when options?.DateJsonToDateTime == true &&
                                      DateTime.TryParse(jsonNode.GetValue<string>(), out var dateTime) => dateTime,
            JsonValueKind.String => jsonNode.GetValue<string>(),
            JsonValueKind.Number => jsonNode.GetNumericValue(),
            JsonValueKind.True or JsonValueKind.False => jsonNode.GetValue<bool>(),
            // TODO: 避免每次获取会触发 new 操作，建议未来版本进行缓存优化
            JsonValueKind.Object or JsonValueKind.Array => new Clay(jsonNode, options),
            _ => null
        };

    /// <summary>
    ///     处理嵌套带空传播字符 <c>?</c> 的标识符
    /// </summary>
    /// <param name="identifier">标识符</param>
    /// <param name="enable">是否启用处理</param>
    /// <param name="isUnchanged">标识符是否未被改变</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal static string ProcessNestedNullPropagationIdentifier(object identifier, bool enable, out bool isUnchanged)
    {
        // 将标识符转换为字符串类型
        var stringIdentifier = identifier.ToString()!;

        // 检查是否启用空传播字符 ? 处理
        if (!enable)
        {
            isUnchanged = true;
            return stringIdentifier;
        }

        // 尝试移除字符串标识符末尾字符 ?
        var finalIdentifier = stringIdentifier.TrimEnd('?');

        // 如果没有变化，则表示未修改
        isUnchanged = stringIdentifier == finalIdentifier;
        return finalIdentifier;
    }

    /// <summary>
    ///     对数组进行 null 值补位
    /// </summary>
    /// <param name="jsonArray">
    ///     <see cref="JsonArray" />
    /// </param>
    internal void ExpandArrayWithNulls(JsonArray jsonArray)
    {
        // 获取最新的数组索引
        var addingIndex = jsonArray.Count;

        // 触发数据变更之前事件
        OnChanging(addingIndex);

        // 追加 null
        jsonArray.Add(null);

        // 触发数据变更之后事件
        OnChanged(addingIndex);
    }

    /// <summary>
    ///     创建 <see cref="JsonNode" /> 选项
    /// </summary>
    /// <param name="options">
    ///     <see cref="ClayOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="Tuple{T1,T2}" />
    /// </returns>
    internal static Tuple<JsonNodeOptions, JsonDocumentOptions> CreateJsonNodeOptions(ClayOptions options)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(options);

        // 初始化 JsonNodeOptions 实例
        var jsonNodeOptions =
            new JsonNodeOptions { PropertyNameCaseInsensitive = options.PropertyNameCaseInsensitive };

        // 初始化 JsonDocumentOptions 实例
        var jsonDocumentOptions = new JsonDocumentOptions
        {
            AllowTrailingCommas = options.JsonSerializerOptions.AllowTrailingCommas,
            CommentHandling = options.JsonSerializerOptions.ReadCommentHandling,
            MaxDepth = options.JsonSerializerOptions.MaxDepth
        };

        return Tuple.Create(jsonNodeOptions, jsonDocumentOptions);
    }

    /// <summary>
    ///     尝试将键值对格式的 JSON 字符串转换为 <see cref="JsonObject" />
    /// </summary>
    /// <param name="jsonNode">
    ///     <see cref="JsonNode" />
    /// </param>
    /// <param name="jsonNodeOptions">
    ///     <see cref="JsonNodeOptions" />
    /// </param>
    /// <param name="jsonDocumentOptions">
    ///     <see cref="JsonDocumentOptions" />
    /// </param>
    /// <param name="jsonObject">
    ///     <see cref="JsonObject" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool TryConvertKeyValueJsonToObject(JsonNode? jsonNode, JsonNodeOptions jsonNodeOptions,
        JsonDocumentOptions jsonDocumentOptions, [NotNullWhen(true)] out JsonObject? jsonObject)
    {
        // 如果不是数组或者为空，则无法转换
        if (jsonNode?.GetValueKind() is not JsonValueKind.Array || jsonNode.AsArray().Count == 0)
        {
            jsonObject = null;
            return false;
        }

        // 将 JsonCanvas 转换为 JsonArray 实例
        var jsonArray = jsonNode.AsArray();

        // 初始化 JsonObject 实例
        jsonObject = new JsonObject(jsonNodeOptions);

        // 初始化 JsonNodeOptions 实例（忽略大小写）
        var caseInsensitiveJsonNodeOptions = new JsonNodeOptions { PropertyNameCaseInsensitive = true };

        // 遍历集合的每一项
        foreach (var item in jsonArray)
        {
            // 如果当前项不是一个有效的对象或不包含两个属性，则不能转换
            if (item?.GetValueKind() is not JsonValueKind.Object || item.AsObject().Count != 2)
            {
                jsonObject = null;
                return false;
            }

            // 使用大小写不敏感的选项克隆当前项
            var itemObject = JsonNode.Parse(item.ToJsonString(), caseInsensitiveJsonNodeOptions, jsonDocumentOptions)
                ?.AsObject()!;

            // 检查当前项是否包含有效的 key 和 value 属性
            if (!itemObject.TryGetPropertyValue("key", out var keyNode) ||
                keyNode?.GetValueKind() is not JsonValueKind.String ||
                !itemObject.TryGetPropertyValue("value", out var valueNode))
            {
                jsonObject = null;
                return false;
            }

            // 将键值对添加 JsonObject 实例中
            jsonObject[keyNode.GetValue<string>()] = valueNode?.DeepClone();
        }

        return true;
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
            throw new InvalidOperationException(
                $"The property `{stringIndex}` was not found in the Clay or is not a valid array index.");
        }

        // 检查索引是否小于 0
        if (intIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index),
                "Negative indices are not allowed. Index must be greater than or equal to 0.");
        }
    }

    /// <summary>
    ///     确保当前实例不在只读模式下。如果实例是只读的，则抛出异常
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    internal void EnsureNotReadOnlyBeforeModify()
    {
        // 检查是否是只读模式
        if (Options.ReadOnly)
        {
            throw new InvalidOperationException(
                "Operation cannot be performed because the Clay is in read-only mode.");
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

    /// <summary>
    ///     如果当前实例是集合或数组且尝试调用不支持的操作，则抛出异常
    /// </summary>
    /// <param name="method">方法名</param>
    /// <exception cref="NotSupportedException"></exception>
    internal void ThrowIfMethodCalledOnArrayCollection(string method)
    {
        // 检查是否是集合或数组
        if (IsArray)
        {
            throw new NotSupportedException(
                $"`{method}` method can only be used for single object operations.");
        }
    }

    /// <summary>
    ///     如果使用不受支持的键类型，则抛出异常
    /// </summary>
    /// <param name="key">键</param>
    /// <exception cref="NotSupportedException"></exception>
    internal static void ThrowIfUnsupportedKeyType(object key)
    {
        switch (key)
        {
            // 检查是否是 Index 实例
            case Index:
                throw new NotSupportedException(
                    $"Accessing or setting properties using System.Index `{key}` is not supported in the Clay.");
            // 检查是否是 Range 实例
            case Range:
                throw new NotSupportedException(
                    $"Accessing or setting properties using System.Range `{key}` is not supported in the Clay.");
        }
    }
}