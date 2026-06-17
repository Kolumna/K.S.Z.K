# Królewski System Zarządzania Królestwem (K.S.Z.K)

Projekt zaliczeniowy realizowany w ramach przedmiotu **Algorytmy i Struktury Danych II**. Projekt ma na celu implementację algorytmów optymalizacyjnych w kontekście zarządzania zasobami królestwa, z wykorzystaniem języka C# i platformy .NET.

System demonstruje wykorzystanie klasycznych algorytmów i struktur danych w scenerii „królewskiego królestwa krasnoludków” – m.in. przydzielanie krasnoludków do zadań, obliczanie tras, przepływy w sieci czy problemy związane z kosztami transportu.

---

## Spis treści

1. [Opis ogólny](#opis-ogólny)
2. [Funkcjonalności](#funkcjonalności)
3. [Stos technologiczny](#stos-technologiczny)
4. [Architektura i struktura projektu](#architektura-i-struktura-projektu)
5. [Wymagania wstępne](#wymagania-wstępne)
6. [Uruchomienie aplikacji webowej](#uruchomienie-aplikacji-webowej)
7. [Uruchomienie testów jednostkowych](#uruchomienie-testów-jednostkowych)
8. [Docker](#docker)
9. [Dokumentacja szczegółowa algorytmów](#dokumentacja-szczegółowa-algorytmów)

---

## Opis ogólny

K.S.Z.K to aplikacja demonstracyjna prezentująca praktyczne wykorzystanie wybranych algorytmów i struktur danych w języku C# z użyciem platformy .NET.

Aplikacja webowa pozwala:

- wizualizować dane oraz wyniki działania algorytmów,
- ładować przygotowane scenariusze z plików,
- interaktywnie analizować działanie algorytmów na przykładowych problemach.

Warstwa logiki biznesowej (biblioteka `Krasnoludki.Core`) jest odseparowana od interfejsu użytkownika (`Krasnoludki.Web`), dzięki czemu algorytmy mogą być łatwo testowane i ponownie wykorzystywane.

---

## Funkcjonalności

- **Implementacje algorytmów i struktur danych**, m.in.:
  - Bellman–Ford,
  - drzewo przedziałowe (Segment Tree),
  - wyszukiwanie wzorców (Rabin–Karp),
  - algorytmy przypisania/przydziału zadań (`DwarfAssigning`).
- **Rozwiązania problemów na grafach i przepływach**, w tym:
  - Min Cost Max Flow (minimalny koszt maksymalnego przepływu),
  - trasy/przepływy w królestwie krasnoludków,
  - kodowanie Huffmana (kompresja danych).
- **Aplikacja webowa ASP.NET Core** umożliwiająca:
  - wizualizację wyników,
  - obsługę scenariuszy z plików,
  - pracę w sesji przeglądarkowej.
- **Testy jednostkowe (xUnit)** weryfikujące poprawność i złożoność czasową kluczowych algorytmów.

---

## Stos technologiczny

- **Język**: C#
- **Platforma**: .NET 9
- **Aplikacja webowa**: ASP.NET Core (Razor Pages)
- **Testy jednostkowe**: xUnit
- **System budowania**: `dotnet` CLI / MSBuild

---

## Architektura i struktura projektu

Projekt jest podzielony na trzy główne części:

- `src/Krasnoludki.Core/`  
  Biblioteka klas zawierająca:
  - modele i DTO,
  - implementacje algorytmów,
  - struktury danych,
  - definicje problemów (np. MinCostMaxFlow, Huffman, Trasa).

- `src/Krasnoludki.Web/`  
  Aplikacja webowa ASP.NET Core (Razor Pages), odpowiedzialna za:
  - interfejs użytkownika,
  - ładowanie scenariuszy z plików (przez `ScenarioFileService`),
  - zarządzanie sesją użytkownika,
  - prezentację wyników działania algorytmów.

- `tests/Krasnoludki.Tests/`  
  Zestaw testów jednostkowych (xUnit) pokrywających:
  - poprawność działania algorytmów,
  - przypadki brzegowe,
  - wybrane scenariusze integracyjne.

Przykładowa struktura katalogów:

```text
K.S.Z.K-main/
├─ src/
│  ├─ Krasnoludki.Core/
│  │  ├─ Algorithms/
│  │  ├─ Graph/
│  │  ├─ Models/
│  │  ├─ Dto/
│  │  └─ Problems/
│  └─ Krasnoludki.Web/
│     ├─ Pages/
│     ├─ Services/
│     ├─ wwwroot/
│     └─ Program.cs
├─ tests/
│  └─ Krasnoludki.Tests/
├─ docs/
└─ README.md
```

---

## Wymagania wstępne

Aby uruchomić projekt, wymagane jest zainstalowane środowisko **.NET SDK 9**.

Pobierz i zainstaluj .NET SDK ze strony:  
[https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)

Sprawdzenie wersji .NET:

```bash
dotnet --version
```

---

## Uruchomienie aplikacji webowej

1. Przejdź do katalogu projektu webowego:

   ```bash
   cd src/Krasnoludki.Web
   ```

2. Uruchom aplikację:

   ```bash
   dotnet run --project src/Krasnoludki.Web
   ```

   lub bezpośrednio (jeśli jesteś już w katalogu projektu):

   ```bash
   dotnet run
   ```

3. Po uruchomieniu aplikacji interfejs będzie dostępny pod adresem (domyślnie):

   ```text
   http://localhost:5145
   ```

---

## Uruchomienie testów jednostkowych

1. Przejdź do katalogu z testami:

   ```bash
   cd tests/Krasnoludki.Tests
   ```

2. Uruchom testy:

   ```bash
   dotnet test
   ```

Polecenie zbuduje rozwiązanie i wykona wszystkie testy jednostkowe, zwracając raport z ich przebiegu.

---

## Docker

Aby uruchomić projekt za pomocą Dockera, należy mieć zainstalowany Docker na swoim systemie. Następnie można użyć poniższych poleceń:

1. Pobranie obrazu Dockera

```bash
docker pull ghcr.io/kolumna/k.s.z.k:latest
```

2. Uruchomienie kontenera

```bash
docker run -d -p 5145:8080 --name kszk ghcr.io/kolumna/k.s.z.k:latest
```

Po uruchomieniu kontenera, interfejs będzie dostępny pod adresem http://localhost:5145.

---

## Dokumentacja

(`src/Krasnoludki.Core/Algorithms`)

- [`docs/algorithms/BellmanFord_PL.md`](docs/algorithms/BellmanFord_PL.md) – algorytm Bellmana–Forda wykorzystywany w Min Cost Max Flow.
- [`docs/algorithms/DwarfAssigning_PL.md`](docs/algorithms/DwarfAssigning_PL.md) – algorytm przydziału krasnoludków do kopalni (Edmonds–Karp / maksymalny przepływ).
- [`docs/algorithms/RabinKarp_PL.md`](docs/algorithms/RabinKarp_PL.md) – algorytm Rabina–Karpa do wyszukiwania wzorca w tekście.
- [`docs/algorithms/SegmentTree_PL.md`](docs/algorithms/SegmentTree_PL.md) – drzewo przedziałowe do wyszukiwania najgłośniejszego dekametrowca.
- [`docs/algorithms/Huffman_PL.md`](docs/algorithms/Huffman_PL.md) – problem kompresji tekstu z użyciem kodowania Huffmana.
  Max Flow krasnoludków i kopalni.
- [`docs/algorithms/GrahamScan_PL.md`](docs/algorithms/GrahamScan_PL.md) – problem trasy/otoczki wypukłej rozwiązany algorytmem Grahama.
- [`docs/algorithms/DocumentationMCMF_PL.md`](docs/algorithms/DocumentationMCMF_PL.md) – szczegółowy opis problemu **Min Cost Max Flow** i jego implementacji

(`Krasnoludki.Core`)

- [`docs/core/Models_PL.md`](docs/core/Models_PL.md) – opis modeli domenowych (krasnoludki, kopalnie, punkty) oraz ich odpowiedników MCMF.
- [`docs/core/GraphAndFlow_PL.md`](docs/core/GraphAndFlow_PL.md) – opis warstwy grafowej i sieci przepływu (EdgeFlow, ResidualNetwork, EdgeGen).
