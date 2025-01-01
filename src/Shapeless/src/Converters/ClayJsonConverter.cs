// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless;

/// <summary>
///     <see cref="Clay" /> JSON 序列化转换器
/// </summary>
public sealed class ClayJsonConverter : JsonConverter<Clay>
{
    /// <inheritdoc />
    public override Clay? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        Clay.Parse(ref reader, new ClayOptions { JsonSerializerOptions = options });

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Clay value, JsonSerializerOptions options) =>
        writer.WriteRawValue(value.ToJsonString(options));
}