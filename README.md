# TourGuide – Performance Optimization & CI Pipeline (.NET)

Refactoring et optimisation d’une API .NET dans un contexte réel proche de la production : correction de bugs, amélioration des performances à grande échelle et mise en place d’une intégration continue.

---

## Objectifs

- Stabiliser une application existante
- Corriger des bugs liés à la concurrence
- Optimiser les performances pour 100 000 utilisateurs
- Mettre en place un pipeline CI automatisé

---

## Résultats

| Feature | Objectif | Résultat |
|--------|--------|---------|
| TrackLocation (100k users) | < 15 min |  ~2 min |
| GetRewards (100k users) | < 20 min |  ~7 min |

 Gain de performance significatif grâce à la concurrence  
 Application stable et tests 100% validés  

---

## Stack technique

- .NET 7 / ASP.NET Core
- xUnit (tests unitaires & performance)
- GitHub Actions (CI)
- Programmation asynchrone (async/await, Task, SemaphoreSlim)

---

## Améliorations clés

### Correction de bugs
- Résolution d’une InvalidOperationException liée à un accès concurrent
- Gestion sécurisée des utilisateurs

### Optimisation performance
- Mise en place de traitements parallèles sur de gros volumes
- Limitation du nombre de threads avec SemaphoreSlim
- Passage à une architecture asynchrone

---

## CI Pipeline

Pipeline GitHub Actions avec :

- Build
- Tests (hors performance)
- Publication
- Génération d’un artefact ZIP téléchargeable
