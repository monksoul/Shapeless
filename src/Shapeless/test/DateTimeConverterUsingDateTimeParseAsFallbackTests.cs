// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless.Tests;

public class DateTimeConverterUsingDateTimeParseAsFallbackTests
{
    [Fact]
    public void Read_Invalid_Parameters()
    {
        const string testDateTimeStr = "2025-03-04 16:45:27";
        const string json = @"""" + testDateTimeStr + @"""";
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<DateTime>(json));
    }

    [Fact]
    public void Read_ReturnOK()
    {
        const string testDateTimeStr = "2025-03-04 16:45:27";
        const string json = @"""" + testDateTimeStr + @"""";
        var jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerOptions.Default);
        jsonSerializerOptions.Converters.Add(new DateTimeConverterUsingDateTimeParseAsFallback());

        var datetime = JsonSerializer.Deserialize<DateTime>(json, jsonSerializerOptions);
        Assert.Equal("2025-03-04T16:45:27.0000000", datetime.ToString("O", CultureInfo.CurrentCulture));
    }

    [Fact]
    public void Write_ReturnOK()
    {
        var datetime = new DateTime(2025, 3, 4, 16, 45, 27, 888, 103, DateTimeKind.Utc);
        Assert.Equal("\"2025-03-04T16:45:27.888103Z\"", JsonSerializer.Serialize(datetime));
        var datetime2 = new DateTime(2025, 3, 4, 16, 45, 27, 888, DateTimeKind.Utc);
        Assert.Equal("\"2025-03-04T16:45:27.888Z\"", JsonSerializer.Serialize(datetime2));
        var datetime3 = new DateTime(2025, 3, 4, 16, 45, 27, DateTimeKind.Utc);
        Assert.Equal("\"2025-03-04T16:45:27Z\"", JsonSerializer.Serialize(datetime3));

        var jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerOptions.Default)
        {
            PropertyNameCaseInsensitive = true
        };
        jsonSerializerOptions.Converters.Add(new DateTimeConverterUsingDateTimeParseAsFallback());
        Assert.Equal("\"2025-03-04T16:45:27.888103Z\"", JsonSerializer.Serialize(datetime, jsonSerializerOptions));
        Assert.Equal("\"2025-03-04T16:45:27.888Z\"", JsonSerializer.Serialize(datetime2, jsonSerializerOptions));
        Assert.Equal("\"2025-03-04T16:45:27Z\"", JsonSerializer.Serialize(datetime3, jsonSerializerOptions));
    }
}