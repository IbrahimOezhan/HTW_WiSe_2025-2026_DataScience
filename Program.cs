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

        public static readonly IReadOnlyList<string> nonGameGenres =
        new List<string>
        {
        "Photo Editing",
        "Video Production",
        "Animation & Modeling",
        "Design & Illustration",
        "Audio Production",
        "Web Publishing",
        "Software Training",
        "Accounting",
        "Game Development",
        "Education",
        "Utilities"
            };

        static void Main(string[] args)
        {
            using var reader = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\steam_march.csv");

            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            });

            var rows = csv.GetRecords<Steam_Game>().ToList();

            var validGames = rows
                .Where(g => g.MedianPlaytimeForever > 0)
                .ToList();


            var mmoCategoryAppIds = validGames
                .Where(g => g.Categories.ToArraySafe().Contains("MMO"))
                .Select(g => g.AppId)
                .ToHashSet();

            var massivelyMultiplayerGenreAppIds = validGames
                .Where(g => g.Genres.ToArraySafe().Contains("Massively Multiplayer"))
                .Select(g => g.AppId)
                .ToHashSet();

            var onlyInCategory = mmoCategoryAppIds
                .Except(massivelyMultiplayerGenreAppIds)
                .ToList();

            var onlyInGenre = massivelyMultiplayerGenreAppIds
                .Except(mmoCategoryAppIds)
                .ToList();

            var intersection = mmoCategoryAppIds
                .Intersect(massivelyMultiplayerGenreAppIds)
                .ToList();

            var union = mmoCategoryAppIds
                .Union(massivelyMultiplayerGenreAppIds)
                .ToList();

            Console.WriteLine($"MMO Kategorie Spiele: {mmoCategoryAppIds.Count}");
            Console.WriteLine($"Massively Multiplayer Genre Spiele: {massivelyMultiplayerGenreAppIds.Count}");
            Console.WriteLine($"Überschneidung: {intersection.Count}");
            Console.WriteLine($"Nur Kategorie: {onlyInCategory.Count}");
            Console.WriteLine($"Nur Genre: {onlyInGenre.Count}");
            Console.WriteLine($"Gesamt (Union): {union.Count}");

            var appIdToName = rows
                .GroupBy(r => r.AppId)
                .ToDictionary(
                    g => g.Key,
                    g => g.First().Name
                );

            Console.WriteLine("\n--- Alle MMO oder Massively-Multiplayer Spiele ---");

            union
                .Select(id => appIdToName.TryGetValue(id, out var name) ? name : $"<Unknown {id}>")
                .OrderBy(name => name)
                .ToList()
                .ForEach(Console.WriteLine);

            Console.WriteLine("Free: " + rows.Where(x => x.Price == 0).Count());

            Console.WriteLine("Paid: " + rows.Where(x => x.Price != 0).Count());

            Console.WriteLine(rows.Count);

            Console.WriteLine($"Total Rows: {rows.Count}");

            Console.WriteLine(
                validGames.Count(game =>
                    !game.Genres.ToArraySafe().Any(g => nonGameGenres.Contains(g)))
            );

            Console.WriteLine(
                "Software Filtered: " + rows.Count(game =>
                    game.MedianPlaytimeForever == 0 &&
                    game.Tags.ParseTagDictionary().ContainsKey("Software"))
            );

            Console.WriteLine(
                "Software Total: " + rows.Count(game =>
                    game.Tags.ParseTagDictionary().ContainsKey("Software"))
            );

            Console.WriteLine(
                $"Games with MedianPlaytime > 0: {validGames.Count}"
            );


            var playerTypeEngagements = validGames
            .Where(game => !game.Genres.ToArraySafe().Any(genres => nonGameGenres.Contains(genres)))
            .SelectMany(game =>
            {
                var modes = game.Categories
                        .ToArraySafe()
                        .Where(c => playerModeTypes.Contains(c))
                        .Distinct()
                        .ToList();

                if (modes.Count == 0)
                    return Enumerable.Empty<(string mode, double es, double w)>();

                double es = (double)game.AveragePlaytimeForever / game.MedianPlaytimeForever;
                double w = 1.0 / modes.Count;

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

            playerTypeEngagements = playerTypeEngagements.OrderByDescending(x => x.ES).ToList();

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

            List<GenreEngagement> genreEngagements =
            rows.Where(game => game.MedianPlaytimeForever > 0)
            .SelectMany(game =>
            game.Genres.ToArraySafe().Select(genre => new GenreEngagement(genre, game.AveragePlaytimeForever / game.MedianPlaytimeForever))
            )
            .GroupBy(x => x.genre)
            .Select(g => new GenreEngagement(g.Key, g.Average(v => v.ES)))
            .ToList();

            genreEngagements = genreEngagements.OrderByDescending(x => x.ES).ToList();

            Utils_Plot.GeneratePlot(
                values,
                labels,
                ScottPlot.Color.Gray(2),
                "Engagement: Einzelspieler im Vergleich mit Mehrspieler",
                "Engagement Score",
                "Spieltyp",
                "Engagement_Einzel_Mehrspieler", 30, 40);

            Utils_Plot.GeneratePlot(
                playerTypeEngagements.Select(x => x.ES).ToArray(),
                playerTypeEngagements.Select(x => x.playerType).ToArray(),
                ScottPlot.Color.Gray(2),
                "Engagement: Einzelspieler im Vergleich mit Mehrspieler Kategorien",
                "Engagement Score",
                "Spieltyp",
                "Engagement_Spieltyp");

            Utils_Plot.GeneratePlot(
                genreEngagements.Select(x => x.ES).ToArray(),
                genreEngagements.Select(x => x.genre).ToArray(),
                ScottPlot.Color.Gray(2),
                "Engagement per Genre",
                "Engagement Score",
                "Genres",
                "Engagement_Genre");
        }
    }
}