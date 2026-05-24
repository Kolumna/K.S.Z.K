using System.Text.Json;
using Krasnoludki.Core.DTOs;
using Krasnoludki.Core.Problems.Huffman;

namespace Krasnoludki.Web.Services;

public class ScenarioFileService
{
    private readonly string _baseDir;
    private readonly string _manifestPath;

    public ScenarioFileService(string baseDir)
    {
        _baseDir = baseDir;
        _manifestPath = Path.Combine(_baseDir, "manifest.json");
        Directory.CreateDirectory(_baseDir);
    }

    public string Save(string nodesJson, string name)
    {
        var id = Guid.NewGuid().ToString("N")[..8];

        var compressed = HuffmanCompressor.Compress(nodesJson);
        var filePath = Path.Combine(_baseDir, $"{id}.hoff");
        File.WriteAllBytes(filePath, compressed);

        var manifest = GetManifest();
        var nodes = JsonSerializer.Deserialize<List<NodeDto>>(nodesJson);

        manifest.Add(new ManifestEntry
        {
            Id = id,
            Name = name,
            CreatedAt = DateTime.Now,
            File = $"{id}.hoff",
            Dwarves = nodes?.Count(n => n.Type == "dwarf") ?? 0,
            Mines = nodes?.Count(n => n.Type == "mine") ?? 0,
            Minerals = nodes?
                .Where(n => n.Type == "mine")
                .SelectMany(n => n.Minerals ?? new())
                .Distinct()
                .ToList() ?? new(),
        });

        SaveManifest(manifest);
        return id;
    }

    public void Update(string id, string nodesJson, string newName)
    {
        var filePath = Path.Combine(_baseDir, $"{id}.hoff");
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Scenariusz {id} nie istnieje");

        var compressed = HuffmanCompressor.Compress(nodesJson);
        File.WriteAllBytes(filePath, compressed);

        var manifest = GetManifest();
        var entry = manifest.FirstOrDefault(m => m.Id == id);
        if (entry != null)
        {
            entry.Name = newName;
        }
        SaveManifest(manifest);
    }

    public string Load(string id)
    {
        var filePath = Path.Combine(_baseDir, $"{id}.hoff");
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Scenariusz {id} nie istnieje");

        var compressed = File.ReadAllBytes(filePath);
        return HuffmanCompressor.Decompress(compressed);
    }

    public List<ManifestEntry> GetManifest()
    {
        if (!File.Exists(_manifestPath))
            return new();

        var json = File.ReadAllText(_manifestPath);
        return JsonSerializer.Deserialize<List<ManifestEntry>>(json) ?? new();
    }

    private void SaveManifest(List<ManifestEntry> manifest)
    {
        File.WriteAllText(_manifestPath,
            JsonSerializer.Serialize(manifest, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
    }

    public void Delete(string id)
    {
        var filePath = Path.Combine(_baseDir, $"{id}.hoff");
        if (File.Exists(filePath))
            File.Delete(filePath);

        var manifest = GetManifest();
        manifest.RemoveAll(m => m.Id == id);
        SaveManifest(manifest);
    }

    public byte[] GetRawBytes(string id)
    {
        var filePath = Path.Combine(_baseDir, $"{id}.hoff");
        return File.ReadAllBytes(filePath);
    }

    public bool Exists(string id)
    {
        return File.Exists(Path.Combine(_baseDir, $"{id}.hoff"));
    }
}

public class ManifestEntry
{
    public string Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public string File { get; set; }
    public int Dwarves { get; set; }
    public int Mines { get; set; }
    public List<string> Minerals { get; set; } = new();
}