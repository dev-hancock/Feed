using Feed.Events;

namespace Feed.Consumers;

public class ScrapeEvent
{
    public Guid Id { get; set; }

    public string Item { get; set; }

    public string Title { get; set; }

    public string Link { get; set; }

    public string Description { get; set; }

    public string Date { get; set; }

    public DateTime Timestamp { get; set; }
}


public class ArtifactChangedEvent : IEvent
{
    public Guid Id { get; set; }

    public string Item { get; set; }

    public string Title { get; set; }

    public string Link { get; set; }

    public string Description { get; set; }

    public string Date { get; set; }

    public DateTime Timestamp { get; set; }
}