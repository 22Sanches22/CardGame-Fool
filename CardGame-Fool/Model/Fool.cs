using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CardGameFool.Model.Cards;
using CardGameFool.Model.Players;

namespace CardGameFool.Model;

/// <summary> It is a model of the card game "Fool". </summary>
public class Fool
{
    private readonly Player _player1;
    private readonly Player _player2;

    private readonly Deck _deck;

    public Fool(Player player1, Player player2, Deck deck)
    {
        _player1 = player1;
        _player2 = player2;

        _deck = deck;
    }

    public event Action<Player>? IdentifiedFirstPlayer;

    /// <returns> Winner or draw. </returns>
    public GameResults StartGame()
    {
        _player1.TakeСardsFromDeck(_deck, Player.MaxCardsCount);
        _player2.TakeСardsFromDeck(_deck, Player.MaxCardsCount);

        Player firstPlayer = IdentifyFirstPlayer();
        Player secondPlayer = (firstPlayer == _player1) ? _player2 : _player1;

        GameResults? gameResult = null;

        List<Card> cardsOnTable = new(Player.MaxCardsCount * 2);


        while (gameResult is null)
        {
            FirstPlayerMove();

            return GameResults.WinnerPlayer1;

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

                TryDetermineGameResult();

                continue;
            }

            Card cardSecondPlayerToBeatCard = secondPlayer.AsyncWaitCardChoice().Result;
            secondPlayer.BeatCard(cardSecondPlayerToBeatCard);

            cardsOnTable.Clear();

            TryDetermineGameResult();

            ReplenishCardsPlayer(firstPlayer);
            ReplenishCardsPlayer(secondPlayer);

            // Exchange of roles, now the one who made the move beat сards.
            (firstPlayer, secondPlayer) = (secondPlayer, firstPlayer);
        }

        return (GameResults)gameResult;


        void FirstPlayerMove()
        {
            //Task<Card> awaitedResult = firstPlayer.AsyncWaitCardChoice();
            //Card cardToMakeMove = awaitedResult.Result;
            return;
            //firstPlayer.MakeMove(cardToMakeMove);

            //cardsOnTable.Add(cardToMakeMove);
        }

        // Determines the result of the game if the deck is empty. 
        void TryDetermineGameResult()
        {
            if (_deck.Count > 0)
            {
                return;
            }

            if ((_player1.CardsCount + _player2.CardsCount) == 0)
            {
                gameResult = GameResults.Draw;
            }
            else if (_player1.CardsCount == 0)
            {
                gameResult = GameResults.WinnerPlayer1;
            }
            else if (_player2.CardsCount == 0)
            {
                gameResult = GameResults.WinnerPlayer2;
            }
        }

        void ReplenishCardsPlayer(Player player)
        {
            if ((player.CardsCount > 0) && (player.CardsCount < Player.MaxCardsCount))
            {
                player.TakeСardsFromDeck(_deck, Player.MaxCardsCount - player.CardsCount);
            }
        }

    }

    /// <returns> The player make move first. </returns>
    private Player IdentifyFirstPlayer()
    {
        Player? firstPlayer = ComparePlayersTrumps();

        if (firstPlayer is not null)
        {
            IdentifiedFirstPlayer?.Invoke(firstPlayer);

            return firstPlayer;
        }

        firstPlayer = ComparePlayersAllCards();

        IdentifiedFirstPlayer?.Invoke(firstPlayer);

        return firstPlayer;


        // Returns the player based on the results of the comparison trump cards.
        Player? ComparePlayersTrumps()
        {
            Card[] trumps1 = _player1.GetTrumpCards();
            Card[] trumps2 = _player2.GetTrumpCards();

            if ((trumps1.Length + trumps2.Length) > 0)
            {
                if (trumps1.Length == 0)
                {
                    return _player2;
                }
                else if (trumps2.Length == 0)
                {
                    return _player1;
                }

                return trumps1[0].Importance < trumps2[0].Importance ? _player1 : _player2;
            }

            return null;
        }

        // Returns the player based on the results of the comparison all cards.
        Player ComparePlayersAllCards()
        {
            Card[] sortedCards1 = _player1.GetSortedCards();
            Card[] sortedCards2 = _player2.GetSortedCards();

            for (int i = 0; i < Math.Min(sortedCards1.Length, sortedCards2.Length); i++)
            {
                if (sortedCards1[i].Importance < sortedCards2[i].Importance)
                {
                    return _player1;
                }
                else if (sortedCards1[i].Importance > sortedCards2[i].Importance)
                {
                    return _player2;
                }
            }

            return new Random().Next(2) == 0 ? _player1 : _player2;
        }
    }
}