using OutilWPF.Données;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OutilWPF
{
    public class ReferenceDataWorkspace : BindableBase
    {
        private List<Tuple<string, string>> lesLogins = new List<Tuple<string, string>>();
        private List<Praticien> lesPraticiens = new List<Praticien>();
        private List<Lapin> listLapins = new List<Lapin>();
        private List<Infosp> listInfosp = new List<Infosp>();

        public List<Tuple<string, string>> LesLogins
        {
            get { return lesLogins; }
            set { SetProperty(ref lesLogins, value); }
        }

        public List<Praticien> LesPraticiens
        {
            get { return lesPraticiens; }
            set { SetProperty(ref lesPraticiens, value); }
        }

        public List<Lapin> ListLapins
        {
            get { return listLapins; }
            set { SetProperty(ref listLapins, value); }
        }

        public List<Infosp> ListInfosp
        {
            get { return listInfosp; }
            set { SetProperty(ref listInfosp, value); }
        }

        public void LoadLoginChoices(IClinicDataService dataService)
        {
            LesLogins = dataService.GetLoginList()
                .Select(lo => new Tuple<string, string>(lo, lo))
                .ToList();
        }

        public void LoadAuthenticatedReferenceData(IClinicDataService dataService)
        {
            ListLapins = dataService.GetLapinList().ToList();
            ListInfosp = dataService.GetInfosPList().ToList();
            LesPraticiens = dataService.GetPraticiensList();
        }
    }
}
