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

    private void WriteHoff(string id, string json)
    {
        Directory.CreateDirectory(_baseDir);

        var compressed = HuffmanCompressor.Compress(json);
        File.WriteAllBytes(Path.Combine(_baseDir, $"{id}.hoff"), compressed);
    }

    public string Save(string nodesJson, string name)
    {
        var id = Guid.NewGuid().ToString("N")[..8];
        var nodes = JsonSerializer.Deserialize<List<NodeDto>>(nodesJson) ?? [];

        var scenario = new ScenarioDto
        {
            Id = id,
            Name = name,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Nodes = nodes,
            Results = new ScenarioResults(),
        };

        WriteHoff(id, JsonSerializer.Serialize(scenario));

        var manifest = GetManifest();
        manifest.Add(new ManifestEntry
        {
            Id = id,
            Name = name,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            File = $"{id}.hoff",
            Dwarves = nodes.Count(n => n.Type == "dwarf"),
            Mines = nodes.Count(n => n.Type == "mine"),
            Minerals = nodes
                .Where(n => n.Type == "mine")
                .SelectMany(n => n.Minerals ?? new())
                .Distinct()
                .ToList(),
        });

        SaveManifest(manifest);
        return id;
    }

    public void Update(ScenarioDto oldScenario, string nodesJson, string newName)
    {
        var id = oldScenario.Id;
        var filePath = Path.Combine(_baseDir, $"{id}.hoff");
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Scenario {id} does not exist");

        var scenario = new ScenarioDto
        {
            Id = id,
            Name = newName,
            CreatedAt = oldScenario.CreatedAt,
            UpdatedAt = DateTime.Now,
            Nodes = JsonSerializer.Deserialize<List<NodeDto>>(nodesJson) ?? [],
            Results = new ScenarioResults(),
        };

        WriteHoff(id, JsonSerializer.Serialize(scenario));

        var manifest = GetManifest();
        var entry = manifest.FirstOrDefault(m => m.Id == id);
        if (entry != null)
        {
            entry.Name = newName;
            entry.UpdatedAt = DateTime.Now;
            entry.HasConvexHull = false;
        }
        SaveManifest(manifest);
    }

    public ScenarioDto Load(string id)
    {
        var filePath = Path.Combine(_baseDir, $"{id}.hoff");
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Scenario {id} does not exist");

        var compressed = File.ReadAllBytes(filePath);
        var json = HuffmanCompressor.Decompress(compressed);
        return JsonSerializer.Deserialize<ScenarioDto>(json)!;
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

    public void SaveResult(string id, string resultKey, object result)
    {
        var scenario = Load(id) ?? throw new Exception($"Cannot find scenario with id: {id}");
        switch (resultKey)
        {
            case "convexHull":
                scenario.Results.ConvexHull =
                    JsonSerializer.Deserialize<ConvexHullResultDto>(JsonSerializer.Serialize(result));
                break;
        }

        scenario.UpdatedAt = DateTime.Now;

        var updatedJson = JsonSerializer.Serialize(scenario);
        var compressed = HuffmanCompressor.Compress(updatedJson);
        File.WriteAllBytes(Path.Combine(_baseDir, $"{id}.hoff"), compressed);

        var manifest = GetManifest();
        var entry = manifest.FirstOrDefault(m => m.Id == id);
        if (entry != null)
        {
            entry.UpdatedAt = DateTime.Now;
            switch (resultKey)
            {
                case "convexHull": entry.HasConvexHull = true; break;
            }
            SaveManifest(manifest);
        }
    }
}

public class ManifestEntry
{
    public string Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string File { get; set; }
    public int Dwarves { get; set; }
    public int Mines { get; set; }
    public List<string> Minerals { get; set; } = new();

    public bool HasConvexHull { get; set; }
}