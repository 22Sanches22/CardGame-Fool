using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame_Fool.GameFool;

internal class Fool
{
    private const int s_numberOfSuits = 4;
    private const int s_numberOfCardsMeanings = 9;
    private const int s_numberOfCards = s_numberOfSuits * s_numberOfCardsMeanings;
    public static readonly Dictionary<Suits, char> s_symbolOfSuits = new()
    {
        [Suits.Spades] = '♤',
        [Suits.Diamonds] = '♢',
        [Suits.Clubs] = '♧',
        [Suits.Hearts] = '♡'
    };

    public readonly Card[] _cardsDeck;
    private readonly Card _trumpCard;

    public Fool()
    {
        _cardsDeck = new Card[s_numberOfCards];

        FillDeck();
        ShuffleDeck(); 

        _trumpCard = _cardsDeck[s_numberOfCards - 1];
    }

    public void StartGame(IPlayer player1, IPlayer player2)
    {
        Stack<Card> cardsDeck = new(_cardsDeck);
        
        player1.TakeСardsFromDeck(cardsDeck, IPlayer.MaximumNumberOfCardsInHand);
        player2.TakeСardsFromDeck(cardsDeck, IPlayer.MaximumNumberOfCardsInHand);

        while (cardsDeck.Count > 0)
        {
            Card topCard = cardsDeck.Pop();
        }
    }

    private void FillDeck()
    {
        var cardNumber = 0;

        for (var i = 0; i < s_numberOfSuits; i++)
        {
            for (var j = 0; j < s_numberOfCardsMeanings; j++)
            {
                // The initial value in the "Ranks" enum is six.
                _cardsDeck[cardNumber++] = new Card((Suits)i, j + Ranks.Six);
            }
        }
    }

    private void ShuffleDeck()
    {
        Random random = new();

        for (var i = 0; i < s_numberOfCards; i++)
        {
            int index1 = random.Next(0, s_numberOfCards);
            int index2 = random.Next(0, s_numberOfCards);

            (_cardsDeck[index1], _cardsDeck[index2]) = (_cardsDeck[index2], _cardsDeck[index1]);
        }
    }
}