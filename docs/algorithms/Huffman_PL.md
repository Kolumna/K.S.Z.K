# Problem kompresji tekstu – kodowanie Huffmana

## Cel problemu

Celem jest **kompresja tekstu** przy użyciu klasycznego algorytmu **kodowania Huffmana**, który przydziela krótsze kody częściej występującym znakom, a dłuższe rzadszym. Pozwala to zmniejszyć całkowity rozmiar danych.

## Lokalizacja w projekcie

- Przestrzeń nazw: `Krasnoludki.Core.Problems.Huffman`
- Plik źródłowy: `src/Krasnoludki.Core/Problems/Huffman/HuffmanCompressor.cs`
- Klasy:
  - `HuffmanNode` – węzeł drzewa Huffmana,
  - `HuffmanCompressor` – logika kompresji i dekompresji tekstu.

## Model danych

- `HuffmanNode`:
  - `char Character` – znak,
  - `int Frequency` – liczba wystąpień znaku w tekście,
  - `HuffmanNode Left`, `Right` – dzieci w drzewie Huffmana,
  - `bool IsLeaf` – informacja, czy węzeł jest liściem.

- Strumień skompresowanych danych (tablica `byte[]`) zawiera:
  - liczbę różnych znaków,
  - częstotliwości każdego znaku,
  - długość ciągu bitów,
  - właściwe zakodowane bity.

## Metody publiczne

- `static byte[] Compress(string text)`
  - Wejście: tekst do skompresowania.
  - Wyjście: tablica bajtów – skompresowany tekst wraz z nagłówkiem opisującym częstotliwości i długość bitów.

- `static string Decompress(byte[] data)`
  - Wejście: tablica bajtów zwrócona przez `Compress`.
  - Wyjście: oryginalny tekst.

## Idea działania (wysoki poziom)

### 1. Kompresja (`Compress`)

1. **Liczenie częstotliwości znaków**
   - Dla każdego znaku w tekście zliczana jest liczba jego wystąpień.

2. **Budowa drzewa Huffmana**
   - Tworzona jest **kolejka priorytetowa** węzłów posortowanych po częstotliwości.
   - Najmniej częste węzły są łączone w nowe węzły aż do powstania jednego korzenia – drzewa Huffmana.

3. **Generowanie kodów**
   - Dla każdego liścia (znaku) generowany jest kod binarny:
     - `0` dla przejścia w lewo,
     - `1` dla przejścia w prawo.
   - Tworzona jest tabela: `char -> string (kod binarny)`.

4. **Kodowanie tekstu**
   - Oryginalny tekst jest zamieniany na ciąg bitów poprzez wstawienie kodów dla kolejnych znaków.

5. **Zapis do bajtów**
   - Do strumienia binarnego zapisywane są:
     - liczba różnych znaków,
     - pary (znak, częstotliwość),
     - długość ciągu bitów,
     - właściwe bity zakodowane do bajtów.

### 2. Dekompresja (`Decompress`)

1. **Odczyt nagłówka**
   - Z danych odczytywana jest liczba znaków oraz częstotliwości każdego z nich.

2. **Odtworzenie drzewa Huffmana**
   - Na podstawie słownika `(znak -> częstotliwość)` budowane jest ponownie drzewo.

3. **Odczyt zakodowanych bitów**
   - Odczytywana jest długość ciągu bitów.
   - Pozostałe bajty są konwertowane do ciągu znaków `'0'` / `'1'`.

4. **Przechodzenie po drzewie**
   - Dla każdego bitu:
     - `0` – przejście do lewego dziecka,
     - `1` – przejście do prawego dziecka.
   - Gdy osiągnięty zostaje liść – znak jest dopisywany do wyniku, a pozycja wraca do korzenia.

5. **Przypadek szczególny – jeden znak**
   - Gdy w drzewie jest tylko jeden liść, wynik to powtórzenie tego znaku wymaganą liczbę razy.

## Złożoność obliczeniowa

Niech `n` będzie długością tekstu, a `k` – liczbą różnych znaków.

- Liczenie częstotliwości: **O(n)**,
- Budowa drzewa (operacje na kolejce priorytetowej): **O(k log k)**,
- Kompresja/dekompresja ciągu bitów: **O(n)**.

Całościowo złożoność jest rzędu **O(n + k log k)**.

## Zastosowanie w projekcie K.S.Z.K

- Problem Huffmana demonstruje, jak **kompresja bezstratna** może zostać zastosowana do tekstów związanych z królestwem krasnoludków (zapis scenariuszy),
- pokazuje również praktyczne wykorzystanie struktur drzewiastych i kolejek priorytetowych.
