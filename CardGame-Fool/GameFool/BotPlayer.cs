using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame_Fool.GameFool;

internal class BotPlayer : IPlayer
{
    private readonly List<Card> _cards = new(IPlayer.MaximumNumberOfCardsInHand);

    public BotPlayer(string playerName)
    {
        PlayerName = playerName;
    }

    public event EventHandlerPlayer? TakedСardsFromDeck;
    public event EventHandlerPlayer? MakeMoved;
    public event EventHandlerPlayer? BeatedCard;
    public event EventHandlerPlayer? TakedEnemyCards;

    public int NumberOfCardsInHand => _cards.Count;
    public string PlayerName { get; }  

    public void TakeСardsFromDeck(Stack<Card> cardsDeck, int numberOfCards)
    {
        if ((numberOfCards < 1)
            || (numberOfCards > IPlayer.MaximumNumberOfCardsInHand)
            || (numberOfCards > cardsDeck.Count))
        {
            throw new ArgumentException("The number of cards is incorrect.");
        }

        for (var i = 0; i < numberOfCards; i++)
        {
            _cards.Add(cardsDeck.Pop());
        }

        TakedСardsFromDeck?.Invoke();
    }

    public void MakeMove(Card cardToMove)
    {
        _cards.Remove(cardToMove);

        MakeMoved?.Invoke();
    }

    public void BeatCard(Card cardToBeat)
    {
        _cards.Remove(cardToBeat);

        BeatedCard?.Invoke();
    }

    public void TakeEnemyCards(IEnumerable<Card> enemyCards)
    {
        foreach (Card card in enemyCards)
        {
            _cards.Add(card);
        }

        TakedEnemyCards?.Invoke();
    }
}