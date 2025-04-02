// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless;

/// <summary>
///     <see cref="Clay" /> 模型绑定
/// </summary>
/// <param name="options">
///     <see cref="IOptions{TOptions}" />
/// </param>
internal sealed class ClayBinder(IOptions<ClayOptions> options) : IModelBinder
{
    /// <inheritdoc />
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(bindingContext);

        // 获取 HttpContext 实例
        var httpContext = bindingContext.HttpContext;

        // 检查是否是 URL 表单（application/x-www-form-urlencoded）内容
        var isFormUrlEncoded = MediaTypeHeaderValue.Parse(httpContext.Request.ContentType!).MediaType ==
                               MediaTypeNames.Application.FormUrlEncoded;

        // 尝试从请求体中读取数据，并将其转换为 Clay 实例
        var (canParse, model) = await TryReadAndConvertBodyToClayAsync(httpContext.Request.Body, options.Value,
            isFormUrlEncoded, httpContext.RequestAborted);

        bindingContext.Result = !canParse ? ModelBindingResult.Failed() : ModelBindingResult.Success(model);
    }

    /// <summary>
    ///     尝试从请求体中读取数据，并将其转换为 <see cref="Clay" /> 实例
    /// </summary>
    /// <param name="stream">请求内容流</param>
    /// <param name="options">
    ///     <see cref="ClayOptions" />
    /// </param>
    /// <param name="isFormUrlEncoded">是否是 <c>application/x-www-form-urlencoded</c> 表单</param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="Tuple{T1,T2}" />
    /// </returns>
    internal static async Task<(bool canParse, Clay? model)> TryReadAndConvertBodyToClayAsync(Stream stream,
        ClayOptions options, bool isFormUrlEncoded, CancellationToken cancellationToken)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(stream);

        // 使用 StreamReader 异步读取请求内容字符串
        using var streamReader = new StreamReader(stream);
        var json = await streamReader.ReadToEndAsync(cancellationToken);

        return string.IsNullOrEmpty(json)
            ? (false, null)
            : (true,
                Clay.Parse(isFormUrlEncoded ? HttpUtility.UrlDecode(json).ParseFormatKeyValueString(['&'], '?') : json,
                    options));
    }

    /// <summary>
    ///     为最小 API 提供模型绑定
    /// </summary>
    /// <remarks>
    ///     <para>由运行时调用。</para>
    ///     <para>参考文献：https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/minimal-apis/parameter-binding?view=aspnetcore-9.0#custom-binding。</para>
    /// </remarks>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="parameter">
    ///     <see cref="ParameterInfo" />
    /// </param>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    internal static async Task<Clay?> BindAsync(HttpContext httpContext, ParameterInfo parameter)
    {
        // 解析 ClayOptions 选项
        var options = httpContext.RequestServices.GetRequiredService<IOptions<ClayOptions>>().Value;

        // 检查是否是 URL 表单（application/x-www-form-urlencoded）内容
        var isFormUrlEncoded = MediaTypeHeaderValue.Parse(httpContext.Request.ContentType!).MediaType ==
                               MediaTypeNames.Application.FormUrlEncoded;

        // 尝试从请求体流中读取数据，并将其转换为 Clay 实例
        var (_, model) = await TryReadAndConvertBodyToClayAsync(httpContext.Request.Body, options, isFormUrlEncoded,
            httpContext.RequestAborted);

        return model;
    }
}