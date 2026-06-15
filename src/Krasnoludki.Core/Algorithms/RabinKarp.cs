namespace Krasnoludki.Core.Algorithms;

public static class RabinKarp
{
  private const int Base = 256;
  private const int Prime = 101;

  public static List<int> ContainsSubstring(string text, string pattern)
  {
    int p = 0; // hash patternu
    int t = 0; // hash aktualnego okna w tekście
    int h = 1; // Base^(m-1) % Prime - mnożnik do usuwania pierwszego znaku z okna

    // Lista indeksów, gdzie znaleziono dopasowanie
    List<int> ans = new List<int>();

    if (string.IsNullOrEmpty(pattern) || string.IsNullOrEmpty(text))
    {
      return ans;
    }

    if (pattern.Length > text.Length)
    {
      return ans;
    }

    // Liczenie h = Base^(m-1) % Prime - czyli hash patternu
    for (int i = 0; i < pattern.Length - 1; i++)
    {
      h = (h * Base) % Prime;
    }

    for (int i = 0; i < pattern.Length; i++)
    {
      p = (Base * p + pattern[i]) % Prime;
      t = (Base * t + text[i]) % Prime;
    }

    for (int i = 0; i <= text.Length - pattern.Length; i++)
    {
      // Jeśli hash patternu i aktualnego okna w tekście są równe, sprawdzamy dokładne dopasowanie
      if (p == t)
      {
        bool match = true;
        for (int j = 0; j < pattern.Length; j++)
        {
          if (text[i + j] != pattern[j])
          {
            match = false;
            break;
          }
        }
        // Jeśli jest git to dodajemy indeks do wyników
        if (match)
        {
          ans.Add(i);
        }
      }

      // Obliczanie hash dla następnego okna tekstu: usuwamy pierwszy znak i dodajemy nowy
      if (i < text.Length - pattern.Length)
      {
        t = (Base * (t - text[i] * h) + text[i + pattern.Length]) % Prime;
        if (t < 0)
        {
          t += Prime;
        }
      }
    }
    
    return ans;
  }
}