using System;
using System.Threading.Tasks;

using CardGameFool.Model.Cards;

namespace CardGameFool.Model.Players;

public class LivePlayer : Player
{
    public LivePlayer(string name) : base(name, PlayerTypes.Live)
    { }

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

    public override async Task<Card> AsyncWaitCardChoice()
    {
        while (_chosenCard is null)
            await Task.Delay(1);

        Card returnCard = (Card)_chosenCard;

        _chosenCard = null;
        return returnCard;
    }
}