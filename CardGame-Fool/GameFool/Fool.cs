using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CardGame_Fool.GameFool;

internal class Fool
{
    private const int s_numberSuits = 4;
    private const int s_numberRanks = 9;
    private const int s_numberOfCards = s_numberSuits * s_numberRanks;
    public static readonly Dictionary<Suits, char> s_symbolsOfSuits = new()
    {
        [Suits.Spades] = '♤',
        [Suits.Diamonds] = '♢',
        [Suits.Clubs] = '♧',
        [Suits.Hearts] = '♡'
    };

    public readonly Card[] _cardsDeck;
    private readonly Card _trumpCard;
    private readonly Dictionary<Card, int> _cardsImportance;

    public Fool()
    {
        _cardsDeck = new Card[s_numberOfCards];

        FillDeck();
        ShuffleDeck();

        _trumpCard = _cardsDeck[0];

        _cardsImportance = new Dictionary<Card, int>(s_numberOfCards);
        foreach (Card card in _cardsDeck)
        {
            _cardsImportance.Add(card, CalculateCardImportance(card));
        }  
    }

    public void StartGame(IPlayer player1, IPlayer player2)
    {
        Stack<Card> cardsDeck = new(_cardsDeck);

        player1.TakeСardsFromDeck(cardsDeck, IPlayer.MaxNumberOfCardsInHand);
        player2.TakeСardsFromDeck(cardsDeck, IPlayer.MaxNumberOfCardsInHand);

        IPlayer firstPlayer = DefinitionFirstPlayer(player1, player2);
        IPlayer secondPlayer = (firstPlayer == player1) ? player2 : player1;

        while (cardsDeck.Count > 0)
        {
            Card topCard = cardsDeck.Pop();


        }
    }

    private void FillDeck()
    {
        var cardNumber = 0;

        for (var i = 0; i < s_numberSuits; i++)
        {
            for (var j = 0; j < s_numberRanks; j++)
            {
                // The initial rank in the "Ranks" enum is six.
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

    private int CalculateCardImportance(Card card)
    {
        return (int)card.Rank + ((card.Suit == _trumpCard.Suit) ? 10 : 0);
    }

    private IPlayer DefinitionFirstPlayer(IPlayer player1, IPlayer player2)
    {
        Card[] playerCards1 = player1.Cards.ToArray();
        Card[] playerCards2 = player2.Cards.ToArray();

        for (var i = 0; i < IPlayer.MaxNumberOfCardsInHand; i++)
        {
            for (var j = 0; j < IPlayer.MaxNumberOfCardsInHand - 1; j++)
            {
                ComparisonAndSwappingCards(ref playerCards1[j], ref playerCards1[j + 1]);
                ComparisonAndSwappingCards(ref playerCards2[j], ref playerCards2[j + 1]);
            }
        }


        List<Card> playerTrumps1 = new();
        List<Card> playerTrumps2 = new();

        for (var i = 0; i < IPlayer.MaxNumberOfCardsInHand; i++)
        {
            if (playerCards1[i].Suit == _trumpCard.Suit)
            {
                playerTrumps1.Add(playerCards1[i]);
            }

            if (playerCards2[i].Suit == _trumpCard.Suit)
            {
                playerTrumps2.Add(playerCards2[i]);
            }
        }

        if ((playerTrumps1.Count + playerTrumps2.Count) > 0)
        {
            for (var i = 0; i < Math.Min(playerTrumps1.Count, playerTrumps2.Count); i++)
            {
                if (_cardsImportance[playerTrumps1[i]] < _cardsImportance[playerTrumps2[i]])
                {
                    return player1;
                }
                else if (_cardsImportance[playerTrumps1[i]] > _cardsImportance[playerTrumps2[i]])
                {
                    return player2;
                }
            }

            if (playerTrumps1.Count != playerTrumps2.Count)
            {
                return playerTrumps1.Count > playerTrumps2.Count ? player1 : player2;
            }
        }


        for (var i = 0; i < IPlayer.MaxNumberOfCardsInHand; i++)
        {
            if (_cardsImportance[playerCards1[i]] < _cardsImportance[playerCards1[i]])
            {
                return player1;
            }
            else if (_cardsImportance[playerCards1[i]] > _cardsImportance[playerCards1[i]])
            {
                return player2;
            }
        }

        return (new Random().Next(0, 2) == 0) ? player1 : player2;


        void ComparisonAndSwappingCards(ref Card card1, ref Card card2)
        {
            if (_cardsImportance[card1] > _cardsImportance[card2])
            {
                (card1, card2) = (card2, card1);
            }
        }
    }
}