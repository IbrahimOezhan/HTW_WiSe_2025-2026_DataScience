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

        public static Dictionary<string, int> ParseTagDictionary(this string input)
        {
            var result = new Dictionary<string, int>();

            if (string.IsNullOrWhiteSpace(input))
                return result;

            input = input.Trim();

            if (input.StartsWith("{") && input.EndsWith("}"))
                input = input[1..^1];

            var entries = input.Split(',');

            foreach (var entry in entries)
            {
                var parts = entry.Split(':', 2);
                if (parts.Length != 2)
                    continue;

                string key = parts[0].Trim().Trim('\'', '"');
                string valueStr = parts[1].Trim();

                if (int.TryParse(valueStr, out int value))
                {
                    result[key] = value;
                }
            }

            return result;
        }
    }

}
