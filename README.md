# Infinite Dungeon

Un donjon infini en console, écrit en C# (.NET 10). Descendez le plus profond possible : à chaque salle, choisissez une des 3 portes, combattez, commercez, et survivez au boss qui vous attend toutes les 10 salles.

## Lancer le jeu

```bash
dotnet run --project InfiniteDungeon
```

Une seed peut être donnée en argument pour la première partie : `dotnet run --project InfiniteDungeon -- 1234`.

## Seed

Chaque partie a une seed (un nombre, ou un texte transformé en nombre). La même seed et les mêmes choix donnent exactement le même donjon : mêmes salles, mêmes ennemis, mêmes marchands. La seed est affichée au début et à la fin de la partie pour pouvoir la rejouer ou la partager. Laisser le champ vide en choisit une au hasard.

## Une partie

```
#####1####2####3#####
#...................#
#...................#
#.........M.........#
#...................#
#.........P.........#
#####################
```

- **Portes 1, 2, 3** : chaque porte donne un indice sur la salle suivante (bruits de combat, lueur dorée, voix qui marchande...).
- **E** combat, **B** boss (toutes les 10 salles), **M** marchand, **T** trésor, **F** feu de camp (soin), **^** piège.
- **Combat** au tour par tour : attaquer (coups critiques possibles), boire une potion, ou fuir (sauf contre un boss).
- **Progression** : or, expérience, niveaux, armes, armures et potions de rareté variable (Commun, Rare, Épique, Légendaire).
- **Marchand** : acheter au prix affiché, revendre à moitié prix.
- **Menu entre deux salles** : `i` inventaire (équiper, boire), `s` statistiques, `q` quitter.

La difficulté monte de plus en plus vite avec la profondeur.

## Structure

| Dossier | Contenu |
| --- | --- |
| `Core/` | `Game` (boucle de jeu, combat, boutique), `Input` (saisie console) |
| `Entity/` | `Character` (abstraite), `Player`, `Enemy`, `Merchant` |
| `Items/` | `Item` (abstraite), `Weapon`, `Armor`, `HealPotion`, `Rarity` |
| `Rooms/` | `Room`, `RoomType` |
| `Factories/` | `EnemyFactory`, `ItemFactory` (pattern Factory) |
