namespace OutilWPF.Services
{
    public interface IDatabasePathStore
    {
        string CurrentPath { get; }
        void SetCurrentPath(string databasePath);
    }
}
