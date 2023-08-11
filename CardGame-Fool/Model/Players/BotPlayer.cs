using System;
using System.Threading.Tasks;

using CardGameFool.Model.Cards;

namespace CardGameFool.Model.Players;

public class BotPlayer : Player
{
    public BotPlayer(string name) : base(name, PlayerTypes.Bot)
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
        Card returnCard = new Card();

        _chosenCard = null;

        return returnCard;
    }
}