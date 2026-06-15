# Algorytm Rabina–Karpa

## Cel algorytmu

Algorytm Rabina–Karpa służy do **wyszukiwania wzorca w tekście** (problem wyszukiwania podciągu). Zamiast porównywać znak po znaku dla każdej możliwej pozycji, wykorzystuje on **funkcję skrótu (hash)**, aby szybko odrzucić większość niedopasowań.

## Lokalizacja w projekcie

- Przestrzeń nazw: `Krasnoludki.Core.Algorithms`
- Plik źródłowy: `src/Krasnoludki.Core/Algorithms/RabinKarp.cs`
- Klasa: `static class RabinKarp`
- Główna metoda: `List<int> ContainsSubstring(string text, string pattern)`

## Dane wejściowe i wyjściowe

- **Wejście**:
  - `string text` – tekst, w którym szukamy,
  - `string pattern` – wzorzec, który chcemy odnaleźć.
- **Wyjście**:
  - `List<int>` – lista indeksów (0-based) w `text`, na których rozpoczyna się pełne dopasowanie `pattern`.

W przypadku:
- pustego `text` lub `pattern`,
- `pattern.Length > text.Length`,

zwracana jest **pusta lista**.

## Idea działania (wysoki poziom)

1. **Parametry haszowania**
   - Ustalona jest podstawa i modulo:
     - `Base = 256` – liczba możliwych znaków (kodów ASCII),
     - `Prime = 101` – liczba pierwsza jako modulo, ograniczająca wartość hasza.

2. **Wstępne obliczenia**
   - Obliczana jest wartość `h = Base^(m-1) % Prime`, gdzie `m` to długość wzorca – służy ona do „usuwania” pierwszego znaku okna przy przesuwaniu.
   - Wyznaczane są hasze:
     - `p` – hash wzorca `pattern`,
     - `t` – hash pierwszego okna tekstu o długości `m`.

3. **Przesuwanie okna po tekście**
   - Dla każdej pozycji `i` od `0` do `text.Length - pattern.Length`:
     - jeśli `p == t`, porównywane są znaki `pattern` i odpowiadające im znaki w `text` (weryfikacja dopasowania),
     - w przypadku dopasowania indeks `i` dodawany jest do listy wyników.

4. **Aktualizacja hasza okna (`rolling hash`)**
   - Jeśli nie jesteśmy na końcu tekstu, hash dla kolejnego okna jest obliczany ze starego hasza `t` poprzez:
     - odjęcie wkładu pierwszego znaku,
     - przesunięcie (mnożenie przez `Base`),
     - dodanie nowego znaku,
     - zredukowanie modulo `Prime` i ewentualną korektę wartości ujemnych.

## Złożoność obliczeniowa

Niech:
- `n` – długość tekstu `text`,
- `m` – długość wzorca `pattern`.

- Obliczenie początkowych haszy: **O(m)**,
- Przejście po wszystkich możliwych oknach: **O(n − m + 1)**,
- Weryfikacja treści przy zbieżności hashy: w najgorszym przypadku **O(m)**, ale zwykle rzadko.

Średnio złożoność wynosi **O(n + m)**, a w najgorszym przypadku (częste zbieżności hashy) **O(n · m)**.

## Zastosowanie w projekcie K.S.Z.K

Algorytm może być stosowany do:

- wyszukiwania wzorców w opisach, nazwach, scenariuszach tekstowych związanych z królestwem krasnoludków,
- szybkiej filtracji potencjalnych dopasowań przy dalszym przetwarzaniu danych tekstowych.

## Przykładowe użycie (koncepcyjnie)

```csharp
string text = "KRASNO-LUDKI-W-KOPALNI";
string pattern = "KOPALNI";

List<int> positions = RabinKarp.ContainsSubstring(text, pattern);
// positions zawiera indeksy, na których występuje "KOPALNI" w `text`
```
