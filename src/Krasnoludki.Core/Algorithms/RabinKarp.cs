namespace Krasnoludki.Core.Algorithms;

public static class RabinKarp
{
  private const int Base = 256;
  private const int Prime = 101;

  public static List<int> ContainsSubstring(string text, string pattern)
  {
    int p = 0; // hash patternu
    int t = 0; // hash aktualnego okna w tekście
    int h = 1; // Base^(m-1) % Prime

    List<int> ans = new List<int>();

    if (string.IsNullOrEmpty(pattern) || string.IsNullOrEmpty(text))
    {
      return ans;
    }

    if (pattern.Length > text.Length)
    {
      return ans;
    }

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
        if (match)
        {
          ans.Add(i);
        }
      }

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