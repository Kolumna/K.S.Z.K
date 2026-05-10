using System;
using Krasnoludki.Core.Problems.Book;

namespace HuffmanCoding.Tests;

public class HuffmanCoderTests
{
    [Fact]
    public void Decode_ShouldReturnOriginalText()
    {
        var coder = new HuffmanCoder();
        coder.Build("Lubię placki!");
        var encoded = coder.Encode("Lubię placki!");
        var decoded = coder.Decode(encoded);
        Assert.Equal("Lubię placki!", decoded);
    }

    [Fact]
    public void Encode_ShouldReturnOnlyBits()
    {
        var coder = new HuffmanCoder();
        coder.Build("hello");
        var encoded = coder.Encode("hello");
        Assert.All(encoded, bit => Assert.Contains(bit, "01"));
    }

    [Fact]
    public void Encode_ShouldBeShorterThanAscii()
    {
        var coder = new HuffmanCoder();
        var text = "aaabbbccc";
        coder.Build(text);
        var encoded = coder.Encode(text);
        Assert.True(encoded.Length < text.Length * 8);
    }

    [Fact]
    public void Build_SingleUniqueChar_ShouldGetCodeZero()
    {
        var coder = new HuffmanCoder();
        coder.Build("aaaa");
        var encoded = coder.Encode("aaaa");
        Assert.Equal("0000", encoded);
    }

    [Fact]
    public void Build_ShouldThrowOnEmptyText()
    {
        var coder = new HuffmanCoder();
        Assert.Throws<ArgumentException>(() => coder.Build(""));
    }

    [Fact]
    public void Encode_ShouldThrowWhenBuildNotCalled()
    {
        var coder = new HuffmanCoder();
        Assert.Throws<InvalidOperationException>(() => coder.Encode("abc"));
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("abrakadabra")]
    [InlineData("hello world")]
    [InlineData("the quick brown fox jumps over the lazy dog")]
    public void RoundTrip_ShouldReturnOriginalText(string text)
    {
        var coder = new HuffmanCoder();
        coder.Build(text);
        var encoded = coder.Encode(text);
        var decoded = coder.Decode(encoded);
        Assert.Equal(text, decoded);
    }
}