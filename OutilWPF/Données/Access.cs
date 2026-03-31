using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Outlook = Microsoft.Office.Interop.Outlook;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Data.SqlClient;

namespace OutilWPF.Données
{
    public class Access : IDisposable
    {
        private Context dataContext;
        public Access()
        {
            dataContext = new Context();
            dataContext.Database.Migrate();

            if (dataContext.Logins.Count() == 0)
            {
                LoadDatabaseWithFakeDatas();
            }
        }

        private void LoadDatabaseWithFakeDatas()
        {
            dataContext.Lapins.Add(new Lapin() { LapinName = "0 lapin", LapinColor = "T" });
            dataContext.Lapins.Add(new Lapin() { LapinName = "1 lapins", LapinColor = "J" });
            dataContext.Lapins.Add(new Lapin() { LapinName = "2 lapins", LapinColor = "R" });
            dataContext.Lapins.Add(new Lapin() { LapinName = "Ne plus donner de RDV", LapinColor = "VIOLET" });
            dataContext.Lapins.Add(new Lapin() { LapinName = "Contentieux", LapinColor = "BLUE" });
            dataContext.Lapins.Add(new Lapin() { LapinName = "Contentieux", LapinColor = "BLACK" });
            dataContext.SaveChanges();

            dataContext.Logins.Add(new Login() { UserName = "SECRETAIRE", PassWord = "", UserType = "S" });
            dataContext.Logins.Add(new Login() { UserName = "Salle 1", PassWord = "", UserType = "P" });
            dataContext.Logins.Add(new Login() { UserName = "Salle 2", PassWord = "", UserType = "P" });
            dataContext.Logins.Add(new Login() { UserName = "Salle 3", PassWord = "", UserType = "P" });
            dataContext.Logins.Add(new Login() { UserName = "Salle 4", PassWord = "", UserType = "P" });
            dataContext.Logins.Add(new Login() { UserName = "Salle 5", PassWord = "", UserType = "P" });
            dataContext.SaveChanges();

            dataContext.Praticiens.Add(new Praticien() { Nom = "BOULEAU", Prénom = "Marine", UserName = "Salle 1" });
            dataContext.Praticiens.Add(new Praticien() { Nom = "SIMON", Prénom = "Eric", UserName = "Salle 2" });
            dataContext.Praticiens.Add(new Praticien() { Nom = "DANTENY", Prénom = "Gérard", UserName = "Salle 3" });
            dataContext.Praticiens.Add(new Praticien() { Nom = "DANTENY", Prénom = "Gérard", UserName = "Salle 4" });
            dataContext.Praticiens.Add(new Praticien() { Nom = "DANTENY", Prénom = "Gérard", UserName = "Salle 5" });
            dataContext.SaveChanges();

            //dataContext.Patients.Add(new Patient() { Civilité = "M.", Nom = "LUC", Prénom = "Pierre", Adresse1 = "rue des Penthièvre", LapinId = 1 });
            //dataContext.Patients.Add(new Patient() { Civilité = "M.", Nom = "BELLANCE", Prénom = "Vanuel", Adresse1 = "rue des acacias", LapinId = 4 });
            //dataContext.SaveChanges();
        }

        internal IEnumerable<Lapin> GetLapinList()
        {
            var _tmp = new List<Lapin>();
            dataContext.Lapins.ToList().ForEach(l => _tmp.Add(l));

            return _tmp;
        }
        internal IEnumerable<Infosp> GetInfosPList()
        {
            var _tmp = new List<Infosp>();
            dataContext.Infosps.ToList().ForEach(l => _tmp.Add(l));

            return _tmp;
        }

        public ObservableCollection<Patient> GetPatients(string sNom, string sPrénom)
        {
            var _patients = dataContext.Patients.Include(p => p.Lapin).Include(p => p.Séances).OrderBy(p => p.Nom).ThenBy(p => p.Prénom).Where(p => p.Nom.ToLower().StartsWith(sNom.ToLower()) && p.Prénom.ToLower().Contains(sPrénom.ToLower()));
            //_patients = _patients.OrderByDescending(p => p.Séances.Count);
            return new ObservableCollection<Patient>(_patients);
        }

        public List<string> GetLoginList()
        {
            var _tmp = new List<string>();
            //if (dataContext.Logins.Any())
            dataContext.Logins.ToList().ForEach(l => _tmp.Add(l.UserName));

            return _tmp;
        }

        private List<Patient> GetPatientsList()
        {
            var _tmp = new List<Patient>();
            dataContext.Patients.ToList().ForEach(l => _tmp.Add(l));

            return _tmp;
        }

        public List<Praticien> GetPraticiensList()
        {
            var _tmp = new List<Praticien>();
            dataContext.Praticiens.ToList().ForEach(l => _tmp.Add(l));

            return _tmp;
        }

        public List<string> GetPraticienList()
        {
            var _tmp = new List<string>();
            dataContext.Logins.AsQueryable().Where(p => p.UserType.Contains("P")).ToList().ForEach(l => _tmp.Add(l.UserName));
            return _tmp;
        }

        public Login CheckLogin(string login, string password)
        {
            Login success = null;
            try
            {
                success = dataContext.Logins.First(p => p.UserName == login && p.PassWord == password);
            }
            catch
            {

            }
            return success;
        }

        public List<string> GetCivilités()
        {
            return new List<string>() { "M.", "Mme" };
        }

        public Patient CreateNewPatient()
        {
            var np = new Patient
            {
                Lapin = dataContext.Lapins.First(),
                //Infosp = dataContext.Infosps.First()
            };
            dataContext.Patients.Add(np);
            dataContext.SaveChanges();
            return np;
        }

        public Séance CreateNewSéance(int patientId, string userName)
        {
            var login = dataContext.Praticiens.First(p => p.UserName == userName);
            var np = new Séance() { PatientId = patientId, PraticienId = login.PraticienId };
            np.DateSéance = DateTime.Today;
            dataContext.Séances.Add(np);
            dataContext.SaveChanges();
            return np;
        }

        public Traitement CreateNewTraitement(Patient patient, Traitement tr, Praticien salle, DateTime dateSéance)
        {
            //var np = new Traitement() { SéanceId = séanceId };
            if (dataContext.Séances.Any(p => p.DateSéance == dateSéance && p.PatientId == patient.PatientId && p.Praticien.PraticienId == salle.PraticienId))
            {
                var sé = dataContext.Séances.First(p => p.DateSéance == dateSéance && p.PatientId == patient.PatientId && p.Praticien.PraticienId == salle.PraticienId);
                tr.SéanceId = sé.SéanceId;
            }
            else
            {
                //var login = dataContext.Praticiens.First(p => p.UserName == userName);
                var sé = new Séance() { PatientId = patient.PatientId, PraticienId = salle.PraticienId, DateSéance = dateSéance };
                //np.DateSéance = DateTime.Today;
                dataContext.Séances.Add(sé); dataContext.SaveChanges();
                tr.Séance = sé;
            }
            dataContext.Traitements.Add(tr); dataContext.SaveChanges();
            return tr;
        }

        public ObservableCollection<RDVOutlook> GetRDVOulook(Patient patient)
        {
            ObservableCollection<RDVOutlook> rdvsoutlook = null;
            if (patient != null)
            {
                var _rdvsoutlook = dataContext.RDVsOutlook.Include(p => p.Praticien).Where(r => r.PatientId == patient.PatientId);
                rdvsoutlook = new ObservableCollection<RDVOutlook>(_rdvsoutlook);
            }
            return rdvsoutlook;
        }

        public ObservableCollection<Séance> GetSéances(Patient patient)
        {
            ObservableCollection<Séance> temp = new ObservableCollection<Séance>();
            if (patient != null)
            {
                var _temp = dataContext.Séances.AsQueryable().OrderByDescending(p => p.DateSéance).Include(p => p.Praticien).Where(r => r.PatientId == patient.PatientId);
                temp = new ObservableCollection<Séance>(_temp);
            }
            return temp;
        }

        public ObservableCollection<Traitement> GetTraitements(Patient patient)
        {
            ObservableCollection<Traitement> temp = new ObservableCollection<Traitement>();
            if (patient != null && dataContext.Traitements.Any())
            {
                var _temp = dataContext.Traitements.Include(p => p.Séance).OrderByDescending(p => p.Séance.DateSéance).ThenBy(p => p.Fluence).Where(r => r.Séance.PatientId == patient.PatientId);
                temp = new ObservableCollection<Traitement>(_temp);
            }
            return temp;
        }

        public void NewRDVOutlook(Patient patient)
        {
            if (patient == null) return;
            Outlook.Application outlookApp = null;

            //var items = value.Split('|');
            try
            {
                outlookApp = System.Runtime.InteropServices.Marshal.GetActiveObject("Outlook.Application") as Outlook.Application;
            }
            catch
            {
                Process.Start("outlook", "/select outlook:calendar");
                System.Threading.Thread.Sleep(1000);
                try
                {
                    outlookApp = System.Runtime.InteropServices.Marshal.GetActiveObject("Outlook.Application") as Outlook.Application;
                }
                catch
                {
                    outlookApp = new Outlook.Application();
                }
            }

            Outlook.AppointmentItem oAppointment = (Outlook.AppointmentItem)outlookApp.CreateItem(Outlook.OlItemType.olAppointmentItem);
            oAppointment.Subject = patient.NomPrenom;
            oAppointment.Display();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oAppointment);
            oAppointment = null;
            System.Runtime.InteropServices.Marshal.ReleaseComObject(outlookApp);
            outlookApp = null;
        }

        public void UpdateRDVOutlook()
        {
            var OutlookRdvs = new List<RDVOutlook>();
            var listPatients = GetPatientsList();
            var listPraticiens = GetPraticiensList();

            Outlook.Application outlookApp = null;

            //var items = value.Split('|');
            try
            {
                outlookApp = System.Runtime.InteropServices.Marshal.GetActiveObject("Outlook.Application") as Outlook.Application;
                //Console.WriteLine("Active");
            }
            catch
            {
                outlookApp = new Outlook.Application();
                //var folder = outlookApp.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderCalendar);
                //folder.Display();
                //System.Runtime.InteropServices.Marshal.ReleaseComObject(folder);
                //folder = null;
            }
            Outlook.Folder calFolder = outlookApp.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderCalendar) as Outlook.Folder;
            Outlook.Items outlookCalendarItems = calFolder.Items;

            foreach (Outlook.AppointmentItem item in outlookCalendarItems)
            {
                //var items = (item.Value as string).Split('|');

                if (listPatients.Any(p => p.NomPrenom == item.Subject))
                {
                    var _patient = listPatients.First(p => p.NomPrenom == item.Subject);

                    var _categories = item.Categories.Split(';');
                    Praticien _praticien = null;

                    foreach (var p in listPraticiens)
                    {
                        foreach (var c in _categories)
                        {
                            if (p.UserName == c)
                            {
                                _praticien = p;
                                break;
                            }
                        }
                        if (_praticien != null) break;
                    }
                    if (_praticien != null)
                        OutlookRdvs.Add(new RDVOutlook() { PatientId = _patient.PatientId, PraticienId = _praticien.PraticienId, DateRDV = item.Start, Commentaires = item.Body });
                }
            }

            System.Runtime.InteropServices.Marshal.ReleaseComObject(outlookCalendarItems);
            outlookCalendarItems = null;
            System.Runtime.InteropServices.Marshal.ReleaseComObject(calFolder);
            calFolder = null;
            outlookApp.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(outlookApp);
            outlookApp = null;


            dataContext.RDVsOutlook.ToList().ForEach(r => dataContext.RDVsOutlook.Remove(r));
            dataContext.SaveChanges();
            dataContext.RDVsOutlook.AddRange(OutlookRdvs);
            dataContext.SaveChanges();
        }

        internal void VisualiserFichePatient(Patient patient)
        {
            if (patient != null)
            {
                var _patient = dataContext.Patients.Include(p => p.Séances).ThenInclude(p => p.Traitements).First(p => p.PatientId == patient.PatientId);
                var templatePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), @"Assets\FichePatient.xltx");
                //var savePathPDF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Fiches à imprimer");
                //Directory.CreateDirectory(savePathPDF);
                //var savePath = Path.Combine(savePathPDF, string.Format("Fiche de {0} {1:MMddyyyHmmss}.xlsx", patient.NomPrenom, DateTime.Now));
                //savePathPDF = Path.Combine(savePathPDF, string.Format("Fiche de {0} {1:MMddyyyHmmss}.pdf", patient.NomPrenom, DateTime.Now));

                var eap = new Excel.Application
                {
                    Visible = true
                };
                var wb = eap.Workbooks.Add(templatePath);
                Excel.Worksheet ws = wb.Worksheets[1];
                ws.Range["B2"].Value = string.Format("{0}", patient.NomPrenom);
                ws.Range["B3"].Value = string.Format("{0}", patient.Adresse1);
                ws.Range["B4"].Value = string.Format("{0} {1}", patient.CodePostal, patient.Ville);
                ws.Range["B5"].Value = string.Format("{0}", patient.TelephonePortable);

                var nr = 8;
                foreach (var seance in _patient.Séances.OrderBy(p => p.DateSéance))
                    foreach (var traitement in seance.Traitements.OrderBy(p => p.Fluence))
                    {
                        ws.Cells[nr, 1].Value = seance.DateSéance;
                        Excel.Range zt = ws.Cells[nr, 2];
                        zt.Value = string.Format("{0}", traitement.ZonesTraitées); zt.WrapText = true;
                        if (traitement.ZonesTraitées?.Contains("lapin") ?? false)
                        {
                            zt.Font.Color = Excel.XlRgbColor.rgbRed;
                        }
                        ws.Cells[nr, 3].Value = string.Format("{0}", traitement.Fluence); ws.Cells[nr, 3].Font.Bold = true; ; ws.Cells[nr, 3].WrapText = true;
                        ws.Cells[nr, 4].Value = string.Format("{0}", traitement.Pulses); ws.Cells[nr, 4].WrapText = true;
                        ws.Cells[nr, 5].Value = string.Format("{0}", traitement.Prix); ws.Cells[nr, 5].WrapText = true;
                        ws.Cells[nr, 6].Value = string.Format("{0}", traitement.Commentaires); ws.Cells[nr, 6].WrapText = true;
                        nr++;
                    }
                var rang = ws.Range[string.Format("A7:F{0}", nr - 1)];
                rang.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                rang.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                rang.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                rang.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                rang.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                rang.Borders[Excel.XlBordersIndex.xlInsideHorizontal].LineStyle = Excel.XlLineStyle.xlContinuous;
                ws.PageSetup.Zoom = false;
                ws.PageSetup.FitToPagesTall = 1;
                ws.PageSetup.FitToPagesWide = 1;
                //ws.PrintPreview();
                ws.PrintOutEx();
                ws = null;
                //wb.SaveAs(savePath);



                //wb.ExportAsFixedFormat(Excel.XlFixedFormatType.xlTypePDF, savePathPDF, OpenAfterPublish: true);
                wb.Close(false);
                wb = null;
                eap.Quit();
                eap = null;
            }
        }

        internal void RemoveSéance(Séance item)
        {
            dataContext.Séances.Remove(item);
            dataContext.SaveChanges();
        }

        internal void RemoveTraitement(Traitement item)
        {
            dataContext.Traitements.Remove(item);
            dataContext.SaveChanges();
        }
        private class PatientExcel
        {
            public string nomPrenom;
            public string couleur = "T";
            public string wbName;
            public string wsName;
        }
        internal void ImporterPatients()
        {

            //string nomPrenom = "IMAMBAKSH ALL Reshma-Marie";
            var pathFichestoImport = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "FICHES PATIENTS");
            if (Directory.Exists(pathFichestoImport))
            {
                string[] filePaths = Directory.GetFiles(pathFichestoImport, "*.xls", SearchOption.TopDirectoryOnly);

                Parallel.ForEach(filePaths, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, (fiche) =>
                //foreach (var fiche in filePaths)
                {
                    var eap = new Excel.Application
                    {
                        Visible = true
                    };

                    var wb = eap.Workbooks.Open(fiche, ReadOnly: true);
                    foreach (Excel.Worksheet ws in wb.Worksheets)
                    {
                        //PatientExcel fp = null;
                        //if (patientNames.Any(p => p.wsName.ToLower().Contains(ws.Name.ToLower())))
                        //{
                        //    fp = patientNames.First(p => p.wsName.ToLower().Contains(ws.Name.ToLower()));
                        //}
                        int nd = 0;
                        if (Convert.ToString(ws.Cells[10, 1].Value) == "Date")
                            nd = 5;
                        else if (Convert.ToString(ws.Cells[11, 1].Value) == "Date")
                            nd = 6;
                        if (nd != 0)
                        {
                            using (var ctx = new Context())
                            {
                                string nom_Prenom = Convert.ToString(ws.Cells[nd, 2].Value);
                                string adresse = Convert.ToString(ws.Cells[nd + 1, 2].Value);
                                string cp_ville = Convert.ToString(ws.Cells[nd + 2, 2].Value);
                                string numPortable = ws.Cells[nd + 3, 2].Text;
                                var nomPrenom = NomPrénom(nom_Prenom);
                                var cpVille = CodePostalVille(cp_ville);

                                var patient = new Patient
                                {
                                    Nom = nomPrenom.Item1,
                                    Prénom = nomPrenom.Item2,
                                    Adresse1 = adresse,
                                    TelephonePortable = numPortable,
                                    CodePostal = cpVille.Item1,
                                    Ville = cpVille.Item2,
                                    AncienneFiche = ws.Name,
                                    LapinId = 1
                                };

                                //if (fp != null)
                                //{
                                //    patient.TarifRéduit = fp.nomPrenom.Contains("/PR");
                                //    patient.LapinId = ctx.Lapins.First(l => l.LapinColor == fp.couleur).LapinId;
                                //}
                                ctx.Patients.Add(patient); ctx.SaveChanges();

                                var nrows = Math.Max(ws.Range["A" + ws.Rows.Count].End[Excel.XlDirection.xlUp].Row, ws.Range["B" + ws.Rows.Count].End[Excel.XlDirection.xlUp].Row);
                                nrows = Math.Max(nrows, ws.Range["C" + ws.Rows.Count].End[Excel.XlDirection.xlUp].Row);
                                nrows = Math.Max(nrows, ws.Range["D" + ws.Rows.Count].End[Excel.XlDirection.xlUp].Row);
                                nrows = Math.Max(nrows, ws.Range["E" + ws.Rows.Count].End[Excel.XlDirection.xlUp].Row);

                                DateTime? DatePrecedente = null;
                                int SéanceId = 0;
                                if (nrows >= nd + 6)
                                    for (var nr = nd + 6; nr <= nrows; nr++)
                                    {
                                        string date = Convert.ToString(ws.Cells[nr, 1].Value);
                                        string ZonesTraitées = Convert.ToString(ws.Cells[nr, 2].Value);
                                        string Fluence = Convert.ToString(ws.Cells[nr, 3].Value);
                                        string pulse = Convert.ToString(ws.Cells[nr, 4].Value);
                                        if (date == null && ZonesTraitées == null && Fluence == null && pulse == null)
                                            DatePrecedente = null;
                                        var npulse = Convert.ToString(ws.Cells[nr, 4].Value);
                                        var isPulse = !string.IsNullOrEmpty(npulse);
                                        string prix = Convert.ToString(ws.Cells[nr, 5].Value);

                                        //Séance
                                        if (DateTime.TryParse(date, out DateTime dateRDV))
                                        {
                                            if (dateRDV != DatePrecedente)
                                            {
                                                var séance = new Séance
                                                {
                                                    DateSéance = dateRDV,
                                                    PatientId = patient.PatientId,
                                                    PraticienId = ctx.Praticiens.ToList().First().PraticienId
                                                };
                                                ctx.Séances.Add(séance); ctx.SaveChanges();
                                                SéanceId = séance.SéanceId;

                                                DatePrecedente = dateRDV;
                                            }
                                        }

                                        //Traitements
                                        if (SéanceId != 0)
                                            if (isPulse)
                                            {
                                                var traitement = new Traitement
                                                {
                                                    SéanceId = SéanceId,
                                                    ZonesTraitées = ZonesTraitées,
                                                    Fluence = Fluence,
                                                    Pulses = pulse,
                                                    Prix = prix
                                                };
                                                ctx.Traitements.Add(traitement); ctx.SaveChanges();
                                            }
                                            else
                                            {
                                                if (ZonesTraitées != null && !ZonesTraitées.ToLower().Contains("lapin"))
                                                {
                                                    var traitement = new Traitement
                                                    {
                                                        SéanceId = SéanceId,
                                                        ZonesTraitées = ZonesTraitées,
                                                        Fluence = Fluence,
                                                        Commentaires = npulse,
                                                        Prix = prix
                                                    };
                                                    ctx.Traitements.Add(traitement); ctx.SaveChanges();
                                                }
                                                else
                                                {
                                                    var traitement = new Traitement
                                                    {
                                                        SéanceId = SéanceId,
                                                        Commentaires = ZonesTraitées,
                                                    };
                                                    ctx.Traitements.Add(traitement); ctx.SaveChanges();
                                                }

                                            }
                                    }
                            }
                        }
                    }
                    wb.Close(false);
                    eap.Quit();
                }
                );

                //Couleurs
                var patientNames = new ConcurrentBag<PatientExcel>();
                var lcol = new List<int> { 1, 3, 5, 7 };
                var ea = new Excel.Application
                {
                    Visible = true
                };
                var wba = ea.Workbooks.Open(Path.Combine(pathFichestoImport, "ALPHA", "Liste alphabétique.xls"), ReadOnly: true);
                foreach (Excel.Worksheet ws in wba.Worksheets)
                    foreach (var l in lcol)
                    {
                        var nrows = ws.Cells[ws.Rows.Count, l].End[Excel.XlDirection.xlUp].Row;
                        for (var nr = 2; nr <= nrows; nr++)
                        {
                            var pe = new PatientExcel();
                            Excel.Range ce = ws.Cells[nr, l];

                            pe.nomPrenom = ce.Value;
                            if (ce.Hyperlinks.Count > 0)
                            {
                                Excel.Hyperlink Hyper = ce.Hyperlinks.Item[1];
                                pe.wbName = Hyper.Address;
                                pe.wsName = Hyper.SubAddress;
                            }
                            var colo = ce.Interior.Color;
                            if (colo == 49407)
                                pe.couleur = "J";
                            else if (colo == 255)
                                pe.couleur = "R";
                            else if (colo == 10498160)
                                pe.couleur = "VIOLET";
                            else if (colo == 6299648)
                                pe.couleur = "BLUE";
                            else if (colo == 0)
                                pe.couleur = "BLACK";
                            if (pe.wsName != null)
                                patientNames.Add(pe);
                        }
                    }

                wba.Close(); ; wba = null;
                ea.Quit();

                var ctxLogins = dataContext.Patients.ToList();
                foreach (Patient p in ctxLogins)
                {
                    PatientExcel fp = null;
                    if (patientNames.Any(n => n.wsName.ToLower().Contains(p.AncienneFiche.ToLower())))
                    {
                        fp = patientNames.First(n => n.wsName.ToLower().Contains(p.AncienneFiche.ToLower()));

                        p.TarifRéduit = fp.nomPrenom.Contains("/PR");
                        p.LapinId = dataContext.Lapins.First(l => l.LapinColor == fp.couleur).LapinId;
                    }
                }
                dataContext.SaveChanges();
            }
        }
        private Tuple<string, string> NomPrénom(string s)
        {
            string nom = s, prenom = null;
            try
            {
                string nomPrenom = s.Trim();
                var _nomPrenom = nomPrenom.Select((c, i) => new { Char = c, Index = i });
                var firstLowIndex = _nomPrenom.ToArray().First(o => Char.IsLower(o.Char)).Index;
                nom = nomPrenom.Substring(0, firstLowIndex - 1);
                nom = nom.Trim();
                prenom = nomPrenom.Substring(firstLowIndex - 1);
                prenom = prenom.Trim();
            }
            catch
            {

            }
            return new Tuple<string, string>(nom, prenom);
        }

        private Tuple<string, string> CodePostalVille(string s)
        {
            string cp = null, ville = s;

            try
            {
                string cpVille = s.Trim();
                var _nomPrenom = cpVille.Select((c, i) => new { Char = c, Index = i });
                var firstLowIndex = _nomPrenom.ToArray().First(o => Char.IsLetter(o.Char)).Index;
                cp = cpVille.Substring(0, firstLowIndex - 1);
                cp = cp.Trim();
                ville = cpVille.Substring(firstLowIndex - 1);
                ville = ville.Trim();
            }
            catch
            {

            }
            return new Tuple<string, string>(cp, ville);
        }

        internal void MigrateDatas()
        {
            //Ajout infos supplémentaires
            //var sql = "CREATE TABLE `Infosps` ( `InfospId` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, `InfosName` TEXT )";
            //var result = dataContext.Database.ExecuteSqlCommand(sql);
            //sql = "CREATE INDEX `IX_Infosps_InfospId` ON `Infosps` ( `InfospId` )";
            //result = dataContext.Database.ExecuteSqlCommand(sql);

            //var ip = new Infosp() { InfosName = null };
            //dataContext.Infosps.Add(ip);
            //dataContext.Infosps.Add(new Infosp() { InfosName = "Annulé le jour même" });
            //dataContext.SaveChanges();

            //sql = "ALTER TABLE `Patients` ADD COLUMN 	`InfospID`	INTEGER  REFERENCES `Infosps`(`InfospID`)";
            //var result2 = dataContext.Database.ExecuteSqlCommand(sql);
            var sql = @" UPDATE [Patients] SET InfospID = @infospID";
            var result = dataContext.Database.ExecuteSqlCommand(sql, new Microsoft.Data.Sqlite.SqliteParameter("@infospID", 1));

            /*
            foreach (var p in dataContext.Patients)
            {
                if (p.Nom != null) p.Nom = p.Nom.ToUpper().Trim(); if (p.Nom == "") p.Nom = null;
            }

            foreach (var t in dataContext.Traitements)
            {
                if (t.Commentaires != null && t.Commentaires.Contains("lapin"))
                {
                    t.ZonesTraitées = "lapin"; if (t.Commentaires == "lapin") t.Commentaires = null;
                }
                if (t.Pulses == "0")
                {
                    t.Pulses = null;
                }
                if (t.ZonesTraitées == null && t.Fluence == null && t.Pulses == null && t.Prix == null && t.Commentaires == null)
                    dataContext.Traitements.Remove(t);
            }
            dataContext.SaveChanges();*/
        }
        internal async void SaveContext()
        {
            //dataContext.SaveChanges();
            await dataContext.SaveChangesAsync();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    dataContext.Dispose();
                    //externalContext.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Access() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

