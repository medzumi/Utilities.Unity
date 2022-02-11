namespace ApplicationScripts.Ecs.Utility
{
    public struct ReferenceComponent<TReference> where TReference : class
    {
        public TReference reference;
    }
}