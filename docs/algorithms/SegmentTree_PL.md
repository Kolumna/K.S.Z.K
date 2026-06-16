# Drzewo przedziałowe (Segment Tree) dla krasnoludków-dekametrowców

## Cel struktury danych

Drzewo przedziałowe (Segment Tree) służy do **szybkiego odpowiadania na zapytania dotyczące przedziałów** na tablicy lub liście. W tym projekcie struktura jest używana do:

- znajdowania **najgłośniejszego dekametrowca** (krasnoludka) w zadanym przedziale indeksów,
- wykonywania zapytań w czasie logarytmicznym względem liczby krasnoludków.

## Lokalizacja w projekcie

- Przestrzeń nazw: `Krasnoludki.Core.Algorithms`
- Plik źródłowy: `src/Krasnoludki.Core/Algorithms/SegmentTree.cs`
- Klasa: `SegmentTree`

## Model danych

- `List<Dwarf> Decametrists` – lista krasnoludków-dekametrowców, na której budowane jest drzewo,
- `_tree` – tablica przechowująca **indeksy** elementów z `Decametrists` reprezentujące wynik na danym przedziale (tu: indeks najgłośniejszego krasnoludka).

Każdy liść drzewa odpowiada jednemu dekametrowcowi, a każdy węzeł wewnętrzny – połączeniu dwóch przedziałów i przechowuje indeks **głośniejszego** z dwóch kandydatów.

## Metody publiczne

- `SegmentTree(List<Dwarf> decametrists)` – konstruktor:
  - zapisuje listę dekametrowców,
  - alokuje tablicę `_tree` o rozmiarze `4 * n`,
  - wywołuje metodę `Build(0, 0, n - 1)`.

- `Dwarf GetLoudestDecametrist()` – zwraca **najgłośniejszego krasnoludka** na całym przedziale `[0, n - 1]`.

- `Dwarf GetLoudestDecametrist(int l, int r)` – zwraca najgłośniejszego krasnoludka w zadanym przedziale indeksów `[l, r]` (0-based).

## Idea działania (wysoki poziom)

1. **Budowa drzewa (`Build`)**
   - Jeśli `start == end`, węzeł reprezentuje pojedynczy element – w `_tree[node]` zapisywany jest indeks `start`.
   - W przeciwnym wypadku:
     - dzielimy przedział `[start, end]` na dwa mniejsze (`[start, mid]` i `[mid+1, end]`),
     - rekurencyjnie budujemy lewe i prawe poddrzewo,
     - w węźle `node` zapisujemy indeks **głośniejszego** z dzieci (`Louder(leftIndex, rightIndex)`).

2. **Zapytanie (`Query`)**
   - Dla danego węzła z przedziałem `[start, end]` i szukanego przedziału `[l, r]`:
     - jeśli przedziały się nie przecinają → zwracamy `-1` (brak kandydata),
     - jeśli przedział węzła jest w całości zawarty w `[l, r]` → zwracamy `_tree[node]`,
     - w przeciwnym razie:
       - pytamy rekurencyjnie lewe i prawe poddrzewo,
       - łączymy wyniki, wybierając głośniejszego dekametrowca (`Louder`).

3. **Porównywanie głośności (`Louder`)**
   - Metoda `Louder(int idxA, int idxB)` porównuje wyniki `GetLoudness()` dla dwóch krasnoludków i zwraca indeks głośniejszego z nich.

## Złożoność obliczeniowa

Niech `n` będzie liczbą dekametrowców:

- budowa drzewa: **O(n)**,
- pojedyncze zapytanie `GetLoudestDecametrist(l, r)`: **O(log n)**,
- pamięć: **O(n)** (dokładniej około `4n` na tablicę `_tree`).

## Zastosowanie w projekcie K.S.Z.K

- Szybkie znajdowanie **najgłośniejszego krasnoludka** na zadanym fragmencie listy,
- Możliwość wykorzystania przy zadaniach optymalizacyjnych, gdzie głośność dekametrowców ma znaczenie (np. wybór lidera grupy na danym odcinku trasy).

## Przykładowe użycie (koncepcyjnie)

```csharp
List<Dwarf> decametrists = ...;

var tree = new SegmentTree(decametrists);

Dwarf loudestAll = tree.GetLoudestDecametrist();
Dwarf loudestPart = tree.GetLoudestDecametrist(5, 10);
```
