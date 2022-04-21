using System.Collections.Generic;
using ApplicationScripts.Ecs;
using Leopotam.EcsLite;

namespace ApplicationScripts.Logic.Features.Indexing
{
    public class IndexedEntityLibrarySystem<TIndexComponent, TIndex> :  IEcsFilterEventListener, IEcsWorldEventListener where TIndexComponent : struct, IIndexComponent<TIndex>
    {
        private static Dictionary<EcsWorld, IndexedEntityLibrarySystem<TIndexComponent, TIndex>> _librarySystems =
            new Dictionary<EcsWorld, IndexedEntityLibrarySystem<TIndexComponent, TIndex>>();
        private readonly Dictionary<TIndex, int> _dictionary = new Dictionary<TIndex, int>();
        private EcsPool<TIndexComponent> _pool;
        private readonly string _worldName;
        private readonly EcsWorld _ecsWorld;

        private IndexedEntityLibrarySystem(EcsWorld world)
        {
            _librarySystems.Add(world, this);
            world
                .Filter<TIndexComponent>()
                .End()
                .AddEventListener(this);
            _pool = world.GetPool<TIndexComponent>();
            _ecsWorld = world;
            _ecsWorld.AddEventListener(this);
        }

        public IndexedEntityLibrarySystem(string worldName)
        {
            _worldName = worldName;
        }

        public static IndexedEntityLibrarySystem<TIndexComponent, TIndex> GetLibrary(EcsWorld world)
        {
            if (!_librarySystems.TryGetValue(world, out var librarySystem))
            {
                _librarySystems[world] = librarySystem = new IndexedEntityLibrarySystem<TIndexComponent, TIndex>(world);
            }

            return librarySystem;
        }
        
        public int GetEntity(TIndex index)
        {
            return _dictionary[index];
        }

        public void OnEntityAdded(int entity)
        {
            _dictionary[_pool.Read(entity).GetIndex()] = entity;
        }

        public void OnEntityRemoved(int entity)
        {
            
        }

        public void OnEntityCreated(int entity)
        {
            
        }

        public void OnEntityChanged(int entity)
        {
        }

        public void OnEntityDestroyed(int entity)
        {
        }

        public void OnFilterCreated(EcsFilter filter)
        {
        }

        public void OnWorldResized(int newSize)
        {
        }

        public void OnWorldDestroyed(EcsWorld world)
        {
            _ecsWorld.RemoveEventListener(this);
            _dictionary.Clear();
            _librarySystems.Remove(_ecsWorld);
        }
    }
}