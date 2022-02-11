using System;
using System.Collections.Generic;
using Leopotam.EcsLite;

namespace ApplicationScripts.Ecs.Utility
{
    [Serializable]
    public struct ListComponent<T> : IEcsAutoReset<ListComponent<T>>
    {
        public List<T> ComponentData;
        public void AutoReset(ref ListComponent<T> c)
        {
            if (c.ComponentData == null)
            {
                c.ComponentData = new List<T>();
            }
            else
            {
                c.ComponentData.Clear();
            }
        }
    }
}