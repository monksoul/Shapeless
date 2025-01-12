namespace Shapeless.Samples.Models;

public class DataTableJsonConverter : JsonConverter<DataTable>
{
    /// <inheritdoc />
    public override DataTable? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DataTable value, JsonSerializerOptions options)
    {
        // 将 DataTable 转换为字典集合
        var dictList = value.AsEnumerable().Select(row =>
            row.Table.Columns.Cast<DataColumn>()
                .ToDictionary(col => col.ColumnName, col => row[col] != DBNull.Value ? row[col] : null)).ToList();

        // 序列化字典列表
        JsonSerializer.Serialize(writer, dictList, options);
    }
}