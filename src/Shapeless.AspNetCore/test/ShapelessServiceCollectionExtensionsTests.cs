// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless.AspNetCore.Tests;

public class ShapelessServiceCollectionExtensionsTests
{
    [Fact]
    public void AddClayOptions_Invalid_Parameters()
    {
        var services = new ServiceCollection();
        Assert.Throws<ArgumentNullException>(() => services.AddClayOptions(null!));
    }

    [Fact]
    public void AddClayOptions_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddClayOptions(options =>
        {
            options.KeyValueJsonToObject = true;
        });

        Assert.Single(services, u => u.ServiceType == typeof(IClay));

        using var serviceProvider = services.BuildServiceProvider();

        var jsonOptions = serviceProvider.GetRequiredService<IOptions<JsonOptions>>().Value;
        Assert.NotNull(jsonOptions.JsonSerializerOptions.Converters);
        Assert.Single(jsonOptions.JsonSerializerOptions.Converters.OfType<ClayJsonConverter>());

        var clayOptions = serviceProvider.GetRequiredService<IOptions<ClayOptions>>().Value;
        Assert.True(clayOptions.KeyValueJsonToObject);

        var mvcOptions = serviceProvider.GetRequiredService<IOptions<MvcOptions>>().Value;
        Assert.Single(mvcOptions.ModelBinderProviders.OfType<ClayBinderProvider>());
    }

    [Fact]
    public void AddClayOptions_Duplicate_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddClayOptions(options =>
        {
            options.KeyValueJsonToObject = true;
        }).AddClayOptions(options =>
        {
            options.KeyValueJsonToObject = true;
        });

        Assert.Single(services, u => u.ServiceType == typeof(IClay));

        using var serviceProvider = services.BuildServiceProvider();

        var jsonOptions = serviceProvider.GetRequiredService<IOptions<JsonOptions>>().Value;
        Assert.NotNull(jsonOptions.JsonSerializerOptions.Converters);
        Assert.Single(jsonOptions.JsonSerializerOptions.Converters.OfType<ClayJsonConverter>());

        var clayOptions = serviceProvider.GetRequiredService<IOptions<ClayOptions>>().Value;
        Assert.True(clayOptions.KeyValueJsonToObject);

        var mvcOptions = serviceProvider.GetRequiredService<IOptions<MvcOptions>>().Value;
        Assert.Single(mvcOptions.ModelBinderProviders.OfType<ClayBinderProvider>());
    }

    [Fact]
    public void AddClayOptions_NoParameters_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddClayOptions();
        Assert.Single(services, u => u.ServiceType == typeof(IClay));

        using var serviceProvider = services.BuildServiceProvider();

        var jsonOptions = serviceProvider.GetRequiredService<IOptions<JsonOptions>>().Value;
        Assert.NotNull(jsonOptions.JsonSerializerOptions.Converters);
        Assert.Single(jsonOptions.JsonSerializerOptions.Converters.OfType<ClayJsonConverter>());

        var clayOptions = serviceProvider.GetRequiredService<IOptions<ClayOptions>>().Value;
        Assert.False(clayOptions.KeyValueJsonToObject);

        var mvcOptions = serviceProvider.GetRequiredService<IOptions<MvcOptions>>().Value;
        Assert.Single(mvcOptions.ModelBinderProviders.OfType<ClayBinderProvider>());
    }
}