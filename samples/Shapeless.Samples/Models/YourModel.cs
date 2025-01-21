namespace Shapeless.Samples.Models;

public class YourModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

public class ClayModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public DateTime? Date { get; set; }
    public bool IsTrue { get; set; }
}

public class ClayModel2
{
    public int Id { get; set; }
    public Clay? Clay { get; set; }
}