// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless;

/// <summary>
///     <see cref="Clay" /> 选项
/// </summary>
public sealed class ClayOptions
{
    /// <summary>
    ///     默认 <see cref="ClayOptions" /> 实例
    /// </summary>
    public static ClayOptions Default => new();

    /// <summary>
    ///     允许访问缺失的属性或数组越界的 <see cref="ClayOptions" /> 实例
    /// </summary>
    public static ClayOptions Flexible => new() { AllowMissingProperty = true, AllowIndexOutOfRange = true };

    /// <summary>
    ///     配置用于包裹非对象和非数组类型的键名
    /// </summary>
    public string ScalarValueKey { get; set; } = "data";

    /// <summary>
    ///     是否允许访问不存在的属性
    /// </summary>
    /// <remarks>当 <see cref="Clay.IsObject" /> 为 <c>true</c> 时有效。默认值为：<c>false</c>，表示遇到缺失属性时将抛出异常。</remarks>
    public bool AllowMissingProperty { get; set; }

    /// <summary>
    ///     是否允许访问越界的数组索引
    /// </summary>
    /// <remarks>当 <see cref="Clay.IsArray" /> 为 <c>true</c> 时有效。默认值为：<c>false</c>，表示遇到无效索引时将抛出异常。</remarks>
    public bool AllowIndexOutOfRange { get; set; }

    /// <summary>
    ///     是否自动创建嵌套的对象实例
    /// </summary>
    /// <remarks>
    ///     当 <see cref="Clay.IsObject" /> 和 <see cref="AllowMissingProperty" /> 为 <c>true</c> 时有效。默认值为：<c>false</c>
    ///     ，当设置为 <c>true</c> 时，如果尝试访问或设置一个不存在的成员并且索引器带 <c>?</c> 后缀的键时，将自动创建一个新的 <see cref="Clay" /> 对象实例。
    /// </remarks>
    public bool AutoCreateNestedObjects { get; set; }

    /// <summary>
    ///     是否自动创建嵌套的数组实例
    /// </summary>
    /// <remarks>
    ///     当 <see cref="Clay.IsArray" /> 和 <see cref="AutoCreateNestedArrays" /> 为 <c>true</c> 时有效。默认值为：<c>false</c>
    ///     ，当设置为 <c>true</c> 时，如果尝试访问或设置超出数组长度的索引并且索引器带 <c>?</c> 后缀的键时，将自动创建一个新的 <see cref="Clay" /> 数组实例。
    /// </remarks>
    public bool AutoCreateNestedArrays { get; set; }

    /// <summary>
    ///     是否超出数组长度时自动补位 <c>null</c>
    /// </summary>
    /// <remarks>
    ///     当 <see cref="Clay.IsArray" /> 和 <see cref="AllowIndexOutOfRange" /> 为 <c>true</c> 时有效。默认值为：<c>false</c>，当设置为
    ///     <c>true</c> 时，如果尝试访问或设置超出数组长度的索引时，将自动进行补位 <c>null</c> 操作。
    /// </remarks>
    public bool AutoExpandArrayWithNulls { get; set; }

    /// <summary>
    ///     是否应在转换后执行数据校验
    /// </summary>
    /// <remarks>当 <see cref="Clay.IsObject" /> 为 <c>true</c> 时有效。默认值为：<c>false</c>，表示不进行转换后的数据校验。</remarks>
    public bool ValidateAfterConversion { get; set; }

    /// <summary>
    ///     控制是否将日期格式的 JSON 转换为 <see cref="DateTime" />
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool DateJsonToDateTime { get; set; }

    /// <summary>
    ///     控制是否将键值对格式的 JSON 转换为单一对象
    /// </summary>
    public bool KeyValueJsonToObject { get; set; }

    /// <summary>
    ///     是否属性名称不区分大小写
    /// </summary>
    /// <remarks>
    ///     <para>当 <see cref="Clay.IsObject" /> 为 <c>true</c> 时有效。默认值为：<c>false</c>，表示访问属性时使用的名称与属性名完全匹配。</para>
    ///     <para>如果在对象初始化之后更改此属性，则会触发深度克隆操作，创建一个新对象以反映更改。</para>
    /// </remarks>
    public bool PropertyNameCaseInsensitive { get; set; }

    /// <summary>
    ///     是否是只读模式
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool ReadOnly { get; set; }

    /// <summary>
    ///     JSON 序列化配置
    /// </summary>
    public JsonSerializerOptions JsonSerializerOptions { get; set; } = new(JsonSerializerOptions.Default)
    {
        PropertyNameCaseInsensitive = true,
        // 允许 String 转 Number
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        // 解决中文乱码问题
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        AllowTrailingCommas = true,
        Converters =
        {
            new ClayJsonConverter(),
            new DateTimeConverterUsingDateTimeParseAsFallback(),
            new DateTimeOffsetConverterUsingDateTimeOffsetParseAsFallback(),
            // 允许 Number 或 Boolean 转 String
            new StringJsonConverter()
        }
    };

    /// <summary>
    ///     自定义配置 <see cref="ClayOptions" /> 实例
    /// </summary>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="ClayOptions" />
    /// </returns>
    public ClayOptions Configure(Action<ClayOptions> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        configure.Invoke(this);

        return this;
    }
}