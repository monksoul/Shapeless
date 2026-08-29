// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless;

/// <summary>
///     流变对象内部实现类
/// </summary>
/// <param name="clayOptions">
///     <see cref="ClayOptions" />
/// </param>
internal sealed class InternalClay(IOptionsMonitor<ClayOptions> clayOptions) : IClay
{
    /// <inheritdoc />
    public Clay Create(ClayOptions? options = null) => new(GetOptions(options));

    /// <inheritdoc />
    public Clay Create(ClayType clayType, ClayOptions? options = null) =>
        new(clayType, GetOptions(options));

    /// <inheritdoc />
    public Clay EmptyObject(ClayOptions? options = null) => Clay.EmptyObject(GetOptions(options));

    /// <inheritdoc />
    public Clay EmptyArray(ClayOptions? options = null) => Clay.EmptyArray(GetOptions(options));

    /// <inheritdoc />
    public Clay Parse(object? obj, ClayOptions? options = null) => Clay.Parse(obj, GetOptions(options));

    /// <inheritdoc />
    public Clay Parse(ref Utf8JsonReader utf8JsonReader, ClayOptions? options = null) =>
        Clay.Parse(ref utf8JsonReader, GetOptions(options));

    /// <inheritdoc />
    public Clay ParseFromFile(string path, ClayOptions? options = null) =>
        Clay.ParseFromFile(path, GetOptions(options));

    /// <summary>
    ///     获取 <see cref="ClayOptions" />
    /// </summary>
    /// <param name="options">
    ///     <see cref="ClayOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="ClayOptions" />
    /// </returns>
    internal ClayOptions GetOptions(ClayOptions? options = null) => options ?? clayOptions.CurrentValue;
}