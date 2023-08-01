using System;
using System.Collections.Generic;

namespace CardGame_Fool.GameFool;

public struct Card : IComparable
{
    public readonly Suits Suit;
    public readonly Ranks Rank;

    private Suits? _trumpSuit;

    public Card(Suits suit, Ranks rank)
    {
        Suit = suit;
        Rank = rank;    
    }
    
    public Suits TrumpSuit
    {
        get
        {
            if (_trumpSuit is null)
            {
                throw new InvalidOperationException();
            }

            return (Suits)_trumpSuit;
        }
        set
        {
            if (_trumpSuit is not null)
            {
                throw new InvalidOperationException();
            }

            _trumpSuit = value;
        }
    }

    public int CompareTo(object? obj)
    {
        return 1;
    }
}

public enum Suits
{
    Spades,
    Diamonds,
    Clubs,
    Hearts
};

public enum Ranks
{
    Six = 6,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    Ace
};