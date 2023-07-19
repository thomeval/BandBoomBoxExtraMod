namespace MediumHardMod;
using System.Text.Json;

public static class Program
{
    public static bool OverwriteExistingCharts { get; set; } = false;
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

        if (args.Contains("-Overwrite"))
            OverwriteExistingCharts = true;

        ProcessFolder(args[0]);
    }

    public static void ProcessFolder(string folder)
    {
        foreach (var subFolder in Directory.GetDirectories(folder))
            ProcessFolder(subFolder);

        foreach (var inputFile in new DirectoryInfo(folder).GetFiles("*.sjson"))
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

        if (song.SongCharts.Count == 0)
            return (false, $"{inputFile} does not contain any charts");

        var mediumChart = song.SongCharts.FirstOrDefault(x => x.Difficulty == (int) Difficulty.Medium);

        if (mediumChart is null)
            return (false, $"No medium song chart found in file \"{inputFile.Name}\"");

        if (mediumChart.Notes is null || mediumChart.Notes.Length == 0)
            return (false, $"No medium notes found in file \"{inputFile.Name}\"");

        if (song.HasChart( Difficulty.Extra) && !OverwriteExistingCharts)
            return (false, $"Extra chart already exists in file \"{inputFile.Name}\"");

        ReplaceNotes.AddExtraDifficulty(song, mediumChart);

        var result = JsonSerializer.Serialize(song, typeof(Song), SongJsonContext.Default);
        File.WriteAllText(inputFile.FullName, result);

        return (true, $"Extra chart successfully created in \"{inputFile}\"");
    }
}