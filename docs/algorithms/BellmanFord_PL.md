# Algorytm Bellmana–Forda

## Cel algorytmu

Algorytm Bellmana–Forda służy do wyznaczania **najkrótszych ścieżek z jednego źródła** w grafie skierowanym z krawędziami o wagach dodatnich, zerowych lub ujemnych (pod warunkiem braku cykli o ujemnej sumie wag osiągalnych ze źródła).

W projekcie K.S.Z.K algorytm ten jest wykorzystywany jako część rozwiązania problemu **Min Cost Max Flow** – do wyszukiwania **najtańszej ścieżki powiększającej przepływ** w sieci residualnej.

## Lokalizacja w projekcie

- Przestrzeń nazw: `Krasnoludki.Core.Algorithms`
- Plik źródłowy: `src/Krasnoludki.Core/Algorithms/Bellman-Ford.cs`
- Klasa: `BellmanFordAlgorithm`
- Główna metoda: `List<GraphEdgeFlow> bellmanFordAlgorithm(ResidualNetwork network, int src)`

## Dane wejściowe i wyjściowe

- **Wejście**:
  - `ResidualNetwork network` – sieć residualna z węzłami, krawędziami, przepustowościami, aktualnym przepływem oraz kosztem na krawędziach,
  - `int src` – identyfikator węzła źródłowego.
- **Wyjście**:
  - `List<GraphEdgeFlow>` – lista krawędzi tworzących **najtańszą ścieżkę** od źródła do ujścia (`network.SinkID`).
  - Pusta lista oznacza, że **brak jest dalszych ścieżek powiększających przepływ**.

## Idea działania (wysoki poziom)

1. **Inicjalizacja**
   - Ustalana jest liczba węzłów na podstawie `network.SinkID + 1`.
   - Tablica `distances` jest wypełniana wartościami nieskończonymi (`long.MaxValue`), poza węzłem źródłowym (`src`), który otrzymuje odległość `0`.
   - Tablica `parentEdge` przechowuje krawędzie prowadzące do danego węzła, co pozwala później odtworzyć ścieżkę.

2. **Relaksacja krawędzi (V–1 iteracji)**
   - W pętli wykonywane są kolejne iteracje relaksacji wszystkich krawędzi z sieci:
     - dla każdej krawędzi `edge` sprawdzane jest, czy:
       - istnieje ścieżka do `edge.From` (odległość nie jest nieskończona),
       - krawędź ma dodatnią przepustowość resztową (`Capacity - CurrFlow > 0`),
       - przejście przez tę krawędź poprawia (zmniejsza) koszt dojścia do węzła `edge.To`.
     - w razie poprawy, aktualizowany jest dystans, a w `parentEdge[edge.To]` zapisywana jest ta krawędź.
   - Analogiczna relaksacja wykonywana jest również dla **krawędzi wstecznych** (`BackwardEdge`), co jest kluczowe w sieciach residualnych.
   - Jeżeli w danej iteracji nie dokonano żadnych zmian (`modified == false`), pętla jest przerywana wcześniej.

3. **Odtworzenie ścieżki**
   - Po zakończeniu relaksacji algorytm sprawdza, czy węzeł ujścia (`network.SinkID`) jest osiągalny (czy jego dystans jest różny od nieskończoności).
   - Jeśli tak, ścieżka jest odtwarzana **od ujścia do źródła** poprzez odczytywanie tablicy `parentEdge` i następnie odwracana (`path.Reverse()`), aby uzyskać ją w poprawnej kolejności.

## Złożoność obliczeniowa

Niech:
- `V` – liczba węzłów w grafie,
- `E` – liczba krawędzi w grafie.

- Część relaksacyjna: do `V - 1` iteracji, każdorazowo po wszystkich krawędziach → **O(V · E)**.
- Odtworzenie ścieżki: w najgorszym przypadku liniowo względem `V` → **O(V)**.

**Łącznie**: dominującą częścią jest relaksacja → **O(V · E)**.

## Zastosowanie w projekcie K.S.Z.K

- Algorytm jest używany przez `MinCostMaxFlowProblem` do znajdowania **najtańszej ścieżki powiększającej przepływ** w sieci krasnoludków i kopalni.
- Dzięki temu możliwe jest uwzględnienie **kosztów podróży** czy preferencji krasnoludków przy przydziale do kopalń.

## Uwagi implementacyjne

- W implementacji wykorzystywane są zarówno krawędzie **przednie**, jak i **wsteczne** (z sieci residualnej), co pozwala na „cofanie” przepływu.
- Brak ścieżki (pusta lista) jest wykorzystywany jako sygnał zakończenia głównej pętli w algorytmie Min Cost Max Flow.
