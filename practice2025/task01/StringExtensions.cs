namespace task01;

public static class StringExtensions
{
    public static bool IsPalindrome(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        var cleanedChars = new List<char>();
        foreach (char c in input.ToLower())
        {
            if (!char.IsWhiteSpace(c) && !char.IsPunctuation(c))
            {
                cleanedChars.Add(c);
            }
        }

        string cleaned = new string(cleanedChars.ToArray());
        char[] reversedChars = cleaned.ToCharArray();
        Array.Reverse(reversedChars);
        string reversed = new string(reversedChars);

        return cleaned == reversed;
    }
}
