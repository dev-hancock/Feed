using Feed.Events;

namespace Feed.Models;

public class ArtifactChangedEvent : IEvent
{
    public string Id { get; set; }

    public string Item { get; set; }

    public string Title { get; set; }

    public string Link { get; set; }

    public string Description { get; set; }

    public string Date { get; set; }

    public DateTime Timestamp { get; set; }
}