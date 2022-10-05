using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.CMD.Domain.Aggregates;

namespace Post.CMD.Infrastructure.Stores;

public class EventStore : IEventStore
{
    private readonly IEventStoreRepository _eventStoreRepository;
    private readonly IEventProducer _eventProducer;

    public EventStore(IEventStoreRepository eventStoreRepository, IEventProducer eventProducer)
    {
        _eventStoreRepository = eventStoreRepository;
        _eventProducer = eventProducer;
    }

    public async Task<List<Guid>> GetAggregatIdsAsync()
    {
        var eventStream = await _eventStoreRepository.FindAllAsync();
        if (eventStream is null || !eventStream.Any())
            throw new ArgumentNullException(nameof(EventStore), "Could not retrieve event stream from the event store!");

        return eventStream.Select(x => x.AggrgateIdentifier).Distinct().ToList();
    }

    public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
    {
        var eventStream = await _eventStoreRepository.FindById(aggregateId);
        if (eventStream is null || !eventStream.Any())
            throw new AggregateNotFoundException("Incorrect post Id provided!");
        return eventStream.OrderBy(x => x.Version).Select(x => x.EventData).ToList();
    }

    public async Task SaveEventAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
    {
        var eventStream = await _eventStoreRepository.FindById(aggregateId);
        if (expectedVersion != -1 && eventStream[^1].Version != expectedVersion)
            throw new ConcurencyException();
        var version = expectedVersion;
        foreach (var @event in events)
        {
            version++;
            @event.Version = version;
            var eventType = @event.GetType().Name;
            var eventModel = new EventModel
            {
                TimeStamp = DateTime.Now,
                AggrgateIdentifier = aggregateId,
                AggrgateType = nameof(PostAggregate),
                Version = version,
                EventType = eventType,
                EventData = @event,
            };
            await _eventStoreRepository.SaveAsync(eventModel);

            var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
            await _eventProducer.ProduceAsync(topic, @event);
        }
    }
}
