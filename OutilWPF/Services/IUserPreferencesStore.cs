using OutilWPF.Configuration;

namespace OutilWPF.Services
{
    public interface IUserPreferencesStore
    {
        UserPreferences Load();
        void Save(UserPreferences preferences);
    }
}
