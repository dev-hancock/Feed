using AngleSharp;
using Feed.Consumers;
using Feed.Events;
using Feed.Models;
using Quartz;

namespace Feed.Services;

public interface IScrapeService
{
    Task<IEnumerable<ScrapeResult>> ScrapeAsync(Plan plan, CancellationToken ct = default);
}

public class ScrapeResult
{

}

public class ScrapeService : IScrapeService
{
    public async Task<IEnumerable<ScrapeResult>> ScrapeAsync(Plan plan, CancellationToken ct = default)
    {
        var context = BrowsingContext.New(Configuration.Default);

        //var document = await context.OpenAsync(request => request.Content())


        return default;
    }
}