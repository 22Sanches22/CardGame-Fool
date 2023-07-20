using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame_Fool.GameFool;

internal delegate void EventHandlerPlayer();

internal interface IPlayer
{
    public const int MaximumNumberOfCardsInHand = 6;

    public event EventHandlerPlayer? TakedСardsFromDeck;
    public event EventHandlerPlayer? MakeMoved;
    public event EventHandlerPlayer? BeatedCard;
    public event EventHandlerPlayer? TakedEnemyCards;

    public int NumberOfCardsInHand { get; }
    public string PlayerName { get; }

    public void TakeСardsFromDeck(Stack<Card> cardsDeck, int numberOfCards);
    public void MakeMove(Card cardToMove);
    public void BeatCard(Card cardToBeat);
    public void TakeEnemyCards(IEnumerable<Card> enemyCards);
}