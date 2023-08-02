﻿using System;
using System.Collections.Generic;

namespace CardGame_Fool.Model;

public readonly struct Card
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

    public readonly int Importance { get; }

    public readonly bool IsTrump { get; }

    private readonly int CalculateImportance()
    {
        const int bonusToTrumpCards = 10;

        return (int)Rank + (IsTrump ? bonusToTrumpCards : 0);
    }
}