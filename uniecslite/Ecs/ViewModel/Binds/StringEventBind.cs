using ApplicationScripts.ViewModel.Data;
using ApplicationScripts.ViewModel.Events;

namespace ApplicationScripts.ViewModel.Binds
{
    public class StringEventBind : EventBind<StringStruct>
    {
        public override Event<StringStruct> Create()
        {
            return new StringEvent();
        }
    }
}