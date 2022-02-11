using System;
using System.Collections.Generic;
using Leopotam.EcsLite;

namespace ApplicationScripts.Ecs
{
    [Flags]
    public enum CollectorEvent
    {
        Added = 1,
        Removed = 2,
        AddedOrRemoved = 3
    }
    
    public class Collector : IDisposable
    {
        private readonly List<int> _entities = new List<int>();
        internal readonly List<IDisposable> _disposables = new List<IDisposable>();

        public List<int>.Enumerator GetEnumerator()
        {
            return _entities.GetEnumerator();
        }

        public Collector()
        {
        }

        public void Clear()
        {
            _entities.Clear();
        }

        internal void AddEntity(int entity)
        {
            _entities.Add(entity);
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
    
    internal class AddCollector : IEcsFilterEventListener, IDisposable
    {
        private readonly Collector _collector;
        private readonly EcsFilter _filter;

        public AddCollector(Collector collector, EcsFilter filter)
        {
            _collector = collector;
            _filter = filter;
            _filter.AddEventListener(this);
        }

        public void OnEntityAdded(int entity)
        {
            _collector.AddEntity(entity);
        }

        public void OnEntityRemoved(int entity)
        {
            
        }

        public void Dispose()
        {
            _filter.RemoveEventListener(this);
        }
    }

    internal class RemoveCollector : IEcsFilterEventListener, IDisposable
    {
        private readonly Collector _collector;
        private readonly EcsFilter _filter;

        public RemoveCollector(EcsFilter filter, Collector collector)
        {
            _filter = filter;
            _filter.AddEventListener(this);
            _collector = collector;
        }

        public void OnEntityAdded(int entity)
        {
        }

        public void OnEntityRemoved(int entity)
        {
            _collector.AddEntity(entity);
        }

        public void Dispose()
        {
            _filter.RemoveEventListener(this);
        }
    }

    public static class CleanCodeExtensions
    {
        public static Collector GetCollector(this EcsFilter filter, CollectorEvent collectorEvent = CollectorEvent.Added)
        {
            var collector = new Collector();
            if ((collectorEvent & CollectorEvent.Added) > 0)
            {
                var addCollector = new AddCollector(collector, filter);
                collector._disposables.Add(addCollector);
            }

            if ((collectorEvent & CollectorEvent.Removed) > 0)
            {
                var removeCollector = new RemoveCollector(filter, collector);
                collector._disposables.Add(removeCollector);
            }

            return collector;
        }
    }
}