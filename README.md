# Królewski System Zarządzania Królestwem (K.S.Z.K)

Projekt zaliczeniowy realizowany w ramach przedmiotu **Algorytmy i Struktury Danych II**. Projekt ma na celu implementację algorytmów optymalizacyjnych w kontekście zarządzania zasobami królestwa, z wykorzystaniem języka C# i platformy .NET.

## Struktura Projektu

* `src/Krasnoludki.Core/` - Zawiera czyste struktury danych, modele i implementacje algorytmów. Nie posiada żadnych zależności od technologii webowych.
* `src/Krasnoludki.Web/` - Interfejs użytkownika zbudowany w technologii ASP.NET Core. Odpowiada za wizualizację wyników.
* `tests/Krasnoludki.Tests/` - Projekt zawierający zautomatyzowane testy jednostkowe (xUnit) weryfikujące poprawność i optymalność algorytmów.

## Uruchomienie Projektu
Aby uruchomić projekt należy mieć zainstalowane środowisko .NET. Aby to zrobić należy pobrać i zainstalować .NET SDK ze strony [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download).

### 1. Uruchomienie Testów Jednostkowych

```bash
dotnet test
```

### 2. Uruchomienie Aplikacji Webowej

```bash
dotnet run --project src/Krasnoludki.Web
```

Po uruchomieniu aplikacji, interfejs będzie dostępny pod adresem `http://localhost:5145`.

## Docker
Aby uruchomić projekt za pomocą Dockera, należy mieć zainstalowany Docker na swoim systemie. Następnie można użyć poniższych poleceń:
### 1. Pobranie obrazu Dockera

```bash 
docker pull ghcr.io/kolumna/k.s.z.k:latest
```
### 2. Uruchomienie kontenera

```bash
docker run -d -p 5145:8080 --name kszk ghcr.io/kolumna/k.s.z.k:latest
```
Po uruchomieniu kontenera, interfejs będzie dostępny pod adresem `http://localhost:5145`.

## Problemy
# Moduł Przydziału: Algorytm Min-Cost Max-Flow (MCMF)

## 1. Opis Problemu

* **Problem** : Przydzielenie krasnoludków do kopalni z jak najmniejszym całkowitym kosztem.
Celem jest znalezienie takiego układu, aby krasnoludek pokonywał jak najmniejszą drogę do pracy.

* **Ograniczenia (Strict Constraints):** Przydział musi być w 100% zgodny z preferencjami (zawodem) krasnoludka. Jeśli brakuje miejsc w preferowanych kopalniach lub krasnoludek nie ma żadnych preferencji, nie zostaje on przydzielony do żadnej pracy (zostaje bezrobotny).

* **Algorytmy** : Koncepcja algorytmu Forda-Fulkersona (dla maksymalizacji przepływu) połączona z algorytmem Bellmana-Forda
do poszukiwania ścieżki o najmniejszym koszcie w sieci rezydualnej.


## 2. Modelowanie Matematyczne (Graf Rezidualny)

* **Źródło (Source) i Ujście (Sink):** Sztucznie dodane wierzchołki, niezbędne do działania algorytmów przepływowych.
Źródło łączy się ze wszystkimi krasnoludkami, a wszystkie kopalnie łączą się z Ujściem.

* **Węzły (Nodes):** Reprezentuja rzeczywiste obiekty: Krasnoludek (Dwarf) i Kopalnia (Mine).

* **Krawędzie (Edges) i Pojemności (Capacity):** Krawędzie to połączenia między obiektami w grafie. Krawędź między krasnoludkiem a kopalnią powstaje **tylko i wyłącznie wtedy**, gdy minerał z kopalni znajduje się na liście preferencji krasnoludka. Pojemność (przepustowość) określa, ile jednostek 
(krasnoludków) może przejść przez daną krawędź (np. krawędź Kopalnia -> Ujście ma pojemność równą liczbie miejsc w kopalni).

* **Koszty (Costs):** Koszt to wartość, którą płacimy za przejście przez krawędź (rzeczywista odległość fizyczna).
W tym modelu odpowiada on rzeczywistej, fizycznej odległości między domem krasnoludka a kopalnią (zwiększonej o rzędy wielkości na czas obliczeń, aby uniknąć problemów z precyzją zmiennoprzecinkową).

## 3. Przebieg Algorytmu

1. **Inicjalizacja:** Zbudowanie sieci rezydualnej (Residual Network) na podstawie danych wejściowych, z rygorystycznym pominięciem niechcianych połączeń.

2. **Poszukiwanie ścieżki:** Zastosowanie algorytmu Bellmana-Forda do znalezienia najtańszej ścieżki powiększającej 
od Źródła do Ujścia, omijając w pełni nasycone krawędzie.

3. **Aktualizacja przepływu i Krawędzie Powrotne:** Po znalezieniu optymalnej ścieżki algorytm przepycha przez nią jednostkę przepływu. Jednocześnie na grafie udostępniane są **krawędzie powrotne** (Backward Edges) o odwróconym (ujemnym) koszcie. Dzięki temu algorytm może w kolejnych iteracjach "wycofać" krasnoludka z kopalni, jeśli znajdzie się inny kandydat, dla którego to miejsce w ogólnym rozrachunku będzie tańsze. To fundament rozwiązywania konfliktów o zasoby.

## 4. Architektura i Komponenty Systemu

* `McmfMapper`: Klasa tłumacząca modele domenowe na obiekty grafowe.
* `ResidualNetwork`: Struktura przechowująca stan grafu i przepustowość. Jej konstruktor odpowiada za ścisłe filtrowanie preferencji krasnoludków.
* `MinCostMaxFlowProblem`: Główny silnik orkiestrujący pętlę algorytmu. Zwraca całkowity koszt, maksymalny przepływ (liczbę zatrudnionych) oraz gotową listę optymalnych przypisań (`AssignmentDto`).

## 5. Złożoność Obliczeniowa

* **Złożoność pojedynczej iteracji:** Szukanie najkrótszej ścieżki algorytmem Bellmana-Forda zajmuje czas $O(V \cdot E)$.
* **Złożoność całkowita:** Uzależniona od całkowitego przepływu $F$ (liczby przypisanych krasnoludków).
Algorytm wykonuje się w pesymistycznym czasie $O(F \cdot V \cdot E)$.

