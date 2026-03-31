using FirstFloor.ModernUI.Windows.Controls;
using Microsoft.Win32;
using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace OutilWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            DataContext = new ViewModel();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var canOperate = false;
            var externalDBsqlite = Properties.Settings.Default.InternalDatabasePath;
            if (!File.Exists(externalDBsqlite)) externalDBsqlite = null;
            if (externalDBsqlite == null)
            {
                var picker = new OpenFileDialog
                {
                    Filter = "Base commune (*db)|*.db"
                };
                picker.ShowDialog();
                externalDBsqlite = picker.FileName;

                if (string.IsNullOrEmpty(externalDBsqlite))
                {
                    var fpicker = new System.Windows.Forms.FolderBrowserDialog();
                    var folder = fpicker.ShowDialog();

                    if (folder == System.Windows.Forms.DialogResult.OK)
                    {
                        externalDBsqlite = Path.Combine(fpicker.SelectedPath, "OutilGestionPatientDB.db");
                    }
                }
            }

            if (!string.IsNullOrEmpty(externalDBsqlite))
                canOperate = true;

            Properties.Settings.Default.InternalDatabasePath = externalDBsqlite;
            Properties.Settings.Default.Save();
            (DataContext as ViewModel).LoadDatas(canOperate);
        }

        private void Valider_Click(object sender, RoutedEventArgs e)
        {
            if (cbLogin.SelectedItem != null)
            {
                var val = (cbLogin.SelectedItem as Tuple<string, string>).Item2;
                (DataContext as ViewModel).CheckLogin(val, password.Password);
            }
        }

        private void CréerNewFichePatient_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as ViewModel).ExecutCreerNouvelleFichePatientCCommand();
        }

        //private void ButtonCommandOutlook(object sender, RoutedEventArgs e)
        //{
        //    (DataContext as ViewModel).TraiterOutlookCalendar((sender as Button).Name);
        //}

        //private void VisualiserFichePatient_Click(object sender, RoutedEventArgs e)
        //{
        //    (DataContext as ViewModel).VisualiserFichePatient();
        //}

        private void Importer_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as ViewModel).ImporterPatients();
        }

        private void ViewListeTraitement_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            (DataContext as ViewModel).SaveChangesToContext();
        }

        private void EditSéanceSalle_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as ComboBox).SelectedIndex = 1;
        }

        private void ViewSéances_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            (DataContext as ViewModel).SaveChangesToContext();
        }

        private void EditPatientPrixRéduit_Checked(object sender, RoutedEventArgs e)
        {
            (DataContext as ViewModel).SaveChangesToContext();
        }

        private void EditPatientLapin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (DataContext as ViewModel).SaveChangesToContext();
        }

        private void TextBox_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            var textBox = ((TextBox)sender);
            textBox.Dispatcher.BeginInvoke(new Action(() =>
            {
                textBox.SelectAll();
            }));
        }

        private void Migrate_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as ViewModel).MigrerDonnées();
        }
    }
}
