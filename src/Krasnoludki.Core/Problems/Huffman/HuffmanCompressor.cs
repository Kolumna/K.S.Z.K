using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Krasnoludki.Core.Problems.Huffman
{
    public class HuffmanNode
    {
        public char Character { get; set; }
        public int Frequency { get; set; }
        public HuffmanNode Left { get; set; }
        public HuffmanNode Right { get; set; }

        public bool IsLeaf => Left == null && Right == null;
    }

    public class HuffmanCompressor
    {
        public static byte[] Compress(string text)
        {
            if (string.IsNullOrEmpty(text))
                return Array.Empty<byte>();

            var frequencies = new Dictionary<char, int>();
            foreach (char ch in text)
            {
                if (frequencies.ContainsKey(ch))
                    frequencies[ch]++;
                else
                    frequencies[ch] = 1;
            }

            var queue = new List<HuffmanNode>();
            foreach (var pair in frequencies)
            {
                queue.Add(new HuffmanNode { Character = pair.Key, Frequency = pair.Value });
            }

            while (queue.Count > 1)
            {
                queue = queue.OrderBy(node => node.Frequency).ToList();

                var left = queue[0];
                var right = queue[1];
                queue.RemoveRange(0, 2);

                var parent = new HuffmanNode
                {
                    Character = '\0',
                    Frequency = left.Frequency + right.Frequency,
                    Left = left,
                    Right = right
                };

                queue.Add(parent);
            }

            var root = queue.Single();

            var huffmanCodes = new Dictionary<char, string>();
            GenerateCodes(root, "", huffmanCodes);

            var bitString = new StringBuilder();
            foreach (char ch in text)
            {
                bitString.Append(huffmanCodes[ch]);
            }

            return ConvertBitStringToBytes(bitString.ToString(), frequencies);
        }

        public static string Decompress(byte[] data)
        {
            if (data == null || data.Length == 0)
                return string.Empty;

            using var memoryStream = new MemoryStream(data);
            using var reader = new BinaryReader(memoryStream);

            int freqCount = reader.ReadInt32();
            var frequencies = new Dictionary<char, int>();
            for (int i = 0; i < freqCount; i++)
            {
                char ch = reader.ReadChar();
                int freq = reader.ReadInt32();
                frequencies[ch] = freq;
            }

            var root = BuildTree(frequencies);

            int bitLength = reader.ReadInt32();

            var remainingBytes = reader.ReadBytes((int)(memoryStream.Length - memoryStream.Position));

            var bitString = new StringBuilder();
            foreach (byte b in remainingBytes)
            {
                for (int i = 7; i >= 0; i--)
                {
                    bitString.Append((b & (1 << i)) != 0 ? '1' : '0');
                }
            }

            var result = new StringBuilder();
            var current = root;

            if (root.IsLeaf)
            {
                for (int i = 0; i < bitLength; i++)
                    result.Append(root.Character);
                return result.ToString();
            }

            for (int i = 0; i < bitLength; i++)
            {
                current = bitString[i] == '0' ? current.Left : current.Right;

                if (current.IsLeaf)
                {
                    result.Append(current.Character);
                    current = root;
                }
            }

            return result.ToString();
        }

        private static HuffmanNode BuildTree(Dictionary<char, int> frequencies)
        {
            var queue = new List<HuffmanNode>();
            foreach (var pair in frequencies)
            {
                queue.Add(new HuffmanNode
                {
                    Character = pair.Key,
                    Frequency = pair.Value
                });
            }

            if (queue.Count == 1)
                return queue[0];

            while (queue.Count > 1)
            {
                queue = queue.OrderBy(node => node.Frequency).ToList();

                var left = queue[0];
                var right = queue[1];
                queue.RemoveRange(0, 2);

                queue.Add(new HuffmanNode
                {
                    Character = '\0',
                    Frequency = left.Frequency + right.Frequency,
                    Left = left,
                    Right = right
                });
            }

            return queue.Single();
        }

        private static void GenerateCodes(HuffmanNode node, string currentCode, Dictionary<char, string> lookupTable)
        {
            if (node == null) return;

            if (node.IsLeaf)
            {
                lookupTable[node.Character] = currentCode;
                return;
            }

            GenerateCodes(node.Left, currentCode + "0", lookupTable);
            GenerateCodes(node.Right, currentCode + "1", lookupTable);
        }

        private static byte[] ConvertBitStringToBytes(string bitString, Dictionary<char, int> frequencies)
        {
            using (var memoryStream = new MemoryStream())
            using (var writer = new BinaryWriter(memoryStream))
            {
                writer.Write(frequencies.Count);
                foreach (var pair in frequencies)
                {
                    writer.Write(pair.Key);
                    writer.Write(pair.Value);
                }

                writer.Write(bitString.Length);

                byte currentByte = 0;
                int bitCount = 0;

                foreach (char bit in bitString)
                {
                    currentByte <<= 1;
                    if (bit == '1')
                    {
                        currentByte |= 1;
                    }

                    bitCount++;

                    if (bitCount == 8)
                    {
                        writer.Write(currentByte);
                        currentByte = 0;
                        bitCount = 0;
                    }
                }

                if (bitCount > 0)
                {
                    currentByte <<= (8 - bitCount);
                    writer.Write(currentByte);
                }

                return memoryStream.ToArray();
            }
        }
    }
}