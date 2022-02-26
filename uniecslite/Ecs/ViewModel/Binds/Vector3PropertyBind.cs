using ApplicationScripts.Properties;
using UnityEngine;

namespace ApplicationScripts.ViewModel.Binds
{
    public class Vector3PropertyBind : PropertyBind<Vector3>
    {
        protected override ReactiveProperty<Vector3> Create()
        {
            return new Vector3ReactiveProperty();
        }
    }
}