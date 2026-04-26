using Xunit;
using Krasnoludki.Core;
using System.Collections.Generic;

namespace Krasnoludki.Tests
{
    public class AlgorytmTests
    {
        [Fact]
        public void TestOtoczkiGrahama()
        {
            // Dane testowe: Kwadrat i jeden punkt w środku
            var punkty = new List<Punkt>
            {
                new Punkt(0, 0),
                new Punkt(4, 0),
                new Punkt(4, 4),
                new Punkt(0, 4),
                new Punkt(2, 2) // Ten jest w środku, nie powinien być w otoczce
            };

            var wynik = WyznacznikOtoczki.WykonajGrahamScan(punkty);

            // Powinniśmy dostać 4 punkty (wierzchołki kwadratu)
            Assert.Equal(4, wynik.Count);
        }
    }
}