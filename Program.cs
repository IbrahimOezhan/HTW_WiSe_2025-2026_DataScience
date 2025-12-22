using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace DataScienceSteam
{
    internal class Program
    {
        public static readonly IReadOnlyList<string> playerModeTypes =
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

        static readonly HashSet<string> multiplayerTypes = new()
                {
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

            Console.WriteLine("Free: " + rows.Where(x => x.Price == 0).Count());
            Console.WriteLine("Paid: " + rows.Where(x => x.Price != 0).Count());

            // Engagement Score per Player Type (fractional weighting per game)
            var playerTypeEngagements =
            rows
            .Where(game => game.MedianPlaytimeForever > 0)
            .SelectMany(game =>
            {
                // categories matching your playerModeTypes list
                var modes = game.Categories
                        .ToArraySafe()
                        .Where(c => playerModeTypes.Contains(c))
                        .Distinct()
                        .ToList();

                if (modes.Count == 0)
                    return Enumerable.Empty<(string mode, double es, double w)>();

                double es = (double)game.AveragePlaytimeForever / game.MedianPlaytimeForever;
                double w = 1.0 / modes.Count; // each game contributes total weight = 1

                return modes.Select(m => (mode: m, es: es, w: w));
            })
            .GroupBy(x => x.mode)
            .Select(g =>
            {
                double weightedAvg = g.Sum(v => v.es * v.w) / g.Sum(v => v.w);
                return new PlayerAmount_Engagement(g.Key, weightedAvg);
            })
            .OrderByDescending(x => x.ES)
            .ToList();

            Utils_Plot.GeneratePlot(
            playerTypeEngagements.Select(x => x.ES).ToArray(),
            playerTypeEngagements.Select(x => x.playerType).ToArray(),
            ScottPlot.Color.Gray(2),
            "Engagement per Player Type",
            "Engagement Score",
            "Player Type",
            "PlayerType_Engagement");

            var singlePlayer = playerTypeEngagements
            .Where(x => x.playerType == "Single-player")
            .Select(x => x.ES)
            .DefaultIfEmpty(0)
            .Average();

            var multiPlayer = playerTypeEngagements
            .Where(x => multiplayerTypes.Contains(x.playerType))
            .Select(x => x.ES)
            .DefaultIfEmpty(0)
            .Average();

            double[] values =
            {
                singlePlayer,
                multiPlayer
                };

            string[] labels =
            {
                "Singleplayer",
                "Multiplayer"
                };

            Utils_Plot.GeneratePlot(
            values,
            labels,
            ScottPlot.Color.Gray(2),
            "Engagement: Singleplayer vs Multiplayer",
            "Engagement Score",
            "Spieltyp",
            "Single_vs_Multi_Engagement", 30, 40);

            Utils_Plot.GeneratePlot(
playerTypeEngagements.Select(x => x.ES).Append(multiPlayer).ToArray(),
            playerTypeEngagements.Select(x => x.playerType).Append(labels[1]).ToArray(),
ScottPlot.Color.Gray(2),
"Engagement: Singleplayer vs Multiplayer",
"Engagement Score",
"Spieltyp",
"Single_vs_Multi_Engagement_2", 30, 40);

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
