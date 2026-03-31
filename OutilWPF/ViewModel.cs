using OutilWPF.Données;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace OutilWPF
{
    public class ViewModel : BindableBase
    {
        private readonly PatientWorkspace patientWorkspace;
        private readonly ReferenceDataWorkspace referenceDataWorkspace;
        private readonly TreatmentWorkspace treatmentWorkspace;
        private Login UserConnected = null;
        public List<Tuple<string, string>> LesLogins
        {
            get { return referenceDataWorkspace.LesLogins; }
            set { referenceDataWorkspace.LesLogins = value; }
        }
        public List<Praticien> LesPraticiens
        {
            get { return referenceDataWorkspace.LesPraticiens; }
            set { referenceDataWorkspace.LesPraticiens = value; }
        }
        //public List<string> LesCivilités { get; } = new List<string>();
        private bool EnableSaveContext = true;
        private bool enableApplication = true;
        public bool EnableApplication
        {
            get { return enableApplication; }
            set
            {
                SetProperty(ref enableApplication, value);
            }
        }

        internal void CheckLogin(string login, string password)
        {
            UserConnected = da.CheckLogin(login, password);
            LoginVisibility = UserConnected != null ? Visibility.Collapsed : Visibility.Visible;
            SpWorkVisibility = LoginVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            if (UserConnected != null)
            {
                referenceDataWorkspace.LoadAuthenticatedReferenceData(da);
                if (LesPraticiens.Any())
                    EditSéanceSalle = LesPraticiens.First();
                RefreshViewPatientsFromDataContext();
            }
            RaisePropertyChanged("InfoPatientEditVisibility");
            RaisePropertyChanged("InfoPatientViewVisibility");
            RaisePropertyChanged("OutlookVisibility");
        }

        private Visibility _loginVisibility = Visibility.Visible;
        public Visibility LoginVisibility
        {
            get { return _loginVisibility; }
            set
            {
                SetProperty(ref _loginVisibility, value);
            }
        }

        private Visibility _spWorkVisibility = Visibility.Collapsed;
        public Visibility SpWorkVisibility
        {
            get { return _spWorkVisibility; }
            set
            {
                SetProperty(ref _spWorkVisibility, value);
            }
        }

        //private Visibility _infoPatientEditVisibility = Visibility.Collapsed;
        public Visibility InfoPatientEditVisibility
        {
            get
            {
                Visibility _infoPatientEditVisibility = Visibility.Collapsed;
                if (UserConnected != null)
                {
                    if (UserConnected.UserType == "S")
                        _infoPatientEditVisibility = Visibility.Visible;
                }
                return _infoPatientEditVisibility;
            }
        }

        //private Visibility _infoPatientViewVisibility = Visibility.Collapsed;
        public Visibility InfoPatientViewVisibility
        {
            get
            {
                Visibility _infoPatientViewVisibility = Visibility.Collapsed;
                if (UserConnected != null)
                {
                    if (UserConnected.UserType == "P")
                        _infoPatientViewVisibility = Visibility.Visible;
                }
                return _infoPatientViewVisibility;
            }
        }

        public Visibility OutlookVisibility
        {
            get
            {
                Visibility _outlookVisibility = Visibility.Collapsed;
                if (UserConnected != null)
                {
                    if (UserConnected.UserType == "S")
                        _outlookVisibility = Visibility.Visible;
                }
                return _outlookVisibility;
            }
            //set
            //{
            //    _outlookVisibility = value;
            //    
            //}
        }
        public Visibility SearchErrorVisibility
        {
            get
            {
                Visibility searchErrorVisibility = Visibility.Collapsed;
                if (PatientsSelectCollection != null && !PatientsSelectCollection.Any())
                    searchErrorVisibility = Visibility.Visible;
                return searchErrorVisibility;
            }
        }

        internal void MigrerDonnées()
        {
            da.MigrateDatas();
        }

        //public bool NewSeanceEnabled
        //{
        //    get
        //    {
        //        bool _newSeanceEnabled = false;
        //        if (UserConnected != null && selectedPatient != null)
        //        {
        //            if (UserConnected.UserType == "P" || UserConnected.UserType == "G")
        //                _newSeanceEnabled = true;
        //        }
        //        return _newSeanceEnabled;
        //    }
        //}

        //public Visibility AddTraitementPanelVisibility
        //{
        //    get
        //    {
        //        var _addTraitementPanelVisibility = Visibility.Collapsed;
        //        if (SelectedPatient != null)
        //            _addTraitementPanelVisibility = Visibility.Visible;
        //        return _addTraitementPanelVisibility;
        //    }
        //}

        public bool AddTraitementPanelEnabled
        {
            get { return treatmentWorkspace.AddTraitementPanelEnabled; }
        }

        public ObservableCollection<PatientSelect> PatientsSelectCollection
        {
            get { return patientWorkspace.PatientsSelectCollection; }
            set { patientWorkspace.PatientsSelectCollection = value; }
        }
        //public ICollectionView PatientsView
        //{
        //    get { return CollectionViewSource.GetDefaultView(Patients); }
        //}

        public string SearchSearchPatientNom
        {
            get { return patientWorkspace.SearchSearchPatientNom; }
            set { patientWorkspace.SearchSearchPatientNom = value; }
        }

        public string SearchSearchPatientPrénom
        {
            get { return patientWorkspace.SearchSearchPatientPrénom; }
            set { patientWorkspace.SearchSearchPatientPrénom = value; }
        }

        internal void FreezeSaveDataContext(bool état)
        {
            EnableSaveContext = état;
        }
        public Patient SelectedPatient
        {
            get { return patientWorkspace.SelectedPatient; }
            set { patientWorkspace.SelectedPatient = value; }
        }
        public PatientSelect SelectedPatientSelected
        {
            get { return patientWorkspace.SelectedPatientSelected; }
            set { patientWorkspace.SelectedPatientSelected = value; }
        }

        //private ObservableCollection<RDVOutlook> _rdvsoutlook;
        //public ObservableCollection<RDVOutlook> RDVsOutlook
        //{
        //    get { return _rdvsoutlook; }
        //    set
        //    {
        //        SetProperty(ref _rdvsoutlook, value);
        //    }
        //}

        public ObservableCollection<Séance> Séances
        {
            get { return treatmentWorkspace.Séances; }
            set { treatmentWorkspace.Séances = value; }
        }

        public ObservableCollection<Traitement> Traitements
        {
            get { return treatmentWorkspace.Traitements; }
            set { treatmentWorkspace.Traitements = value; }
        }

        public List<Lapin> ListLapins
        {
            get { return referenceDataWorkspace.ListLapins; }
            set { referenceDataWorkspace.ListLapins = value; }
        }

        public List<Infosp> ListInfosp
        {
            get { return referenceDataWorkspace.ListInfosp; }
            set { referenceDataWorkspace.ListInfosp = value; }
        }


        public DateTime EditSéanceDate
        {
            get { return treatmentWorkspace.EditSéanceDate; }
            set { treatmentWorkspace.EditSéanceDate = value; }
        }

        public Infosp EditInfosp
        {
            get { return treatmentWorkspace.EditInfosp; }
            set { treatmentWorkspace.EditInfosp = value; }
        }

        public Praticien EditSéanceSalle
        {
            get { return treatmentWorkspace.EditSéanceSalle; }
            set { treatmentWorkspace.EditSéanceSalle = value; }
        }

        public string EditSéanceZoneTraitée
        {
            get { return treatmentWorkspace.EditSéanceZoneTraitée; }
            set { treatmentWorkspace.EditSéanceZoneTraitée = value; }
        }

        public string EditSéanceMS_Fluence
        {
            get { return treatmentWorkspace.EditSéanceMS_Fluence; }
            set { treatmentWorkspace.EditSéanceMS_Fluence = value; }
        }

        public string EditSéanceNb_Pulses
        {
            get { return treatmentWorkspace.EditSéanceNb_Pulses; }
            set { treatmentWorkspace.EditSéanceNb_Pulses = value; }
        }

        public string EditSéanceCommentaires
        {
            get { return treatmentWorkspace.EditSéanceCommentaires; }
            set { treatmentWorkspace.EditSéanceCommentaires = value; }
        }

        public string EditSéancePrix
        {
            get { return treatmentWorkspace.EditSéancePrix; }
            set { treatmentWorkspace.EditSéancePrix = value; }
        }

        public void ExecutCreerNouvelleFichePatientCCommand()
        {
            patientWorkspace.CreateNewPatient();
        }


        private DelegateCommand executeCreerNouveauxTraitementCommand;
        public DelegateCommand ExecuteCreerNouveauxTraitementCommand
        {
            get
            {
                return executeCreerNouveauxTraitementCommand ?? (executeCreerNouveauxTraitementCommand = new DelegateCommand(ExecuteCreerNouveauxTraitement, () => CanSubmit()));
            }
        }

        private void ExecuteCreerNouveauxTraitement()
        {
            EnableApplication = false;
            treatmentWorkspace.CreateNewTraitement();
            EnableApplication = true;
        }
        private bool CanSubmit()
        {
            bool execute = SelectedPatient != null;
            return execute;
        }

        private DelegateCommand filtrerPatients;
        public DelegateCommand FiltrerPatients
        {
            get
            {
                return filtrerPatients ?? (filtrerPatients = new DelegateCommand(RefreshViewPatientsFromDataContext, () => true));
            }
        }

        private void RefreshViewPatientsFromDataContext()
        {
            patientWorkspace.RefreshPatients();
            RaisePropertyChanged("SearchErrorVisibility");
        }

        private DelegateCommand saveInfosClients;
        public DelegateCommand SaveInfosClients
        {
            get
            {
                return saveInfosClients ?? (saveInfosClients = new DelegateCommand(SaveChangesToContext, () => CanSaveInfosClients()));
            }
        }

        private bool CanSaveInfosClients()
        {
            return SelectedPatient != null;
        }


        private DelegateCommand<Traitement> deleteTraitementCommand;
        public DelegateCommand<Traitement> DeleteTraitementCommand
        {
            get
            {
                return deleteTraitementCommand ?? (deleteTraitementCommand = new DelegateCommand<Traitement>(DeleteTraitement, (param) => true));
            }
        }
        private void DeleteTraitement(Traitement traitement)
        {
            treatmentWorkspace.DeleteTraitement(traitement);
        }

        private DelegateCommand<Traitement> effacerChampsTraitementCommand;
        public DelegateCommand<Traitement> EffacerChampsTraitementCommand
        {
            get
            {
                return effacerChampsTraitementCommand ?? (effacerChampsTraitementCommand = new DelegateCommand<Traitement>(EffacerChampsTraitementExecuteCommand, (param) => true));
            }
        }
        private void EffacerChampsTraitementExecuteCommand(Traitement traitement)
        {
            traitement.Commentaires = null;
            traitement.Prix = null;
            traitement.Pulses = null;
            traitement.Fluence = null;
            traitement.ZonesTraitées = null;
            SaveChangesToContext();
        }


        private DelegateCommand<string> selectedPatientCommand;
        public DelegateCommand<string> SelectedPatientCommand
        {
            get
            {
                return selectedPatientCommand ?? (selectedPatientCommand = new DelegateCommand<string>(SelectedPatientExecuteCommand, (name) => CanCopyInfosClients(name)));
            }
        }
        private void SelectedPatientExecuteCommand(string name)
        {
            if (name == "VisualiserFichePatient")
                VisualiserFichePatient();
            else if (name == "OpenOutlook")
                TraiterOutlookCalendar();
            else if (name == "CopyName")
                Clipboard.SetText(SelectedPatient.NomPrenom);
        }
        private bool CanCopyInfosClients(string name)
        {
            return SelectedPatient != null;
        }


        private DelegateCommand<Séance> changeSalleSéanceCommand;
        public DelegateCommand<Séance> ChangerSalleSéanceCommand
        {
            get
            {
                return changeSalleSéanceCommand ?? (changeSalleSéanceCommand = new DelegateCommand<Séance>(ChangeSalleSéanceExecuteCommand, (param) => true));
            }
        }

        private void ChangeSalleSéanceExecuteCommand(Séance séance)
        {
            treatmentWorkspace.ChangeSalleSéance(séance);
        }


        private DelegateCommand<Séance> deleteSéanceCommand;
        public DelegateCommand<Séance> DeleteSéanceCommand
        {
            get
            {
                return deleteSéanceCommand ?? (deleteSéanceCommand = new DelegateCommand<Séance>(DeleteSéance, (param) => true));
            }
        }

        private void DeleteSéance(Séance séance)
        {
            treatmentWorkspace.DeleteSéance(séance);
        }

        private IClinicDataService da = null;
        public ViewModel()
        {
            patientWorkspace = new PatientWorkspace();
            referenceDataWorkspace = new ReferenceDataWorkspace();
            treatmentWorkspace = new TreatmentWorkspace();
            patientWorkspace.PropertyChanged += PatientWorkspace_PropertyChanged;
            referenceDataWorkspace.PropertyChanged += ReferenceDataWorkspace_PropertyChanged;
            treatmentWorkspace.PropertyChanged += TreatmentWorkspace_PropertyChanged;
        }

        internal void LoadDatas(bool oui)
        {
            if (oui)
            {
                try
                {
                    da = new Access();
                    patientWorkspace.AttachDataService(da);
                    treatmentWorkspace.AttachDataService(da);
                    referenceDataWorkspace.LoadLoginChoices(da);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur lors de l'initialisation de la base de donnees : " + ex.Message, "Centre etoile LASER", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    Application.Current.Shutdown();
                }
            }
            else
            {
                MessageBox.Show("Erreur de base de données. L'application va se fermer", "Centre étoile LASER", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Application.Current.Shutdown();
            }
        }

        internal void SaveChangesToContext()
        {
            if (EnableSaveContext)
                da.SaveContext();
        }

        private void TraiterOutlookCalendar()
        {

            EnableApplication = false;
            da.NewRDVOutlook(SelectedPatient);
            EnableApplication = true;
        }

        private void VisualiserFichePatient()
        {
            EnableApplication = false;
            da.VisualiserFichePatient(SelectedPatient);
            EnableApplication = true;
        }

        internal void ImporterPatients()
        {
            da.ImporterPatients();
        }

        private void EffacerChampsTraitement()
        {
            treatmentWorkspace.ResetEditor();
        }

        private void PatientWorkspace_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);

            if (e.PropertyName == nameof(PatientsSelectCollection))
                RaisePropertyChanged(nameof(SearchErrorVisibility));

            if (e.PropertyName == nameof(SelectedPatient))
            {
                treatmentWorkspace.SetSelectedPatient(SelectedPatient);
                RaisePropertyChanged(nameof(AddTraitementPanelEnabled));
                SelectedPatientCommand?.RaiseCanExecuteChanged();
                SaveInfosClients?.RaiseCanExecuteChanged();
                ExecuteCreerNouveauxTraitementCommand?.RaiseCanExecuteChanged();
            }
        }

        private void ReferenceDataWorkspace_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        private void TreatmentWorkspace_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);

            if (e.PropertyName == nameof(EditSéanceZoneTraitée) || e.PropertyName == nameof(EditSéanceMS_Fluence) || e.PropertyName == nameof(SelectedPatient))
                ExecuteCreerNouveauxTraitementCommand?.RaiseCanExecuteChanged();
        }
    }
    public class PatientSelect : BindableBase
    {
        private Patient patient;
        private bool _IsSelected = false;
        public Patient Patient
        {
            get { return patient; }
            set
            {
                SetProperty(ref patient, value);
            }
        }
        public virtual bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                SetProperty(ref _IsSelected, value);
            }
        }
    }
}
