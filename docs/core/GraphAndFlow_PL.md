# Warstwa grafu i sieci przepływu w K.S.Z.K

## Cel warstwy grafowej

Warstwa grafowa w projekcie K.S.Z.K służy do modelowania:

- relacji pomiędzy **krasnoludkami** a **kopalniami**,
- przepływu „zasobów” (np. liczby zatrudnionych krasnoludków) w sieci,
- kosztów przejścia pomiędzy węzłami (odległości, kary za niepożądane przydziały).

Na tej warstwie opierają się algorytmy:

- maksymalnego przepływu (Edmonds–Karp),
- Min Cost Max Flow (maksymalny przepływ o minimalnym koszcie).

---

## Kluczowe elementy

### 1. `EdgeFlow` – krawędź przepływu

- Plik: `src/Krasnoludki.Core/Graph/EdgeFlow.cs`
- Przestrzeń nazw: `Krasnoludki.Core.Graph`

Reprezentuje skierowaną krawędź w sieci przepływu:

- `int From` – ID węzła początkowego,
- `int To` – ID węzła końcowego,
- `int Capacity` – pojemność krawędzi (maksymalny możliwy przepływ),
- `int CurrFlow` – aktualny przepływ,
- `double Cost` – koszt przejścia jednorazowej jednostki przepływu,
- `EdgeFlow BackwardEdge` – powiązana krawędź wsteczna (dla sieci residualnej).

#### Specjalizowane konstruktory

W projekcie występują trzy główne typy krawędzi (dla problemu przydziału krasnoludków do kopalni):

1. **Źródło → krasnoludek**
   - `EdgeFlow(Models.Source source, Models.Dwarf dwarf)`
   - `Capacity = 1` (po jednym krasnoludku), `Cost = 0`.

2. **Krasnoludek → kopalnia**
   - `EdgeFlow(Models.Dwarf dwarf, Models.Mine mine)`
   - `Capacity = 1`.
   - `Cost` zależy od preferencji i odległości:
     - jeśli kopalnia zawiera preferowany surowiec krasnoludka → koszt = euklidesowa odległość pomiędzy `(dwarf.x, dwarf.y)` a `(mine.x, mine.y)`,
     - w przeciwnym razie → bardzo duży koszt (`1000000`), co modeluje brak preferencji.

3. **Kopalnia → ujście**
   - `EdgeFlow(Models.Mine mine, Models.Sink sink)`
   - `Capacity = mine.Capacity`, `Cost = 0`.

#### Krawędź wsteczna (`BackwardEdge`)

- Dla każdej krawędzi głównej tworzy się automatycznie krawędź wsteczną poprzez prywatny konstruktor:
  - odwrócone `From` / `To`,
  - `Cost` ze znakiem przeciwnym,
  - `CurrFlow` ustawiony na pełną pojemność (symuluje w pełni „zajęty” przepływ w kierunku wstecznym).
- Pozwala to łatwo korygować przepływ w sieci (cofanie części przepływu w algorytmach przepływu).

#### Metody pomocnicze

- `int ReturnCapacity()` – zwraca przepustowość rezydualną `Capacity - CurrFlow`.
- `void AddFlow(int flow)` – zwiększa przepływ na krawędzi o `flow` i odpowiednio modyfikuje krawędź wsteczną (`BackwardEdge.CurrFlow -= flow`).

---

### 2. `EdgeGen` – generator krawędzi

- Plik: `src/Krasnoludki.Core/Graph/EdgeGen.cs`
- Klasa: `EdgeGen`
- Metoda statyczna: `GenerateEdges(List<Dwarf> dwarves, List<Mine> mines, Source source, Sink sink)`

Buduje pełną listę krawędzi przepływu dla „prostego” modelu przydziału krasnoludków do kopalni:

1. `source -> każdy krasnoludek` (po jednej krawędzi na krasnoludka),
2. `każdy krasnoludek -> każda kopalnia` (gdzie koszt odpowiada preferencjom/odległościom, patrz `EdgeFlow`),
3. `każda kopalnia -> sink` (z pojemnością równą liczbie dostępnych miejsc w kopalni).

Zwraca:

- `List<EdgeFlow>` – listę wszystkich krawędzi wykorzystywaną później przez algorytmy przepływu (`DwarfAssigning`, itp.).

---

### 3. `GraphNode<T>` i `IGraphNode` – węzły grafu

- Pliki: `src/Krasnoludki.Core/Graph/GraphNode.cs`, `IGraphNode.cs`

`GraphNode<T>` jest prostą, generyczną reprezentacją węzła grafu:

- `int GraphId` – identyfikator węzła w sieci,
- `T Data` – obiekt domenowy powiązany z węzłem (np. `GraphDwarf`, `GraphMine`).

Interfejs `IGraphNode` pozwala traktować różne typy węzłów (np. krasnoludki, kopalnie) w sposób ujednolicony, co jest wykorzystywane w `ResidualNetwork`.

---

### 4. `ResidualNetwork` – sieć residualna dla Min Cost Max Flow

- Plik: `src/Krasnoludki.Core/Graph/ResidualNetwork.cs`
- Przestrzeń nazw: `Krasnoludki.Core.Graph`

Reprezentuje sieć przepływu wykorzystywaną przez algorytm **Min Cost Max Flow**.

#### Główne pola

- `int DwarvesCount` – liczba krasnoludków (węzłów typu `GraphDwarf`),
- `int MinesCount` – liczba kopalni (`GraphMine`),
- `List<IGraphNode> _nodes` – kolekcja węzłów grafu (krasnoludki + kopalnie),
- `List<GraphEdgeFlow> Edges` – lista wszystkich krawędzi w sieci,
- `int SourceID` – identyfikator węzła źródłowego (zawsze `0`),
- `int SinkID` – identyfikator węzła ujścia (`DwarvesCount + MinesCount + 1`).

#### Konstruktor główny

`ResidualNetwork(List<GraphDwarf> dwarves, List<GraphMine> mines)`

Buduje kompletną sieć:

1. **Węzły krasnoludków**
   - Tworzone są węzły `GraphNode<GraphDwarf>` o ID od `1` do `DwarvesCount`.
   - Dla każdego krasnoludka dodawana jest krawędź `SourceID -> DwarfId` o pojemności 1.

2. **Krawędzie krasnoludek → kopalnia**
   - Indeksy kopalni zaczynają się od `DwarvesCount + 1`.
   - Dla każdej pary (krasnoludek, kopalnia), jeśli kopalnia zawiera preferowany przez krasnoludka surowiec:
     - wyliczana jest odległość `distance` pomiędzy domem krasnoludka a kopalnią,
     - koszt `cost` jest skalowany (np. `distance * 100000`),
     - tworzona jest krawędź `GraphEdgeFlow(CurrDwarfId, CurrMineId, 1, cost)`.

3. **Węzły kopalni + krawędzie do ujścia**
   - Tworzone są węzły `GraphNode<GraphMine>` dla każdej kopalni.
   - Dodawane są krawędzie `MineId -> SinkID` z pojemnością `mine.Capacity` i kosztem `0`.

#### Konstruktor testowy

`ResidualNetwork(List<IGraphNode> nodes, List<GraphEdgeFlow> edges, int sourceId, int sinkId)`

- Umożliwia ręczne wstrzyknięcie kolekcji węzłów i krawędzi – używany w testach jednostkowych, aby symulować konkretne topologie grafu.

#### Metoda `GetNode(int id)`

- Zwraca węzeł o podanym ID (1-based) z listy `_nodes`,
- Dla ID spoza zakresu zgłaszany jest `ArgumentOutOfRangeException` z komunikatem diagnostycznym.

---

## Związek z algorytmami

- `ResidualNetwork` + `GraphEdgeFlow` tworzą bazę dla:
  - `MinCostMaxFlowProblem` (Min Cost Max Flow),
  - algorytmu Bellmana–Forda (wyszukiwanie najtańszych ścieżek powiększających przepływ).
- `EdgeFlow` i `EdgeGen` są używane w „klasycznej” wersji problemu przydziału krasnoludków do kopalni (`DwarfAssigning`, Edmonds–Karp).

Dzięki wspólnej warstwie grafowej można stosunkowo łatwo dopisywać nowe problemy i algorytmy operujące na sieciach przepływu, bez zmiany logiki UI.
