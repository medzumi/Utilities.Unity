using ApplicationScripts.Properties;

namespace ApplicationScripts.ViewModel.Binds
{
    public class FloatPropertyBind : PropertyBind<float>
    {
        protected override ReactiveProperty<float> Create()
        {
            return new FloatReactiveProperty();
        }
    }
}