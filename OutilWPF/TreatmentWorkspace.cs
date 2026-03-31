using OutilWPF.Données;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace OutilWPF
{
    public class TreatmentWorkspace : BindableBase
    {
        private IClinicDataService dataService;
        private Patient selectedPatient;
        private ObservableCollection<Séance> séances = new ObservableCollection<Séance>();
        private ObservableCollection<Traitement> traitements = new ObservableCollection<Traitement>();
        private DateTime editSéanceDate = DateTime.Today;
        private Infosp editInfosp;
        private Praticien editSéanceSalle;
        private string editSéanceZoneTraitée;
        private string editSéanceMS_Fluence;
        private string editSéanceNb_Pulses;
        private string editSéanceCommentaires;
        private string editSéancePrix;

        public ObservableCollection<Séance> Séances
        {
            get { return séances; }
            set { SetProperty(ref séances, value); }
        }

        public ObservableCollection<Traitement> Traitements
        {
            get { return traitements; }
            set { SetProperty(ref traitements, value); }
        }

        public DateTime EditSéanceDate
        {
            get { return editSéanceDate; }
            set { SetProperty(ref editSéanceDate, value); }
        }

        public Infosp EditInfosp
        {
            get { return editInfosp; }
            set
            {
                if (SetProperty(ref editInfosp, value) && value != null)
                    EditSéanceZoneTraitée = value.InfosName;
            }
        }

        public Praticien EditSéanceSalle
        {
            get { return editSéanceSalle; }
            set { SetProperty(ref editSéanceSalle, value); }
        }

        public string EditSéanceZoneTraitée
        {
            get { return editSéanceZoneTraitée; }
            set { SetProperty(ref editSéanceZoneTraitée, value); }
        }

        public string EditSéanceMS_Fluence
        {
            get { return editSéanceMS_Fluence; }
            set { SetProperty(ref editSéanceMS_Fluence, value); }
        }

        public string EditSéanceNb_Pulses
        {
            get { return editSéanceNb_Pulses; }
            set { SetProperty(ref editSéanceNb_Pulses, value); }
        }

        public string EditSéanceCommentaires
        {
            get { return editSéanceCommentaires; }
            set { SetProperty(ref editSéanceCommentaires, value); }
        }

        public string EditSéancePrix
        {
            get { return editSéancePrix; }
            set { SetProperty(ref editSéancePrix, value); }
        }

        public bool AddTraitementPanelEnabled
        {
            get { return SelectedPatient != null; }
        }

        public Patient SelectedPatient
        {
            get { return selectedPatient; }
            private set
            {
                if (SetProperty(ref selectedPatient, value))
                {
                    LoadSelectedPatientDetails();
                    RaisePropertyChanged(nameof(AddTraitementPanelEnabled));
                }
            }
        }

        public void AttachDataService(IClinicDataService dataService)
        {
            this.dataService = dataService;
        }

        public void SetSelectedPatient(Patient patient)
        {
            SelectedPatient = patient;
        }

        public void ResetEditor()
        {
            EditSéanceDate = DateTime.Today;
            EditSéanceZoneTraitée = null;
            EditSéanceMS_Fluence = null;
            EditSéanceNb_Pulses = null;
            EditSéanceCommentaires = null;
            EditSéancePrix = null;
            EditInfosp = null;
        }

        public void ChangeSalleSéance(Séance séance)
        {
            if (séance == null || EditSéanceSalle == null || dataService == null)
                return;

            séance.Praticien = EditSéanceSalle;
            dataService.SaveContext();
        }

        public void DeleteSéance(Séance séance)
        {
            if (séance == null || dataService == null)
                return;

            foreach (var traitement in Traitements.Where(t => t.SéanceId == séance.SéanceId).ToList())
                Traitements.Remove(traitement);

            Séances.Remove(séance);
            dataService.RemoveSéance(séance);
        }

        public void DeleteTraitement(Traitement traitement)
        {
            if (traitement == null || dataService == null)
                return;

            Traitements.Remove(traitement);
            dataService.RemoveTraitement(traitement);
        }

        public void CreateNewTraitement()
        {
            if (dataService == null || SelectedPatient == null)
                return;

            var traitement = new Traitement
            {
                ZonesTraitées = EditSéanceZoneTraitée,
                Fluence = EditSéanceMS_Fluence,
                Pulses = EditSéanceNb_Pulses,
                Commentaires = EditSéanceCommentaires,
                Prix = EditSéancePrix
            };

            dataService.CreateNewTraitement(SelectedPatient, traitement, EditSéanceSalle, EditSéanceDate);
            Traitements.Add(traitement);
            Traitements = new ObservableCollection<Traitement>(Traitements.OrderByDescending(p => p.Séance.DateSéance).ThenBy(p => p.Fluence));

            if (traitement.Séance != null && !Séances.Any(s => s.SéanceId == traitement.Séance.SéanceId))
                Séances.Add(traitement.Séance);

            ResetEditor();
        }

        private void LoadSelectedPatientDetails()
        {
            if (dataService == null || SelectedPatient == null)
            {
                Séances = new ObservableCollection<Séance>();
                Traitements = new ObservableCollection<Traitement>();
                ResetEditor();
                return;
            }

            Séances = dataService.GetSéances(SelectedPatient);
            Traitements = dataService.GetTraitements(SelectedPatient);
            ResetEditor();
        }
    }
}
