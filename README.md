# OutilGestionPatientWPF

Application WPF .NET Framework 4.8 pour la gestion de patients, de seances et de traitements.

## Prerequis

- SDK .NET 10
- Windows avec les composants WPF
- Office installe pour les fonctions Excel/Outlook

## Restaurer les packages

```powershell
dotnet restore OutilGestionPatientWPF.sln
```

## Compiler

```powershell
dotnet build OutilGestionPatientWPF.sln -c Debug
```

## Lancer

Executable genere :

`OutilWPF\bin\Debug\net10.0-windows\outilcle.exe`

Au premier lancement, l'application peut demander le chemin de la base SQLite a utiliser.
