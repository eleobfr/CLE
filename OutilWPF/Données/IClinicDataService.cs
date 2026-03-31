using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OutilWPF.Données
{
    public interface IClinicDataService
    {
        void Initialize();
        Login CheckLogin(string login, string password);
        List<string> GetLoginList();
        List<Praticien> GetPraticiensList();
        IEnumerable<Lapin> GetLapinList();
        IEnumerable<Infosp> GetInfosPList();

        ObservableCollection<Patient> GetPatients(string nom, string prénom);
        ObservableCollection<Séance> GetSéances(Patient patient);
        ObservableCollection<Traitement> GetTraitements(Patient patient);

        Patient CreateNewPatient();
        Traitement CreateNewTraitement(Patient patient, Traitement traitement, Praticien salle, DateTime dateSéance);
        void RemoveSéance(Séance séance);
        void RemoveTraitement(Traitement traitement);
        void SaveContext();

        void NewRDVOutlook(Patient patient);
        void VisualiserFichePatient(Patient patient);
        void ImporterPatients();
        void MigrateDatas();
    }
}
