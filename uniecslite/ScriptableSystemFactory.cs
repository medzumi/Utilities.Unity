using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Leopotam.EcsLite
{
    public abstract class ScriptableSystemFactory : ScriptableObject
    {
        public abstract IEcsSystem CreateSystem();
    }
}
