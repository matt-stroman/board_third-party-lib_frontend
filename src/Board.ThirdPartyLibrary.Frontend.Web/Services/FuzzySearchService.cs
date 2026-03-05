using FuzzySharp;

namespace Board.ThirdPartyLibrary.Frontend.Web.Services;

/// <summary>
/// Lightweight fuzzy matching helper for in-memory title and organization browse filtering.
/// </summary>
internal static class FuzzySearchService
{
    /// <summary>
    /// Computes the highest fuzzy score for a query against candidate text values.
    /// </summary>
    /// <param name="query">Search query.</param>
    /// <param name="candidates">Candidate strings to score.</param>
    /// <returns>Best score from 0-100.</returns>
    public static int BestScore(string query, params string?[] candidates)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return 0;
        }

        var normalizedQuery = query.Trim();
        var bestScore = 0;

        foreach (var candidate in candidates)
        {
            if (string.IsNullOrWhiteSpace(candidate))
            {
                continue;
            }

            // Use token-set matching to stay robust to word order and partial phrase input.
            var score = Fuzz.TokenSetRatio(normalizedQuery, candidate);
            if (score > bestScore)
            {
                bestScore = score;
                if (bestScore == 100)
                {
                    break;
                }
            }
        }

        return bestScore;
    }

    /// <summary>
    /// Gets a practical minimum threshold for interactive title search.
    /// </summary>
    /// <param name="query">Search query.</param>
    /// <returns>Threshold from 0-100.</returns>
    public static int GetMinimumScoreThreshold(string query)
    {
        var length = query.Trim().Length;
        return length switch
        {
            <= 2 => 72,
            <= 4 => 64,
            _ => 56
        };
    }
}
