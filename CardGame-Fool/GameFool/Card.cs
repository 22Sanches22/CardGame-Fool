using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame_Fool.GameFool;

public readonly struct Card
{
    public Card(Suits suit, Ranks rank)
    {
        (Suit, Rank) = (suit, rank);
    }

    public Suits Suit { get; }
    public Ranks Rank { get; }
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