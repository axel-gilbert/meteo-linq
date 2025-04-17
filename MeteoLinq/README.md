# Application Météo LINQ pour Toulouse

Cette application console en C# récupère les prévisions météorologiques pour la ville de Toulouse et permet de les manipuler en utilisant LINQ.

## Fonctionnalités

- **Récupération des données** : Obtention des prévisions météorologiques de Toulouse via l'API Open-Meteo (gratuite et sans clé API)
- **Filtrage des données** : 
  - Par date
  - Par plage de dates
  - Par plage de températures
  - Par condition météorologique (pluie, nuage, soleil, etc.)
- **Tri des données** :
  - Par température (croissant/décroissant)
  - Par date (croissant/décroissant)
- **Regroupement des données** :
  - Par date
  - Par condition météorologique
  - Température moyenne par jour
  - Température maximale par jour
  - Température minimale par jour
- **Affichage des données** : 
  - Visualisation des données avec pagination
  - Affichage automatique après chaque filtrage ou tri
- **Exportation des données** :
  - Format JSON
  - Format CSV
  - Possibilité de sélectionner les champs à exporter
- **Réinitialisation** :
  - Réinitialisation des filtres
  - Réinitialisation de tous les filtres et tris

## Prérequis

- .NET Core SDK 8.0 ou supérieur
- Connexion Internet pour accéder à l'API météo

## Installation et exécution

1. Clonez ce dépôt sur votre machine locale.
2. Accédez au répertoire du projet.
3. Exécutez la commande suivante pour restaurer les packages :
   ```
   dotnet restore
   ```
4. Exécutez l'application avec :
   ```
   dotnet run
   ```

## Utilisation

L'application présente une interface console simple avec des menus pour naviguer entre les différentes fonctionnalités.

1. Au démarrage, l'application charge les données météorologiques pour Toulouse.
2. Utilisez le menu principal pour choisir l'action à effectuer.
3. Suivez les instructions à l'écran pour filtrer, trier, regrouper ou exporter les données.
4. Les résultats s'affichent automatiquement après chaque opération de filtrage ou de tri.
5. Pour réinitialiser tous les filtres et tris, utilisez l'option dédiée dans le menu principal.
6. Pour quitter l'application, sélectionnez l'option "0" dans le menu principal.

## Structure du projet

Le projet est organisé de manière à suivre les principes SOLID et à faciliter la maintenance :

- **Models** : Classes représentant les données météorologiques
- **Services** : Services pour la récupération et l'exportation des données
- **Utils** : Extensions LINQ pour manipuler les données
- **UI** : Interface utilisateur console

## Exemples d'utilisation de LINQ

L'application utilise intensivement LINQ pour manipuler les données météorologiques :

```csharp
// Filtrage par date
var todayData = data.Where(d => d.GetDateTime().Date == DateTime.Today);

// Calcul de la température moyenne
var avgTemp = data.Average(d => d.Main.Temperature);

// Regroupement par condition météo
var weatherGroups = data.GroupBy(d => d.GetWeatherDescription());
```

## Remarques

- L'application utilise l'API gratuite Open-Meteo qui fournit les prévisions sur 7 jours.
- Les données sont actualisées à chaque démarrage de l'application.
- Les exports sont sauvegardés dans le répertoire d'exécution de l'application.
- L'interface affiche automatiquement les résultats après chaque action de filtrage ou de tri. 