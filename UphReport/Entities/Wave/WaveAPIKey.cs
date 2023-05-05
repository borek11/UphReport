namespace UphReport.Entities.Wave;

public class WaveAPIKey
{
    public Guid Id { get; set; }
    public string APIKey { get; set; }
    public int CreditsRemaining { get; set; }

    public WaveAPIKey()
    {

    }
}
