using System;
using System.Linq;
using System.Threading.Tasks;

using CardGameFool.Model.Cards;

namespace CardGameFool.Model.Players;

public class LivePlayer : Player
{
    private Card? _chosenCard = null;
    private PlayerActions? _chosenAction = null;

    public LivePlayer(string name) : base(name)
    { }

    public event Action<bool>? WaitingActionChoiceHandler;

    public override async Task<Card> AsyncWaitChoiceCard()
    {
        while (_chosenCard is null)
        {
            await Task.Delay(1);
        }

        Card returnedValue = (Card)_chosenCard;
        _chosenCard = null;

        return returnedValue;
    }

    public void SetChosenCard(Card card)
    {
        _chosenCard = card;
    }

    public override async Task<PlayerActions> AsyncWaitActionСhoice(PlayerActions[] allowedActions)
    {
        WaitingActionChoiceHandler?.Invoke(true);

        while (_chosenAction is null || !allowedActions.Contains((PlayerActions)_chosenAction)) 
        {
            await Task.Delay(1);
        }

        PlayerActions returnedValue = (PlayerActions)_chosenAction;
        _chosenAction = null;

        WaitingActionChoiceHandler?.Invoke(false);

        return returnedValue;
    }

    public void SetChosenAction(PlayerActions action)
    {
        _chosenAction = action;
    }
}