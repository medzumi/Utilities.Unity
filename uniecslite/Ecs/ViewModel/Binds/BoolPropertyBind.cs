using ApplicationScripts.Properties;

namespace ApplicationScripts.ViewModel.Binds
{
    public class BoolPropertyBind : PropertyBind<bool>
    {
        protected override ReactiveProperty<bool> Create()
        {
            return new BoolReactiveProperty();
        }
    }
}