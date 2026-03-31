using System.Collections.Generic;

namespace OutilWPF.Données
{
    public interface IReferenceDataService
    {
        Login CheckLogin(string login, string password);
        List<string> GetLoginList();
        List<Praticien> GetPraticiensList();
        IEnumerable<Lapin> GetLapinList();
        IEnumerable<Infosp> GetInfosPList();
    }
}
