# K.S.Z.K – Algorytm Grahama (Convex Hull)

## Opis projektu

Projekt przedstawia implementację algorytmu Grahama (Graham Scan) służącego do wyznaczania otoczki wypukłej dla zbioru punktów na płaszczyźnie dwuwymiarowej.

Otoczka wypukła to najmniejszy wielokąt wypukły zawierający wszystkie punkty danego zbioru.

---


## Zastosowany algorytm

W projekcie wykorzystano algorytm Graham Scan.

Złożoność obliczeniowa wynosi:

O(n log n)

gdzie dominującą operacją jest sortowanie punktów według kąta biegunowego.

---

## Opis działania algorytmu

### 1. Wybór punktu startowego
Algorytm wybiera punkt:
- o najmniejszej współrzędnej Y,
- w przypadku remisu o najmniejszej współrzędnej X.

Punkt ten zawsze należy do otoczki wypukłej.

---

### 2. Sortowanie punktów
Pozostałe punkty są sortowane:
- według kąta biegunowego względem punktu startowego,
- w przypadku identycznego kąta według odległości od punktu startowego.

---

### 3. Budowa otoczki
Algorytm wykorzystuje strukturę stosu.

Dla kolejnych punktów:
- sprawdzana jest orientacja trzech ostatnich punktów,
- usuwane są punkty powodujące skręt w prawo,
- pozostają punkty tworzące otoczkę wypukłą.

---

### 4. Wyznaczanie orientacji punktów

Orientacja trzech punktów obliczana jest przy użyciu iloczynu wektorowego.

Interpretacja:
- 0 – punkty współliniowe
- 1 – skręt w prawo
- 2 – skręt w lewo

---

## Plik źródłowy

Główna implementacja znajduje się w pliku:

**AlgorytmGrahama.cs**

Plik zawiera:
- definicję punktu (record Point)
- klasę implementującą algorytm Graham Scan
- metody pomocnicze:
  - GetOrientation
  - DistanceSquared
  - GrahamScan

---

## Złożoność obliczeniowa

| Etap | Złożoność |
|------|----------|
| Sortowanie punktów | O(n log n) |
| Przetwarzanie stosu | O(n) |
| Całkowita złożoność | O(n log n) |

---

## Wymagania

- .NET 6 lub nowszy
- C# 10 lub nowszy

---

## Podsumowanie

Zaimplementowany algorytm Graham Scan umożliwia efektywne wyznaczenie otoczki wypukłej zbioru punktów w czasie O(n log n). Zastosowanie sortowania kątowego oraz analizy orientacji punktów pozwala na eliminację punktów wewnętrznych i pozostawienie jedynie punktów brzegowych.