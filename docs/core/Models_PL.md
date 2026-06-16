# Modele domenowe w K.S.Z.K

## Cel warstwy modeli

Warstwa modeli (`Krasnoludki.Core.Models` oraz `Krasnoludki.Core.McmfAlgorithm.Models`) opisuje **byty domenowe** używane w algorytmach i w aplikacji webowej:

- krasnoludki i ich cechy (głośność, preferencje surowców),
- kopalnie i ich pojemności,
- abstrakcyjne punkty na mapie,
- analogiczne modele dla wersji grafowej (MCMF), używane w sieci przepływu.

---

## Podstawowe typy wspólne

### `PointType` i `MineralType`

- Plik: `src/Krasnoludki.Core/Models/Point.cs`

```csharp
public enum PointType { Source, Dwarf, Mine, Sink }
public enum MineralType { Gold, Quartz, Silver, Coal, Uranium, None }
```

- `PointType` – typ logiczny punktu na mapie:
  - `Source` – źródło,
  - `Dwarf` – krasnoludek,
  - `Mine` – kopalnia,
  - `Sink` – ujście.
- `MineralType` – typ wydobywanego surowca w kopalni (lub `None`).

---

## Klasa bazowa `Point`

- Plik: `src/Krasnoludki.Core/Models/Point.cs`
- Przestrzeń nazw: `Krasnoludki.Core.Models`

Abstrakcyjna reprezentacja punktu na mapie królestwa:

- `int PointId` – unikalny identyfikator punktu,
- `double x`, `double y` – współrzędne punktu na mapie,
- `PointType? Type` – (opcjonalnie) typ punktu.

Klasę tę dziedziczą m.in. `Dwarf` i `Mine`.

---

## `Dwarf` – model krasnoludka

- Plik: `src/Krasnoludki.Core/Models/Dwarf.cs`

Dziedziczy po `Point` i reprezentuje pojedynczego krasnoludka:

- `int VoiceLoudness` – głośność głosu (wykorzystywana m.in. w drzewie przedziałowym),
- `List<MineralType> PreferredMinerals` – lista preferowanych surowców,
- `Mine? WorksIn` – aktualnie przypisana kopalnia (jeśli istnieje przydział).

Konstruktor:

```csharp
public Dwarf(int pointId, double x, double y,
             List<MineralType> preferredMinerals,
             int voiceLoudness) : base(pointId, x, y) { ... }
```

Metody:

- `void AssignMine(Mine mine)` – przypisuje krasnoludka do kopalni,
- `int GetLoudness()` – zwraca głośność (`VoiceLoudness`).

---

## `Mine` – model kopalni

- Plik: `src/Krasnoludki.Core/Models/Mine.cs`

Dziedziczy po `Point` i opisuje kopalnię:

- `MineralType Resource` – rodzaj wydobywanego surowca,
- `int Capacity` – maksymalna liczba krasnoludków, które mogą pracować w kopalni,
- `List<Dwarf> Workers` – aktualna lista krasnoludków pracujących w kopalni,
- `bool IsFull` – informacja, czy kopalnia osiągnęła pełną obsadę.

Konstruktor:

```csharp
public Mine(int pointId, double x, double y,
           MineralType resource, int capacity) : base(pointId, x, y) { ... }
```

Metody:

- `void AddWorker(Dwarf dwarf)` – dodaje krasnoludka do `Workers` i aktualizuje `IsFull` po osiągnięciu limitu.

---

## `Source` i `Sink` – sztuczne węzły źródła i ujścia

- Pliki: `src/Krasnoludki.Core/Models/Source.cs`, `Sink.cs`

### `Source`

- `int PointId` – zawsze `0` (źródło ma stały identyfikator).
- Konstruktor bezparametrowy:

```csharp
public Source()
{
    PointId = 0;
}
```

### `Sink`

- `int PointId` – ID ujścia zależne od liczby punktów w sieci.
- Konstruktor:

```csharp
public Sink(int id)
{
    PointId = id + 1;
}
```

Wartość `id` jest zazwyczaj obliczana na podstawie liczby krasnoludków i kopalni.

---

## Modele MCMF (`Krasnoludki.Core.McmfAlgorithm.Models`)

Te klasy stanowią **grafową reprezentację** krasnoludków i kopalni, wykorzystywaną w algorytmie Min Cost Max Flow oraz w `ResidualNetwork`.

### `GraphPoint`

- Plik: `src/Krasnoludki.Core/Models/ModelsMCMF/PointMCMF.cs`

```csharp
public class GraphPoint
{
    public int PointId;
    public double x { get; init; }
    public double y { get; init; }
    public PointType? Type;

    public double CalculateDistance(GraphPoint other) { ... }
}
```

- `PointId`, `x`, `y` – analogicznie jak w `Point`,
- `PointType? Type` – typ punktu (Source/Dwarf/Mine/Sink) w wersji grafowej,
- `CalculateDistance(GraphPoint other)` – oblicza odległość euklidesową do innego punktu; wykorzystywane m.in. do wyznaczania kosztu na krawędziach dwarf–mine.

> Uwaga: w tej przestrzeni nazw istnieje własny `enum PointType`, niezależny od `Krasnoludki.Core.Models.PointType`.

### `GraphDwarf`

- Plik: `src/Krasnoludki.Core/Models/ModelsMCMF/DwarfMCMF.cs`

Reprezentacja krasnoludka w sieci przepływu:

- `GraphPoint HomeLocation` – położenie na mapie,
- `int Id` – to samo ID, które pochodzi z warstwy frontendowej,
- `int VoiceLoudness` – głośność,
- `List<MineralType> PreferredMinerals` – preferowane surowce,
- `GraphMine? WorksIn` – kopalnia, w której krasnoludek pracuje (po przydziale).

Konstruktor:

```csharp
public GraphDwarf(int id, double x, double y,
                  List<MineralType> minerals, int loudness)
{
    HomeLocation = new GraphPoint(id, x, y);
    VoiceLoudness = loudness;
    PreferredMinerals = minerals;
}
```

Metoda wewnętrzna:

- `internal void AssignMine(GraphMine mine)` – ustawia referencję do kopalni; wywoływana wyłącznie z `GraphMine.AddWorker` (dwukierunkowe powiązanie).

### `GraphMine`

- Plik: `src/Krasnoludki.Core/Models/ModelsMCMF/MineMCMF.cs`

Reprezentacja kopalni w sieci przepływu:

- `GraphPoint Location` – położenie kopalni,
- `int Id` – identyfikator kopalni,
- `MineralType Resource` – wydobywany surowiec,
- `int Capacity` – maksymalna liczba krasnoludków,
- `IReadOnlyList<GraphDwarf> Workers` – lista przydzielonych krasnoludków (tylko do odczytu),
- `bool IsFull` – informacja, czy osiągnięto limit pojemności.

Metoda:

```csharp
public void AddWorker(GraphDwarf dwarf)
{
    if (IsFull)
        throw new InvalidOperationException($"Algorytm próbował przepełnić kopalnię {Id}!");

    _workers.Add(dwarf);
    dwarf.AssignMine(this);
}
```

- Zapewnia spójne, dwukierunkowe powiązanie między `GraphMine` i `GraphDwarf`.

---

## Jak te modele współgrają z algorytmami

- **`Dwarf` / `Mine` / `Point` / `Source` / `Sink`** – używane są bezpośrednio w prostszych algorytmach przydziału (`DwarfAssigning`, `EdgeGen`, `EdgeFlow`).
- **`GraphDwarf` / `GraphMine` / `GraphPoint`** – służą do budowy `ResidualNetwork` dla problemu **Min Cost Max Flow**.
- W aplikacji webowej dane wejściowe (np. scenariusze) są deserializowane do modeli z przestrzeni `Krasnoludki.Core.Models`, a następnie w razie potrzeby mapowane do odpowiedników MCMF.

Wszystkie te klasy razem tworzą spójny model królestwa krasnoludków, który jest później przetwarzany przez różne algorytmy opisane w dokumentacji.
