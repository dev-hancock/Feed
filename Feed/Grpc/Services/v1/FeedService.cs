using Feed.Features.Feeds;
using Grpc.Core;

namespace Feed.Grpc.Services.v1;

public class FeedService(CreateFeedHandler create, DeleteFeedHandler delete, StreamFeedHandler stream) : Feed.FeedBase
{
    public override Task<CreateFeedResponse> Create(CreateFeedRequest request, ServerCallContext context)
    {
        return create.Handle(request, context.CancellationToken);
    }

    public override Task<DeleteFeedResponse> Delete(DeleteFeedRequest request, ServerCallContext context)
    {
        return delete.Handle(request, context.CancellationToken);
    }

    public override async Task Stream(StreamFeedRequest request, IServerStreamWriter<FeedEvent> response, ServerCallContext context)
    {
        await foreach (var item in stream.Handle(request, context.CancellationToken))
        {
            await response.WriteAsync(item, context.CancellationToken);
        }
    }
}