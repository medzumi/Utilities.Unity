using ApplicationScripts.Properties;
using UnityEngine;

namespace ApplicationScripts.ViewModel.Binds
{
    public class Vector2PropertyBind : PropertyBind<Vector2>
    {
        protected override ReactiveProperty<Vector2> Create()
        {
            return new Vector2ReactiveProperty();
        }
    }
}