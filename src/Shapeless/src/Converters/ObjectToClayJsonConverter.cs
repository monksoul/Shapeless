// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless;

/// <summary>
///     <see cref="object" /> 转 <see cref="Clay" /> JSON 序列化转换器
/// </summary>
public sealed class ObjectToClayJsonConverter : JsonConverter<object>
{
    /// <inheritdoc />
    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // 将 Utf8JsonReader 转换为 JsonElement
        var jsonElement = JsonElement.ParseValue(ref reader);

        // 检查 JSON 是否是对象或数组类型
        if (jsonElement.ValueKind is JsonValueKind.Object or JsonValueKind.Array)
        {
            return Clay.Parse(jsonElement.ToString(), new ClayOptions { JsonSerializerOptions = options });
        }

        return jsonElement;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options) =>
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
}