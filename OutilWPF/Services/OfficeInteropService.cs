using Microsoft.EntityFrameworkCore;
using OutilWPF.Données;
using Outlook = Microsoft.Office.Interop.Outlook;
using Excel = Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace OutilWPF.Services
{
    [SupportedOSPlatform("windows")]
    public class OfficeInteropService : IOfficeInteropService
    {
        private readonly IDatabasePathStore databasePathStore;

        public OfficeInteropService(IDatabasePathStore databasePathStore)
        {
            this.databasePathStore = databasePathStore;
        }

        public void NewRDVOutlook(Patient patient)
        {
            if (patient == null)
                return;

            var outlookApp = new Outlook.Application();
            Outlook.AppointmentItem appointment = (Outlook.AppointmentItem)outlookApp.CreateItem(Outlook.OlItemType.olAppointmentItem);
            appointment.Subject = patient.NomPrenom;
            appointment.Display();
            Marshal.ReleaseComObject(appointment);
            Marshal.ReleaseComObject(outlookApp);
        }

        public void UpdateRDVOutlook()
        {
            using var dataContext = CreateContext();

            var outlookRdvs = new List<RDVOutlook>();
            var listPatients = dataContext.Patients.ToList();
            var listPraticiens = dataContext.Praticiens.ToList();

            var outlookApp = new Outlook.Application();
            Outlook.Folder calendarFolder = outlookApp.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderCalendar) as Outlook.Folder;
            Outlook.Items outlookCalendarItems = calendarFolder.Items;

            foreach (Outlook.AppointmentItem item in outlookCalendarItems)
            {
                if (!listPatients.Any(p => p.NomPrenom == item.Subject))
                    continue;

                var patient = listPatients.First(p => p.NomPrenom == item.Subject);
                var categories = (item.Categories ?? string.Empty).Split(';');
                Praticien praticien = null;

                foreach (var candidate in listPraticiens)
                {
                    foreach (var category in categories)
                    {
                        if (candidate.UserName == category)
                        {
                            praticien = candidate;
                            break;
                        }
                    }

                    if (praticien != null)
                        break;
                }

                if (praticien != null)
                {
                    outlookRdvs.Add(new RDVOutlook
                    {
                        PatientId = patient.PatientId,
                        PraticienId = praticien.PraticienId,
                        DateRDV = item.Start,
                        Commentaires = item.Body
                    });
                }
            }

            Marshal.ReleaseComObject(outlookCalendarItems);
            Marshal.ReleaseComObject(calendarFolder);
            outlookApp.Quit();
            Marshal.ReleaseComObject(outlookApp);

            dataContext.RDVsOutlook.ToList().ForEach(r => dataContext.RDVsOutlook.Remove(r));
            dataContext.SaveChanges();
            dataContext.RDVsOutlook.AddRange(outlookRdvs);
            dataContext.SaveChanges();
        }

        public void VisualiserFichePatient(Patient patient)
        {
            if (patient == null)
                return;

            using var dataContext = CreateContext();
            var loadedPatient = dataContext.Patients.Include(p => p.Séances).ThenInclude(p => p.Traitements).First(p => p.PatientId == patient.PatientId);
            var templatePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), @"Assets\FichePatient.xltx");

            var excelApp = new Excel.Application { Visible = true };
            var workbook = excelApp.Workbooks.Add(templatePath);
            dynamic worksheet = workbook.Worksheets[1];
            worksheet.Range["B2"].Value = loadedPatient.NomPrenom;
            worksheet.Range["B3"].Value = loadedPatient.Adresse1;
            worksheet.Range["B4"].Value = string.Format("{0} {1}", loadedPatient.CodePostal, loadedPatient.Ville);
            worksheet.Range["B5"].Value = loadedPatient.TelephonePortable;

            var rowNumber = 8;
            foreach (var seance in loadedPatient.Séances.OrderBy(p => p.DateSéance))
            {
                foreach (var traitement in seance.Traitements.OrderBy(p => p.Fluence))
                {
                    worksheet.Cells[rowNumber, 1].Value = seance.DateSéance;
                    dynamic zoneCell = worksheet.Cells[rowNumber, 2];
                    zoneCell.Value = traitement.ZonesTraitées;
                    zoneCell.WrapText = true;
                    if (traitement.ZonesTraitées?.Contains("lapin") ?? false)
                        zoneCell.Font.Color = Excel.XlRgbColor.rgbRed;

                    worksheet.Cells[rowNumber, 3].Value = traitement.Fluence;
                    worksheet.Cells[rowNumber, 3].Font.Bold = true;
                    worksheet.Cells[rowNumber, 3].WrapText = true;
                    worksheet.Cells[rowNumber, 4].Value = traitement.Pulses;
                    worksheet.Cells[rowNumber, 4].WrapText = true;
                    worksheet.Cells[rowNumber, 5].Value = traitement.Prix;
                    worksheet.Cells[rowNumber, 5].WrapText = true;
                    worksheet.Cells[rowNumber, 6].Value = traitement.Commentaires;
                    worksheet.Cells[rowNumber, 6].WrapText = true;
                    rowNumber++;
                }
            }

            dynamic range = worksheet.Range[string.Format("A7:F{0}", rowNumber - 1)];
            range.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            range.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            range.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            range.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            range.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
            range.Borders[Excel.XlBordersIndex.xlInsideHorizontal].LineStyle = Excel.XlLineStyle.xlContinuous;
            worksheet.PageSetup.Zoom = false;
            worksheet.PageSetup.FitToPagesTall = 1;
            worksheet.PageSetup.FitToPagesWide = 1;
            worksheet.PrintOutEx();

            workbook.Close(false);
            excelApp.Quit();
        }

        public void ImporterPatients()
        {
            var databasePath = RequireDatabasePath();
            var pathFichestoImport = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "FICHES PATIENTS");
            if (!Directory.Exists(pathFichestoImport))
                return;

            string[] filePaths = Directory.GetFiles(pathFichestoImport, "*.xls", SearchOption.TopDirectoryOnly);

            Parallel.ForEach(filePaths, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, fiche =>
            {
                var excelApp = new Excel.Application { Visible = true };
                var workbook = excelApp.Workbooks.Open(fiche, ReadOnly: true);

                foreach (var worksheetValue in workbook.Worksheets)
                {
                    dynamic worksheet = worksheetValue;
                    int startRow = 0;
                    if (Convert.ToString(worksheet.Cells[10, 1].Value) == "Date")
                        startRow = 5;
                    else if (Convert.ToString(worksheet.Cells[11, 1].Value) == "Date")
                        startRow = 6;

                    if (startRow == 0)
                        continue;

                    using var context = new Context(databasePath);
                    string nomPrenomRaw = Convert.ToString(worksheet.Cells[startRow, 2].Value);
                    string adresse = Convert.ToString(worksheet.Cells[startRow + 1, 2].Value);
                    string cpVilleRaw = Convert.ToString(worksheet.Cells[startRow + 2, 2].Value);
                    string numPortable = Convert.ToString(worksheet.Cells[startRow + 3, 2].Text);
                    var nomPrenom = NomPrénom(nomPrenomRaw);
                    var cpVille = CodePostalVille(cpVilleRaw);

                    var patient = new Patient
                    {
                        Nom = nomPrenom.Item1,
                        Prénom = nomPrenom.Item2,
                        Adresse1 = adresse,
                        TelephonePortable = numPortable,
                        CodePostal = cpVille.Item1,
                        Ville = cpVille.Item2,
                        AncienneFiche = worksheet.Name,
                        LapinId = 1
                    };

                    context.Patients.Add(patient);
                    context.SaveChanges();

                    var nrows = Math.Max((int)worksheet.Range["A" + worksheet.Rows.Count].End[Excel.XlDirection.xlUp].Row, (int)worksheet.Range["B" + worksheet.Rows.Count].End[Excel.XlDirection.xlUp].Row);
                    nrows = Math.Max(nrows, (int)worksheet.Range["C" + worksheet.Rows.Count].End[Excel.XlDirection.xlUp].Row);
                    nrows = Math.Max(nrows, (int)worksheet.Range["D" + worksheet.Rows.Count].End[Excel.XlDirection.xlUp].Row);
                    nrows = Math.Max(nrows, (int)worksheet.Range["E" + worksheet.Rows.Count].End[Excel.XlDirection.xlUp].Row);

                    DateTime? previousDate = null;
                    int seanceId = 0;
                    if (nrows >= startRow + 6)
                    {
                        for (var nr = startRow + 6; nr <= nrows; nr++)
                        {
                            string date = Convert.ToString(worksheet.Cells[nr, 1].Value);
                            string zonesTraitees = Convert.ToString(worksheet.Cells[nr, 2].Value);
                            string fluence = Convert.ToString(worksheet.Cells[nr, 3].Value);
                            string pulse = Convert.ToString(worksheet.Cells[nr, 4].Value);
                            if (date == null && zonesTraitees == null && fluence == null && pulse == null)
                                previousDate = null;

                            string notesOrPulse = Convert.ToString(worksheet.Cells[nr, 4].Value);
                            bool isPulse = !string.IsNullOrEmpty(notesOrPulse);
                            string prix = Convert.ToString(worksheet.Cells[nr, 5].Value);

                            if (DateTime.TryParse(date, out DateTime dateRDV) && dateRDV != previousDate)
                            {
                                var seance = new Séance
                                {
                                    DateSéance = dateRDV,
                                    PatientId = patient.PatientId,
                                    PraticienId = context.Praticiens.ToList().First().PraticienId
                                };
                                context.Séances.Add(seance);
                                context.SaveChanges();
                                seanceId = seance.SéanceId;
                                previousDate = dateRDV;
                            }

                            if (seanceId == 0)
                                continue;

                            if (isPulse)
                            {
                                context.Traitements.Add(new Traitement
                                {
                                    SéanceId = seanceId,
                                    ZonesTraitées = zonesTraitees,
                                    Fluence = fluence,
                                    Pulses = pulse,
                                    Prix = prix
                                });
                            }
                            else if (zonesTraitees != null && !zonesTraitees.ToLower().Contains("lapin"))
                            {
                                context.Traitements.Add(new Traitement
                                {
                                    SéanceId = seanceId,
                                    ZonesTraitées = zonesTraitees,
                                    Fluence = fluence,
                                    Commentaires = notesOrPulse,
                                    Prix = prix
                                });
                            }
                            else
                            {
                                context.Traitements.Add(new Traitement
                                {
                                    SéanceId = seanceId,
                                    Commentaires = zonesTraitees,
                                });
                            }

                            context.SaveChanges();
                        }
                    }
                }

                workbook.Close(false);
                excelApp.Quit();
            });

            var patientNames = new ConcurrentBag<PatientExcel>();
            var columns = new List<int> { 1, 3, 5, 7 };
            var listExcel = new Excel.Application { Visible = true };
            var listWorkbook = listExcel.Workbooks.Open(Path.Combine(pathFichestoImport, "ALPHA", "Liste alphabétique.xls"), ReadOnly: true);

            foreach (var worksheetValue in listWorkbook.Worksheets)
            {
                dynamic worksheet = worksheetValue;
                foreach (var column in columns)
                {
                    var nrows = (int)worksheet.Cells[worksheet.Rows.Count, column].End[Excel.XlDirection.xlUp].Row;
                    for (var nr = 2; nr <= nrows; nr++)
                    {
                        var patientExcel = new PatientExcel();
                        dynamic cell = worksheet.Cells[nr, column];

                        patientExcel.nomPrenom = Convert.ToString(cell.Value);
                        if (cell.Hyperlinks.Count > 0)
                        {
                            dynamic hyperlink = cell.Hyperlinks.Item[1];
                            patientExcel.wbName = hyperlink.Address;
                            patientExcel.wsName = hyperlink.SubAddress;
                        }

                        var color = Convert.ToInt32(cell.Interior.Color);
                        if (color == 49407)
                            patientExcel.couleur = "J";
                        else if (color == 255)
                            patientExcel.couleur = "R";
                        else if (color == 10498160)
                            patientExcel.couleur = "VIOLET";
                        else if (color == 6299648)
                            patientExcel.couleur = "BLUE";
                        else if (color == 0)
                            patientExcel.couleur = "BLACK";

                        if (patientExcel.wsName != null)
                            patientNames.Add(patientExcel);
                    }
                }
            }

            listWorkbook.Close();
            listExcel.Quit();

            using var finalContext = new Context(databasePath);
            foreach (Patient patient in finalContext.Patients.ToList())
            {
                if (!patientNames.Any(n => n.wsName.ToLower().Contains(patient.AncienneFiche.ToLower())))
                    continue;

                var matchingPatient = patientNames.First(n => n.wsName.ToLower().Contains(patient.AncienneFiche.ToLower()));
                patient.TarifRéduit = matchingPatient.nomPrenom.Contains("/PR");
                patient.LapinId = finalContext.Lapins.First(l => l.LapinColor == matchingPatient.couleur).LapinId;
            }

            finalContext.SaveChanges();
        }

        private Context CreateContext()
        {
            return new Context(RequireDatabasePath());
        }

        private string RequireDatabasePath()
        {
            if (string.IsNullOrWhiteSpace(databasePathStore.CurrentPath))
                throw new InvalidOperationException("Le chemin de la base de donnees n'est pas configure.");

            return databasePathStore.CurrentPath;
        }

        private class PatientExcel
        {
            public string nomPrenom;
            public string couleur = "T";
            public string wbName;
            public string wsName;
        }

        private Tuple<string, string> NomPrénom(string s)
        {
            string nom = s, prenom = null;
            try
            {
                string nomPrenom = s.Trim();
                var source = nomPrenom.Select((c, i) => new { Char = c, Index = i });
                var firstLowIndex = source.ToArray().First(o => Char.IsLower(o.Char)).Index;
                nom = nomPrenom.Substring(0, firstLowIndex - 1).Trim();
                prenom = nomPrenom.Substring(firstLowIndex - 1).Trim();
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
                var source = cpVille.Select((c, i) => new { Char = c, Index = i });
                var firstLetterIndex = source.ToArray().First(o => Char.IsLetter(o.Char)).Index;
                cp = cpVille.Substring(0, firstLetterIndex - 1).Trim();
                ville = cpVille.Substring(firstLetterIndex - 1).Trim();
            }
            catch
            {
            }

            return new Tuple<string, string>(cp, ville);
        }
    }
}
