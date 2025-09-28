using AngleSharp.Css.Dom;

namespace Feed.Models
{
    public class Plan
    {
        public Guid Id { get; init; } = Guid.NewGuid();

        public string Url { get; private set; }

        public int Interval { get; private set; } = 300;

        public bool IsActive { get; private set; } = true;

        public DateTime CreatedAt { get; } = DateTime.UtcNow;

        public DateTime ChangedAt { get; private set; }

        public DateTime CheckedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        public void CheckedNow() => CheckedAt = DateTime.UtcNow;

        public void UpdatedNow() => UpdatedAt = DateTime.UtcNow;

        public void ChangedNow() => ChangedAt = DateTime.UtcNow;

        public void AddArtifact(Artifact artifact) => Artifacts.Add(artifact);

        public static Plan Create(string url, int interval) => new()
        {
            Url = url,
            Interval = interval <= 0 ? 1 : interval
        };

        public void Activate() => IsActive = true;

        public void Deactivate() => IsActive = false;

        public void Update(string? url, int? interval, Dictionary<string, string>? selectors = null)
        {
            Url = url ?? Url;
            Interval = interval ?? Interval;
        }

        public void AddArtifact(string item, string title, string link, string description, string date)
        {
            var artifact = new Artifact
            {
                Fields = Fields.Create(
                    item,
                    title,
                    link,
                    description,
                    date
                ),
                CreatedAt = CreatedAt
            };
            Artifacts.Add(artifact);
        }

        public void UpdateArtifact(string item, string title, string link, string description, string date)
        {
            // TODO:
        }

        public virtual List<Artifact> Artifacts { get; set; } = [];
    }

    public class Artifact
    {
        public Guid Id { get; set; }

        public Guid FeedId { get; set; }

        public Fields Fields { get; set; } = new();

        public DateTime CreatedAt { get; set; }

        public DateTime ChangedAt { get; set; }

        public DateTime CheckedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public virtual Plan Plan { get; set; } = null!;
    }

    public class Field
    {
        public string Selector { get; set; } = null!;

        public string? Value { get; set; }

        public static Field Create(string selector)
        {
            return new Field
            {
                Selector = selector
            };
        }
    }

    public class Fields
    {
        public Field Item { get; set; } = null!;

        public Field Title { get; set; } = null!;
        
        public Field Link { get; set; } = null!;

        public Field Description { get; set; } = null!;

        public Field Date { get; set; } = null!;

        public static Fields Create(string item, string title, string link, string description, string date)
        {
            return new Fields
            {
                Item = Field.Create(item),
                Title = Field.Create(title),
                Link = Field.Create(link),
                Description = Field.Create(description),
                Date = Field.Create(date)
            };
        }
    }
}
