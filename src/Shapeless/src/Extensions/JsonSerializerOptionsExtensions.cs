// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless.Extensions;

/// <summary>
///     <see cref="JsonSerializerOptions" /> 拓展类
/// </summary>
public static class JsonSerializerOptionsExtensions
{
    /// <summary>
    ///     添加 <see cref="Clay" /> JSON 序列化配置
    /// </summary>
    /// <param name="jsonSerializerOptions">
    ///     <see cref="JsonSerializerOptions" />
    /// </param>
    public static void AddClayConverters(this JsonSerializerOptions jsonSerializerOptions)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(jsonSerializerOptions);

        // 处理对象序列化为 Clay 的问题
        if (!jsonSerializerOptions.Converters.OfType<ClayJsonConverter>().Any())
        {
            jsonSerializerOptions.Converters.Add(new ClayJsonConverter());
        }

        // 处理 object/dynamic 类型对象序列化为 Clay 的问题
        if (!jsonSerializerOptions.Converters.OfType<ObjectToClayJsonConverter>().Any())
        {
            jsonSerializerOptions.Converters.Add(new ObjectToClayJsonConverter());
        }
    }
}