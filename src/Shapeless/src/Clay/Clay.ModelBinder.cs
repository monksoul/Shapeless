// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless;

/// <summary>
///     流变对象
/// </summary>
/// <remarks>
///     <para>为最小 API 提供模型绑定。</para>
///     <para>参考文献：https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/minimal-apis/parameter-binding?view=aspnetcore-9.0#custom-binding。</para>
/// </remarks>
public partial class Clay
{
    /// <summary>
    ///     初始化 <c>ClayBinder.BindAsync(HttpContext, ParameterInfo)</c> 静态发方法
    /// </summary>
    internal static readonly Lazy<MethodInfo> _bindAsyncMethod = new(() =>
    {
        // 获取流变对象模块程序集名称
        var shapelessAssemblyName = typeof(Clay).Assembly.GetName().Name;

        // 加载 ClayBinder 模型绑定类型并获取该类型的 BindAsync 静态方法
        var clayBinderType =
            System.Type.GetType($"{shapelessAssemblyName}.ClayBinder, {shapelessAssemblyName}.AspNetCore");

        return clayBinderType?.GetMethod("BindAsync", BindingFlags.NonPublic | BindingFlags.Static)!;
    });

    /// <summary>
    ///     为最小 API 提供模型绑定
    /// </summary>
    /// <remarks>由运行时调用。</remarks>
    /// <param name="httpContext"><c>HttpContext</c> 实例</param>
    /// <param name="parameter">
    ///     <see cref="ParameterInfo" />
    /// </param>
    /// <returns>
    ///     <see cref="Clay" />
    /// </returns>
    public static async ValueTask<Clay?> BindAsync(dynamic httpContext, ParameterInfo parameter) =>
        await (Task<Clay?>)_bindAsyncMethod.Value.Invoke(null, [httpContext, parameter])!;
}