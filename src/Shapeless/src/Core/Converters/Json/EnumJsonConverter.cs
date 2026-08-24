// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless.Core.Converters.Json;

/// <summary>
///     枚举 JSON 序列化转换器
/// </summary>
/// <remarks>支持将 JSON 数字、字符串数字（如 "1"）或枚举名称（忽略大小写）反序列化为枚举。</remarks>
public class EnumJsonConverter : JsonConverter<object>
{
    /// <summary>
    ///     控制序列化时是否将枚举输出为字符串（枚举名称）
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool WriteAsString { get; set; }

    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsEnum;

    /// <inheritdoc />
    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // 处理 JSON 数字 token
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (reader.TokenType == JsonTokenType.Number)
        {
            object numValue;

            if (reader.TryGetInt64(out var longValue))
            {
                numValue = longValue;
            }
            else if (reader.TryGetUInt64(out var ulongValue))
            {
                numValue = ulongValue;
            }
            else
            {
                throw new JsonException($"The JSON number could not be converted to enum {typeToConvert.Name}.");
            }

            // 将指定的整数值转换为枚举成员
            return Enum.ToObject(typeToConvert, numValue);
        }

        // 处理 JSON 字符串 token
        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();

            // 尝试按枚举名称解析（忽略大小写）
            if (Enum.TryParse(typeToConvert, stringValue, true, out var nameResult))
            {
                return nameResult;
            }

            // 获取枚举的底层类型
            var underlyingType = Enum.GetUnderlyingType(typeToConvert);

            try
            {
                // 尝试将字符串解析为底层类型的数字
                var numericValue = Convert.ChangeType(stringValue, underlyingType);

                // 将指定的整数值转换为枚举成员
                return Enum.ToObject(typeToConvert, numericValue!);
            }
            catch
            {
                // ignored
            }

            throw new JsonException(
                $"The JSON string \"{stringValue}\" could not be converted to enum {typeToConvert.Name}.");
        }

        throw new JsonException($"Unexpected token {reader.TokenType} when parsing enum {typeToConvert.Name}.");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        // 检查是否将枚举输出为字符串（枚举名称）
        if (WriteAsString)
        {
            writer.WriteStringValue(value.ToString());
        }
        else
        {
            writer.WriteNumberValue(Convert.ToDecimal(value));
        }
    }
}