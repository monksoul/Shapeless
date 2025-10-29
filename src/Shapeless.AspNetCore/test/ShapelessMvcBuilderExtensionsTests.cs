// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless.AspNetCore.Tests;

public class ShapelessMvcBuilderExtensionsTests
{
    [Fact]
    public void AddClayOptions_Invalid_Parameters()
    {
        var builder = WebApplication.CreateBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.Services.AddControllers().AddJsonOptions(null!));
    }

    [Fact]
    public void AddClayOptions_ReturnOK()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddControllers().AddClayOptions(options =>
        {
            options.KeyValueJsonToObject = true;
        });

        using var app = builder.Build();

        var jsonOptions = app.Services.GetRequiredService<IOptions<JsonOptions>>().Value;
        Assert.NotNull(jsonOptions.JsonSerializerOptions.Converters);
        Assert.Single(jsonOptions.JsonSerializerOptions.Converters.OfType<ClayJsonConverter>());
        Assert.Single(jsonOptions.JsonSerializerOptions.Converters.OfType<ObjectToClayJsonConverter>());

        var clayOptions = app.Services.GetRequiredService<IOptions<ClayOptions>>().Value;
        Assert.True(clayOptions.KeyValueJsonToObject);

        var mvcOptions = app.Services.GetRequiredService<IOptions<MvcOptions>>().Value;
        Assert.Single(mvcOptions.ModelBinderProviders.OfType<ClayBinderProvider>());
    }

    [Fact]
    public void AddClayOptions_Repeat_ReturnOK()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddControllers().AddClayOptions(options =>
        {
            options.KeyValueJsonToObject = true;
        }).AddClayOptions(options =>
        {
            options.KeyValueJsonToObject = true;
        });

        using var app = builder.Build();

        var jsonOptions = app.Services.GetRequiredService<IOptions<JsonOptions>>().Value;
        Assert.NotNull(jsonOptions.JsonSerializerOptions.Converters);
        Assert.Single(jsonOptions.JsonSerializerOptions.Converters.OfType<ClayJsonConverter>());

        var clayOptions = app.Services.GetRequiredService<IOptions<ClayOptions>>().Value;
        Assert.True(clayOptions.KeyValueJsonToObject);

        var mvcOptions = app.Services.GetRequiredService<IOptions<MvcOptions>>().Value;
        Assert.Single(mvcOptions.ModelBinderProviders.OfType<ClayBinderProvider>());
    }

    [Fact]
    public void AddClayOptions_NoParameters_ReturnOK()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddControllers().AddClayOptions();

        using var app = builder.Build();

        var jsonOptions = app.Services.GetRequiredService<IOptions<JsonOptions>>().Value;
        Assert.NotNull(jsonOptions.JsonSerializerOptions.Converters);
        Assert.Single(jsonOptions.JsonSerializerOptions.Converters.OfType<ClayJsonConverter>());

        var clayOptions = app.Services.GetRequiredService<IOptions<ClayOptions>>().Value;
        Assert.False(clayOptions.KeyValueJsonToObject);

        var mvcOptions = app.Services.GetRequiredService<IOptions<MvcOptions>>().Value;
        Assert.Single(mvcOptions.ModelBinderProviders.OfType<ClayBinderProvider>());
    }
}