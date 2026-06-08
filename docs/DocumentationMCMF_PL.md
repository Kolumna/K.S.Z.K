# Moduł Przydziału: Algorytm Min-Cost Max-Flow (MCMF) - Wersja z Karami (Soft Constraints)

## 1. Opis Problemu

* **Problem** : Przydzielenie krasnoludków do kopalni z jak najmniejszym całkowitym kosztem.
Celem jest znalezienie takiego układu, aby krasnoludek pokonywał jak najmniejszą drogę do pracy.
Priorytetem jest jednak przydział zgodny z preferencjami (zawodem) krasnoludka.

* **Optymalizacja** : Zachowanie maksymalnego przepływu (zatrudnienie jak największej liczby krasnoludków).
Jeśli w preferowanych kopalniach brakuje miejsc, algorytm przydziela bezrobotnych krasnoludków do najbliższych wolnych kopalni (niezgodnych z preferencjami),
nakładając na ten przydział odpowiednią karę.

* **Algorytmy** : Koncepcja algorytmu Forda-Fulkersona (dla maksymalizacji przepływu) połączona z algorytmem Bellmana-Forda
do poszukiwania ścieżki o najmniejszym koszcie w sieci rezydualnej.


## 2. Modelowanie Matematyczne (Graf Rezidualny)

* **Źródło (Source) i Ujście (Sink):** Sztucznie dodane wierzchołki, niezbędne do działania algorytmów przepływowych.
Źródło łączy się ze wszystkimi krasnoludkami, a wszystkie kopalnie łączą się z Ujściem.

* **Węzły (Nodes):** Reprezentuja rzeczywiste obiekty: Krasnoludek (Dwarf) i Kopalnia (Mine).

* **Krawędzie (Edges) i Pojemności (Capacity):** Krawędzie to połączenia między obiektami w grafie.
Pojemność (przepustowość) określa, ile jednostek 
(krasnoludków) może przejść przez daną krawędź (np. krawędź Kopalnia -> Ujście ma pojemność równą liczbie miejsc w kopalni).

* **Koszty (Costs) i Kary:** Koszt to wartość, którą płacimy za przejście przez krawędź (rzeczywista odległość fizyczna).
Kara NON_PREFERRED_PENALTY wymusza przestrzeganie preferencji. Jest dodawana do kosztu krawędzi (Krasnoludek -> Kopalnia),
gdy dany minerał nie znajduje się na liście preferencji krasnoludka. Dzięki temu Bellman-Ford zawsze najpierw wybierze tańsze,
preferowane połączenia.

## 3. Przebieg Algorytmu

1. **Inicjalizacja:** Zbudowanie sieci rezydualnej (Residual Network) na podstawie danych wejściowych.

2. **Poszukiwanie ścieżki:** Zastosowanie algorytmu Bellmana-Forda do znalezienia najtańszej ścieżki powiększającej 
od Źródła do Ujścia, omijając w pełni nasycone krawędzie.

3. **Aktualizacja przepływu i Krawędzie Powrotne:** Po znalezieniu ścieżki algorytm przepycha przez nią jednostkę przepływu 
(zmniejszając wolną przepustowość). Jednocześnie algorytm udostępnia tę samą wartość przepływu na krawędziach powrotnych 
(Backward Edges), które mają odwrócony (ujemny) koszt. 
Dzięki temu w kolejnych iteracjach algorytm ma matematyczną możliwość "wycofania" krasnoludka z kopalni,
jeśli znajdzie się inny kandydat, dla którego to miejsce będzie bardziej optymalne. 
To główny mechanizm rozwiązywania konfliktów.

## 4. Architektura i Komponenty Systemu

* `McmfMapper`: Klasa tłumacząca modele domenowe na obiekty grafowe.
* `ResidualNetwork`: Struktura przechowująca stan grafu i przepustowość w danym momencie.
* `MinCostMaxFlowProblem`: Główny silnik orkiestrujący pętlę algorytmu. 
Poszukuje ścieżek powiększających z minimalnym kosztem oraz oblicza maksymalny przepływ i 
surowy minimalny koszt (odległości razem z nałożonymi karami).

## 5. Złożoność Obliczeniowa

* **Złożoność pojedynczej iteracji:** Szukanie najkrótszej ścieżki algorytmem Bellmana-Forda zajmuje czas $O(V \cdot E)$.
* **Złożoność całkowita:** Uzależniona od całkowitego przepływu $F$ (liczby przypisanych krasnoludków).
Algorytm wykonuje się w pesymistycznym czasie $O(F \cdot V \cdot E)$.
