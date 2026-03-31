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
        private Login UserConnected = null;
        public List<Tuple<string, string>> LesLogins { get; } = new List<Tuple<string, string>>();
        public List<Praticien> LesPraticiens { get; } = new List<Praticien>();
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
                foreach (Lapin la in da.GetLapinList())
                    ListLapins.Add(la);
                foreach (Infosp ip in da.GetInfosPList())
                    ListInfosp.Add(ip);
                foreach (var lo in da.GetPraticiensList())
                    LesPraticiens.Add(lo);

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
            get
            {
                var _addTraitementPanelEnabled = false;
                if (SelectedPatient != null)
                    _addTraitementPanelEnabled = true;
                return _addTraitementPanelEnabled;
            }
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
            get { return patientWorkspace.Séances; }
            set { patientWorkspace.Séances = value; }
        }

        public ObservableCollection<Traitement> Traitements
        {
            get { return patientWorkspace.Traitements; }
            set { patientWorkspace.Traitements = value; }
        }

        private List<Lapin> listLapins = new List<Lapin>();
        public List<Lapin> ListLapins
        {
            get { return listLapins; }
            set
            {
                SetProperty(ref listLapins, value);
            }
        }

        private List<Infosp> listInfosp = new List<Infosp>();
        public List<Infosp> ListInfosp
        {
            get { return listInfosp; }
            set
            {
                SetProperty(ref listInfosp, value);
            }
        }


        private DateTime editSéanceDate = DateTime.Today;
        public DateTime EditSéanceDate
        {
            get { return editSéanceDate; }
            set
            {
                SetProperty(ref editSéanceDate, value);
            }
        }

        private Infosp editInfosp = null;
        public Infosp EditInfosp
        {
            get { return editInfosp; }
            set
            {
                SetProperty(ref editInfosp, value);
                if (value != null) EditSéanceZoneTraitée = value.InfosName;
            }
        }

        private Praticien editSéanceSalle;
        public Praticien EditSéanceSalle
        {
            get { return editSéanceSalle; }
            set
            {
                SetProperty(ref editSéanceSalle, value);
            }
        }

        private string editSéanceZoneTraitée = null;
        public string EditSéanceZoneTraitée
        {
            get { return editSéanceZoneTraitée; }
            set
            {
                SetProperty(ref editSéanceZoneTraitée, value);
                ExecuteCreerNouveauxTraitementCommand.RaiseCanExecuteChanged();
            }
        }

        private string editSéanceMS_Fluence = null;
        public string EditSéanceMS_Fluence
        {
            get { return editSéanceMS_Fluence; }
            set
            {
                SetProperty(ref editSéanceMS_Fluence, value);
                ExecuteCreerNouveauxTraitementCommand.RaiseCanExecuteChanged();
            }
        }

        private string editSéanceNb_Pulses = null;
        public string EditSéanceNb_Pulses
        {
            get { return editSéanceNb_Pulses; }
            set
            {
                SetProperty(ref editSéanceNb_Pulses, value);
            }
        }

        private string editSéanceCommentaires = null;
        public string EditSéanceCommentaires
        {
            get { return editSéanceCommentaires; }
            set
            {
                SetProperty(ref editSéanceCommentaires, value);
            }
        }

        private string editSéancePrix = null;
        public string EditSéancePrix
        {
            get { return editSéancePrix; }
            set
            {
                SetProperty(ref editSéancePrix, value);
            }
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
            //var = zone EditSéanceZoneTraitée
            Traitement tr = new Traitement() { ZonesTraitées = EditSéanceZoneTraitée, Fluence = EditSéanceMS_Fluence, Pulses = EditSéanceNb_Pulses, Commentaires = EditSéanceCommentaires, Prix = editSéancePrix };

            da.CreateNewTraitement(SelectedPatient, tr, EditSéanceSalle, EditSéanceDate);
            Traitements.Add(tr);
            Traitements = new ObservableCollection<Traitement>(Traitements.OrderByDescending(p => p.Séance.DateSéance).ThenBy(p => p.Fluence));
            if (!Séances.Select(s => s.SéanceId).Contains(tr.Séance.SéanceId)) Séances.Add(tr.Séance);
            EffacerChampsTraitement();
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
            Traitements.Remove(traitement);
            da.RemoveTraitement(traitement);
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
            séance.Praticien = EditSéanceSalle;
            //RaisePropertyChanged("Séances");
            SaveChangesToContext();
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
            if (séance.Traitements != null)
                foreach (var t in séance.Traitements)
                    Traitements.Remove(t);
            Séances.Remove(séance);
            da.RemoveSéance(séance);//s'occupe d'enlever les traitements de la bd
        }

        private IClinicDataService da = null;
        public ViewModel()
        {
            patientWorkspace = new PatientWorkspace();
            patientWorkspace.PropertyChanged += PatientWorkspace_PropertyChanged;
        }

        internal void LoadDatas(bool oui)
        {
            if (oui)
            {
                try
                {
                    da = new Access();
                    patientWorkspace.AttachDataService(da);
                    foreach (var lo in da.GetLoginList())
                        LesLogins.Add(new Tuple<string, string>(lo, lo));
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
            EditSéanceDate = DateTime.Today;
            EditSéanceZoneTraitée = null;
            EditSéanceMS_Fluence = null;
            EditSéanceNb_Pulses = null;
            EditSéanceCommentaires = null;
            EditSéancePrix = null;
            EditInfosp = null;
        }

        private void PatientWorkspace_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);

            if (e.PropertyName == nameof(PatientsSelectCollection))
                RaisePropertyChanged(nameof(SearchErrorVisibility));

            if (e.PropertyName == nameof(SelectedPatient))
            {
                EffacerChampsTraitement();
                RaisePropertyChanged(nameof(AddTraitementPanelEnabled));
                SelectedPatientCommand?.RaiseCanExecuteChanged();
                SaveInfosClients?.RaiseCanExecuteChanged();
                ExecuteCreerNouveauxTraitementCommand?.RaiseCanExecuteChanged();
            }
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
