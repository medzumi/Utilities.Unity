namespace ApplicationScripts.Logic.Features.Unity
{
    public interface IUpdatable<T>
    {
        public void UpdateData(T data);

        public void Clear();
    }
}