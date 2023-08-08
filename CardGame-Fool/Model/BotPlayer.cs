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

    public override void MakeMove(Card card)
    {
        if (!_cards.Contains(card))
        {
            throw new ArgumentException("The requested card is not in the player's hand.");
        }

        _cards.Remove(card);

        MakeMoved?.Invoke();
    }

    public override void BeatCard(Card card)
    {
        if (!_cards.Contains(card))
        {
            throw new ArgumentException("The requested card is not in the player's hand.");
        }

        _cards.Remove(card);

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