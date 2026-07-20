# RestaurantApi

API REST de gestion des commandes d'un restaurant, construite autour de 5 design patterns, un par
besoin métier de l'énoncé.

Le diagramme de classes complet et la légende des patterns se trouvent dans [UML.md](UML.md).

---

## Démarrage

### Prérequis

- .NET 8 SDK
- Un IDE C# (Visual Studio, VS Code ou Rider), facultatif

### Lancer l'API

```bash
cd RestaurantApi
dotnet run
```

L'API démarre sur `http://localhost:5205` (port défini dans `Properties/launchSettings.json`).

### Tester

Swagger est disponible sur `http://localhost:5205/swagger`.

Les notifications inter-services (besoin 4) s'affichent dans la console qui exécute `dotnet run`,
pas dans la réponse HTTP. Garde cette fenêtre visible pendant les tests.

---

## Exemples de requêtes

Toutes les énumérations (`category`, `policy`) s'écrivent en toutes lettres, en entrée comme en
sortie. Les réponses ci-dessous sont celles réellement renvoyées par l'API.

### Créer un plat

```bash
curl -X POST http://localhost:5205/api/dishes \
  -H "Content-Type: application/json" \
  -d '{"name":"Tartare de saumon","price":12.5,"category":"Starter"}'
```

```json
{
  "name": "Tartare de saumon",
  "price": 12.5,
  "category": "Starter",
  "description": "Starter: Tartare de saumon, served in 10 min.",
  "preparationTimeMinutes": 10
}
```

Le temps de préparation est optionnel : chaque fabrique applique le défaut de sa catégorie
(entrée 10 min, plat 25 min, dessert 8 min, boisson 2 min). `isAlcoholic` n'est lu que pour les
boissons :

```bash
curl -X POST http://localhost:5205/api/dishes \
  -H "Content-Type: application/json" \
  -d '{"name":"Pinot noir","price":6.0,"category":"Beverage","isAlcoholic":true}'
```

```json
{
  "name": "Pinot noir",
  "price": 6,
  "category": "Beverage",
  "description": "Beverage (alcoholic): Pinot noir.",
  "preparationTimeMinutes": 2
}
```

### Créer une commande

```bash
curl -X POST http://localhost:5205/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "tableNumber": 12,
    "policy": "Menu",
    "items": [
      {"name":"Salade cesar","price":8.5,"category":"Starter"},
      {"name":"Entrecote grillee","price":22.0,"category":"MainCourse"},
      {"name":"Tarte tatin","price":6.5,"category":"Dessert"},
      {"name":"Eau petillante","price":3.0,"category":"Beverage"}
    ]
  }'
```

```json
{
  "id": "7c05aac9-4b0a-407b-8331-c53924d98a42",
  "tableNumber": 12,
  "totalPrice": 28,
  "status": "ReceivedState",
  "policy": "Menu",
  "pricingDescription": "Menu pricing strategy: Fixed price of 25 for a complete menu (starter, main course, dessert)"
}
```

L'entrée, le plat et le dessert sont forfaitisés à 25, la boisson reste à son prix, d'où 28.

### Changer la politique tarifaire d'une commande existante

```bash
curl -X PATCH "http://localhost:5205/api/orders/{id}/policy?policy=HappyHour"
```

```json
{ "totalPrice": 32, "description": "Happy Hour pricing strategy: 20% off all dishes" }
```

Politiques disponibles : `Standard`, `HappyHour`, `Group`, `Menu`. Sur la commande ci-dessus, dont
les plats totalisent 40 au prix de base, les quatre donnent des résultats différents :

| Politique | Total | Explication |
|---|---|---|
| `Standard` | 40 | somme des prix de base |
| `HappyHour` | 32 | 20 % de remise |
| `Group` | 40 | inchangé, le seuil de 50 n'est pas atteint |
| `Menu` | 28 | entrée, plat et dessert forfaitisés à 25, plus la boisson à 3 |

### Faire avancer la commande dans son workflow

```bash
curl -X PUT http://localhost:5205/api/orders/{id}/state
```

Chaque appel avance d'une étape : `ReceivedState`, `PreparationState`, `ReadyState`, `ServedState`,
`PaidState`. Un appel de plus sur une commande payée renvoie `409 Conflict` :

```json
{ "error": "La commande est deja payee." }
```

### Gérer les abonnements aux notifications

```bash
curl http://localhost:5205/api/orders/{id}/observers
# ["Kitchen","Dining","Billing"]

curl -X DELETE http://localhost:5205/api/orders/{id}/observers/Dining
# ["Kitchen","Billing"]
```

Trace console correspondante, avant puis après le désabonnement de `Dining` :

```
[Order 421440e8] Status = ReceivedState -> Send to 3 service(s)
  [Kitchen] New order for table 7 : 4 dish(es) to prepare.
  [Kitchen]   - Entrecote grillee (25 min)
  [Billing] Opened bill for table 7 : 50,00 EUR (Group).
[Order 421440e8] Service 'Dining' unsubscribed.
[Order 421440e8] Status = PreparationState -> Send to 2 service(s)
  [Kitchen] Start preparation for table 7 (estimated time: 60 min).
[Order 421440e8] Status = ReadyState -> Send to 2 service(s)
[Order 421440e8] Status = PaidState -> Send to 2 service(s)
  [Billing] Payment received: 40,00 EUR for table 7. Bill closed.
```

La ligne `ReadyState` ne produit plus aucune sortie : c'était `DiningService` qui réagissait à cet
état, il n'est plus abonné.

### Consulter le menu et la configuration

```bash
curl http://localhost:5205/api/menu        # carte groupée par catégorie
curl http://localhost:5205/api/restaurant  # horaires et paramètres globaux
```

### Autres endpoints

| Méthode | Route | Rôle |
|---|---|---|
| `GET` | `/api/orders` | liste toutes les commandes |
| `GET` | `/api/orders/{id}` | détail d'une commande |

---

## Architecture

### Vue d'ensemble

| Besoin | Pattern | Classes principales |
|---|---|---|
| 1. Types de plats | **Factory Method** | `DishFactory`, `StarterFactory`, `MainCourseFactory`, `DessertFactory`, `BeverageFactory`, `DishFactoryProvider` |
| 2. Calcul du prix | **Strategy** | `IPriceOrder`, `StandardStrategy`, `HappyHourStrategy`, `GroupStrategy`, `MenuStrategy`, `PriceCalculator` |
| 3. Workflow | **State** | `OrderState`, `ReceivedState`, `PreparationState`, `ReadyState`, `ServedState`, `PaidState` |
| 4. Notifications | **Observer** | `INotificationSubject`, `INotificationObserver`, `KitchenService`, `DiningService`, `BillingService` |
| 5. Configuration | **Singleton** | `RestaurantConfiguration` |

### Besoin 1 : gestion de différents types de plats

**Pattern retenu : Factory Method.**

Les 4 catégories partagent le contrat `IDish` mais diffèrent sur deux points : leur temps de
préparation par défaut et leurs données propres (`Beverage` porte un `IsAlcoholic` que les autres
n'ont pas). C'est exactement le cas d'usage du Factory Method : une classe abstraite `DishFactory`
tient l'algorithme commun, et chaque sous-classe décide quel produit concret instancier.

Un Abstract Factory aurait été surdimensionné, il n'y a pas de familles de produits à créer
ensemble. Une simple factory statique avec un `switch` aurait marché, mais elle aurait violé la
contrainte « facile d'ajouter de nouvelles catégories » : il aurait fallu rouvrir le `switch` à
chaque ajout.

**Ce que ça résout :** la création est centralisée dans `Factories/`, et le code appelant ne connaît
que `IDish`. `DishFactoryProvider` sert de registre catégorie vers fabrique, donc ajouter une
catégorie revient à écrire une classe produit, une classe fabrique et une entrée de dictionnaire,
sans toucher aux endpoints.

### Besoin 2 : calcul flexible du prix total

**Pattern retenu : Strategy.**

Les 4 politiques sont 4 algorithmes interchangeables qui répondent à la même question, calculer un
total à partir d'une commande. Elles n'ont ni état persistant ni transitions entre elles, seulement
un calcul, ce qui exclut le State et désigne le Strategy.

`PriceCalculator` joue le rôle de contexte et résout la politique via un dictionnaire, ce qui
supprime les `if/else` de sélection. `Order` ne contient aucun algorithme de prix, elle porte
seulement la politique choisie.

**Ce que ça résout :** la politique se choisit à la création (`policy` dans le corps de la requête)
ou se change ensuite via `PATCH /api/orders/{id}/policy`, avec recalcul immédiat. Ajouter une
politique revient à implémenter `IPriceOrder` et à l'enregistrer dans le dictionnaire.

À noter : `MenuStrategy` retire les plats forfaitisés du panier avant de facturer le reste au prix
unitaire, et prend les plats les plus chers en premier pour que le menu soit toujours à l'avantage
du client.

### Besoin 3 : workflow de traitement des commandes

**Pattern retenu : State.**

Le statut n'est plus une chaîne mais un objet `OrderState`. Chaque état concret implémente
`BuildNextState()` et sait uniquement quel est son successeur, ce qui rend la chaîne de transitions
explicite et impossible à court-circuiter. `PaidState` est terminal : il lève une
`InvalidOperationException`, traduite en `409 Conflict` par l'endpoint.

**Ce que ça résout :** il n'y a aucun `switch` sur le statut dans le code de transition, la logique
d'enchaînement vit dans les états eux-mêmes. Ajouter une étape revient à insérer une classe et à
changer le `BuildNextState()` de son prédécesseur.

La transition et la notification sont centralisées dans la méthode `NextState()` de la classe
abstraite, seul endroit où State et Observer se rencontrent.

### Besoin 4 : notifications inter-services

**Pattern retenu : Observer.**

`Order` implémente `INotificationSubject` et ne connaît que l'interface `INotificationObserver`,
jamais `KitchenService`, `DiningService` ni `BillingService`. Les trois services sont enregistrés
dans le conteneur d'injection de dépendances et abonnés à la commande à sa création.

**Ce que ça résout :** le découplage est total dans le sens commande vers services. Chaque
observateur filtre lui-même sur le type d'état auquel il veut réagir, donc un même événement produit
trois comportements différents. L'abonnement est dynamique, `DELETE /api/orders/{id}/observers/{name}`
retire un service en cours de route, et ajouter un service n'implique aucune modification de `Order`.

| État atteint | `KitchenService` | `DiningService` | `BillingService` |
|---|:---:|:---:|:---:|
| `ReceivedState` | liste les plats à préparer | · | ouvre l'addition |
| `PreparationState` | annonce le temps estimé | · | · |
| `ReadyState` | · | fait servir la table | · |
| `ServedState` | · | · | · |
| `PaidState` | · | · | encaisse et clôture |

### Besoin 5 : configuration globale du restaurant

**Pattern retenu : Singleton.**

`RestaurantConfiguration` est `sealed`, son constructeur est privé et l'instance est exposée via un
`Lazy<RestaurantConfiguration>` statique. `Lazy<T>` est thread-safe par défaut, ce qui couvre la
contrainte d'accès concurrent depuis plusieurs requêtes sans verrou écrit à la main, et garantit une
initialisation unique au premier accès.

**Ce que ça résout :** un point d'accès unique, `RestaurantConfiguration.Instance`, pour le menu, les
horaires et les paramètres globaux. Le message `[Singleton] RestaurantConfiguration initialized`
n'apparaît qu'une fois dans la console, quel que soit le nombre d'appels à `/api/menu`.

À noter : la carte est construite par les fabriques du besoin 1, il n'y a aucun `new Starter(...)`
dans le Singleton.

### Le cas particulier de `Order`

`Order` porte deux rôles à la fois : contexte du State (elle détient son `Status`) et sujet concret
de l'Observer (elle détient la liste des abonnés). C'est volontaire, c'est l'objet dont le cycle de
vie est observé, et les deux patterns se rejoignent en un seul point, l'appel à `Notify()` dans
`NextState()`.

### Structure du projet

```
RestaurantApi/
├── RestaurantApi.sln
└── RestaurantApi/
    ├── Program.cs            Composition root, injection de dépendances et endpoints
    ├── Categories/           Produits concrets : Starter, MainCourse, Dessert, Beverage
    ├── Dtos/                 Contrats d'entrée et de sortie de l'API
    ├── Enums/                DishCategory, PricingPolicy
    ├── Factories/            Besoin 1, Factory Method
    ├── Interfaces/           IDish, IPriceOrder, INotificationSubject, INotificationObserver
    ├── Models/               Order, OpeningHours
    ├── Repositories/         Stockage in-memory des commandes
    ├── Services/             Besoin 4, observateurs concrets
    ├── Singletons/           Besoin 5, RestaurantConfiguration
    ├── States/               Besoin 3, State
    └── Strategies/           Besoin 2, Strategy
```
