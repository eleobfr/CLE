using OutilWPF;
using OutilWPF.Données;
using OutilWPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;

namespace CLE.Tests
{
    public class WorkspaceTests
    {
        [Fact]
        public void ReferenceDataWorkspace_LoadLoginChoices_MapsValues()
        {
            var workspace = new ReferenceDataWorkspace();
            var service = new FakeClinicDataService();
            service.Logins.AddRange(new[] { "SECRETAIRE", "Salle 1" });

            workspace.LoadLoginChoices(service);

            Assert.Equal(2, workspace.LesLogins.Count);
            Assert.Equal("SECRETAIRE", workspace.LesLogins[0].Item1);
            Assert.Equal("Salle 1", workspace.LesLogins[1].Item2);
        }

        [Fact]
        public void PatientWorkspace_RefreshPatients_ReplacesCollection()
        {
            var workspace = new PatientWorkspace();
            var service = new FakeClinicDataService();
            service.Patients.Add(new Patient { PatientId = 1, Nom = "DURAND", Prénom = "Alice" });
            service.Patients.Add(new Patient { PatientId = 2, Nom = "DUPONT", Prénom = "Bob" });
            workspace.AttachDataService(service);
            workspace.SearchSearchPatientNom = "DU";

            workspace.RefreshPatients();

            Assert.Equal(2, workspace.PatientsSelectCollection.Count);
            Assert.All(workspace.PatientsSelectCollection, p => Assert.StartsWith("DU", p.Patient.Nom));
        }

        [Fact]
        public void PatientWorkspace_CreateNewPatient_SelectsCreatedPatient()
        {
            var workspace = new PatientWorkspace();
            var service = new FakeClinicDataService();
            workspace.AttachDataService(service);

            var patient = workspace.CreateNewPatient();

            Assert.NotNull(patient);
            Assert.Same(patient, workspace.SelectedPatient);
            Assert.Single(workspace.PatientsSelectCollection);
        }

        [Fact]
        public void TreatmentWorkspace_CreateNewTraitement_AddsAndResetsEditor()
        {
            var workspace = new TreatmentWorkspace();
            var service = new FakeClinicDataService();
            var patient = new Patient { PatientId = 7, Nom = "MARTIN", Prénom = "Zoé" };
            var praticien = new Praticien { PraticienId = 3, UserName = "Salle 3" };
            workspace.AttachDataService(service);
            workspace.SetSelectedPatient(patient);
            workspace.EditSéanceSalle = praticien;
            workspace.EditSéanceDate = new DateTime(2026, 3, 31);
            workspace.EditSéanceZoneTraitée = "Aisselles";
            workspace.EditSéanceMS_Fluence = "22";
            workspace.EditSéanceNb_Pulses = "120";
            workspace.EditSéanceCommentaires = "RAS";
            workspace.EditSéancePrix = "60";

            workspace.CreateNewTraitement();

            Assert.Single(workspace.Traitements);
            Assert.Single(workspace.Séances);
            Assert.Null(workspace.EditSéanceZoneTraitée);
            Assert.Null(workspace.EditSéancePrix);
            Assert.Equal(new DateTime(2026, 3, 31), service.CreatedTraitements.Single().Séance.DateSéance);
        }

        [Fact]
        public void TreatmentWorkspace_DeleteSeance_RemovesLinkedTraitements()
        {
            var workspace = new TreatmentWorkspace();
            var service = new FakeClinicDataService();
            var patient = new Patient { PatientId = 4, Nom = "LEROY", Prénom = "Nina" };
            var seance = new Séance { SéanceId = 42, PatientId = patient.PatientId, DateSéance = new DateTime(2026, 1, 2) };
            var traitement = new Traitement { TraitementId = 13, SéanceId = 42, Séance = seance, ZonesTraitées = "Jambes" };
            service.Seances.Add(seance);
            service.Traitements.Add(traitement);
            workspace.AttachDataService(service);
            workspace.SetSelectedPatient(patient);

            workspace.DeleteSéance(seance);

            Assert.Empty(workspace.Séances);
            Assert.Empty(workspace.Traitements);
            Assert.Contains(seance, service.DeletedSeances);
        }

        [Fact]
        public void DatabasePathStore_StoresCurrentPath()
        {
            var store = new DatabasePathStore();

            store.SetCurrentPath(@"C:\data\patients.db");

            Assert.Equal(@"C:\data\patients.db", store.CurrentPath);
        }
    }

    internal class FakeClinicDataService : IClinicDataService
    {
        public List<string> Logins { get; } = new List<string>();
        public List<Praticien> Praticiens { get; } = new List<Praticien>();
        public List<Lapin> Lapins { get; } = new List<Lapin>();
        public List<Infosp> Infosps { get; } = new List<Infosp>();
        public List<Patient> Patients { get; } = new List<Patient>();
        public List<Séance> Seances { get; } = new List<Séance>();
        public List<Traitement> Traitements { get; } = new List<Traitement>();
        public List<Séance> DeletedSeances { get; } = new List<Séance>();
        public List<Traitement> DeletedTraitements { get; } = new List<Traitement>();
        public List<Traitement> CreatedTraitements { get; } = new List<Traitement>();
        private int nextPatientId = 1;
        private int nextSeanceId = 1;

        public void Initialize()
        {
        }

        public Login CheckLogin(string login, string password)
        {
            return new Login { UserName = login, PassWord = password, UserType = "S" };
        }

        public List<string> GetLoginList() => Logins.ToList();
        public List<Praticien> GetPraticiensList() => Praticiens.ToList();
        public IEnumerable<Lapin> GetLapinList() => Lapins.ToList();
        public IEnumerable<Infosp> GetInfosPList() => Infosps.ToList();

        public ObservableCollection<Patient> GetPatients(string nom, string prénom)
        {
            var items = Patients
                .Where(p => (p.Nom ?? string.Empty).StartsWith(nom ?? string.Empty, StringComparison.OrdinalIgnoreCase))
                .Where(p => (p.Prénom ?? string.Empty).Contains(prénom ?? string.Empty, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return new ObservableCollection<Patient>(items);
        }

        public ObservableCollection<Séance> GetSéances(Patient patient)
        {
            return new ObservableCollection<Séance>(Seances.Where(s => s.PatientId == patient.PatientId));
        }

        public ObservableCollection<Traitement> GetTraitements(Patient patient)
        {
            return new ObservableCollection<Traitement>(Traitements.Where(t => t.Séance != null && t.Séance.PatientId == patient.PatientId));
        }

        public Patient CreateNewPatient()
        {
            var patient = new Patient { PatientId = nextPatientId++, Nom = "NEW", Prénom = "PATIENT" };
            Patients.Insert(0, patient);
            return patient;
        }

        public Traitement CreateNewTraitement(Patient patient, Traitement traitement, Praticien salle, DateTime dateSéance)
        {
            var seance = new Séance
            {
                SéanceId = nextSeanceId++,
                PatientId = patient.PatientId,
                Patient = patient,
                DateSéance = dateSéance,
                Praticien = salle,
                PraticienId = salle?.PraticienId ?? 0
            };
            traitement.Séance = seance;
            traitement.SéanceId = seance.SéanceId;
            Seances.Add(seance);
            Traitements.Add(traitement);
            CreatedTraitements.Add(traitement);
            return traitement;
        }

        public void RemoveSéance(Séance séance)
        {
            DeletedSeances.Add(séance);
            Seances.Remove(séance);
            Traitements.RemoveAll(t => t.SéanceId == séance.SéanceId);
        }

        public void RemoveTraitement(Traitement traitement)
        {
            DeletedTraitements.Add(traitement);
            Traitements.Remove(traitement);
        }

        public void SaveContext()
        {
        }

        public void NewRDVOutlook(Patient patient)
        {
        }

        public void VisualiserFichePatient(Patient patient)
        {
        }

        public void ImporterPatients()
        {
        }

        public void MigrateDatas()
        {
        }
    }

    internal static class ListExtensions
    {
        public static void RemoveAll<T>(this List<T> list, Func<T, bool> predicate)
        {
            var items = list.Where(predicate).ToList();
            foreach (var item in items)
                list.Remove(item);
        }
    }
}
