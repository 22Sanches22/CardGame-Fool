using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CardGame_Fool.GameFool;

public class Fool
{
    private const int s_suitsCount = 4;
    private const int s_ranksCount = 9;
    private const int s_cardsCount = s_suitsCount * s_ranksCount;
    public static readonly Dictionary<Suits, char> s_suitSymbols = new()
    {
        [Suits.Spades] = '♤',
        [Suits.Diamonds] = '♢',
        [Suits.Clubs] = '♧',
        [Suits.Hearts] = '♡'
    };

    public readonly Card[] _cardsDeck = new Card[s_cardsCount];
    private Card _trumpCard;
    private readonly Dictionary<Card, int> _cardsImportance = new(s_cardsCount);

    public Fool()
    {
        FillDeck();

        CalculateCardsImportance();   
    }

    public IPlayer StartGame(IPlayer player1, IPlayer player2)
    {
        ShuffleDeck();

        _trumpCard = _cardsDeck[0];

        Stack<Card> cardsDeck = new(_cardsDeck);

        player1.TakeСardsFromDeck(cardsDeck, IPlayer.MaxCardsCount);
        player2.TakeСardsFromDeck(cardsDeck, IPlayer.MaxCardsCount);

        IPlayer firstPlayer = DefinitionFirstPlayer(player1, player2);
        IPlayer secondPlayer = (firstPlayer == player1) ? player2 : player1;

        IPlayer? winner = null;

        while (winner is null)
        {
            if (PlayerTurn(firstPlayer, secondPlayer))
            {
                continue;
            }

        SecondPlayerTurn:

            if (PlayerTurn(secondPlayer, firstPlayer))
            {
                goto SecondPlayerTurn;
            }
        }

        return winner;

        // Returns true if player2 has taken the cards and player1 can repeat the move.
        bool PlayerTurn(IPlayer player1, IPlayer player2)
        {
            player1.MakeMove();

            PlayerActions waitingResult = player2.WaitingСhoice();

            if (waitingResult == PlayerActions.TakeCards)
            {
                player2.TakeCards();

                return true;
            }
            else if (waitingResult == PlayerActions.BeatCard)
            {
                player2.BeatCard();

                player1.TakeСardsFromDeck(cardsDeck, (IPlayer.MaxCardsCount - player1.CardsCount).ToUint());
                player2.TakeСardsFromDeck(cardsDeck, (IPlayer.MaxCardsCount - player2.CardsCount).ToUint());
                
                TryGetWinner();
            }

            return false;
        }

        void TryGetWinner()
        {
            if ((firstPlayer.CardsCount == 0) || (secondPlayer.CardsCount == 0))
            {
                if (firstPlayer.CardsCount == secondPlayer.CardsCount)
                {
                    winner = new BotPlayer("Draw");
                }
                else if (firstPlayer.CardsCount == 0)
                {
                    winner = player1;
                }
                else if (firstPlayer.CardsCount == 0)
                {
                    winner = player2;
                }
            }
        }
    }

    private void FillDeck()
    {
        var cardNumber = 0;

        for (var i = 0; i < s_suitsCount; i++)
        {
            for (var j = 0; j < s_ranksCount; j++)
            {
                // The initial rank in the "Ranks" enum is six.
                _cardsDeck[cardNumber++] = new Card((Suits)i, j + Ranks.Six);
            }
        }
    }

    private void ShuffleDeck()
    {
        Random random = new();

        for (var i = 0; i < s_cardsCount; i++)
        {
            int index1 = random.Next(0, s_cardsCount);
            int index2 = random.Next(0, s_cardsCount);

            (_cardsDeck[index1], _cardsDeck[index2]) = (_cardsDeck[index2], _cardsDeck[index1]);
        }
    }

    private void CalculateCardsImportance()
    {
        foreach (Card card in _cardsDeck)
        {
            _cardsImportance.Add(card, (int)card.Rank + ((card.Suit == _trumpCard.Suit) ? 10 : 0));
        }
    }

    private IPlayer DefinitionFirstPlayer(IPlayer player1, IPlayer player2)
    {
        Card[] playerCards1 = player1.GetCards().ToArray();
        Card[] playerCards2 = player2.GetCards().ToArray();

        for (var i = 0; i < IPlayer.MaxCardsCount; i++)
        {
            for (var j = 0; j < IPlayer.MaxCardsCount - 1; j++)
            {
                CompareAndSwappingCards(ref playerCards1[j], ref playerCards1[j + 1]);
                CompareAndSwappingCards(ref playerCards2[j], ref playerCards2[j + 1]);
            }
        }


        List<Card> playerTrumps1 = new();
        List<Card> playerTrumps2 = new();

        // Search trump cards in players.
        for (var i = 0; i < IPlayer.MaxCardsCount; i++)
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
            IPlayer? trumpsCardComparisonResult =
                ComparePlayersCards(playerTrumps1.ToArray(), playerTrumps2.ToArray());

            if (trumpsCardComparisonResult is not null)
            {
                return trumpsCardComparisonResult;
            }

            return playerTrumps1.Count > playerTrumps2.Count ? player1 : player2;
        }


        IPlayer? allCardComparisonResult = ComparePlayersCards(playerCards1, playerCards2);

        if (allCardComparisonResult is not null)
        {
            return allCardComparisonResult;
        }

        return (new Random().Next(0, 2) == 0) ? player1 : player2;


        void CompareAndSwappingCards(ref Card card1, ref Card card2)
        {
            if (_cardsImportance[card1] > _cardsImportance[card2])
            {
                (card1, card2) = (card2, card1);
            }
        }

        IPlayer? ComparePlayersCards(Card[] collection1, Card[] collection2)
        {
            for (var i = 0; i < Math.Min(collection1.Length, collection2.Length); i++)
            {
                if (_cardsImportance[collection1[i]] < _cardsImportance[collection2[i]])
                {
                    return player1;
                }
                else if (_cardsImportance[collection1[i]] > _cardsImportance[collection2[i]])
                {
                    return player2;
                }
            }

            return null;
        }
    }
}