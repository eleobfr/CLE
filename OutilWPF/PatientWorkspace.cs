using OutilWPF.Données;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace OutilWPF
{
    public class PatientWorkspace : BindableBase
    {
        private IClinicDataService dataService;
        private ObservableCollection<PatientSelect> patientsSelectCollection = new ObservableCollection<PatientSelect>();
        private string searchSearchPatientNom = string.Empty;
        private string searchSearchPatientPrénom = string.Empty;
        private Patient selectedPatient;
        private PatientSelect selectedPatientSelected;
        private ObservableCollection<Séance> séances = new ObservableCollection<Séance>();
        private ObservableCollection<Traitement> traitements = new ObservableCollection<Traitement>();

        public ObservableCollection<PatientSelect> PatientsSelectCollection
        {
            get { return patientsSelectCollection; }
            set { SetProperty(ref patientsSelectCollection, value); }
        }

        public string SearchSearchPatientNom
        {
            get { return searchSearchPatientNom; }
            set { SetProperty(ref searchSearchPatientNom, value); }
        }

        public string SearchSearchPatientPrénom
        {
            get { return searchSearchPatientPrénom; }
            set { SetProperty(ref searchSearchPatientPrénom, value); }
        }

        public Patient SelectedPatient
        {
            get { return selectedPatient; }
            set
            {
                if (SetProperty(ref selectedPatient, value))
                    UpdateSelectedPatientDetails();
            }
        }

        public PatientSelect SelectedPatientSelected
        {
            get { return selectedPatientSelected; }
            set
            {
                if (SetProperty(ref selectedPatientSelected, value))
                    SelectedPatient = value?.Patient;
            }
        }

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

        public void AttachDataService(IClinicDataService dataService)
        {
            this.dataService = dataService;
        }

        public void RefreshPatients()
        {
            if (dataService == null)
                return;

            SelectedPatientSelected = null;
            SelectedPatient = null;
            var patients = dataService.GetPatients(SearchSearchPatientNom, SearchSearchPatientPrénom);
            PatientsSelectCollection = new ObservableCollection<PatientSelect>(patients.Select(p => new PatientSelect() { Patient = p }));

            if (SelectedPatientSelected != null)
            {
                SelectedPatientSelected.Patient = null;
                SelectedPatientSelected.IsSelected = false;
            }
        }

        public Patient CreateNewPatient()
        {
            if (dataService == null)
                throw new InvalidOperationException("Le service de donnees patient n'est pas initialise.");

            var patient = dataService.CreateNewPatient();
            PatientsSelectCollection.Insert(0, new PatientSelect() { Patient = patient });
            SelectedPatientSelected = PatientsSelectCollection.FirstOrDefault();

            return patient;
        }

        private void UpdateSelectedPatientDetails()
        {
            if (dataService == null || SelectedPatient == null)
            {
                Séances = new ObservableCollection<Séance>();
                Traitements = new ObservableCollection<Traitement>();
                return;
            }

            Séances = dataService.GetSéances(SelectedPatient);
            Traitements = dataService.GetTraitements(SelectedPatient);
        }
    }
}
