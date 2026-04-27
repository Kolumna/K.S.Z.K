using System;
using System.Collections.Generic;
using System.Linq;

namespace Krasnoludki.Core
{
    public record Point(long X, long Y);

    public class ConvexHullSolver
    {
        private static Point startPoint;

        private static int GetOrientation(Point p, Point q, Point r)
        {
            long value = (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);
            if (value == 0) return 0;        // punkty współliniowe
            return (value > 0) ? 1 : 2;      // 1 = zgodnie z ruchem wskazówek zegara, 2 = przeciwnie
        }

        private static long DistanceSquared(Point p1, Point p2)
        {
            // zwraca kwadrat odległości (bez pierwiastka – szybciej)
            return (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y);
        }

        public static List<Point> GrahamScan(List<Point> points)
        {
            int n = points.Count;
            if (n < 3) return points; // otoczka nie istnieje dla mniej niż 3 punktów

            // znajdź punkt startowy (najniższy, a przy remisie najbardziej na lewo)
            startPoint = points
                .OrderBy(p => p.Y)
                .ThenBy(p => p.X)
                .First();

            // sortowanie punktów względem kąta względem punktu startowego
            var sorted = points
                .Where(p => p != startPoint)
                .OrderBy(p => Math.Atan2(p.Y - startPoint.Y, p.X - startPoint.X))
                .ThenBy(p => DistanceSquared(startPoint, p))
                .ToList();

            Stack<Point> stack = new Stack<Point>();

            // inicjalizacja stosu pierwszymi trzema punktami
            stack.Push(startPoint);
            stack.Push(sorted[0]);
            stack.Push(sorted[1]);

            // przechodzimy po kolejnych punktach
            for (int i = 2; i < sorted.Count; i++)
            {
                // sprawdzamy czy skręt jest w lewo (czyli część otoczki)
                while (stack.Count > 1)
                {
                    Point top = stack.Pop();         // ostatni punkt
                    Point nextToTop = stack.Peek();  // przedostatni punkt

                    // jeśli skręt przeciwnie do ruchu wskazówek zegara – OK
                    if (GetOrientation(nextToTop, top, sorted[i]) == 2)
                    {
                        stack.Push(top);
                        break;
                    }
                }

                // dodajemy punkt do otoczki
                stack.Push(sorted[i]);
            }

            // zwracamy wynik w poprawnej kolejności
            return stack.Reverse().ToList();
        }
    }
}