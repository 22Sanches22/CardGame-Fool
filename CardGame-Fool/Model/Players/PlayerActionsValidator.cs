using System;
using System.Collections.Generic;

using CardGameFool.Model.Cards;

namespace CardGameFool.Model.Players;

public static class PlayerActionsValidator
{
    private static readonly Dictionary<PlayerActions, Func<ICollection<Card>, bool>> _actionValidators = new()
    {
        [PlayerActions.MakeMove] = IsValidMakeMoveAction,
        [PlayerActions.BeatCard] = IsValidBeatCardAction,
        [PlayerActions.TakeCard] = IsValidTakeCardAction,
        [PlayerActions.DiscardCards] = IsValidDiscardCardsAction
    };

    public static bool IsValidAction(PlayerActions action, ICollection<Card> playerCards)
    {
        return _actionValidators[action](playerCards);
    }

    private static bool IsValidMakeMoveAction(ICollection<Card> playerCards)
    {
        return !(playerCards.Count < 1);
    }

    private static bool IsValidBeatCardAction(ICollection<Card> playerCards)
    {
        return !(playerCards.Count < 1);
    }

    private static bool IsValidTakeCardAction(ICollection<Card> playerCards)
    {
        return true;
    }

    private static bool IsValidDiscardCardsAction(ICollection<Card> playerCards)
    {
        return true;
    }
}