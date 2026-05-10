using System;
using System.Collections.Generic;
using System.Text;

namespace Krasnoludki.Core.Problems.Book;

public class HuffmanNode : IComparable<HuffmanNode>
{
  public char Character { get; set; }
  public int Frequency { get; set; }
  public HuffmanNode Left { get; set; }
  public HuffmanNode Right { get; set; }

  public bool IsLeaf => Left == null && Right == null;

  public int CompareTo(HuffmanNode other) => Frequency.CompareTo(other.Frequency);
}

public class HuffmanCoder
{
  private HuffmanNode _root;
  private Dictionary<char, string> _codes = new();

  public void Build(string text)
  {
    if (string.IsNullOrEmpty(text))
    {
      throw new ArgumentException("Input text cannot be empty.");
    }

    var freq = new Dictionary<char, int>();
    foreach (char c in text)
    {
      freq[c] = freq.GetValueOrDefault(c) + 1;
    }

    var pq = new PriorityQueue<HuffmanNode, int>();
    foreach (var (ch, f) in freq)
    {
      pq.Enqueue(new HuffmanNode { Character = ch, Frequency = f }, f);
    }

    while (pq.Count > 1)
    {
      var left = pq.Dequeue();
      var right = pq.Dequeue();

      var parent = new HuffmanNode
      {
        Character = '\0',
        Frequency = left.Frequency + right.Frequency,
        Left = left,
        Right = right
      };

      pq.Enqueue(parent, parent.Frequency);
    }

    _root = pq.Dequeue();

    _codes.Clear();
    GenerateCodes(_root, "");
  }

  private void GenerateCodes(HuffmanNode node, string code)
  {
    if (node == null) return;

    if (node.IsLeaf)
    {
      _codes[node.Character] = code.Length > 0 ? code : "0";
      return;
    }

    GenerateCodes(node.Left, code + "0");
    GenerateCodes(node.Right, code + "1");
  }

  public string Encode(string text)
  {
    if (_root == null)
      throw new InvalidOperationException("First call Build() with the text to create the Huffman tree.");

    var sb = new StringBuilder();
    foreach (char c in text)
      sb.Append(_codes[c]);
    return sb.ToString();
  }
  public string Decode(string encoded)
  {
    var sb = new StringBuilder();
    var current = _root;

    foreach (char bit in encoded)
    {
      current = bit == '0' ? current.Left : current.Right;

      if (current.IsLeaf)
      {
        sb.Append(current.Character);
        current = _root;
      }
    }

    return sb.ToString();
  }

  public void PrintCodes()
  {
    Console.WriteLine("\n┌──────────┬───────────┬──────────┐");
    Console.WriteLine("│  Znak    │   Kod     │  Długość │");
    Console.WriteLine("├──────────┼───────────┼──────────┤");

    foreach (var (ch, code) in _codes)
    {
      string display = ch == ' ' ? "SPACJA" : ch == '\n' ? "\\n" : ch.ToString();
      Console.WriteLine($"│  {display,-8}│  {code,-9}│  {code.Length,-8}│");
    }

    Console.WriteLine("└──────────┴───────────┴──────────┘");
  }

  public void PrintStats(string original, string encoded)
  {
    int originalBits = original.Length * 8;
    int encodedBits = encoded.Length;

    Console.WriteLine($"\nOryginał:    {original.Length} znaków = {originalBits} bitów");
    Console.WriteLine($"Zakodowany:  {encodedBits} bitów");
    Console.WriteLine($"Kompresja:   {(1.0 - (double)encodedBits / originalBits) * 100:F1}%");
  }
}
