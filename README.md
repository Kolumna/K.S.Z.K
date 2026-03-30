# Królewski System Zarządzania Królestwem (K.S.Z.K)

Projekt zaliczeniowy realizowany w ramach przedmiotu **Algorytmy i Struktury Danych II**.

## Struktura Projektu

Projekt został zrealizowany w środowisku .NET (C#).

* `src/Krasnoludki.Core/` - Zawiera czyste struktury danych, modele i implementacje algorytmów. Nie posiada żadnych zależności od technologii webowych.
* `src/Krasnoludki.Web/` - Interfejs użytkownika zbudowany w technologii ASP.NET Core. Odpowiada za wizualizację wyników.
* `tests/Krasnoludki.Tests/` - Projekt zawierający zautomatyzowane testy jednostkowe (xUnit) weryfikujące poprawność i optymalność algorytmów.

## Uruchomienie Projektu
Aby uruchomić projekt należy mieć zainstalowane środowisko .NET. Aby to zrobić należy pobrać i zainstalować .NET SDK ze strony [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download).

### 1. Uruchomienie Testów Jednostkowych

```bashcd tests/Krasnoludki.Tests
dotnet test
```

### 2. Uruchomienie Aplikacji Webowej

```bashcd src/Krasnoludki.Web
dotnet run --project src/Krasnoludki.Web
```

Po uruchomieniu aplikacji, interfejs będzie dostępny pod adresem `http://localhost:5000`.