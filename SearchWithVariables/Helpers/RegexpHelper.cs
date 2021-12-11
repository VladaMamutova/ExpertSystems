using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SearchWithVariables.Model;

namespace SearchWithVariables.Helpers
{
    public static class RegexpHelper
    {
        const string SEPARATOR = @"(?:, )?";

        const string ARGUMENT_NAME = @"([a-zA-Z0-9]+)";
        static readonly string ARGUMENT_LIST_PATTERN = $"{ARGUMENT_NAME}{SEPARATOR}";

        const string PREDICATE_NAME = @"([a-zA-Z])";
        const string PREDICATE_ARGUMENTS = @"([a-zA-Z0-9, ]*)";
        static readonly string PREDICATE_PATTERN =
            $@"{PREDICATE_NAME}\({PREDICATE_ARGUMENTS}\)";

        static readonly string PREDICATE_LIST_PATTERN =
            $@"(?:{PREDICATE_PATTERN}{SEPARATOR})";

        private const string RULE_PATTERN = @"(\d)\) (.*) -> (.*)";

        internal static List<Vertex> MatchPredicateList(string value)
        {
            MatchCollection matches = Regex.Matches(value, PREDICATE_LIST_PATTERN);

            var predicates = new List<Vertex>();
            foreach (Match match in matches)
            {
                predicates.Add(new Vertex(match.Groups[1].Value[0],
                    MatchArgumentList(match.Groups[2].Value)));
            }

            return predicates;
        }

        internal static Vertex MatchPredicate(string value)
        {
            Match match = Regex.Match(value, PREDICATE_LIST_PATTERN);
            return new Vertex(match.Groups[1].Value[0],
                MatchArgumentList(match.Groups[2].Value));
        }

        internal static List<string> MatchArgumentList(string value)
        {
            string argumentName = @"([a-zA-Z0-9]+)";
            string separator = @"(?:, )";
            string argumentPattern = $"{argumentName}{separator}?"; // "H", "M1"

            MatchCollection matches = Regex.Matches(value, argumentPattern);

            return (from Match match in matches select match.Groups[1].Value).ToList();
        }

        internal static Rule MatchRule(string value)
        {
            Match match = Regex.Match(value, RULE_PATTERN);
            return new Rule(match.Groups[1].Value[0],
                MatchPredicateList(match.Groups[2].Value),
                MatchPredicate(match.Groups[3].Value));
        }
    }
}
