using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Leopotam.EcsLite
{
    public abstract class ScriptableWorldFactory : ScriptableObject
    {
        [SerializeField] private string _worldName;

        public string WorldName => _worldName;
        
        public abstract EcsWorld CreateWorld();
    }
}
