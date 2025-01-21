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

        return context.Metadata.ModelType == typeof(Clay) ? new BinderTypeModelBinder(typeof(ClayBinder)) : null;
    }
}