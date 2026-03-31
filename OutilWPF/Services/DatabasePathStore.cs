namespace OutilWPF.Services
{
    public class DatabasePathStore : IDatabasePathStore
    {
        public string CurrentPath { get; private set; }

        public void SetCurrentPath(string databasePath)
        {
            CurrentPath = databasePath;
        }
    }
}
