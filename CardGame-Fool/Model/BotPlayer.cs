using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CardGameFool.Model;

internal class BotPlayer : Player
{
    public BotPlayer(string name) : base(name) { }

    public override event Action? MakeMoved;
    public override event Action? BeatedCard;

    public override void MakeMove(Card cardToMove)
    {
        if (!_cards.Contains(cardToMove))
        {
            throw new ArgumentException("The requested card is not in the player's hand.");
        }

        _cards.Remove(cardToMove);

        MakeMoved?.Invoke();
    }

    public override void BeatCard(Card cardToBeat)
    {
        if (!_cards.Contains(cardToBeat))
        {
            throw new ArgumentException("The requested card is not in the player's hand.");
        }

        _cards.Remove(cardToBeat);

        BeatedCard?.Invoke();
    }

    public override Card WaitCardChoiceToMakeMove()
    {
        return new Card(Ranks.Seven, Suits.Clubs, Suits.Clubs);
    }

    public override Card WaitCardChoiceToBeatCard()
    {
        return new Card(Ranks.Seven, Suits.Clubs, Suits.Clubs);
    }
}