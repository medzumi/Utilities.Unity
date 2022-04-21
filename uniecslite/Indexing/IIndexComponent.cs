using System.Collections.Generic;

namespace ApplicationScripts.Logic.Features.Indexing
{
    public interface IIndexComponent<T>
    {
        T GetIndex();
    }

    public interface IIndexesComponent<T>
    {
        List<T> GetIndexes();
    }
}