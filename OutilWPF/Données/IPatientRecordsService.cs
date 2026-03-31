using System;
using System.Collections.ObjectModel;

namespace OutilWPF.Données
{
    public interface IPatientRecordsService
    {
        ObservableCollection<Patient> GetPatients(string nom, string prénom);
        ObservableCollection<Séance> GetSéances(Patient patient);
        ObservableCollection<Traitement> GetTraitements(Patient patient);
        Patient CreateNewPatient();
        Traitement CreateNewTraitement(Patient patient, Traitement traitement, Praticien salle, DateTime dateSéance);
        void RemoveSéance(Séance séance);
        void RemoveTraitement(Traitement traitement);
    }
}
