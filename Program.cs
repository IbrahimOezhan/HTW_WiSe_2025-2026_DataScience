using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace DataScienceSteam
{
    internal class Program
    {
        public static readonly IReadOnlyList<string> PlayerModeTypes =
    new List<string>
    {
        "Single-player",
        "Multi-player",
        "Co-op",
        "Online Co-op",
        "LAN Co-op",
        "Shared/Split Screen Co-op",
        "Shared/Split Screen",

        "PvP",
        "Online PvP",
        "LAN PvP",

        "Cross-Platform Multiplayer",
        "MMO",

        "Remote Play Together"
    };


        static void Main(string[] args)
        {
            using var reader = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\steam_march.csv");

            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            });

            var rows = csv.GetRecords<Steam_Game>().ToList();

            List<PlayerAmount_Engagement> playerAmount = 
                rows
                .Where(game => game.MedianPlaytimeForever > 0)
                .SelectMany(game =>
                game.Categories.
                ToArraySafe().
                Where(element => PlayerModeTypes.Contains(element))
                Select(x => )

            List<GenreAvergePlaytime> genrePlaytime =
                rows
                .Where(game => game.MedianPlaytimeForever > 0)
                .SelectMany(game =>
                    game.Genres.
                    ToArraySafe().
                    Select(genre => 
                    new GenreAvergePlaytime
                    {
                        genre = genre,
                        avgPlaytime = game.AveragePlaytimeForever / game.EstOwners()
                    })
                )
                .GroupBy(x => x.genre)
                .Select(g =>
                new GenreAvergePlaytime
                {
                    genre = g.Key,
                    avgPlaytime = g.Average(v => v.avgPlaytime)
                })
                .ToList();

            List<GenreEngagement> genreEngagements =
                rows.Where(game => game.MedianPlaytimeForever > 0)
                .SelectMany(game =>
                    game.Genres.ToArraySafe().Select(genre => new GenreEngagement(genre, game.AveragePlaytimeForever / game.MedianPlaytimeForever))
                )
                .GroupBy(x => x.genre)
                .Select(g => new GenreEngagement(g.Key, g.Average(v => v.ES)))
                .ToList();

            Utils_Plot.GeneratePlot(
                genrePlaytime.Select(x => x.avgPlaytime).ToArray(),
                genreEngagements.Select(x => x.ES).ToArray(),
                genreEngagements.Select(x => x.genre).ToArray(),
                ScottPlot.Color.Gray(2),
                "Engagement per Genre",
                "Engagement Score",
                "Genres",
                "File");
        }
    }
}
