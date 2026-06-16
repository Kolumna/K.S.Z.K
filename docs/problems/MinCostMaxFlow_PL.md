# Problem Min Cost Max Flow krasnoludków i kopalni

## Cel problemu

Celem jest znalezienie takiego **przepływu w sieci**, który:

- ma **maksymalną możliwą wartość przepływu** (liczbę zatrudnionych krasnoludków),
- przy **minimalnym koszcie całkowitym** (np. odległości podróży, preferencjach krasnoludków względem kopalni).

Jest to klasyczny problem **Min Cost Max Flow** – połączenie maksymalnego przepływu z minimalnym kosztem.

## Lokalizacja w projekcie

- Przestrzeń nazw: `Krasnoludki.Core.Problems`
- Główny plik: `src/Krasnoludki.Core/Problems/MinCostMaxFlow/MinCostMaxFlow.cs`
- Klasa: `MinCostMaxFlowProblem`
- Dodatkowe pliki wspierające:
  - `MCMFMapper.cs` – mapowanie danych domenowych na graf przepływu,
  - `MCMFRunner.cs` – przygotowanie i uruchomienie algorytmu.

## Model sieci przepływu

Sieć residualna (`ResidualNetwork`) zawiera:

- węzeł źródłowy (**source**) – reprezentuje „początek” przepływu (np. pulę dostępnych krasnoludków),
- węzły krasnoludków,
- węzły kopalni,
- węzeł ujścia (**sink**) – reprezentuje „koniec” przepływu (łączny zatrudniony potencjał),
- krawędzie o:
  - pojemności (`Capacity`),
  - aktualnym przepływie (`CurrFlow`),
  - koszcie (`Cost`) – np. odległość, kara za niepożądaną kopalnię.

## Metody klasy `MinCostMaxFlowProblem`

### `public (double, int) MinCostMaxFlow(ResidualNetwork network)`

- **Wejście**: w pełni skonfigurowana sieć residualna (`ResidualNetwork`).
- **Wyjście**: krotka `(MinCost, MaxFlow)`:
  - `MinCost` – minimalny koszt całkowity przepływu (skalowany z jednostek kosztu krawędzi),
  - `MaxFlow` – maksymalna wartość przepływu.

#### Idea działania

1. Ustalany jest węzeł źródłowy `source`.
2. Tworzony jest obiekt `BellmanFordAlgorithm`.
3. W pętli:
   - wyszukiwana jest **najtańsza ścieżka powiększająca przepływ** z `source` do `sink` przy użyciu Bellmana–Forda,
   - jeśli ścieżka nie istnieje (`path.Count == 0`), algorytm kończy działanie,
   - wyznaczana jest minimalna przepustowość rezydualna na ścieżce (`residualCapacity`),
   - przepływ na każdej krawędzi ścieżki jest zwiększany o `residualCapacity` (`AddFlow`),
   - aktualizowany jest łączny przepływ (`MaxFlow`) i łączny koszt (`MinCost += residualCapacity * edge.Cost`).
4. Zwracany jest wynik – koszt przeskalowany (np. podzielony przez `100000`) oraz maksymalny przepływ.

### `public (List<AssignmentDto>, int) ExtractAssignments(ResidualNetwork networkAfterMCMF)`

- **Wejście**: sieć residualna po wykonaniu Min Cost Max Flow (krawędzie zawierają `CurrFlow`).
- **Wyjście**:
  - lista `AssignmentDto` (przypisania krasnoludek–kopalnia z rzeczywistym dystansem),
  - liczba zatrudnionych krasnoludków (`employedDwarfsCount`).

#### Idea działania

1. Przeglądane są wszystkie krawędzie w sieci.
2. Dla krawędzi spełniających warunki:
   - `edge.CurrFlow > 0` – faktycznie płynie przepływ,
   - `edge.From` odpowiada krasnoludkowi,
   - `edge.To` odpowiada kopalni,

   wyciągane są:
   - `DwarfId` i `MineId` z odpowiednich węzłów grafu (`GraphDwarf`, `GraphMine`),
   - rzeczywisty dystans (koszt) z wartości `edge.Cost` po skalowaniu (`ActualDistance`).

3. Dla każdej takiej krawędzi powstaje `AssignmentDto`, a licznik zatrudnionych krasnoludków jest zwiększany.

## Złożoność obliczeniowa

Niech:
- `V` – liczba węzłów w sieci,
- `E` – liczba krawędzi.

Dla głównej części algorytmu Min Cost Max Flow z Bellmanem–Fordem:

- pojedyncze wywołanie Bellmana–Forda: **O(V · E)**,
- liczba iteracji (ile razy znajdziemy ścieżkę powiększającą przepływ) zależy od struktury sieci i pojemności,
- w praktyce dla typowych danych (rozsądna liczba krasnoludków i kopalni) jest to akceptowalne.

## Zastosowanie w projekcie K.S.Z.K

- Problem Min Cost Max Flow modeluje **globalny przydział krasnoludków do kopalni** z uwzględnieniem kosztów i ograniczeń pojemności,
- pozwala na znalezienie rozwiązania, które maksymalizuje zatrudnienie przy jednoczesnej minimalizacji sumy „kosztów podróży” lub kar za niepożądane przydziały,
- wyniki (przypisania i koszty) mogą być prezentowane w aplikacji webowej w postaci tabeli lub wizualizacji.

Więcej szczegółów dotyczących modelowania i interpretacji danych można znaleźć w:

- `docs/DocumentationMCMF_PL.md` – szczegółowy opis problemu i implementacji w języku polskim,
- `docs/DocumentationMCMF_EN.md` – odpowiednik w języku angielskim.
