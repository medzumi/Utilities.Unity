using ApplicationScripts.ViewModel.Events;

namespace ApplicationScripts.ViewModel.Binds
{
    public class BoolEventBind : EventBind<bool>
    {
        public override Event<bool> Create()
        {
            return new BoolEvent();
        }
    }
}