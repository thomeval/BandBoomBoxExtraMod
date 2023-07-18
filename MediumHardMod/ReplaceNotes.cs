﻿using System.Text;
namespace MediumHardMod;

internal static class ReplaceNotes
{
    private static readonly Dictionary<char, char> _replacements = new()
    {
        ['4'] = 'D',
        ['5'] = 'E',
        ['7'] = 'A',
        ['8'] = 'B',
        ['D'] = '4',
        ['E'] = '5',
        ['A'] = '7',
        ['B'] = '8'
    };

    private static char ReplaceChar(char x) => 
        _replacements.ContainsKey(x) ? _replacements[x] : x;
    private static string? ReplaceSubNote(string subNote) => 
        $"{subNote[0]}{ReplaceChar(subNote[1])}{ReplaceChar(subNote[2])}{subNote[3]}";

    private static string ReplaceAllNotes(string note, ref bool replace)
    {
        var result = new StringBuilder();

        foreach (var subNote in note.Split(' '))
        {
            if (result.Length != 0) result.Append(' ');

            result.Append(replace ? ReplaceSubNote(subNote) : subNote);

            if (subNote != "0000") replace = !replace;
        }

        return result.ToString();
    }

    public static void AddMedHardDifficulty(Song song, SongChart mediumChart)
    {
        if (song?.SongCharts is null) return;

        SongChart medHardChart = new()
        {
            Difficulty = 10,
            Notes = new string[mediumChart.Notes?.Length ?? 0],
            DifficultyLevel = mediumChart.DifficultyLevel + 1,
            Group = mediumChart.Group
        };

        bool replace = true;

        for (int i = 0; i < mediumChart?.Notes?.Length; i++)
        {
            var newNote = ReplaceAllNotes(mediumChart.Notes[i], ref replace);
            medHardChart.Notes[i] = newNote;
        }

        song.SongCharts.Add(medHardChart);
        song.SongCharts = song?.SongCharts?.OrderBy(x => x.DifficultyLevel).ToList();
    }
}
