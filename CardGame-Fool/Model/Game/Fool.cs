using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CardGameFool.Model.Cards;
using CardGameFool.Model.Players;

namespace CardGameFool.Model.Game;

/// <summary> It is a model of the card game "Fool". </summary>
public class Fool
{
    public const int PlayersNumber = 2;

    private static readonly PlayerActions[] _possibleActionsFirstPlayer =
        { PlayerActions.MakeMove, PlayerActions.DiscardCards };
    private static readonly PlayerActions[] _possibleActionsSecondPlayer =
        { PlayerActions.BeatCard, PlayerActions.TakeCard };

    private readonly Player _player1;
    private readonly Player _player2;

    private readonly Deck _deck;
    private readonly CardsTable _cardsTable;

    public Fool(Player player1, Player player2, Deck deck, CardsTable cardsTable)
    {
        _player1 = player1;
        _player2 = player2;

        _deck = deck;
        _cardsTable = cardsTable;
    }

    public event Action<Player>? ChangeActivePlayer;

    /// <returns> Winner or draw. </returns>
    public async Task<Results> AsyncStartGame()
    {
        ReplenishCardsPlayer(_player1);
        ReplenishCardsPlayer(_player2);

        Player firstPlayer = IdentifyFirstPlayer();
        Player secondPlayer = firstPlayer == _player1 ? _player2 : _player1;

        Results? gameResult = null;

        while (gameResult is null)
        {
            await PerformActionWithCard(firstPlayer, PlayerActions.MakeMove);

            PlayerActions chosenActionSecondPlayer = await GetChosenAction(secondPlayer, _possibleActionsSecondPlayer);

            if (!_possibleActionsSecondPlayer.Contains(chosenActionSecondPlayer))
            {
                throw new InvalidOperationException("Incorrect action in this context.");
            }
            else if (chosenActionSecondPlayer == PlayerActions.TakeCard)
            {
                secondPlayer.TakeCards(_cardsTable.Cards);
                _cardsTable.Clear();

                ReplenishCardsPlayer(firstPlayer);

                TryDetermineGameResult();

                continue;
            }

            await PerformActionWithCard(secondPlayer, PlayerActions.BeatCard);

            PlayerActions chosenActionFirstPlayer = await GetChosenAction(firstPlayer, _possibleActionsFirstPlayer);

            if (!_possibleActionsFirstPlayer.Contains(chosenActionFirstPlayer))
            {
                throw new InvalidOperationException("Incorrect action in this context.");
            }
            else if (chosenActionFirstPlayer == PlayerActions.MakeMove)
            {
                TryDetermineGameResult();

                continue;
            }

            _cardsTable.Clear();

            ReplenishCardsPlayer(firstPlayer);
            ReplenishCardsPlayer(secondPlayer);

            TryDetermineGameResult();

            // Exchange of roles, now the one who maked move, beat сards.
            (firstPlayer, secondPlayer) = (secondPlayer, firstPlayer);

            await Task.Delay(400);
        }

        return (Results)gameResult;


        async Task PerformActionWithCard(Player player, PlayerActions action)
        {
            ChangeActivePlayer?.Invoke(player);

            Card cardToAction = await player.AsyncWaitCardChoice();

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

            _cardsTable.AddCard(cardToAction, action);

            await Task.Delay(500);
        }

        // Determines the result of the game if the deck is empty.
        void TryDetermineGameResult()
        {
            if (_deck.Count > 0)
            {
                return;
            }

            if (_player1.CardsCount + _player2.CardsCount == 0)
            {
                gameResult = Results.Draw;
            }
            else if (_player1.CardsCount == 0)
            {
                gameResult = Results.WinnerPlayer1;
            }
            else if (_player2.CardsCount == 0)
            {
                gameResult = Results.WinnerPlayer2;
            }
        }
    }

    /// <returns> The player make move first. </returns>
    private Player IdentifyFirstPlayer()
    {
        Player? firstPlayer = ComparePlayersTrumps();

        if (firstPlayer is not null)
        {
            ChangeActivePlayer?.Invoke(firstPlayer);

            return firstPlayer;
        }

        firstPlayer = ComparePlayersAllCards();
        ChangeActivePlayer?.Invoke(firstPlayer);

        return firstPlayer;


        // Returns the player based on the results of the comparison trump cards.
        Player? ComparePlayersTrumps()
        {
            Card[] trumps1 = _player1.GetTrumpCards();
            Card[] trumps2 = _player2.GetTrumpCards();

            if (trumps1.Length + trumps2.Length > 0)
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

    private async Task<PlayerActions> GetChosenAction(Player player, IEnumerable<PlayerActions> possibleActions)
    {
        PlayerActions[] allowedActionsFirstPlayer = possibleActions
            .Where(action => PlayerActionsValidator.IsValidAction(action, player.Cards))
            .ToArray();

        ChangeActivePlayer?.Invoke(player);

        return await player.AsyncWaitActionСhoice(allowedActionsFirstPlayer);
    }

    private void ReplenishCardsPlayer(Player player)
    {
        if (player.CardsCount < Player.StartingCardsCount && _deck.Count > 0)
        {
            player.TakeСardsFromDeck(_deck, Player.StartingCardsCount - player.CardsCount);
        }
    }
}