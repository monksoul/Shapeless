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
    ///     初始化 <c>Microsoft.AspNetCore.Mvc.JsonResult</c> 类型
    /// </summary>
    internal static readonly Lazy<Type> _jsonResultType = new(() =>
        System.Type.GetType("Microsoft.AspNetCore.Mvc.JsonResult, Microsoft.AspNetCore.Mvc.Core")!);

    /// <summary>
    ///     <inheritdoc cref="Clay" />
    /// </summary>
    /// <param name="options">
    ///     <see cref="ClayOptions" />
    /// </param>
    public Clay(ClayOptions? options = null)
        : this(ClayType.Object, options)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="Clay" />
    /// </summary>
    /// <param name="clayType">
    ///     <see cref="ClayType" />
    /// </param>
    /// <param name="options">
    ///     <see cref="ClayOptions" />
    /// </param>
    public Clay(ClayType clayType, ClayOptions? options = null)
    {
        // 初始化 ClayOptions
        Options = options ?? ClayOptions.Default;

        // 创建 JsonNode 选项
        var (jsonNodeOptions, jsonDocumentOptions) = CreateJsonNodeOptions(Options);

        // 创建 JsonObject 实例并指示属性名称是否不区分大小写
        JsonCanvas = JsonNode.Parse(clayType is ClayType.Object ? "{}" : "[]", jsonNodeOptions, jsonDocumentOptions)!;

        IsObject = clayType is ClayType.Object;
        IsArray = clayType is ClayType.Array;
        Type = clayType;
    }

    /// <summary>
    ///     索引
    /// </summary>
    /// <param name="identifier">标识符，可以是键（字符串）或索引（整数）或索引运算符（Index）或范围运算符（Range）</param>
    public object? this[object identifier]
    {
        get => GetValue(identifier);
        set => SetValue(identifier, value);
    }

    /// <summary>
    ///     <see cref="Range" /> 索引
    /// </summary>
    /// <remarks>截取 <see cref="Clay" /> 并返回新的 <see cref="Clay" />。</remarks>
    /// <param name="range">
    ///     <see cref="Range" />
    /// </param>
    public Clay this[Range range] => (Clay)this[range as object]!;

    /// <summary>
    ///     判断是否为单一对象
    /// </summary>
    public bool IsObject { get; }

    /// <summary>
    ///     判断是否为集合或数组
    /// </summary>
    public bool IsArray { get; }

    /// <summary>
    ///     获取流变对象的基本类型
    /// </summary>
    public ClayType Type { get; }

    /// <summary>
    ///     判断是否为只读模式
    /// </summary>
    public bool IsReadOnly => Options.ReadOnly;

    /// <inheritdoc />
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        // 空检查
        if (format is null)
        {
            return JsonCanvas.ToString();
        }

        // 将格式化字符串转换为字符数组
        var chars = format.ToUpper().ToCharArray();

        // 命名策略不能同时指定
        if (chars.Contains('C') && chars.Contains('P'))
        {
            throw new FormatException(
                $"The format string `{format}` cannot contain both 'C' and 'P', as they specify conflicting naming strategies.");
        }

        // 初始化 JsonSerializerOptions 实例
        var jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerOptions.Default) { WriteIndented = true };

        // 添加压缩（取消格式化）处理
        if (chars.Contains('Z'))
        {
            jsonSerializerOptions.WriteIndented = false;
        }

        // 添加取消中文 Unicode 编码处理
        if (chars.Contains('U'))
        {
            jsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        }

        // 添加小驼峰命名处理
        if (chars.Contains('C'))
        {
            jsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        }

        // 添加帕斯卡（大驼峰）命名处理
        if (chars.Contains('P'))
        {
            jsonSerializerOptions.PropertyNamingPolicy = new PascalCaseNamingPolicy();
        }

        return ToJsonString(jsonSerializerOptions);
    }

    /// <summary>
    ///     创建空的单一对象
    /// </summary>
    /// <param name="options">
    ///     <see cref="ClayOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    public static Clay EmptyObject(ClayOptions? options = null) => new(options);

    /// <summary>
    ///     创建空的集合或数组
    /// </summary>
    /// <param name="options">
    ///     <see cref="ClayOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    public static Clay EmptyArray(ClayOptions? options = null) => new(ClayType.Array, options);

    /// <summary>
    ///     将对象转换为 <see cref="Clay" /> 实例
    /// </summary>
    /// <param name="obj">
    ///     <see cref="object" />
    /// </param>
    /// <param name="options">
    ///     <see cref="ClayOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    public static Clay Parse(object? obj, ClayOptions? options = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(obj);

        // 初始化 ClayOptions 实例
        var clayOptions = options ?? ClayOptions.Default;

        // 创建 JsonNode 选项
        var (jsonNodeOptions, jsonDocumentOptions) = CreateJsonNodeOptions(clayOptions);

        // 将对象转换为 JsonNode 实例
        var jsonNode = obj switch
        {
            string rawJson => JsonNode.Parse(rawJson, jsonNodeOptions, jsonDocumentOptions),
            Stream utf8Json => JsonNode.Parse(utf8Json, jsonNodeOptions, jsonDocumentOptions),
            byte[] utf8JsonBytes => JsonNode.Parse(utf8JsonBytes, jsonNodeOptions, jsonDocumentOptions),
            _ => SerializeToNode(obj, clayOptions)
        };

        // 处理是否将键值对格式的 JSON 字符串解析为单一对象
        if (clayOptions.KeyValueJsonToObject &&
            TryConvertKeyValueJsonToObject(jsonNode, jsonNodeOptions, jsonDocumentOptions, out var jsonObject))
        {
            jsonNode = jsonObject;
        }

        return new Clay(jsonNode, clayOptions);
    }

    /// <summary>
    ///     将对象转换为 <see cref="Clay" /> 实例
    /// </summary>
    /// <param name="obj">
    ///     <see cref="object" />
    /// </param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    public static Clay Parse(object? obj, Action<ClayOptions> configure) =>
        Parse(obj, ClayOptions.Default.Configure(configure));

    /// <summary>
    ///     将 <see cref="Utf8JsonReader" /> 转换为 <see cref="Clay" /> 实例
    /// </summary>
    /// <param name="utf8JsonReader">
    ///     <see cref="Utf8JsonReader" />
    /// </param>
    /// <param name="options">
    ///     <see cref="ClayOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    public static Clay Parse(ref Utf8JsonReader utf8JsonReader, ClayOptions? options = null) =>
        Parse(utf8JsonReader.GetRawText(), options);

    /// <summary>
    ///     将 <see cref="Utf8JsonReader" /> 转换为 <see cref="Clay" /> 实例
    /// </summary>
    /// <param name="utf8JsonReader">
    ///     <see cref="Utf8JsonReader" />
    /// </param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    public static Clay Parse(ref Utf8JsonReader utf8JsonReader, Action<ClayOptions> configure) =>
        Parse(ref utf8JsonReader, ClayOptions.Default.Configure(configure));

    /// <summary>
    ///     检查标识符是否定义
    /// </summary>
    /// <param name="identifier">标识符，可以是键（字符串）或索引（整数）或索引运算符（Index）或范围运算符（Range）</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public bool Contains(object identifier)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(identifier);

        // 检查是否是单一对象
        if (IsObject)
        {
            // 检查键是否是不受支持的类型
            ThrowIfUnsupportedKeyType(identifier);

            // 将标识符转换为字符串类型
            var propertyName = identifier.ToString()!;

            return DelegateMap.ContainsKey(propertyName) || JsonCanvas.AsObject().ContainsKey(propertyName);
        }

        // 检查是否是 Range 实例
        if (identifier is Range)
        {
            throw new NotSupportedException(
                $"Checking containment using a System.Range `{identifier}` is not supported in the Clay.");
        }

        // 将 JsonCanvas 转换为 JsonArray 实例
        var jsonArray = JsonCanvas.AsArray();

        // 检查是否是 Index 实例
        var stringIndex = (identifier is Index idx
            ? idx.IsFromEnd ? jsonArray.Count - idx.Value : idx.Value
            : identifier).ToString();

        // 尝试将字符串标识符转换为整数索引
        if (int.TryParse(stringIndex, out var intIndex))
        {
            return intIndex >= 0 && intIndex < jsonArray.Count;
        }

        return false;
    }

    /// <summary>
    ///     检查标识符是否定义
    /// </summary>
    /// <remarks>兼容旧版本粘土对象。</remarks>
    /// <param name="identifier">标识符，可以是键（字符串）或索引（整数）或索引运算符（Index）或范围运算符（Range）</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public bool IsDefined(object identifier) => Contains(identifier);

    /// <summary>
    ///     根据标识符获取值
    /// </summary>
    /// <param name="identifier">标识符，可以是键（字符串）或索引（整数）或索引运算符（Index）或范围运算符（Range）</param>
    /// <returns>
    ///     <see cref="object" />
    /// </returns>
    public object? Get(object identifier) => GetValue(identifier);

    /// <summary>
    ///     截取 <see cref="Clay" /> 并返回新的 <see cref="Clay" />
    /// </summary>
    /// <param name="range">
    ///     <see cref="Range" />
    /// </param>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    /// <exception cref="NotSupportedException"></exception>
    public Clay Get(Range range)
    {
        // 检查是否是单一对象实例调用
        ThrowIfMethodCalledOnSingleObject($"{nameof(Get)}(Range)");

        return this[range];
    }

    /// <summary>
    ///     根据标识符获取目标类型的值
    /// </summary>
    /// <param name="identifier">标识符，可以是键（字符串）或索引（整数）或索引运算符（Index）或范围运算符（Range）</param>
    /// <param name="resultType">转换的目标类型</param>
    /// <param name="jsonSerializerOptions">
    ///     <see cref="JsonSerializerOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="object" />
    /// </returns>
    /// <exception cref="InvalidCastException"></exception>
    public object? Get(object identifier, Type resultType, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        // 尝试根据键获取委托
        if (TryGetDelegate(identifier, out var @delegate))
        {
            // 空检查或检查目标委托类型是否一致
            if (@delegate is null || @delegate.GetType() == resultType)
            {
                return @delegate;
            }

            throw new InvalidCastException(
                $"The delegate type `{@delegate.GetType().FullName}` cannot be cast to the target type `{resultType.FullName}`.");
        }

        // 根据标识符查找 JsonNode 节点
        var jsonNode = FindNode(identifier);

        return IsClay(resultType)
            ? new Clay(jsonNode, Options)
            : Helpers.DeserializeNode(jsonNode, resultType, jsonSerializerOptions ?? Options.JsonSerializerOptions);
    }

    /// <summary>
    ///     根据标识符获取目标类型的值
    /// </summary>
    /// <param name="identifier">标识符，可以是键（字符串）或索引（整数）或索引运算符（Index）或范围运算符（Range）</param>
    /// <param name="jsonSerializerOptions">
    ///     <see cref="JsonSerializerOptions" />
    /// </param>
    /// <typeparam name="TResult">转换的目标类型</typeparam>
    /// <returns>
    ///     <typeparamref name="TResult" />
    /// </returns>
    public TResult? Get<TResult>(object identifier, JsonSerializerOptions? jsonSerializerOptions = null) =>
        (TResult?)Get(identifier, typeof(TResult), jsonSerializerOptions);

    /// <summary>
    ///     根据标识符查找 <see cref="JsonNode" /> 节点
    /// </summary>
    /// <param name="identifier">标识符，可以是键（字符串）或索引（整数）或索引运算符（Index）或范围运算符（Range）</param>
    /// <returns>
    ///     <see cref="JsonNode" />
    /// </returns>
    public JsonNode? FindNode(object identifier)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(identifier);

        return IsObject ? GetNodeFromObject(identifier) : GetNodeFromArray(identifier);
    }

    /// <summary>
    ///     根据标识符设置值
    /// </summary>
    /// <param name="identifier">标识符，可以是键（字符串）或索引（整数）或索引运算符（Index）或范围运算符（Range）</param>
    /// <param name="value">值</param>
    public void Set(object identifier, object? value) => SetValue(identifier, value);

    /// <summary>
    ///     在指定索引处插入项
    /// </summary>
    /// <remarks>当 <see cref="IsArray" /> 为 <c>true</c> 时有效。</remarks>
    /// <param name="index">索引</param>
    /// <param name="value">值</param>
    /// <exception cref="NotSupportedException"></exception>
    public void Insert(int index, object? value)
    {
        // 检查是否是单一对象实例调用
        ThrowIfMethodCalledOnSingleObject(nameof(Insert));

        SetValue(index, value, true);
    }

    /// <summary>
    ///     在指定索引处插入项
    /// </summary>
    /// <remarks>当 <see cref="IsArray" /> 为 <c>true</c> 时有效。</remarks>
    /// <param name="index">索引</param>
    /// <param name="value">值</param>
    /// <exception cref="NotSupportedException"></exception>
    public void Insert(Index index, object? value)
    {
        // 检查是否是单一对象实例调用
        ThrowIfMethodCalledOnSingleObject(nameof(Insert));

        Insert(index.IsFromEnd ? JsonCanvas.AsArray().Count - index.Value : index.Value, value);
    }

    /// <summary>
    ///     在指定索引处批量插入项
    /// </summary>
    /// <remarks>当 <see cref="IsArray" /> 为 <c>true</c> 时有效。</remarks>
    /// <param name="index">索引</param>
    /// <param name="values">值集合</param>
    /// <exception cref="NotSupportedException"></exception>
    public void InsertRange(int index, params object?[] values)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(values);

        // 检查是否是单一对象实例调用
        ThrowIfMethodCalledOnSingleObject(nameof(InsertRange));

        // 初始化待插入索引位置
        var currentIndex = index;

        // 逐条在指定索引处插入项
        foreach (var value in values)
        {
            SetValue(currentIndex++, value, true);
        }
    }

    /// <summary>
    ///     在指定索引处批量插入项
    /// </summary>
    /// <remarks>当 <see cref="IsArray" /> 为 <c>true</c> 时有效。</remarks>
    /// <param name="index">索引</param>
    /// <param name="values">值集合</param>
    /// <exception cref="NotSupportedException"></exception>
    public void InsertRange(Index index, params object?[] values)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(values);

        // 检查是否是单一对象实例调用
        ThrowIfMethodCalledOnSingleObject(nameof(InsertRange));

        InsertRange(index.IsFromEnd ? JsonCanvas.AsArray().Count - index.Value : index.Value, values);
    }

    /// <summary>
    ///     在末尾处添加项
    /// </summary>
    /// <remarks>当 <see cref="IsArray" /> 为 <c>true</c> 时有效。</remarks>
    /// <param name="value">值</param>
    /// <exception cref="NotSupportedException"></exception>
    public void Add(object? value)
    {
        // 检查是否是单一对象实例调用
        ThrowIfMethodCalledOnSingleObject(nameof(Add));

        SetValue(JsonCanvas.AsArray().Count, value);
    }

    /// <summary>
    ///     在末尾处追加项
    /// </summary>
    /// <remarks>当 <see cref="IsArray" /> 为 <c>true</c> 时有效。</remarks>
    /// <param name="value">值</param>
    /// <exception cref="NotSupportedException"></exception>
    public void Append(object? value)
    {
        // 检查是否是单一对象实例调用
        ThrowIfMethodCalledOnSingleObject(nameof(Append));

        SetValue(JsonCanvas.AsArray().Count, value);
    }

    /// <summary>
    ///     在末尾处批量添加项
    /// </summary>
    /// <remarks>当 <see cref="IsArray" /> 为 <c>true</c> 时有效。</remarks>
    /// <param name="values">值集合</param>
    /// <exception cref="NotSupportedException"></exception>
    public void AddRange(params object?[] values)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(values);

        // 检查是否是单一对象实例调用
        ThrowIfMethodCalledOnSingleObject(nameof(AddRange));

        // 将 JsonCanvas 转换为 JsonArray 实例
        var jsonArray = JsonCanvas.AsArray();

        // 逐条追加项
        foreach (var value in values)
        {
            SetValue(jsonArray.Count, value);
        }
    }

    /// <summary>
    ///     在末尾处添加项
    /// </summary>
    /// <remarks>当 <see cref="IsArray" /> 为 <c>true</c> 时有效。</remarks>
    /// <param name="value">值</param>
    /// <exception cref="NotSupportedException"></exception>
    public void Push(object? value)
    {
        // 检查是否是单一对象实例调用
        ThrowIfMethodCalledOnSingleObject(nameof(Push));

        SetValue(JsonCanvas.AsArray().Count, value);
    }

    /// <summary>
    ///     移除末尾处的项
    /// </summary>
    /// <remarks>当 <see cref="IsArray" /> 为 <c>true</c> 时有效。</remarks>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    /// <exception cref="NotSupportedException"></exception>
    public bool Pop()
    {
        // 检查是否是单一对象实例调用
        ThrowIfMethodCalledOnSingleObject(nameof(Pop));

        // 获取 JsonArray 最大索引
        var maxIndex = JsonCanvas.AsArray().Count - 1;

        return maxIndex > -1 && RemoveValue(maxIndex);
    }

    /// <summary>
    ///     反转 <see cref="Clay" /> 并返回新 <see cref="Clay" />
    /// </summary>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    public Clay Reverse(ClayOptions? options = null) =>
        Parse(IsObject ? AsEnumerateObject().Reverse().ToDictionary() : Values.Reverse(), options);

    /// <summary>
    ///     截取 <see cref="Clay" /> 并返回新的 <see cref="Clay" />
    /// </summary>
    /// <param name="range">
    ///     <see cref="Range" />
    /// </param>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    /// <exception cref="NotSupportedException"></exception>
    public Clay Slice(Range range)
    {
        // 检查是否是单一对象实例调用
        ThrowIfMethodCalledOnSingleObject(nameof(Slice));

        return this[range];
    }

    /// <summary>
    ///     截取 <see cref="Clay" /> 并返回新的 <see cref="Clay" />
    /// </summary>
    /// <param name="start">范围的包含起始索引</param>
    /// <param name="end">范围的非包含结束索引</param>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    /// <exception cref="NotSupportedException"></exception>
    public Clay Slice(Index start, Index end)
    {
        // 检查是否是单一对象实例调用
        ThrowIfMethodCalledOnSingleObject(nameof(Slice));

        return Slice(new Range(start, end));
    }

    /// <summary>
    ///     组合多个 <see cref="Clay" /> 并返回新 <see cref="Clay" />
    /// </summary>
    /// <param name="clays">
    ///     <see cref="Clay" /> 集合
    /// </param>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public Clay Combine(params Clay[] clays)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(clays);

        // 检查是否有任何 Clay 对象为空
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (clays.Any(clay => clay is null))
        {
            throw new ArgumentException("Clay array contains one or more null elements.", nameof(clays));
        }

        // 检查是流变对象类型是否一致
        if (clays.Any(u => u.Type != Type))
        {
            throw new InvalidOperationException("All Clay objects must be of the same type.");
        }

        // 检查是否是集合或数组
        if (IsArray)
        {
            return Parse(Values.Concat(clays.SelectMany(u => u.Values)));
        }

        // 深度克隆当前 Clay 实例
        var combineClay = DeepClone();

        // 遍历所有 Clay 并设置值
        foreach (var clay in clays)
        {
            foreach (var (key, value) in clay.AsEnumerateObject())
            {
                combineClay[key] = value;
            }
        }

        return combineClay;
    }

    /// <summary>
    ///     根据标识符删除数据
    /// </summary>
    /// <param name="identifier">标识符，可以是键（字符串）或索引（整数）或索引运算符（Index）或范围运算符（Range）</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public bool Remove(object identifier) => RemoveValue(identifier);

    /// <summary>
    ///     根据范围删除数据
    /// </summary>
    /// <param name="start">范围的包含起始索引</param>
    /// <param name="end">范围的非包含结束索引</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    /// <exception cref="NotSupportedException"></exception>
    public bool Remove(Index start, Index end)
    {
        // 检查是否是单一对象实例调用
        ThrowIfMethodCalledOnSingleObject(nameof(Remove));

        return Remove(new Range(start, end));
    }

    /// <summary>
    ///     根据标识符删除数据
    /// </summary>
    /// <remarks>兼容旧版本粘土对象。</remarks>
    /// <param name="identifier">标识符，可以是键（字符串）或索引（整数）或索引运算符（Index）或范围运算符（Range）</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public bool Delete(object identifier) => Remove(identifier);

    /// <summary>
    ///     根据范围删除数据
    /// </summary>
    /// <param name="start">范围的包含起始索引</param>
    /// <param name="end">范围的非包含结束索引</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    /// <exception cref="NotSupportedException"></exception>
    public bool Delete(Index start, Index end)
    {
        // 检查是否是单一对象实例调用
        ThrowIfMethodCalledOnSingleObject(nameof(Delete));

        return Remove(start, end);
    }

    /// <summary>
    ///     将 <see cref="Clay" /> 转换为目标类型
    /// </summary>
    /// <param name="resultType">转换的目标类型</param>
    /// <param name="jsonSerializerOptions">
    ///     <see cref="JsonSerializerOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="object" />
    /// </returns>
    public object? As(Type resultType, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        // 检查是否是 Clay 类型或 IEnumerable<dynamic?> 类型
        if (IsClay(resultType) || resultType == typeof(IEnumerable<dynamic?>))
        {
            return this;
        }

        // 检查是否是 IEnumerable<KeyValuePair<object, dynamic?>> 类型
        if (resultType == typeof(IEnumerable<KeyValuePair<object, dynamic?>>))
        {
            return AsEnumerable();
        }

        // 检查是否是 IEnumerable<KeyValuePair<string, dynamic?>> 类型且是单一对象
        if (resultType == typeof(IEnumerable<KeyValuePair<string, dynamic?>>) && IsObject)
        {
            return AsEnumerateObject();
        }

        // 检查是否是 IEnumerable<KeyValuePair<int, dynamic?>> 类型且是集合或数组
        if (resultType == typeof(IEnumerable<KeyValuePair<int, dynamic?>>) && IsArray)
        {
            return AsEnumerateArray().Select((item, index) => new KeyValuePair<int, dynamic?>(index, item));
        }

        // 检查是否是 IActionResult 类型
        if (resultType.FullName?.Contains("Microsoft.AspNetCore.Mvc.IActionResult") == true)
        {
            return Activator.CreateInstance(_jsonResultType.Value, this);
        }

        // 将 JsonNode 转换为目标类型
        var result = Helpers.DeserializeNode(JsonCanvas, resultType,
            jsonSerializerOptions ?? Options.JsonSerializerOptions);

        // 检查是否启用转换后执行模型验证
        if (result is not null && Options.ValidateAfterConversion)
        {
            Validator.ValidateObject(result, new ValidationContext(result), true);
        }

        return result;
    }

    /// <summary>
    ///     将 <see cref="Clay" /> 转换为目标类型
    /// </summary>
    /// <param name="jsonSerializerOptions">
    ///     <see cref="JsonSerializerOptions" />
    /// </param>
    /// <typeparam name="TResult">转换的目标类型</typeparam>
    /// <returns>
    ///     <typeparamref name="TResult" />
    /// </returns>
    public TResult? As<TResult>(JsonSerializerOptions? jsonSerializerOptions = null) =>
        (TResult?)As(typeof(TResult), jsonSerializerOptions);

    /// <summary>
    ///     深度克隆
    /// </summary>
    /// <remarks>该操作不会复制自定义委托方法。</remarks>
    /// <param name="options">
    ///     <see cref="ClayOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    public Clay DeepClone(ClayOptions? options = null) => Parse(JsonCanvas.ToJsonString(), options);

    /// <summary>
    ///     清空数据
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public void Clear()
    {
        // 确保当前实例不在只读模式下
        EnsureNotReadOnlyBeforeModify();

        // 检查是否是单一对象
        if (IsObject)
        {
            JsonCanvas.AsObject().Clear();
            DelegateMap.Clear();
        }
        else
        {
            JsonCanvas.AsArray().Clear();
        }
    }

    /// <summary>
    ///     写入提供的 <see cref="Utf8JsonWriter" /> 作为 JSON
    /// </summary>
    /// <param name="writer">
    ///     <see cref="Utf8JsonWriter" />
    /// </param>
    /// <param name="jsonSerializerOptions">
    ///     <see cref="JsonSerializerOptions" />
    /// </param>
    public void WriteTo(Utf8JsonWriter writer, JsonSerializerOptions? jsonSerializerOptions = null) =>
        JsonCanvas.WriteTo(writer, jsonSerializerOptions ?? Options.JsonSerializerOptions);

    /// <summary>
    ///     设置为只读模式
    /// </summary>
    public void AsReadOnly() => Options.ReadOnly = true;

    /// <summary>
    ///     设置为可变（默认）模式
    /// </summary>
    public void AsMutable() => Options.ReadOnly = false;

    /// <summary>
    ///     支持格式化字符串输出
    /// </summary>
    /// <param name="format">格式化字符串</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public string ToString(string? format) => ToString(format, null);

    /// <inheritdoc />
    public override string ToString() => ToString(null, null);

    /// <summary>
    ///     将 <see cref="Clay" /> 输出为 JSON 格式字符串
    /// </summary>
    /// <param name="jsonSerializerOptions">
    ///     <see cref="JsonSerializerOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public string ToJsonString(JsonSerializerOptions? jsonSerializerOptions = null)
    {
        // 获取提供的 JSON 序列化选项或默认选项
        var serializerOptions = jsonSerializerOptions ?? Options.JsonSerializerOptions;

        // 如果指定了命名策略，则对 JsonCanvas 进行键名转换；否则直接使用原 JsonCanvas
        var jsonCanvasToSerialize = serializerOptions.PropertyNamingPolicy is not null
            ? JsonCanvas.TransformKeysWithNamingPolicy(serializerOptions.PropertyNamingPolicy)
            : JsonCanvas;

        // 空检查
        ArgumentNullException.ThrowIfNull(jsonCanvasToSerialize);

        return jsonCanvasToSerialize.ToJsonString(serializerOptions);
    }

    /// <summary>
    ///     将 <see cref="Clay" /> 输出为 XML 格式字符串
    /// </summary>
    /// <param name="xmlWriterSettings">
    ///     <see cref="XmlWriterSettings" />
    /// </param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public string ToXmlString(XmlWriterSettings? xmlWriterSettings = null)
    {
        // 初始化 Utf8StringWriter 实例
        using var stringWriter = new Utf8StringWriter();

        // 初始化 XmlWriter 实例
        // 注意：如果使用 using var xmlWriter = ...; 代码方式，则需要手动调用 xmlWriter.Flush(); 方法来确保所有数据都被写入
        using (var xmlWriter = XmlWriter.Create(stringWriter, xmlWriterSettings))
        {
            // 将 XElement 的内容保存到 XmlWriter 中
            As<XElement>()?.Save(xmlWriter);
        }

        return stringWriter.ToString();
    }

    /// <summary>
    ///     检查类型是否是 <see cref="Clay" /> 类型
    /// </summary>
    /// <param name="type">
    ///     <see cref="Type" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public static bool IsClay(Type type) => type == typeof(Clay) || typeof(Clay).IsAssignableFrom(type);

    /// <summary>
    ///     检查类型是否是 <see cref="Clay" /> 类型
    /// </summary>
    /// <param name="obj">
    ///     <see cref="object" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public static bool IsClay(object? obj) => obj is not null && IsClay(obj as Type ?? obj.GetType());

    /// <summary>
    ///     按照键升序排序并返回新的 <see cref="Clay" />
    /// </summary>
    /// <param name="options">
    ///     <see cref="ClayOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    /// <exception cref="NotSupportedException"></exception>
    public Clay KSort(ClayOptions? options = null)
    {
        // 检查是否是集合或数组实例调用
        ThrowIfMethodCalledOnArrayCollection(nameof(KSort));

        // 初始化升序排序字典
        var sorted =
            new SortedDictionary<string, JsonNode?>(JsonCanvas.AsObject().ToDictionary());

        return Parse(sorted, options);
    }

    /// <summary>
    ///     按照键降序排序并返回新的 <see cref="Clay" />
    /// </summary>
    /// <param name="options">
    ///     <see cref="ClayOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    /// <exception cref="NotSupportedException"></exception>
    // ReSharper disable once InconsistentNaming
    public Clay KRSort(ClayOptions? options = null)
    {
        // 检查是否是集合或数组实例调用
        ThrowIfMethodCalledOnArrayCollection(nameof(KRSort));

        // 初始化降序排序字典
        var sortedDesc =
            new SortedDictionary<string, JsonNode?>(Comparer<string>.Create((x, y) =>
                string.Compare(y, x, StringComparison.InvariantCulture)));

        // 将 JsonCanvas 转换为 JsonObject 实例
        var jsonObject = JsonCanvas.AsObject();

        // 遍历 JsonObject 所有键并追加到字典中
        foreach (var property in jsonObject)
        {
            sortedDesc.Add(property.Key, property.Value);
        }

        return Parse(sortedDesc, options);
    }

    /// <summary>
    ///     重建 <see cref="Clay" /> 实例
    /// </summary>
    /// <param name="options">
    ///     <see cref="ClayOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    public Clay Rebuilt(ClayOptions? options = null)
    {
        // 初始化 ClayOptions 实例
        var clayOptions = options ?? ClayOptions.Default;

        // 创建 JsonNode 选项
        var (jsonNodeOptions, jsonDocumentOptions) = CreateJsonNodeOptions(clayOptions);

        // 处理是否将键值对格式的 JSON 字符串解析为单一对象
        if (clayOptions.KeyValueJsonToObject &&
            TryConvertKeyValueJsonToObject(JsonCanvas, jsonNodeOptions, jsonDocumentOptions, out var jsonObject))
        {
            JsonCanvas = jsonObject;
        }
        else
        {
            JsonCanvas = JsonNode.Parse(JsonCanvas.ToJsonString(), jsonNodeOptions, jsonDocumentOptions)!;
        }

        Options = clayOptions;

        return this;
    }

    /// <summary>
    ///     重建 <see cref="Clay" /> 实例
    /// </summary>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    public Clay Rebuilt(Action<ClayOptions> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        return Rebuilt(Options.Configure(configure));
    }

    /// <summary>
    ///     单一对象
    /// </summary>
    public sealed class Object : Clay
    {
        /// <summary>
        ///     <inheritdoc cref="Object" />
        /// </summary>
        /// <param name="options">
        ///     <see cref="ClayOptions" />
        /// </param>
        public Object(ClayOptions? options = null) : base(options)
        {
        }
    }

    /// <summary>
    ///     集合或数组
    /// </summary>
    public sealed class Array : Clay
    {
        /// <summary>
        ///     <inheritdoc cref="Array" />
        /// </summary>
        /// <param name="options">
        ///     <see cref="ClayOptions" />
        /// </param>
        public Array(ClayOptions? options = null) : base(ClayType.Array, options)
        {
        }
    }
}