using ApplicationScripts.Properties;
using ApplicationScripts.ViewModel.Data;

namespace ApplicationScripts.ViewModel.Binds
{
    public class StringPropertyBind : PropertyBind<StringStruct>
    {
        protected override ReactiveProperty<StringStruct> Create()
        {
            return new StringReactiveProperty();
        }
    }
}