using System.Collections.Generic;

namespace Utilities.Unity.Editor
{
    public class EditorContext : Dictionary<string, object>
    {
        private static EditorContext _instance;

        public static EditorContext Instance
        {
            get => _instance ??= new EditorContext();
        }

        private EditorContext()
        {
            
        }
    }
}