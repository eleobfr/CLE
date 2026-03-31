using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;

namespace OutilWPF.Données
{
    public class Login
    {
        [Key, Required]
        public string UserName { get; set; }
        public string PassWord { get; set; }
        [Required]
        public string UserType { get; set; }
    }

    public class Patient : BindableBase
    {
        public int PatientId { get; set; }

        private string nom;
        private string prénom;
        private bool tarifRéduit;

        public string Nom
        {
            get { return nom; }
            set { SetProperty(ref nom, value); }
        }

        public string Prénom
        {
            get { return prénom; }
            set { SetProperty(ref prénom, value); }
        }

        public string Civilité { get; set; }
        public string Adresse1 { get; set; }
        public string Adresse2 { get; set; }
        public string CodePostal { get; set; }
        public string Ville { get; set; }

        [Phone]
        [Display(Name = "Téléphone portable")]
        public string TelephonePortable { get; set; }

        public string Commentaires { get; set; }
        public string AncienneFiche { get; set; }

        public bool TarifRéduit
        {
            get { return tarifRéduit; }
            set { SetProperty(ref tarifRéduit, value); }
        }

        public Brush CouleurLapin
        {
            get
            {
                var color = Lapin?.LapinColor;
                Color co = Colors.Transparent;

                if (color == "J")
                    co = Colors.Orange;
                else if (color == "R")
                    co = Colors.Red;
                else if (color == "VIOLET")
                    co = Colors.DarkViolet;
                else if (color == "BLUE")
                    co = Colors.DarkBlue;
                else if (color == "BLACK")
                    co = Colors.Black;

                return new SolidColorBrush(co);
            }
        }

        private int lapinID;
        public int LapinId
        {
            get { return lapinID; }
            set
            {
                SetProperty(ref lapinID, value);
                RaisePropertyChanged("CouleurLapin");
            }
        }

        public virtual Lapin Lapin { get; set; }
        public virtual ICollection<Séance> Séances { get; set; }
        public virtual ICollection<RDVOutlook> RDVsOutlook { get; set; }

        public string NomPrenom
        {
            get { return string.Format("{0} {1}", Nom, Prénom); }
        }
    }

    public class Séance : BindableBase
    {
        [Key]
        public int SéanceId { get; set; }

        private DateTime dateSéance;
        public DateTime DateSéance
        {
            get { return dateSéance; }
            set { SetProperty(ref dateSéance, value); }
        }

        public int PatientId { get; set; }
        public virtual Patient Patient { get; set; }
        public int PraticienId { get; set; }

        private Praticien praticien;
        public virtual Praticien Praticien
        {
            get { return praticien; }
            set { SetProperty(ref praticien, value); }
        }

        public virtual ICollection<Traitement> Traitements { get; set; }
    }

    public class RDVOutlook : BindableBase
    {
        [Key]
        public int RDVOutlookId { get; set; }
        public DateTime DateRDV { get; set; }
        public string Commentaires { get; set; }

        public int PatientId { get; set; }
        public virtual Patient Patient { get; set; }

        public int PraticienId { get; set; }
        public virtual Praticien Praticien { get; set; }
    }

    public class Praticien : BindableBase
    {
        [Key]
        public int PraticienId { get; set; }
        public string Nom { get; set; }
        public string Prénom { get; set; }

        [Required]
        public string UserName { get; set; }

        public virtual ICollection<Séance> Séances { get; set; }
        public virtual ICollection<RDVOutlook> RDVOutlooks { get; set; }
    }

    public class Traitement : BindableBase
    {
        [Key]
        public int TraitementId { get; set; }

        private string zonestraitées;
        private string fluence;
        private string pulses;
        private string commentaires;
        private string prix;

        public string ZonesTraitées
        {
            get { return zonestraitées; }
            set { SetProperty(ref zonestraitées, value); }
        }

        public string Fluence
        {
            get { return fluence; }
            set { SetProperty(ref fluence, value); }
        }

        public string Pulses
        {
            get { return pulses; }
            set { SetProperty(ref pulses, value); }
        }

        public string Commentaires
        {
            get { return commentaires; }
            set { SetProperty(ref commentaires, value); }
        }

        public string Prix
        {
            get { return prix; }
            set { SetProperty(ref prix, value); }
        }

        public int SéanceId { get; set; }
        public virtual Séance Séance { get; set; }
    }

    public class Lapin
    {
        public int LapinId { get; set; }
        public string LapinName { get; set; }
        public string LapinColor { get; set; }
        public virtual ICollection<Patient> Patients { get; set; }
    }

    public class Infosp
    {
        public int InfospId { get; set; }
        public string InfosName { get; set; }
        public virtual ICollection<Patient> Patients { get; set; }
    }
}
