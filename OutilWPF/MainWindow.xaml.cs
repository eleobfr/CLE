using Microsoft.Extensions.Options;
using OutilWPF.Configuration;
using OutilWPF.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace OutilWPF
{
    public partial class MainWindow : Window
    {
        private readonly ViewModel viewModel;
        private readonly IDatabaseSelectionService databaseSelectionService;

        public MainWindow(
            ViewModel viewModel,
            IDatabaseSelectionService databaseSelectionService,
            IOptions<ApplicationOptions> options)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            this.databaseSelectionService = databaseSelectionService;
            Title = options.Value.ApplicationTitle;
            Loaded += MainWindow_Loaded;
            DataContext = viewModel;
            SizeChanged += MainWindow_SizeChanged;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var canOperate = databaseSelectionService.TryEnsureDatabasePath();
            viewModel.LoadDatas(canOperate);
            ApplyResponsiveLayout();
        }

        private void Valider_Click(object sender, RoutedEventArgs e)
        {
            if (cbLogin.SelectedItem != null)
            {
                var val = (cbLogin.SelectedItem as Tuple<string, string>).Item2;
                viewModel.CheckLogin(val, password.Password);
            }
        }

        private void CréerNewFichePatient_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ExecutCreerNouvelleFichePatientCCommand();
        }

        private void Importer_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ImporterPatients();
        }

        private void ViewListeTraitement_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            viewModel.SaveChangesToContext();
        }

        private void EditSéanceSalle_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as System.Windows.Controls.ComboBox).SelectedIndex = 1;
        }

        private void ViewSéances_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            viewModel.SaveChangesToContext();
        }

        private void EditPatientPrixRéduit_Checked(object sender, RoutedEventArgs e)
        {
            viewModel.SaveChangesToContext();
        }

        private void EditPatientLapin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModel.SaveChangesToContext();
        }

        private void TextBox_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            var textBox = (System.Windows.Controls.TextBox)sender;
            textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
        }

        private void Migrate_Click(object sender, RoutedEventArgs e)
        {
            viewModel.MigrerDonnées();
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ApplyResponsiveLayout();
        }

        private void ApplyResponsiveLayout()
        {
            if (!IsLoaded)
                return;

            var stackMainPanels = ActualWidth < 1450;
            WorkColumnLeft.Width = stackMainPanels ? new GridLength(1, GridUnitType.Star) : new GridLength(330);
            WorkColumnRight.Width = stackMainPanels ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
            WorkRowBottom.Height = stackMainPanels ? GridLength.Auto : new GridLength(0);
            Grid.SetColumn(PatientsCard, 0);
            Grid.SetRow(PatientsCard, 0);
            Grid.SetColumn(DetailsCard, stackMainPanels ? 0 : 2);
            Grid.SetRow(DetailsCard, stackMainPanels ? 1 : 0);
            DetailsCard.Margin = stackMainPanels ? new Thickness(0, 18, 0, 0) : new Thickness(0);

            var stackClinicalPanels = ActualWidth < 1680;
            ClinicalColumnLeft.Width = stackClinicalPanels ? new GridLength(1, GridUnitType.Star) : new GridLength(320);
            ClinicalColumnRight.Width = stackClinicalPanels ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
            ClinicalRowBottom.Height = stackClinicalPanels ? GridLength.Auto : new GridLength(0);
            Grid.SetColumn(SessionsPanel, 0);
            Grid.SetRow(SessionsPanel, 0);
            Grid.SetColumn(TreatmentsPanel, stackClinicalPanels ? 0 : 2);
            Grid.SetRow(TreatmentsPanel, stackClinicalPanels ? 1 : 0);
            TreatmentsPanel.Margin = stackClinicalPanels ? new Thickness(0, 18, 0, 0) : new Thickness(0);

            PatientSectionsGrid.Columns = ActualWidth < 1560 ? 1 : 2;
        }
    }
}
