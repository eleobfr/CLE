namespace OutilWPF.Configuration
{
    public class ApplicationOptions
    {
        public string ApplicationTitle { get; set; } = "CLE Patients";
        public string ApplicationSubtitle { get; set; } = "Centre Etoile Laser";
        public string DefaultDatabaseFileName { get; set; } = "OutilGestionPatientDB.db";
        public string PreferencesFolderName { get; set; } = "CLE";
    }
}
