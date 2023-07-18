using System.Text.Json.Serialization;

namespace MediumHardMod;

[JsonSerializable(typeof(Song))]
[JsonSourceGenerationOptions(WriteIndented = true)]
internal partial class SongJsonContext : JsonSerializerContext { }