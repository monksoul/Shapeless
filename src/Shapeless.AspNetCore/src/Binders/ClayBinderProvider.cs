// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless;

/// <summary>
///     <see cref="Clay" /> 模型绑定提供器
/// </summary>
internal sealed class ClayBinderProvider : IModelBinderProvider
{
    /// <inheritdoc />
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(context);

        // 获取模型类型和参数特性列表
        var modelType = context.Metadata.ModelType;
        var parameterAttributes = (context.Metadata as DefaultModelMetadata)?.Attributes.ParameterAttributes;

        return modelType == typeof(Clay) ||
               // 确保参数类型为 dynamic 且贴有 [Clay] 特性
               (modelType == typeof(object) && parameterAttributes?.OfType<ClayAttribute>().Any() == true &&
                parameterAttributes.OfType<DynamicAttribute>().Any())
            ? new BinderTypeModelBinder(typeof(ClayBinder))
            : null;
    }
}