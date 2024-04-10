using System;
using System.Collections.Generic;


public static class ShuffleDeck
{
  private static readonly Random Random = new Random();

  // Shuffles the elements of the given list using the Fisher-Yates algorithm.
  public static void Shuffle<T>(this IList<T> list)
  {
    int n = list.Count;
    while (n > 1)
    {
      n--;
      int k = Random.Next(n + 1);
      (list[k], list[n]) = (list[n], list[k]);
    }
  }
}