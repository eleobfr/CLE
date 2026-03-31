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
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var canOperate = databaseSelectionService.TryEnsureDatabasePath();
            viewModel.LoadDatas(canOperate);
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
    }
}
