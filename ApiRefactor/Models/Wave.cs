namespace ApiRefactor.Models;

public class Wave
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime WaveDate { get; set; }
}