using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame_Fool.GameFool;

internal readonly struct Card
{
    public Card(Suits suit, Ranks rank)
    {
        (Suit, Rank) = (suit, rank);
    }

    public Suits Suit { get; }
    public Ranks Rank { get; }
}

internal enum Suits
{
    Spades,
    Diamonds,
    Clubs,
    Hearts
};

internal enum Ranks
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