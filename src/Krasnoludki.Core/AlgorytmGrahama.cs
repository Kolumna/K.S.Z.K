using System;
using System.Collections.Generic;
using System.Linq;

namespace Krasnoludki.Core
{
   
    public record Punkt(long X, long Y);

    public class WyznacznikOtoczki
    {
        private static Punkt punktStartowy;

        private static int WyznaczOrientacje(Punkt p, Punkt q, Punkt r)
        {
            long wynik = (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);
            if (wynik == 0) return 0;
            return (wynik > 0) ? 1 : 2;
        }

        private static long KwadratOdleglosci(Punkt p1, Punkt p2)
        {
            return (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y);
        }

        public static List<Punkt> WykonajGrahamScan(List<Punkt> punkty)
        {
            int n = punkty.Count;
            if (n < 3) return punkty;

            // Znajdź najniższy punkt
            punktStartowy = punkty.OrderBy(p => p.Y).ThenBy(p => p.X).First();

            // Sortowanie kątowe
            var posortowane = punkty
                .Where(p => p != punktStartowy)
                .OrderBy(p => Math.Atan2(p.Y - punktStartowy.Y, p.X - punktStartowy.X))
                .ThenBy(p => KwadratOdleglosci(punktStartowy, p))
                .ToList();

            Stack<Punkt> stos = new Stack<Punkt>();
            stos.Push(punktStartowy);
            stos.Push(posortowane[0]);
            stos.Push(posortowane[1]);

            for (int i = 2; i < posortowane.Count; i++)
            {
                while (stos.Count > 1)
                {
                    Punkt gora = stos.Pop();
                    Punkt przedostatni = stos.Peek();
                    if (WyznaczOrientacje(przedostatni, gora, posortowane[i]) == 2)
                    {
                        stos.Push(gora);
                        break;
                    }
                }
                stos.Push(posortowane[i]);
            }

            return stos.Reverse().ToList();
        }
    }
}