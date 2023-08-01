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

        IPlayer firstPlayer = IdentifyFirstPlayer();
        IPlayer secondPlayer = (firstPlayer == _player1) ? _player2 : _player1;

        GameResults? gameResult = null;
        
        List<Card> cardsOnTable = new(IPlayer.MaxCardsCount * 2);

        while (gameResult is null)
        {
            FirstPlayerMove();

            PlayerActions chosenActionSecondPlayer = secondPlayer.WaitСhoiceAction();
            
            if (chosenActionSecondPlayer == PlayerActions.TakeCards)
            {
                secondPlayer.TakeCards(cardsOnTable);

                PlayerActions chosenActionFirstPlayer = firstPlayer.WaitСhoiceAction();

                while (chosenActionFirstPlayer == PlayerActions.MakeMove)
                {
                    FirstPlayerMove();

                    chosenActionFirstPlayer = firstPlayer.WaitСhoiceAction();
                }

                gameResult = TryGetGameResult(firstPlayer, secondPlayer);

                continue;
            }

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

        
        void FirstPlayerMove()
        {
            Card cardToMakeMoveFirstPlayer = firstPlayer.WaitCardChoiceToMakeMove();
            firstPlayer.MakeMove(cardToMakeMoveFirstPlayer);

            cardsOnTable.Add(cardToMakeMoveFirstPlayer);
        }

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
    private IPlayer IdentifyFirstPlayer()
    {
        IPlayer? trumpsCompareResult = ComparePlayersTrumps();

        if (trumpsCompareResult is not null)
        {
            return trumpsCompareResult;
        }

        return ComparePlayersAllCards(_player1, _player2);
        

        IPlayer? ComparePlayersTrumps()
        {
            Card[] player1Trumps = _player1.GetTrumpCards(_cardsDeck.TrumpSuit);
            Card[] player2Trumps = _player2.GetTrumpCards(_cardsDeck.TrumpSuit);

            if ((player1Trumps.Length + player2Trumps.Length) > 0)
            {
                if (player1Trumps.Length == 0)
                {
                    return _player2;
                }
                else if (player2Trumps.Length == 0)
                {
                    return _player1;
                }

                return _cardsDeck.CardsImportance[player1Trumps[0]] < _cardsDeck.CardsImportance[player2Trumps[0]]
                       ? _player1
                       : _player2;
            }

            return null;
        }

        // Returns the player based on the results of the comparison.
        IPlayer ComparePlayersAllCards(IPlayer player1, IPlayer player2)
        {
            Card[] sortedPlayer1Cards = _cardsDeck.SortCardsCollection(player1.Cards);
            Card[] sortedPlayer2Cards = _cardsDeck.SortCardsCollection(player2.Cards);

            for (int i = 0; i < Math.Min(sortedPlayer1Cards.Length, sortedPlayer2Cards.Length); i++)
            {
                if (_cardsDeck.CardsImportance[sortedPlayer1Cards[i]]
                    < _cardsDeck.CardsImportance[sortedPlayer2Cards[i]])
                {
                    return player1;
                }
                else if (_cardsDeck.CardsImportance[sortedPlayer1Cards[i]]
                    > _cardsDeck.CardsImportance[sortedPlayer2Cards[i]])
                {
                    return player2;
                }
            }

            return new Random().Next(2) == 0 ? player1: player2;
        }
    }
}

public enum GameResults
{
    WinnerPlayer1,
    WinnerPlayer2,
    Draw
}