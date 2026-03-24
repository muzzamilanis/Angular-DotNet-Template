namespace SimpleApi.Services
{
    public record CountryRecord(string Name, string Code, string Currency, string Symbol);

    public interface ICountryService
    {
        IReadOnlyList<CountryRecord> GetAll();
        CountryRecord? Lookup(string hint);
    }

    /// <summary>
    /// Single source of truth for the supported country/currency reference data.
    /// Registered as a singleton — data is static and allocated once.
    /// </summary>
    public class CountryService : ICountryService
    {
        private static readonly IReadOnlyList<CountryRecord> Countries = new CountryRecord[]
        {
            new("United Arab Emirates", "UAE", "AED", "د.إ"),
            new("Saudi Arabia",         "SA",  "SAR", "﷼"),
            new("United States",        "US",  "USD", "$"),
            new("United Kingdom",       "UK",  "GBP", "£"),
            new("European Union",       "EU",  "EUR", "€"),
            new("Canada",               "CA",  "CAD", "C$"),
            new("Australia",            "AU",  "AUD", "A$"),
            new("Switzerland",          "CH",  "CHF", "CHF"),
            new("China",                "CN",  "CNY", "¥"),
            new("India",                "IN",  "INR", "₹"),
            new("Japan",                "JP",  "JPY", "¥"),
            new("Pakistan",             "PK",  "PKR", "₨"),
            new("Turkey",               "TR",  "TRY", "₺"),
            new("Malaysia",             "MY",  "MYR", "RM"),
            new("Singapore",            "SG",  "SGD", "S$"),
            new("Kuwait",               "KW",  "KWD", "KD"),
            new("Qatar",                "QA",  "QAR", "QR"),
            new("Bahrain",              "BH",  "BHD", "BD"),
            new("Oman",                 "OM",  "OMR", "OMR"),
        };

        /// <summary>
        /// Maps cities, common aliases, and currency names to ISO country codes.
        /// </summary>
        private static readonly Dictionary<string, string> Aliases = new(StringComparer.OrdinalIgnoreCase)
        {
            // UAE
            ["dubai"]        = "UAE", ["abu dhabi"]  = "UAE", ["sharjah"]   = "UAE", ["ajman"]      = "UAE",
            ["dirham"]       = "UAE", ["dirhams"]    = "UAE",
            // Saudi Arabia
            ["riyadh"]       = "SA",  ["jeddah"]     = "SA",  ["mecca"]     = "SA",  ["medina"]     = "SA",
            ["riyal"]        = "SA",  ["riyals"]     = "SA",
            // United Kingdom
            ["london"]       = "UK",  ["england"]    = "UK",  ["scotland"]  = "UK",  ["wales"]      = "UK",
            ["pound"]        = "UK",  ["pounds"]     = "UK",
            // United States
            ["new york"]     = "US",  ["california"] = "US",  ["america"]   = "US",  ["usa"]        = "US",
            ["texas"]        = "US",  ["chicago"]    = "US",
            ["dollar"]       = "US",  ["dollars"]    = "US",
            // Pakistan
            ["karachi"]      = "PK",  ["lahore"]     = "PK",  ["islamabad"] = "PK",  ["rawalpindi"] = "PK",
            ["rupee"]        = "PK",  ["rupees"]     = "PK",
            // India
            ["mumbai"]       = "IN",  ["delhi"]      = "IN",  ["bangalore"] = "IN",  ["chennai"]    = "IN",
            // Qatar
            ["doha"]         = "QA",
            // Bahrain
            ["manama"]       = "BH",
            // Oman
            ["muscat"]       = "OM",
            // Kuwait
            ["kuwait city"]  = "KW",
            // Turkey
            ["istanbul"]     = "TR",  ["ankara"]     = "TR",
            // Australia
            ["sydney"]       = "AU",  ["melbourne"]  = "AU",
            // Canada
            ["toronto"]      = "CA",  ["vancouver"]  = "CA",
            // Switzerland
            ["zurich"]       = "CH",  ["geneva"]     = "CH",
            // China
            ["beijing"]      = "CN",  ["shanghai"]   = "CN",
            // Japan
            ["tokyo"]        = "JP",  ["osaka"]      = "JP",
            // Malaysia
            ["kuala lumpur"] = "MY",
            // Singapore
            ["singapore city"] = "SG",
        };

        public IReadOnlyList<CountryRecord> GetAll() => Countries;

        /// <summary>
        /// Resolves a free-text hint (country name, city, alias, currency name, ISO code)
        /// to a canonical <see cref="CountryRecord" />, or returns null when not found.
        /// Resolution order:
        ///   1. Exact match on country name
        ///   2. Exact match on ISO country code
        ///   3. Exact match on currency code
        ///   4. Alias / city dictionary lookup
        ///   5. Partial name contains match
        /// </summary>
        public CountryRecord? Lookup(string hint)
        {
            if (string.IsNullOrWhiteSpace(hint)) return null;

            var h = hint.Trim();

            var match = Countries.FirstOrDefault(c => c.Name.Equals(h, StringComparison.OrdinalIgnoreCase));
            if (match is not null) return match;

            match = Countries.FirstOrDefault(c => c.Code.Equals(h, StringComparison.OrdinalIgnoreCase));
            if (match is not null) return match;

            match = Countries.FirstOrDefault(c => c.Currency.Equals(h, StringComparison.OrdinalIgnoreCase));
            if (match is not null) return match;

            if (Aliases.TryGetValue(h, out var code))
            {
                match = Countries.FirstOrDefault(c => c.Code == code);
                if (match is not null) return match;
            }

            return Countries.FirstOrDefault(c =>
                c.Name.Contains(h, StringComparison.OrdinalIgnoreCase) ||
                h.Contains(c.Name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
