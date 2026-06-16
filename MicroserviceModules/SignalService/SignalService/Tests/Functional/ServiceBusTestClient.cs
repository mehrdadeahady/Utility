using SignalService.Application.Contracts;

namespace SignalService.Tests.Functional
{
    public class FakeServiceBusTestClient : IServiceBusTestClient
    {
        private readonly List<string> _messages = new();
        private readonly List<Func<string, Task>> _subscribers = new();

        public Task PublishJobAsync(string payloadJson)
        {
            _messages.Add(payloadJson);
            Console.WriteLine($"[FakeServiceBus] Message stored: {payloadJson}");

            // Notify all subscribers
            foreach (var handler in _subscribers)
            {
                _ = handler(payloadJson); // fire-and-forget
            }

            return Task.CompletedTask;
        }

        public Task SubscribeAsync(Func<string, Task> handler)
        {
            _subscribers.Add(handler);
            Console.WriteLine("[FakeServiceBus] Subscriber added");
            return Task.CompletedTask;
        }

        public IReadOnlyList<string> GetPublishedMessages() => _messages;
    }
}
