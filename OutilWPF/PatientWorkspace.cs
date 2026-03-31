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
            set { SetProperty(ref selectedPatient, value); }
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

        public void AttachDataService(IClinicDataService dataService)
        {
            this.dataService = dataService;
        }

        public void RefreshPatients()
        {
            if (dataService == null)
                return;

            SelectedPatientSelected = null;
            var patients = dataService.GetPatients(SearchSearchPatientNom, SearchSearchPatientPrénom);
            PatientsSelectCollection = new ObservableCollection<PatientSelect>(patients.Select(p => new PatientSelect() { Patient = p }));
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
    }
}
