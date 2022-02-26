namespace ApplicationScripts.ViewModel.Data
{
    public struct StringStruct
    {
        public string Data;

        public StringStruct(string data)
        {
            Data = data;
        }

        public override string ToString()
        {
            return Data;
        }
    }
}