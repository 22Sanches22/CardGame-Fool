using System.Collections.Generic;

namespace CardGame_Fool.GameFool;

public readonly struct Card
{
    public readonly Suits Suit;
    public readonly Ranks Rank;

    public Card(Suits suit, Ranks rank)
    {
        Suit = suit;
        Rank = rank;    
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