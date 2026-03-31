using Microsoft.Extensions.Options;
using Microsoft.Win32;
using OutilWPF.Configuration;
using System.IO;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace OutilWPF.Services
{
    [SupportedOSPlatform("windows")]
    public class DatabaseSelectionService : IDatabaseSelectionService
    {
        private readonly IUserPreferencesStore preferencesStore;
        private readonly IDatabasePathStore databasePathStore;
        private readonly ApplicationOptions options;

        public DatabaseSelectionService(
            IUserPreferencesStore preferencesStore,
            IDatabasePathStore databasePathStore,
            IOptions<ApplicationOptions> options)
        {
            this.preferencesStore = preferencesStore;
            this.databasePathStore = databasePathStore;
            this.options = options.Value;
        }

        public bool TryEnsureDatabasePath()
        {
            var preferences = preferencesStore.Load();
            var databasePath = preferences.InternalDatabasePath;

            if (!string.IsNullOrWhiteSpace(databasePath) && File.Exists(databasePath))
            {
                databasePathStore.SetCurrentPath(databasePath);
                return true;
            }

            var picker = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Base commune (*db)|*.db"
            };

            if (picker.ShowDialog() == true)
                databasePath = picker.FileName;

            if (string.IsNullOrWhiteSpace(databasePath))
            {
                using var folderPicker = new FolderBrowserDialog();
                var result = folderPicker.ShowDialog();
                if (result == DialogResult.OK)
                    databasePath = Path.Combine(folderPicker.SelectedPath, options.DefaultDatabaseFileName);
            }

            if (string.IsNullOrWhiteSpace(databasePath))
                return false;

            preferences.InternalDatabasePath = databasePath;
            preferencesStore.Save(preferences);
            databasePathStore.SetCurrentPath(databasePath);
            return true;
        }
    }
}
