using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System.Globalization;

namespace DataScienceSteam
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // CSV einlesen
            using var reader = new StreamReader("data.csv");

            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            });

            var rows = csv.GetRecords<SteamGameRow>().ToList();

            // Zeile finden, z.B. wo Genre == "Horror"
            var result = rows.FirstOrDefault(r => r.Name == "Dicey Chess");

            if (result != null)
            {
                Console.WriteLine($"Name: {result.Name}, Price: {result.Price}, Genre: {ToArraySafe(result.Genres)[0]}");
            }
            else
            {
                Console.WriteLine("Keine passende Zeile gefunden.");
            }

        }

        public static string[] ToArraySafe(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return Array.Empty<string>();

            string json = raw
                .Trim()
                .Replace("'", "\"");

            return System.Text.Json.JsonSerializer.Deserialize<string[]>(json)
                   ?? Array.Empty<string>();
        }

    }

    public class SteamGameRow
    {
        [Name("appid")]
        public int AppId { get; set; }

        [Name("name")]
        public string Name { get; set; }

        [Name("release_date")]
        public DateOnly ReleaseDate { get; set; }  // stabiler als DateTime

        [Name("required_age")]
        public int RequiredAge { get; set; }

        [Name("price")]
        public decimal Price { get; set; }

        [Name("dlc_count")]
        public int DlcCount { get; set; }

        [Name("detailed_description")]
        public string DetailedDescription { get; set; }

        [Name("about_the_game")]
        public string AboutTheGame { get; set; }

        [Name("short_description")]
        public string ShortDescription { get; set; }

        [Name("reviews")]
        public string Reviews { get; set; }

        [Name("header_image")]
        public string HeaderImage { get; set; }

        [Name("website")]
        public string Website { get; set; }

        [Name("support_url")]
        public string SupportUrl { get; set; }

        [Name("support_email")]
        public string SupportEmail { get; set; }

        [Name("windows")]
        public bool Windows { get; set; }

        [Name("mac")]
        public bool Mac { get; set; }

        [Name("linux")]
        public bool Linux { get; set; }

        [Name("metacritic_score")]
        public int MetacriticScore { get; set; }

        [Name("metacritic_url")]
        public string MetacriticUrl { get; set; }

        [Name("achievements")]
        public int Achievements { get; set; }

        [Name("recommendations")]
        public int Recommendations { get; set; }

        [Name("notes")]
        public string Notes { get; set; }

        [Name("supported_languages")]
        public string SupportedLanguages { get; set; } // JSON array

        [Name("full_audio_languages")]
        public string FullAudioLanguages { get; set; } // JSON array

        [Name("packages")]
        public string Packages { get; set; } // JSON object

        [Name("developers")]
        public string Developers { get; set; } // JSON array

        [Name("publishers")]
        public string Publishers { get; set; }

        [Name("categories")]
        public string Categories { get; set; }

        [Name("genres")]
        public string Genres { get ; set; }

        [Name("screenshots")]
        public string Screenshots { get; set; }

        [Name("movies")]
        public string Movies { get; set; }

        [Name("user_score")]
        public int UserScore { get; set; }

        [Name("score_rank")]
        public double? ScoreRank { get; set; }

        [Name("positive")]
        public int Positive { get; set; }

        [Name("negative")]
        public int Negative { get; set; }

        [Name("estimated_owners")]
        public string EstimatedOwners { get; set; }

        [Name("average_playtime_forever")]
        public int AveragePlaytimeForever { get; set; }

        [Name("average_playtime_2weeks")]
        public int AveragePlaytime2Weeks { get; set; }

        [Name("median_playtime_forever")]
        public int MedianPlaytimeForever { get; set; }

        [Name("median_playtime_2weeks")]
        public int MedianPlaytime2Weeks { get; set; }

        [Name("discount")]
        public int Discount { get; set; }

        [Name("peak_ccu")]
        public int PeakCcu { get; set; }

        [Name("tags")]
        public string Tags { get; set; } // JSON dict

        [Name("pct_pos_total")]
        public int PctPosTotal { get; set; }

        [Name("num_reviews_total")]
        public int NumReviewsTotal { get; set; }

        [Name("pct_pos_recent")]
        public int PctPosRecent { get; set; }

        [Name("num_reviews_recent")]
        public int NumReviewsRecent { get; set; }
    }

}
