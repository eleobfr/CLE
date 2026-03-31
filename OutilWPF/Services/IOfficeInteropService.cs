using OutilWPF.Données;

namespace OutilWPF.Services
{
    public interface IOfficeInteropService
    {
        void NewRDVOutlook(Patient patient);
        void UpdateRDVOutlook();
        void VisualiserFichePatient(Patient patient);
        void ImporterPatients();
    }
}
