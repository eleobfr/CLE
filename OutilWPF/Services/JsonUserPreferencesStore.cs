using Microsoft.Extensions.Options;
using OutilWPF.Configuration;
using System;
using System.IO;
using System.Text.Json;

namespace OutilWPF.Services
{
    public class JsonUserPreferencesStore : IUserPreferencesStore
    {
        private readonly string preferencesPath;

        public JsonUserPreferencesStore(IOptions<ApplicationOptions> options)
        {
            var folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                options.Value.PreferencesFolderName);

            Directory.CreateDirectory(folder);
            preferencesPath = Path.Combine(folder, "user-preferences.json");
        }

        public UserPreferences Load()
        {
            if (!File.Exists(preferencesPath))
                return new UserPreferences();

            var content = File.ReadAllText(preferencesPath);
            if (string.IsNullOrWhiteSpace(content))
                return new UserPreferences();

            return JsonSerializer.Deserialize<UserPreferences>(content) ?? new UserPreferences();
        }

        public void Save(UserPreferences preferences)
        {
            var content = JsonSerializer.Serialize(preferences, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(preferencesPath, content);
        }
    }
}
