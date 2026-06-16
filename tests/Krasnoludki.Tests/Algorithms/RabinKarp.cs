using System.Collections.Generic;
using Xunit;
using Krasnoludki.Core.Algorithms;

namespace Krasnoludki.Tests
{
    public class RabinKarpTests
    {
        [Theory]
        [InlineData("rabarbar", "bar", new int[] { 2, 5 })]
        [InlineData("hello world", "world", new int[] { 6 })]
        [InlineData("testowanie", "xyz", new int[] { })] // Brak dopasowania

        [InlineData("aaaa", "aa", new int[] { 0, 1, 2 })] 

        [InlineData("abrakadabra", "abra", new int[] { 0, 7 })] 
        [InlineData("start i koniec", "start", new int[] { 0 })]
        [InlineData("start i koniec", "koniec", new int[] { 8 })]

        [InlineData("", "abc", new int[] { })]
        [InlineData("abc", "", new int[] { })]
        [InlineData("", "", new int[] { })]

        [InlineData("abc", "abcdef", new int[] { })]
        
        [InlineData("dokladnie to samo", "dokladnie to samo", new int[] { 0 })]
        public void ContainsSubstring_ShouldReturnCorrectIndices(string text, string pattern, int[] expected)
        {
            var expectedList = new List<int>(expected);
            var result = RabinKarp.ContainsSubstring(text, pattern);
            
            Assert.Equal(expectedList, result);
        }
    }
}