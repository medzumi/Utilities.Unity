using ApplicationScripts.Properties;

namespace ApplicationScripts.ViewModel.Binds
{
    public class IntPropertyBind : PropertyBind<int>
    {
        protected override ReactiveProperty<int> Create()
        {
            return new IntReactiveProperty();
        }
    }
}