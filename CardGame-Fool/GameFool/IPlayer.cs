using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame_Fool.GameFool;

public interface IPlayer
{
    public const int MaxCardsCount = 6;

    public event Action? TakedСardsFromDeck;
    public event Action? MakeMoved;
    public event Action? BeatedCard;
    public event Action? TakedCards;

    public string Name { get; }

    public ReadOnlyCollection<Card> Cards { get; }
    public int CardsCount { get; }

    public void SetAction(PlayerActions action);
    public PlayerActions WaitСhoiceAction();

    public void TakeСardsFromDeck(Deck cardsDeck, int cardsCount);
    public void MakeMove(Card cardToMove);
    public void BeatCard(Card cardToBeat);
    public void TakeCards(IEnumerable<Card> сards);

    public Card WaitCardChoiceToMakeMove();
    public Card WaitCardChoiceToBeatCard();
}

public enum PlayerActions
{
    MakeMove,
    BeatCard,
    TakeCards
}