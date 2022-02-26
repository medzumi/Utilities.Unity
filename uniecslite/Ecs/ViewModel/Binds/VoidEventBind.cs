using ApplicationScripts.ViewModel.Data;
using ApplicationScripts.ViewModel.Events;

namespace ApplicationScripts.ViewModel.Binds
{
    public class VoidEventBind : EventBind<VoidStruct>
    {
        public override Event<VoidStruct> Create()
        {
            return new VoidEvent();
        }
    }
}