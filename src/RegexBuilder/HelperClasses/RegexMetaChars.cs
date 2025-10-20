namespace RegexBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class RegexMetaChars
    {
        public const string AnyCharacter = ".";
        public const string LineStart = "^";
        public const string LineEnd = "$";
        public const string StringStart = "\\A";
        public const string WordBoundary = "\\b";
        public const string NonWordBoundary = "\\B";
        public const string Digit = "\\d";
        public const string NonDigit = "\\D";
        public const string Escape = "\\e";
        public const string FormFeed = "\\f";
        public const string ConsecutiveMatch = "\\G";
        public const string NewLine = "\\n";
        public const string CarriageReturn = "\\r";
        public const string WhiteSpace = "\\s";
        public const string NonwhiteSpace = "\\S";
        public const string Tab = "\\t";
        public const string VerticalTab = "\\v";
        public const string WordCharacter = "\\w";
        public const string NonWordCharacter = "\\W";
        public const string StringEnd = "\\Z";

        /// <summary>
        /// Supported Unicode general categories and named blocks for use with \p{} and \P{} constructs.
        /// </summary>
        private static readonly HashSet<string> SupportedUnicodeCategories = new HashSet<string>(StringComparer.Ordinal)
        {
            // General Categories
            "L", "Lu", "Ll", "Lt", "Lm", "Lo",
            "N", "Nd", "Nl", "No",
            "P", "Pc", "Pd", "Ps", "Pe", "Pi", "Pf", "Po",
            "M", "Mn", "Mc", "Me",
            "Z", "Zs", "Zl", "Zp",
            "S", "Sm", "Sc", "Sk", "So",
            "C", "Cc", "Cf", "Cs", "Co", "Cn",

            // Common Named Blocks
            "IsBasicLatin",
            "IsLatin1Supplement",
            "IsLatinExtended-A",
            "IsLatinExtended-B",
            "IsIPA",
            "IsSpacingModifierLetters",
            "IsCombiningDiacriticalMarks",
            "IsGreekandCoptic",
            "IsCyrillic",
            "IsCyrillicSupplementary",
            "IsArmenian",
            "IsHebrew",
            "IsArabic",
            "IsSyriac",
            "IsThaana",
            "IsDevanagari",
            "IsBengali",
            "IsGurmukhi",
            "IsGujarati",
            "IsOriya",
            "IsTamil",
            "IsTelugu",
            "IsKannada",
            "IsMalayalam",
            "IsSinhala",
            "IsThai",
            "IsLao",
            "IsTibetan",
            "IsMyanmar",
            "IsGeorgian",
            "IsHangul",
            "IsEthiopic",
            "IsCherokee",
            "IsKhmer",
            "IsMongolian",
            "IsJamo",
            "IsHiragana",
            "IsKatakana",
            "IsBopomofo",
            "IsHanCompatibilityJamo",
            "IsKanbun",
            "IsEnclosedCharacters",
            "IsCJKUnifiedIdeographs",
            "IsPrivateUseArea",
            "IsCJKCompatibilityIdeographs",
            "IsAlphabeticPresentationForms",
            "IsArabicPresentationFormsA",
            "IsVariationSelectors",
            "IsArabicPresentationFormsB",
            "IsCJKCompatibilityIdeographsSupplement",
            "IsTags",
            "IsDeseret",
            "IsOsmanya",
            "IsShavian",
            "IsOgham",
            "IsRunic",
            "IsTagalog",
            "IsHanunoo",
            "IsBuhid",
            "IsTagbanwa",
            "IsKhmerSymbols",
            "IsLimbu",
            "IsTaiLe",
            "IsKhmerSymbols",
            "IsNewTaiLue",
            "IsKhmerSymbols",
            "IsYijingHexagramSymbols",
            "IsSylotiNagri",
            "IsOldPersian",
            "IsKharoshthi",
            "IsSamaritan",
            "IsOldItalic",
            "IsGothic",
            "IsOldPermic",
            "IsUgaritic",
            "IsOldPersian",
            "IsDeseret",
            "IsShavian",
            "IsOsmanya",
            "IsCypriot",
            "IsPhoenetician",
            "IsLydian",
            "IsKharoshthi",
            "IsBactrian",
            "IsInscriptionalParthian",
            "IsInscriptionalPahlavi",
            "IsSarmatian",
            "IsOldSouthArabian",
            "IsOldNorthArabian",
            "IsManichaean",
            "IsAvestan",
            "IsInscriptionalPahlavi",
            "IsInscriptionalParthian",
            "IsPsalterPahlavi",
            "IsOldTurkic",
            "IsOldHungarian",
            "IsHanifiSyllabary",
            "IsBrahmi",
            "IsKaithi",
            "IsSoraSompeng",
            "IsCakchikiBraille",
            "IsBrahmi",
            "IsKaithi",
            "IsSoraSompeng",
            "IsChakma",
            "IsMahajani",
            "IsSharada",
            "IsSinhalaArchaicNumbers",
            "IsKhojki",
            "IsMultani",
            "IsKhudawadi",
            "IsGrantha",
            "IsNewa",
            "IsSiddhams",
            "IsModiSiyaqNumbers",
            "IsMarchenIdeographs",
            "IsMro",
            "IsMro",
            "IsTangut",
            "IsTangutComponents",
            "IsKhitanSmallScript",
            "IsSmallKanaExtension",
            "IsNushu",
            "IsDuployan",
            "IsShorthandFormatControls",
            "IsByzzantineMusicalSymbols",
            "IsMusicalSymbols",
            "IsAncientGreekMusicalNotation",
            "IsMayan",
            "IsTalismans",
            "IsKhmerSymbols",
            "IsBoxDrawing",
            "IsBlockElements",
            "IsGeometricShapes",
            "IsMiscellaneousSymbols",
            "IsDingbats",
            "IsMiscellaneousSymbolsandArrows",
            "IsSupplementalArrows-A",
            "IsSupplementalArrows-B",
            "IsChessSymbols",
            "IsMusicalSymbols",
            "IsAncientGreekMusicalNotation",
            "IsMayan",
            "IsTalismans",
            "IsAlchemicalSymbols",
            "IsArabicMathematicalAlphabeticSymbols",
            "IsSortingHat",
            "IsBoxDrawing",
            "IsBlockElements",
            "IsGeometricShapes",
            "IsMiscellaneousSymbols",
            "IsDingbats",
            "IsMiscellaneousSymbolsandArrows",
            "IsSupplementalMathematicalOperators",
            "IsSupplementalPunctuation",
            "IsCurrencySymbols",
            "IsLetterlikeSymbols",
            "IsNumberForms",
            "IsArrows",
            "IsMathematicalOperators",
            "IsMiscellaneousTechnical",
            "IsControlPictures",
            "IsOpticalCharacterRecognition",
            "IsEnclosedAlphanumerics",
            "IsBoxDrawing",
            "IsBlockElements",
            "IsGeometricShapes",
            "IsMiscellaneousSymbols",
            "IsDingbats",
            "IsMiscellaneousSymbolsandArrows",
            "IsSupplementalArrows-A",
            "IsSupplementalArrows-B",
            "IsChessSymbols",
            "IsWeatherSymbols",
            "IsAstrologicalSymbols",
            "IsAlchemicalSymbols",
            "IsGeometricShapesExtended",
            "IsSupplementalMathematicalOperators",
            "IsArrowsExtended",
            "IsSupplementalPunctuation",
            "IsSupplementalLettersandPunctuation",
        };

        /// <summary>
        /// Validates whether a Unicode category name is supported.
        /// </summary>
        /// <param name="categoryName">The category name to validate.</param>
        /// <returns>True if the category is supported; otherwise, false.</returns>
        public static bool IsValidUnicodeCategory(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                return false;
            }

            return SupportedUnicodeCategories.Contains(categoryName);
        }

        /// <summary>
        /// Gets all supported Unicode general categories.
        /// </summary>
        /// <returns>An enumeration of supported Unicode general category names.</returns>
        public static IEnumerable<string> GetGeneralCategories()
        {
            var generalCategories = new[]
            {
                "L", "Lu", "Ll", "Lt", "Lm", "Lo",
                "N", "Nd", "Nl", "No",
                "P", "Pc", "Pd", "Ps", "Pe", "Pi", "Pf", "Po",
                "M", "Mn", "Mc", "Me",
                "Z", "Zs", "Zl", "Zp",
                "S", "Sm", "Sc", "Sk", "So",
                "C", "Cc", "Cf", "Cs", "Co", "Cn"
            };
            return generalCategories;
        }

        /// <summary>
        /// Gets all supported Unicode named blocks.
        /// </summary>
        /// <returns>An enumeration of supported Unicode named block names.</returns>
        public static IEnumerable<string> GetNamedBlocks()
        {
            return SupportedUnicodeCategories.Where(c => c.StartsWith("Is", StringComparison.Ordinal));
        }
    }
}
