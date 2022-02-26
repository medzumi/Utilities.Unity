using ApplicationScripts.ViewModel.Events;

namespace ApplicationScripts.ViewModel.Binds
{
    public class IntEventBind : EventBind<int>
    {
        public override Event<int> Create()
        {
            return new IntEvent();
        }
    }
}