namespace MediumHardMod;
using System.Text.Json;

public static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]) || !Directory.Exists(args[0]))
        {
            Output.WriteLine(true, """ 
                Please supply a valid input directory as the first argument.
            
                Example:
                .\MediumHardMod C:\\BandBoomboxSongs
            """);
            return;
        }

        foreach (var inputFile in new DirectoryInfo(args[0]).GetFiles("*.sjson"))
        {
            var (success, message) = ProcessFile(inputFile);
            Output.WriteLine(!success, message);
        }
    }

    public static (bool, string) ProcessFile(FileInfo inputFile)
    {
        var songText = File.ReadAllText(inputFile.FullName);
        var song = JsonSerializer.Deserialize(songText, SongJsonContext.Default.Song);

        if (song is null)
            return (false, $"{inputFile} is not a valid song file");

        if (song?.SongCharts is null || song?.SongCharts.Count == 0)
            return (false, $"{inputFile} does not contain any charts");

        var mediumChart = song?.SongCharts?.FirstOrDefault(x => x.Difficulty == 1);

        if (mediumChart is null)
            return (false, $"No medium song chart found in file \"{inputFile.Name}\"");

        if (mediumChart.Notes is null || mediumChart.Notes.Length == 0)
            return (false, $"No medium notes found in file \"{inputFile.Name}\"");

        ReplaceNotes.AddMedHardDifficulty(song!, mediumChart);

        var result = JsonSerializer.Serialize(song, typeof(Song), SongJsonContext.Default);
        var outputFile = $"{Path.GetFileNameWithoutExtension(inputFile.Name)}-medHard.sjson";
        File.WriteAllText(Path.Combine(inputFile.DirectoryName!, "Output", outputFile), result);
        return (true, $"{outputFile} created");
    }
}