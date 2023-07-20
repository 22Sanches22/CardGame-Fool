using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame_Fool.GameFool;

internal readonly struct Card
{
    public Card(Suits suit, Ranks meaning)
    {
        (Suit, Meaning) = (suit, meaning);
    }

    public Suits Suit { get; }
    public Ranks Meaning { get; }
}
