using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;


namespace OutilWPF.Données
{
    public class Context : DbContext
    {
        private static bool _created = false;
        private readonly string databasePath;

        public Context(string databasePath)
        {
            this.databasePath = databasePath;
            if (!_created)
            {
                Database.EnsureCreated();
                _created = true;
            }
        }
        public DbSet<Login> Logins { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Séance> Séances { get; set; }
        public DbSet<Praticien> Praticiens { get; set; }
        public DbSet<Traitement> Traitements { get; set; }
        public DbSet<RDVOutlook> RDVsOutlook { get; set; }
        public DbSet<Lapin> Lapins { get; set; }
        public DbSet<Infosp> Infosps { get; set; }
        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder() { DataSource = databasePath }.ToString();
            var connection = new SqliteConnection(connectionStringBuilder);
            optionsBuilder.UseSqlite(connection);
        }
    }
}
