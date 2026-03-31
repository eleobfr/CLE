using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace OutilWPF.Données
{
    public class PatientRecordsService : IPatientRecordsService
    {
        private readonly Context dataContext;

        public PatientRecordsService(Context dataContext)
        {
            this.dataContext = dataContext;
        }

        public ObservableCollection<Patient> GetPatients(string nom, string prénom)
        {
            nom = nom ?? string.Empty;
            prénom = prénom ?? string.Empty;

            var patients = dataContext.Patients
                .Include(p => p.Lapin)
                .Include(p => p.Séances)
                .Where(p => (p.Nom ?? string.Empty).ToLower().StartsWith(nom.ToLower())
                    && (p.Prénom ?? string.Empty).ToLower().Contains(prénom.ToLower()))
                .OrderBy(p => p.Nom)
                .ThenBy(p => p.Prénom);

            return new ObservableCollection<Patient>(patients);
        }

        public ObservableCollection<Séance> GetSéances(Patient patient)
        {
            if (patient == null)
                return new ObservableCollection<Séance>();

            var séances = dataContext.Séances
                .AsQueryable()
                .OrderByDescending(p => p.DateSéance)
                .Include(p => p.Praticien)
                .Where(r => r.PatientId == patient.PatientId);

            return new ObservableCollection<Séance>(séances);
        }

        public ObservableCollection<Traitement> GetTraitements(Patient patient)
        {
            if (patient == null || !dataContext.Traitements.Any())
                return new ObservableCollection<Traitement>();

            var traitements = dataContext.Traitements
                .Include(p => p.Séance)
                .OrderByDescending(p => p.Séance.DateSéance)
                .ThenBy(p => p.Fluence)
                .Where(r => r.Séance.PatientId == patient.PatientId);

            return new ObservableCollection<Traitement>(traitements);
        }

        public Patient CreateNewPatient()
        {
            var patient = new Patient
            {
                Lapin = dataContext.Lapins.First(),
            };

            dataContext.Patients.Add(patient);
            dataContext.SaveChanges();

            return patient;
        }

        public Traitement CreateNewTraitement(Patient patient, Traitement traitement, Praticien salle, DateTime dateSéance)
        {
            if (dataContext.Séances.Any(p => p.DateSéance == dateSéance && p.PatientId == patient.PatientId && p.Praticien.PraticienId == salle.PraticienId))
            {
                var séance = dataContext.Séances.First(p => p.DateSéance == dateSéance && p.PatientId == patient.PatientId && p.Praticien.PraticienId == salle.PraticienId);
                traitement.SéanceId = séance.SéanceId;
                traitement.Séance = séance;
            }
            else
            {
                var séance = new Séance() { PatientId = patient.PatientId, PraticienId = salle.PraticienId, DateSéance = dateSéance };
                dataContext.Séances.Add(séance);
                dataContext.SaveChanges();
                traitement.Séance = séance;
            }

            dataContext.Traitements.Add(traitement);
            dataContext.SaveChanges();

            return traitement;
        }

        public void RemoveSéance(Séance séance)
        {
            dataContext.Séances.Remove(séance);
            dataContext.SaveChanges();
        }

        public void RemoveTraitement(Traitement traitement)
        {
            dataContext.Traitements.Remove(traitement);
            dataContext.SaveChanges();
        }
    }
}
