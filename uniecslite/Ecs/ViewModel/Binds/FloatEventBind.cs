using ApplicationScripts.ViewModel.Events;

namespace ApplicationScripts.ViewModel.Binds
{
    public class FloatEventBind : EventBind<float>
    {
        public override Event<float> Create()
        {
            return new FloatEvent();
        }
    }
}