namespace DataScienceSteam
{
    internal static class Utils
    {
        public static string[] ToArraySafe(this string raw)
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

}
