using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class StringCalculator
{
    private int callCount = 0;

    // Event declaration
    public event Action<string, int> AddOccured;

    public int Add(string numbers)
    {
        callCount++; // Increment call count each time Add() is called

        if (string.IsNullOrEmpty(numbers))
        {
            TriggerEvent(numbers, 0);
            return 0;
        }

        var delimiters = new List<string> { ",", "\n" };
        var numbersWithoutDelimiters = numbers;

        // Custom delimiter logic
        if (numbers.StartsWith("//"))
        {
            var match = Regex.Match(numbers, @"//(\[.*?\])+\n");
            if (match.Success)
            {
                // Multiple custom delimiters with any length
                var delimiterSection = match.Value;
                var customDelimiters = Regex.Matches(delimiterSection, @"\[(.*?)\]")
                    .Cast<Match>()
                    .Select(m => Regex.Escape(m.Groups[1].Value))
                    .ToArray();
                delimiters.AddRange(customDelimiters);
                numbersWithoutDelimiters = numbers.Substring(match.Length);
            }
            else
            {
                // Single custom delimiter
                var delimiter = numbers[2];
                delimiters.Add(Regex.Escape(delimiter.ToString()));
                numbersWithoutDelimiters = numbers.Substring(4);
            }
        }

        var numberStrings = Regex.Split(numbersWithoutDelimiters, string.Join("|", delimiters.ToArray()));

        var negativeNumbers = new List<int>();
        var sum = 0;

        foreach (var num in numberStrings)
        {
            if (int.TryParse(num, out int n))
            {
                if (n < 0)
                {
                    negativeNumbers.Add(n);
                }
                else if (n <= 1000)
                {
                    sum += n;
                }
            }
        }

        if (negativeNumbers.Any())
        {
            throw new ArgumentException("Negatives not allowed: " + string.Join(", ", negativeNumbers));
        }

        // Trigger the event after every Add() call
        TriggerEvent(numbers, sum);

        return sum;
    }

    private void TriggerEvent(string input, int result)
    {
        if (AddOccured != null)
        {
            AddOccured(input, result);
        }
    }

    // Count the number of times Add() has been called
    public int GetCalledCount()
    {
        return callCount;
    }
}
