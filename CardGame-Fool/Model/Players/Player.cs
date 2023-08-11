using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CardGameFool.Model.Cards;

namespace CardGameFool.Model.Players;

public abstract class Player
{
    /// <summary> The maximum count of cards in a player's hands. </summary>
    public const int MaxCardsCount = 6;

    protected readonly List<Card> _cards = new(MaxCardsCount);

    protected Card? _chosenCard = null;
    private PlayerActions? _chosenAction = null;

    public Player(string name, PlayerTypes type)
    {
        Name = name;
        Type = type;
    }

    public Card[] Cards => _cards.ToArray();
    public int CardsCount => _cards.Count;

    public event Action<Player>? TakedСardsFromDeck;
    public event Action? TakedCards;

    public abstract event Action? MakeMoved;
    public abstract event Action? BeatedCard;

    public string Name { get; }
    public PlayerTypes Type { get; }

    public Card[] GetTrumpCards()
    {
        return _cards.Where(card => card.IsTrump).ToArray();
    }

    public Card[] GetSortedCards()
    {
        return _cards.OrderBy(card => card.Importance).ToArray();
    }

    public void SetChosenCard(Card card)
    {
        _chosenCard = card;
    }

    public void SetChosenAction(PlayerActions action)
    {
        _chosenAction = action;
    }

    public PlayerActions WaitСhoiceAction()
    {
        while (_chosenAction is null)
            ;

        PlayerActions returnedValue = (PlayerActions)_chosenAction;
        _chosenAction = null;

        return returnedValue;
    }

    public void TakeСardsFromDeck(Deck deck, int cardsCount)
    {
        if (cardsCount < 0 || cardsCount > MaxCardsCount)
        {
            throw new InvalidOperationException("The number of cards requested is less than zero " +
                "or greater than the maximum allowed.");
        }

        // If there are not enough cards in the deck, he takes all that is.
        if (cardsCount > deck.Count)
        {
            cardsCount = deck.Count;
        }

        for (int i = 0; i < cardsCount; i++)
        {
            _cards.Add(deck.GetTopCard());
        }

        TakedСardsFromDeck?.Invoke(this);
    }

    public void TakeCards(IEnumerable<Card> сards)
    {
        _cards.AddRange(_cards);

        TakedCards?.Invoke();
    }

    public abstract void MakeMove(Card card);
    public abstract void BeatCard(Card card);

    public abstract Task<Card> AsyncWaitCardChoice();
}
