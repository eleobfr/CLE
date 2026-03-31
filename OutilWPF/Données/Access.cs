using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using OutilWPF.Services;
using System.Runtime.Versioning;

namespace OutilWPF.Données
{
    [SupportedOSPlatform("windows")]
    public class Access : IDisposable
        , IClinicDataService
    {
        private readonly IDatabasePathStore databasePathStore;
        private readonly IOfficeInteropService officeInteropService;
        private Context dataContext;
        private IPatientRecordsService patientRecordsService;
        private IReferenceDataService referenceDataService;
        public Access(IDatabasePathStore databasePathStore, IOfficeInteropService officeInteropService)
        {
            this.databasePathStore = databasePathStore;
            this.officeInteropService = officeInteropService;
        }

        private void LoadDatabaseWithFakeDatas()
        {
            dataContext.Lapins.Add(new Lapin() { LapinName = "0 lapin", LapinColor = "T" });
            dataContext.Lapins.Add(new Lapin() { LapinName = "1 lapins", LapinColor = "J" });
            dataContext.Lapins.Add(new Lapin() { LapinName = "2 lapins", LapinColor = "R" });
            dataContext.Lapins.Add(new Lapin() { LapinName = "Ne plus donner de RDV", LapinColor = "VIOLET" });
            dataContext.Lapins.Add(new Lapin() { LapinName = "Contentieux", LapinColor = "BLUE" });
            dataContext.Lapins.Add(new Lapin() { LapinName = "Contentieux", LapinColor = "BLACK" });
            dataContext.SaveChanges();

            dataContext.Logins.Add(new Login() { UserName = "SECRETAIRE", PassWord = "", UserType = "S" });
            dataContext.Logins.Add(new Login() { UserName = "Salle 1", PassWord = "", UserType = "P" });
            dataContext.Logins.Add(new Login() { UserName = "Salle 2", PassWord = "", UserType = "P" });
            dataContext.Logins.Add(new Login() { UserName = "Salle 3", PassWord = "", UserType = "P" });
            dataContext.Logins.Add(new Login() { UserName = "Salle 4", PassWord = "", UserType = "P" });
            dataContext.Logins.Add(new Login() { UserName = "Salle 5", PassWord = "", UserType = "P" });
            dataContext.SaveChanges();

            dataContext.Praticiens.Add(new Praticien() { Nom = "BOULEAU", Prénom = "Marine", UserName = "Salle 1" });
            dataContext.Praticiens.Add(new Praticien() { Nom = "SIMON", Prénom = "Eric", UserName = "Salle 2" });
            dataContext.Praticiens.Add(new Praticien() { Nom = "DANTENY", Prénom = "Gérard", UserName = "Salle 3" });
            dataContext.Praticiens.Add(new Praticien() { Nom = "DANTENY", Prénom = "Gérard", UserName = "Salle 4" });
            dataContext.Praticiens.Add(new Praticien() { Nom = "DANTENY", Prénom = "Gérard", UserName = "Salle 5" });
            dataContext.SaveChanges();

            //dataContext.Patients.Add(new Patient() { Civilité = "M.", Nom = "LUC", Prénom = "Pierre", Adresse1 = "rue des Penthièvre", LapinId = 1 });
            //dataContext.Patients.Add(new Patient() { Civilité = "M.", Nom = "BELLANCE", Prénom = "Vanuel", Adresse1 = "rue des acacias", LapinId = 4 });
            //dataContext.SaveChanges();
        }

        public IEnumerable<Lapin> GetLapinList()
        {
            EnsureInitialized();
            return referenceDataService.GetLapinList();
        }
        public IEnumerable<Infosp> GetInfosPList()
        {
            EnsureInitialized();
            return referenceDataService.GetInfosPList();
        }

        public ObservableCollection<Patient> GetPatients(string sNom, string sPrénom)
        {
            EnsureInitialized();
            return patientRecordsService.GetPatients(sNom, sPrénom);
        }

        public List<string> GetLoginList()
        {
            EnsureInitialized();
            return referenceDataService.GetLoginList();
        }

        private List<Patient> GetPatientsList()
        {
            var _tmp = new List<Patient>();
            dataContext.Patients.ToList().ForEach(l => _tmp.Add(l));

            return _tmp;
        }

        public List<Praticien> GetPraticiensList()
        {
            EnsureInitialized();
            return referenceDataService.GetPraticiensList();
        }

        public List<string> GetPraticienList()
        {
            var _tmp = new List<string>();
            dataContext.Logins.AsQueryable().Where(p => p.UserType.Contains("P")).ToList().ForEach(l => _tmp.Add(l.UserName));
            return _tmp;
        }

        public Login CheckLogin(string login, string password)
        {
            EnsureInitialized();
            return referenceDataService.CheckLogin(login, password);
        }

        public List<string> GetCivilités()
        {
            return new List<string>() { "M.", "Mme" };
        }

        public Patient CreateNewPatient()
        {
            EnsureInitialized();
            return patientRecordsService.CreateNewPatient();
        }

        public Séance CreateNewSéance(int patientId, string userName)
        {
            EnsureInitialized();
            var login = dataContext.Praticiens.First(p => p.UserName == userName);
            var np = new Séance() { PatientId = patientId, PraticienId = login.PraticienId };
            np.DateSéance = DateTime.Today;
            dataContext.Séances.Add(np);
            dataContext.SaveChanges();
            return np;
        }

        public Traitement CreateNewTraitement(Patient patient, Traitement tr, Praticien salle, DateTime dateSéance)
        {
            EnsureInitialized();
            return patientRecordsService.CreateNewTraitement(patient, tr, salle, dateSéance);
        }

        public ObservableCollection<RDVOutlook> GetRDVOulook(Patient patient)
        {
            EnsureInitialized();
            ObservableCollection<RDVOutlook> rdvsoutlook = null;
            if (patient != null)
            {
                var _rdvsoutlook = dataContext.RDVsOutlook.Include(p => p.Praticien).Where(r => r.PatientId == patient.PatientId);
                rdvsoutlook = new ObservableCollection<RDVOutlook>(_rdvsoutlook);
            }
            return rdvsoutlook;
        }

        public ObservableCollection<Séance> GetSéances(Patient patient)
        {
            EnsureInitialized();
            return patientRecordsService.GetSéances(patient);
        }

        public ObservableCollection<Traitement> GetTraitements(Patient patient)
        {
            EnsureInitialized();
            return patientRecordsService.GetTraitements(patient);
        }

        public void NewRDVOutlook(Patient patient)
        {
            EnsureInitialized();
            officeInteropService.NewRDVOutlook(patient);
        }

        public void UpdateRDVOutlook()
        {
            EnsureInitialized();
            officeInteropService.UpdateRDVOutlook();
        }

        public void VisualiserFichePatient(Patient patient)
        {
            EnsureInitialized();
            officeInteropService.VisualiserFichePatient(patient);
        }

        public void RemoveSéance(Séance item)
        {
            EnsureInitialized();
            patientRecordsService.RemoveSéance(item);
        }

        public void RemoveTraitement(Traitement item)
        {
            EnsureInitialized();
            patientRecordsService.RemoveTraitement(item);
        }
        public void ImporterPatients()
        {
            EnsureInitialized();
            officeInteropService.ImporterPatients();
        }

        public void MigrateDatas()
        {
            EnsureInitialized();
            //Ajout infos supplémentaires
            //var sql = "CREATE TABLE `Infosps` ( `InfospId` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, `InfosName` TEXT )";
            //var result = dataContext.Database.ExecuteSqlCommand(sql);
            //sql = "CREATE INDEX `IX_Infosps_InfospId` ON `Infosps` ( `InfospId` )";
            //result = dataContext.Database.ExecuteSqlCommand(sql);

            //var ip = new Infosp() { InfosName = null };
            //dataContext.Infosps.Add(ip);
            //dataContext.Infosps.Add(new Infosp() { InfosName = "Annulé le jour même" });
            //dataContext.SaveChanges();

            //sql = "ALTER TABLE `Patients` ADD COLUMN 	`InfospID`	INTEGER  REFERENCES `Infosps`(`InfospID`)";
            //var result2 = dataContext.Database.ExecuteSqlCommand(sql);
            var sql = @" UPDATE [Patients] SET InfospID = @infospID";
            var result = dataContext.Database.ExecuteSqlRaw(sql, new Microsoft.Data.Sqlite.SqliteParameter("@infospID", 1));

            /*
            foreach (var p in dataContext.Patients)
            {
                if (p.Nom != null) p.Nom = p.Nom.ToUpper().Trim(); if (p.Nom == "") p.Nom = null;
            }

            foreach (var t in dataContext.Traitements)
            {
                if (t.Commentaires != null && t.Commentaires.Contains("lapin"))
                {
                    t.ZonesTraitées = "lapin"; if (t.Commentaires == "lapin") t.Commentaires = null;
                }
                if (t.Pulses == "0")
                {
                    t.Pulses = null;
                }
                if (t.ZonesTraitées == null && t.Fluence == null && t.Pulses == null && t.Prix == null && t.Commentaires == null)
                    dataContext.Traitements.Remove(t);
            }
            dataContext.SaveChanges();*/
        }
        public void SaveContext()
        {
            EnsureInitialized();
            dataContext.SaveChanges();
        }

        public void Initialize()
        {
            EnsureInitialized();
        }

        private void EnsureInitialized()
        {
            if (dataContext != null)
                return;

            if (string.IsNullOrWhiteSpace(databasePathStore.CurrentPath))
                throw new InvalidOperationException("Le chemin de la base de donnees n'est pas configure.");

            dataContext = new Context(databasePathStore.CurrentPath);
            patientRecordsService = new PatientRecordsService(dataContext);
            referenceDataService = new ReferenceDataService(dataContext);

            if (dataContext.Logins.Count() == 0)
                LoadDatabaseWithFakeDatas();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    dataContext.Dispose();
                    //externalContext.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Access() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

