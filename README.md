## 📁 Structure du projet

```
RestaurantApi/
├── RestaurantApi.sln
└── RestaurantApi/
    ├── Program.cs                    ← Démarrage + Swagger + endpoint exemple
    ├── RestaurantApi.csproj
    ├── Models/
    │   ├── Order.cs                  ← Version naïve
    │   └── MenuItem.cs               ← Version naïve
    └── Repositories/
        └── OrderRepository.cs        ← Stockage in-memory (fourni)
```
---

## 🚀 Démarrage
### Prérequis
- .NET 8 SDK installé
- Un IDE C# (Visual Studio, VS Code, ou Rider)

### Lancer l'API
```bash
cd RestaurantApi
dotnet run
```

### Tester
Ouvrez votre navigateur sur l'URL affichée (généralement https://localhost:xxxx/swagger) pour accéder à l'interface Swagger.

Vous pouvez aussi utiliser le guide Postman fourni.

---

## 🎯 Votre travail

À partir de cette base naïve, vous devez :

- **Analyser** les besoins métier du sujet
- **Identifier** les design patterns appropriés pour chaque besoin
- **Restructurer** le code (créez vos propres dossiers et fichiers)
- **Implémenter** les endpoints manquants
- **Documenter** vos choix (UML + README)


⚠️ Le code fourni est volontairement simpliste. Par exemple :

- MenuItem ne distingue pas les catégories par des types dédiés
- Order.Status est une simple chaîne de caractères
- Le calcul du prix n'est pas implémenté
- Il n'y a aucune gestion de transitions d'état

**C'est à vous de transformer cette base en une architecture propre, en appliquant les patterns que vous jugez pertinents.**

## 📦 Livrables

- Code source complet (sans bin/ ni obj/)
- Diagramme UML identifiant les patterns utilisés
- README avec justification de vos choix de patterns