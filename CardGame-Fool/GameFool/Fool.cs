using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CardGame_Fool.GameFool;

/// <summary> It is a model of the card game "Fool". </summary>
public class Fool
{
    private readonly IPlayer _player1;
    private readonly IPlayer _player2;

    private readonly Deck _cardsDeck = new();

    public Fool(IPlayer player1, IPlayer player2)
    {
        _player1 = player1;
        _player2 = player2;
    }

    /// <returns> Winner or draw. </returns>
    public GameResults StartGame()
    {
        _player1.TakeСardsFromDeck(_cardsDeck, IPlayer.MaxCardsCount);
        _player2.TakeСardsFromDeck(_cardsDeck, IPlayer.MaxCardsCount);

        IPlayer firstPlayer = IdentifyFirstPlayer(_player1, _player2);
        IPlayer secondPlayer = (firstPlayer == _player1) ? _player2 : _player1;

        GameResults? gameResult = null;
        
        List<Card> cardsOnTable = new(IPlayer.MaxCardsCount * 2);

        while (gameResult is null)
        {
            Card cardToMakeMoveFirstPlayer = firstPlayer.WaitCardChoiceToMakeMove();
            firstPlayer.MakeMove(cardToMakeMoveFirstPlayer);

            cardsOnTable.Add(cardToMakeMoveFirstPlayer);

            PlayerActions chosenActionSecondPlayer = secondPlayer.WaitСhoiceAction();
            
            if (chosenActionSecondPlayer == PlayerActions.TakeCards)
            {
                secondPlayer.TakeCards(cardsOnTable);

                PlayerActions chosenActionFirstPlayer = firstPlayer.WaitСhoiceAction();

                while (chosenActionFirstPlayer == PlayerActions.MakeMove)
                {
                    cardToMakeMoveFirstPlayer = firstPlayer.WaitCardChoiceToMakeMove();
                    firstPlayer.MakeMove(cardToMakeMoveFirstPlayer);
                      
                    cardsOnTable.Add(cardToMakeMoveFirstPlayer);

                    chosenActionFirstPlayer = firstPlayer.WaitСhoiceAction();
                }

                gameResult = TryGetGameResult(firstPlayer, secondPlayer);

                continue;
            }

            // If waitingResult == PlayerActions.BeatCard.

            Card cardSecondPlayerToBeatCard = secondPlayer.WaitCardChoiceToBeatCard();
            secondPlayer.BeatCard(cardSecondPlayerToBeatCard);

            cardsOnTable.Clear();

            gameResult = TryGetGameResult(firstPlayer, secondPlayer);

            // Exchange of roles, now the one who made the move beat сards.
            (firstPlayer, secondPlayer) = (secondPlayer, firstPlayer);

            ReplenishCards(_player1);
            ReplenishCards(_player2);
        }

        return (GameResults)gameResult;


        void ReplenishCards(IPlayer player)
        {
            if ((player.CardsCount > 0) && (player.CardsCount < IPlayer.MaxCardsCount))
            {
                player.TakeСardsFromDeck(_cardsDeck, IPlayer.MaxCardsCount - player.CardsCount);
            }
        }
    }

    /// <returns> Draw if both players have no cards or winner if there is one, otherwise null. </returns>
    private static GameResults? TryGetGameResult(IPlayer player1, IPlayer player2)
    {
        if ((player1.CardsCount == 0) || (player2.CardsCount == 0))
        {
            if (player1.CardsCount == player2.CardsCount)
            {
                return GameResults.Draw;
            }
            else if (player1.CardsCount == 0)
            {
                return GameResults.WinnerPlayer1;
            }
            else if (player2.CardsCount == 0)
            {
                return GameResults.WinnerPlayer2;
            }
        }

        return null;
    }

    /// <returns> The player make move first. </returns>
    private IPlayer IdentifyFirstPlayer(IPlayer player1, IPlayer player2)
    {
        Card[] player1Cards = SortPlayerCards(player1.Cards);
        Card[] player2Cards = SortPlayerCards(player2.Cards);

        Card[] player1Trumps = SearchTrumpCardsPlayer(player1Cards);
        Card[] player2Trumps = SearchTrumpCardsPlayer(player2Cards);


        if ((player1Trumps.Length + player2Trumps.Length) > 0)
        {
            if (player1Trumps.Length == 0)
            {
                return player2;
            }
            else if (player2Trumps.Length == 0)
            {
                return player1;
            }

            return (_cardsDeck.CardsImportance[player1Trumps[0]] < _cardsDeck.CardsImportance[player2Trumps[0]]
                ? player1
                : player2);
        }

        return ComparePlayersCards(player1Cards, player2Cards);


        Card[] SortPlayerCards(ReadOnlyCollection<Card> playerCards)
        {
            Card[] cards = playerCards.ToArray();

            for (int i = 0; i < cards.Length; i++)
            {
                for (int j = 0; j < cards.Length - 1; j++)
                {
                    if (_cardsDeck.CardsImportance[cards[j]] > _cardsDeck.CardsImportance[cards[j + 1]])
                    {
                        (cards[j], cards[j + 1]) = (cards[j + 1], cards[j]);
                    }
                }
            }

            return cards;
        }

        Card[] SearchTrumpCardsPlayer(Card[] cards)
        {
            List<Card> trumpCards = new();

            foreach (Card card in cards)
            {
                if (card.Suit == _cardsDeck.TrumpSuit)
                {
                    trumpCards.Add(card);
                }
            }

            return trumpCards.ToArray();
        }

        // Returns the player based on the results of the comparison.
        IPlayer ComparePlayersCards(Card[] cards1, Card[] cards2)
        {
            for (int i = 0; i < Math.Min(cards1.Length, cards2.Length); i++)
            {
                if (_cardsDeck.CardsImportance[cards1[i]] < _cardsDeck.CardsImportance[cards2[i]])
                {
                    return player1;
                }
                else if (_cardsDeck.CardsImportance[cards1[i]] > _cardsDeck.CardsImportance[cards2[i]])
                {
                    return player2;
                }
            }

            // Unreachable but necessary code.
            return player1;
        }
    }
}

public enum GameResults
{
    WinnerPlayer1,
    WinnerPlayer2,
    Draw
}