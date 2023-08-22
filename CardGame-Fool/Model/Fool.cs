using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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

    public event Action<Player>? ChangedFirstPlayer;

    public event Action<Card, PlayerActions>? AddingСardsOnTable;
    public event Action? ClearedСardsOnTable;

    /// <returns> Winner or draw. </returns>
    public async Task<GameResults> AsyncStartGame()
    {
        _player1.TakeСardsFromDeck(_deck, Player.DefaultCardsCount);
        _player1.TakeСardsFromDeck(_deck, Player.DefaultCardsCount);
        _player1.TakeСardsFromDeck(_deck, Player.DefaultCardsCount);
        _player2.TakeСardsFromDeck(_deck, Player.DefaultCardsCount);
        _player2.TakeСardsFromDeck(_deck, Player.DefaultCardsCount);
        _player2.TakeСardsFromDeck(_deck, Player.DefaultCardsCount);

        Player firstPlayer = _player1;// IdentifyFirstPlayer();
        Player secondPlayer = (firstPlayer == _player1) ? _player2 : _player1;

        PlayerActions[] allowedActionsToFirstPlayer = new[] { PlayerActions.MakeMove, PlayerActions.DiscardCards };
        PlayerActions[] allowedActionsToSecondPlayer = new[] { PlayerActions.BeatCard, PlayerActions.TakeCard };

        GameResults? gameResult = null;

        List<Card> cardsOnTable = new(Player.DefaultCardsCount * 2);

        while (gameResult is null)
        {
            await PerformActionWithCard(firstPlayer, PlayerActions.MakeMove);

            PlayerActions chosenActionSecondPlayer =
                await secondPlayer.AsyncWaitActionСhoice(allowedActionsToSecondPlayer);

            if (!allowedActionsToSecondPlayer.Contains(chosenActionSecondPlayer))
            {
                throw new InvalidOperationException("Incorrect action in this context.");
            }
            else if (chosenActionSecondPlayer == PlayerActions.TakeCard)
            {
                secondPlayer.TakeCards(cardsOnTable);

                cardsOnTable.Clear();
                ClearedСardsOnTable?.Invoke();

                TryDetermineGameResult();

                continue;
            }

            await PerformActionWithCard(secondPlayer, PlayerActions.BeatCard);

            PlayerActions chosenActionFirstPlayer =
                await firstPlayer.AsyncWaitActionСhoice(allowedActionsToFirstPlayer);

            if (!allowedActionsToFirstPlayer.Contains(chosenActionFirstPlayer))
            {
                throw new InvalidOperationException("Incorrect action in this context.");
            }
            else if (chosenActionFirstPlayer == PlayerActions.MakeMove)
            {
                continue;
            }

            cardsOnTable.Clear();
            ClearedСardsOnTable?.Invoke();

            TryDetermineGameResult();

            ReplenishCardsPlayer(firstPlayer);
            ReplenishCardsPlayer(secondPlayer);

            // Exchange of roles, now the one who made the move beat сards.
            (firstPlayer, secondPlayer) = (secondPlayer, firstPlayer);
            ChangedFirstPlayer?.Invoke(firstPlayer);

            await Task.Delay(400);
        }

        return (GameResults)gameResult;


        async Task PerformActionWithCard(Player player, PlayerActions action)
        {
            Card cardToAction = await player.AsyncWaitChoiceCard();

            if (action == PlayerActions.MakeMove)
            {
                player.MakeMove(cardToAction);
            }
            else if (action == PlayerActions.BeatCard)
            {
                player.BeatCard(cardToAction);
            }
            else
            {
                throw new InvalidOperationException("Incorrect action in this context.");
            }

            cardsOnTable.Add(cardToAction);
            AddingСardsOnTable?.Invoke(cardToAction, action);

            await Task.Delay(500);
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
    }

    /// <returns> The player make move first. </returns>
    private Player IdentifyFirstPlayer()
    {
        Player? firstPlayer = ComparePlayersTrumps();

        if (firstPlayer is not null)
        {
            ChangedFirstPlayer?.Invoke(firstPlayer);

            return firstPlayer;
        }

        firstPlayer = ComparePlayersAllCards();
        ChangedFirstPlayer?.Invoke(firstPlayer);

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

    private void ReplenishCardsPlayer(Player player)
    {
        if ((player.CardsCount > 0) && (player.CardsCount < Player.DefaultCardsCount))
        {
            player.TakeСardsFromDeck(_deck, Player.DefaultCardsCount - player.CardsCount);
        }
    }
}