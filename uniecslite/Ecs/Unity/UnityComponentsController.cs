using System.Collections.Generic;
using ApplicationScripts.Ecs;
using ApplicationScripts.Ecs.Unity;
using ApplicationScripts.Ecs.Utility;
using ApplicationScripts.Unity;
using Leopotam.EcsLite;
using UnityEngine;

namespace ApplicationScripts.Logic.Features.Unity
{
    [DefaultExecutionOrder(-105)]
    public class UnityComponentsController : MonoBehaviour, IMonoProvider
    {
        [SerializeField] private List<UnityComponent> _viewComponents;

        private EcsPool<ListComponent<UnityComponentsController>> _pool;
        private int _entity;
        private bool _isDisposed = false;

        public int Entity => _entity;
        protected EcsWorld world { get; private set; }

        private void Reset()
        {
            BuildUp();
        }

        public void Initialize(EcsWorld ecsWorld, int entity)
        {
            world = ecsWorld;
            _isDisposed = false;
            _pool = ecsWorld.GetPool<ListComponent<UnityComponentsController>>();
            foreach (var view in _viewComponents)
            {
                view.Initialize(ecsWorld, entity);
            }

            _entity = entity;
            _pool.Ensure(entity).ComponentData.Add(this);
        }

        public void Dispose()
        {
            if (_isDisposed || _pool == null)
            {
                return;
            }

            _isDisposed = true;
            foreach (var view in _viewComponents)
            {
                view.Dispose();
            }
            _pool.SafeDel(_entity, world);
            _pool = null;
        }

        public void Convert(int entity, EcsWorld world)
        {
            Initialize(world, entity);
        }

        private void OnDestroy()
        {
            Dispose();
        }

        [Button(Name = "Build Up Collection")]
        private void BuildUp()
        {
            GetComponentsInChildren<UnityComponent>(_viewComponents);
            
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}