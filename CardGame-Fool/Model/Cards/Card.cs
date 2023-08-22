using System;

namespace CardGameFool.Model.Cards;

public readonly struct Card : IComparable
{
    public Card(Ranks rank, Suits suit, Suits trumpSuit)
    {
        Rank = rank;
        Suit = suit;

        IsTrump = suit == trumpSuit;

        Importance = CalculateImportance();
    }

    public readonly Ranks Rank { get; }
    public readonly Suits Suit { get; }

    public readonly bool IsTrump { get; }
    public readonly int Importance { get; }

    public int CompareTo(object? obj)
    {     
        if (obj is null)
        {
            return -1;
        }

        Card card = (Card)obj;
        return card.Importance - Importance;
    }

    private readonly int CalculateImportance()
    {
        const int bonusToTrumpCards = 10;

        return (int)Rank + (IsTrump ? bonusToTrumpCards : 0);
    }
}