# OutilGestionPatientWPF

Application WPF .NET Framework 4.8 pour la gestion de patients, de seances et de traitements.

## Prerequis

- Visual Studio avec les outils WPF/.NET Framework
- MSBuild Visual Studio
- NuGet CLI ou la restauration NuGet de Visual Studio

## Restaurer les packages

```powershell
nuget restore OutilGestionPatientWPF.sln
```

## Compiler

```powershell
& "C:\Program Files\Microsoft Visual Studio\18\Professional\MSBuild\Current\Bin\MSBuild.exe" OutilGestionPatientWPF.sln /t:Build /p:Configuration=Debug /nologo
```

## Lancer

Executable genere :

`OutilWPF\bin\Debug\outilcle.exe`

Au premier lancement, l'application peut demander le chemin de la base SQLite a utiliser.
