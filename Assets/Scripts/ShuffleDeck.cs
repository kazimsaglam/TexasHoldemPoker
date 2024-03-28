using System;
using System.Collections.Generic;


public static class ShuffleDeck
{
    private static Random random = new Random();

    // Shuffles the elements of the given list using the Fisher-Yates algorithm.
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

}
