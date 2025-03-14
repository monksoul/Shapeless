// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless.Tests;

public class DateTimeOffsetConverterUsingDateTimeOffsetParseAsFallbackTests
{
    [Fact]
    public void Read_Invalid_Parameters()
    {
        const string testDateTimeStr = "2025-03-04 16:45:27";
        const string json = @"""" + testDateTimeStr + @"""";
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<DateTimeOffset>(json));
    }

    [Fact]
    public void Read_ReturnOK()
    {
        const string testDateTimeStr = "2025-03-04 16:45:27";
        const string json = @"""" + testDateTimeStr + @"""";
        var jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerOptions.Default);
        jsonSerializerOptions.Converters.Add(new DateTimeOffsetConverterUsingDateTimeOffsetParseAsFallback());

        var datetime = JsonSerializer.Deserialize<DateTimeOffset>(json, jsonSerializerOptions);
        Assert.Equal("2025-03-04T16:45:27.0000000+08:00", datetime.ToString("O", CultureInfo.CurrentCulture));
    }

    [Fact]
    public void Write_ReturnOK()
    {
        var datetime = new DateTimeOffset(new DateTime(2025, 3, 4, 16, 45, 27, 888, 103, DateTimeKind.Utc));
        Assert.Equal("\"2025-03-04T16:45:27.888103+00:00\"", JsonSerializer.Serialize(datetime));
        var datetime2 = new DateTimeOffset(new DateTime(2025, 3, 4, 16, 45, 27, 888, DateTimeKind.Utc));
        Assert.Equal("\"2025-03-04T16:45:27.888+00:00\"", JsonSerializer.Serialize(datetime2));
        var datetime3 = new DateTimeOffset(new DateTime(2025, 3, 4, 16, 45, 27, DateTimeKind.Utc));
        Assert.Equal("\"2025-03-04T16:45:27+00:00\"", JsonSerializer.Serialize(datetime3));

        var jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerOptions.Default)
        {
            PropertyNameCaseInsensitive = true
        };
        jsonSerializerOptions.Converters.Add(new DateTimeOffsetConverterUsingDateTimeOffsetParseAsFallback());
        Assert.Equal("\"2025-03-04T16:45:27.888103+00:00\"", JsonSerializer.Serialize(datetime, jsonSerializerOptions));
        Assert.Equal("\"2025-03-04T16:45:27.888+00:00\"", JsonSerializer.Serialize(datetime2, jsonSerializerOptions));
        Assert.Equal("\"2025-03-04T16:45:27+00:00\"", JsonSerializer.Serialize(datetime3, jsonSerializerOptions));
    }
}