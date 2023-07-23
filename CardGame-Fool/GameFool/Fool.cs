using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CardGame_Fool.GameFool;

/// <summary>
/// It is a model of the card game "Fool",
/// with the ability to launch the game process multiple times if there is 1 instance of the class.
/// </summary>
public class Fool
{
    private const int s_suitsCount = 4;
    private const int s_ranksCount = 9;
    private const int s_cardsCount = s_suitsCount * s_ranksCount;

    private readonly IPlayer _player1;
    private readonly IPlayer _player2;

    public readonly Stack<Card> _cardsDeck;
    private readonly Dictionary<Card, int> _cardsImportance = new(s_cardsCount);

    private readonly Card _trumpCard;

    public Fool(IPlayer player1, IPlayer player2)
    {
        _player1 = player1;
        _player2 = player2;

        Card[] cardsDeck = GenerateDeck();
        ShuffleDeck(cardsDeck);

        _cardsDeck = new Stack<Card>(cardsDeck);

        _trumpCard = cardsDeck[0];

        CalculateCardsImportance();
    }

    /// <summary> Starts the game and returns the winner at the end. </summary>
    public IPlayer StartGame()
    {
        _player1.TakeСardsFromDeck(_cardsDeck, IPlayer.MaxCardsCount);
        _player2.TakeСardsFromDeck(_cardsDeck, IPlayer.MaxCardsCount);

        IPlayer firstPlayer = DefinitionFirstPlayer(_player1, _player2);
        IPlayer secondPlayer = (firstPlayer == _player1) ? _player2 : _player1;

        IPlayer? winner = null;

        while (winner is null)
        {
            if (PlayerTurn(firstPlayer, secondPlayer))
            {
                winner = TryGetWinner(firstPlayer, secondPlayer);

                if (winner is not null)
                {
                    return winner;
                }

                continue;
            }

            winner = TryGetWinner(firstPlayer, secondPlayer);

            if (winner is not null)
            {
                return winner;
            }

        SecondPlayerTurn:

            if (PlayerTurn(secondPlayer, firstPlayer))
            {
                winner = TryGetWinner(secondPlayer, firstPlayer);

                if (winner is not null)
                {
                    return winner;
                }

                goto SecondPlayerTurn;
            }

            winner = TryGetWinner(secondPlayer, firstPlayer);
        }
        
        return winner;
    }

    private static Card[] GenerateDeck()
    {
        var cardsDeck = new Card[s_cardsCount];

        var cardNumber = 0;

        for (var i = 0; i < s_suitsCount; i++)
        {
            for (var j = 0; j < s_ranksCount; j++)
            {
                // The initial rank in the "Ranks" enum is six.
                cardsDeck[cardNumber++] = new Card((Suits)i, j + Ranks.Six);
            }
        }

        return cardsDeck;
    }

    private static void ShuffleDeck(Card[] cardsDeck)
    {
        Random random = new();

        for (var i = 0; i < s_cardsCount; i++)
        {
            int index1 = random.Next(0, s_cardsCount);
            int index2 = random.Next(0, s_cardsCount);

            (cardsDeck[index1], cardsDeck[index2]) = (cardsDeck[index2], cardsDeck[index1]);
        }
    }

    private static IPlayer? TryGetWinner(IPlayer player1, IPlayer player2)
    {
        if ((player1.CardsCount == 0) || (player2.CardsCount == 0))
        {
            if (player1.CardsCount == player2.CardsCount)
            {
                return new BotPlayer("Draw");
            }
            else if (player1.CardsCount == 0)
            {
                return player1;
            }
            else if (player2.CardsCount == 0)
            {
                return player2;
            }
        }

        return null;
    }

    /// <summary> Returns true if player2 has taken the cards and player1 can repeat the move. </summary>
    private bool PlayerTurn(IPlayer player1, IPlayer player2)
    {
        player1.MakeMove();

        PlayerActions waitingResult = player2.WaitСhoiceAction();

        if (waitingResult == PlayerActions.TakeCards)
        {
            player2.TakeCards();

            return true;
        }

        // If waitingResult == PlayerActions.BeatCard.
        player2.BeatCard();

        player1.TakeСardsFromDeck(_cardsDeck, (IPlayer.MaxCardsCount - player1.CardsCount).ToUint());
        player2.TakeСardsFromDeck(_cardsDeck, (IPlayer.MaxCardsCount - player2.CardsCount).ToUint());

        return false;
    }

    private void CalculateCardsImportance()
    {
        _cardsImportance.Clear();

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