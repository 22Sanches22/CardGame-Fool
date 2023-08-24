using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CardGameFool.Model.Cards;

namespace CardGameFool.Model.Players;

public abstract class Player
{
    /// <summary> The maximum count of cards in a player's hands. </summary>
    public const int DefaultCardsCount = 6;

    protected readonly List<Card> _cards = new(DefaultCardsCount);

    public Player(string name)
    {
        Name = name;
    }

    public Card[] Cards => _cards.ToArray();
    public int CardsCount => _cards.Count;

    public event Action<Player, IEnumerable<Card>>? TakedCards;
    public event Action<Player, Card>? MakeMoved;
    public event Action<Player, Card>? BeatedCard;

    public string Name { get; }

    public Card[] GetTrumpCards()
    {
        return _cards.Where(card => card.IsTrump).ToArray();
    }

    public Card[] GetSortedCards()
    {
        return _cards.Order().ToArray();
    }

    public void TakeСardsFromDeck(Deck deck, int cardsCount)
    {
        if (cardsCount < 0 || cardsCount > DefaultCardsCount)
        {
            throw new InvalidOperationException("The number of cards requested is less than zero " +
                "or greater than the maximum allowed.");
        }

        // If there are not enough cards in the deck, he takes all that is.
        if (cardsCount > deck.Count)
        {
            cardsCount = deck.Count;
        }

        Card[] addedCards = new Card[cardsCount];

        for (int i = 0; i < cardsCount; i++)
        {
            Card card = deck.GetTopCard();

            _cards.Add(card);
            addedCards[i] = card;
        }

        TakedCards?.Invoke(this, addedCards);
    }

    public void TakeCards(IEnumerable<Card> cards)
    {
        _cards.AddRange(cards);

        TakedCards?.Invoke(this, cards);
    }

    public abstract Task<Card> AsyncWaitChoiceCard();
    public abstract Task<PlayerActions> AsyncWaitActionСhoice(PlayerActions[] allowedActions);

    public void MakeMove(Card card)
    {
        RemoveCard(card);

        MakeMoved?.Invoke(this, card);
    }

    public void BeatCard(Card card)
    {
        RemoveCard(card);

        BeatedCard?.Invoke(this, card);
    }

    private void RemoveCard(Card card)
    {
        if (!_cards.Contains(card))
        {
            throw new ArgumentException("The requested card is not in the player's hands.");
        }

        _cards.Remove(card);
    }
}