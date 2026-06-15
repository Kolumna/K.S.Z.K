# Algorytm przydziału krasnoludków do kopalni (Edmonds–Karp)

## Cel algorytmu

Celem algorytmu jest **optymalny przydział krasnoludków do kopalni** w taki sposób, aby uwzględnić:

- przepustowości (ile krasnoludków może pracować w danej kopalni),
- preferencje i „koszty” przydziału,
- ograniczenia struktury sieci (źródło, kopalnie, ujście).

W warstwie algorytmicznej jest to klasyczny problem **maksymalnego przepływu** w sieci (algorytm **Edmondsa–Karpa** – BFS na ścieżkach powiększających przepływ), rozszerzony o specyfikę danych domenowych.

## Lokalizacja w projekcie

- Przestrzeń nazw: `Krasnoludki.Core`
- Plik źródłowy: `src/Krasnoludki.Core/Algorithms/DwarfAssigning.cs`
- Główna klasa: `DwarfAssigning`
- Istotne metody:
  - `static bool BFS(Source source, Sink sink, List<EdgeFlow> edges, ref EdgeFlow[] parent)` – wyszukiwanie ścieżki powiększającej przepływ,
  - `static int EdmondsKarp(Source source, Sink sink, List<EdgeFlow> edges)` – implementacja algorytmu maksymalnego przepływu,
  - `static List<int[]> Assign(List<Dwarf> dwarves, List<Mine> mines)` – metoda wysokopoziomowa zwracająca przypisania krasnoludek–kopalnia.

## Model danych

- `Dwarf` – reprezentuje krasnoludka (m.in. unikalne ID, cechy/parametry).
- `Mine` – reprezentuje kopalnię (m.in. pojemność, ID).
- `EdgeFlow` – krawędź w sieci przepływu z:
  - `From`, `To` – ID węzłów,
  - `Capacity` – maksymalna przepustowość,
  - `CurrFlow` – aktualny przepływ,
  - `Cost` – koszt przypisania (np. odległość, brak preferencji).
- `Source`, `Sink` – specjalne węzły źródła i ujścia dla sieci.

Krawędzie są generowane przez pomocniczą klasę `EdgeGen.GenerateEdges(dwarves, mines, source, sink)`.

## Idea działania (wysoki poziom)

1. **Budowa sieci przepływu**
   - Na podstawie list krasnoludków i kopalni generowane są krawędzie:
     - `source -> krasnoludki`,
     - `krasnoludki -> kopalnie`,
     - `kopalnie -> sink`.
   - Krawędzie reprezentują możliwe przypisania i ograniczenia pojemności.

2. **Wyszukiwanie ścieżki BFS (`BFS`)**
   - W każdej iteracji wyszukiwany jest **ciąg krawędzi** od źródła do ujścia, po których można jeszcze „przepchnąć” dodatkowy przepływ:
     - brane są pod uwagę tylko krawędzie z dodatnią **przepustowością resztową** (`CurrFlow < Capacity`),
     - ignorowane są krawędzie o bardzo dużym koszcie (`Cost >= 1_000_000` – brak preferencji),
     - aktualizowana jest tablica `parent`, pozwalająca odtworzyć ścieżkę.

3. **Algorytm Edmondsa–Karpa (`EdmondsKarp`)**
   - Dopóki `BFS` znajduje ścieżkę od źródła do ujścia:
     - wyznaczana jest minimalna przepustowość rezydualna na tej ścieżce (`newFlow`),
     - przepływ na krawędziach ścieżki jest zwiększany o `newFlow` (`AddFlow`),
     - zwiększana jest zmienna `MaxFlow`.

4. **Ekstrakcja wyniku (`Assign`)**
   - Po zakończeniu algorytmu krawędzie z dodatnim przepływem, łączące krasnoludków z kopalniami, są przeglądane i na ich podstawie budowana jest lista wynikowa:
     - każda pozycja to `int[] { dwarfNodeId, mineNodeId }`.

## Złożoność obliczeniowa

Niech:
- `V` – liczba węzłów w sieci,
- `E` – liczba krawędzi,
- `F` – maksymalny przepływ (w jednostkach).

Dla algorytmu Edmondsa–Karpa (
BFS w każdej iteracji):

- pojedyncze BFS: **O(E)**,
- liczba iteracji: **O(V · E)** w najgorszym przypadku,
- łącznie: **O(V · E²)**.

W praktycznych scenariuszach w projekcie liczba węzłów i krawędzi jest ograniczona przez liczbę krasnoludków i kopalni.

## Zastosowanie w projekcie K.S.Z.K

- Algorytm służy do **przydziału krasnoludków do kopalni** zgodnie z pojemnościami i preferencjami,
- jest wykorzystywany jako samodzielny moduł, a także w powiązaniu z problemem Min Cost Max Flow,
- krawędzie „niepożądane” (brak preferencji) są modelowane poprzez **bardzo duży koszt**, dzięki czemu są wybierane tylko w ostateczności.

## Przykładowe użycie (koncepcyjnie)

```csharp
List<Dwarf> dwarves = ...;
List<Mine> mines   = ...;

List<int[]> assignments = DwarfAssigning.Assign(dwarves, mines);

// assignments[i][0] – ID węzła krasnoludka
// assignments[i][1] – ID węzła kopalni
```
