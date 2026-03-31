using System.Collections.Generic;
using System.Linq;

namespace OutilWPF.Données
{
    public class ReferenceDataService : IReferenceDataService
    {
        private readonly Context dataContext;

        public ReferenceDataService(Context dataContext)
        {
            this.dataContext = dataContext;
        }

        public Login CheckLogin(string login, string password)
        {
            return dataContext.Logins.FirstOrDefault(p => p.UserName == login && p.PassWord == password);
        }

        public List<string> GetLoginList()
        {
            return dataContext.Logins.ToList().Select(l => l.UserName).ToList();
        }

        public List<Praticien> GetPraticiensList()
        {
            return dataContext.Praticiens.ToList();
        }

        public IEnumerable<Lapin> GetLapinList()
        {
            return dataContext.Lapins.ToList();
        }

        public IEnumerable<Infosp> GetInfosPList()
        {
            return dataContext.Infosps.ToList();
        }
    }
}
