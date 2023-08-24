using System;
using System.Linq;
using System.Threading.Tasks;

using CardGameFool.Model.Cards;

namespace CardGameFool.Model.Players;

public class LivePlayer : Player
{
    private Card? _chosenCard = null;
    private PlayerActions? _chosenAction = null;

    private bool _isWaitingCard = false;

    public LivePlayer(string name) : base(name)
    { }

    public event Action<bool>? WaitingActionChoiceHandler;

    public override async Task<Card> AsyncWaitChoiceCard()
    {
        _isWaitingCard = true;
        await AsyncWait(() => _chosenCard is null);
        _isWaitingCard = false;

        return Nullifies(ref _chosenCard);
    }

    public override async Task<PlayerActions> AsyncWaitActionСhoice(PlayerActions[] allowedActions)
    {
        WaitingActionChoiceHandler?.Invoke(true);
        await AsyncWait(() => _chosenAction is null || !allowedActions.Contains((PlayerActions)_chosenAction));
        WaitingActionChoiceHandler?.Invoke(false);

        return Nullifies(ref _chosenAction);
    }

    public void SetChosenCard(Card card)
    {
        if (_isWaitingCard)
        {
            _chosenCard = card;
        }
    }

    public void SetChosenAction(PlayerActions action)
    {
        _chosenAction = action;
    }

    private static async Task AsyncWait(Func<bool> condition)
    {
        while (condition())
        {
            await Task.Delay(1);
        }
    }

    /// <summary> Value type variable nulling. </summary>
    /// <returns> The value that was stored before nulling. </returns>
    private static T Nullifies<T>(ref T? variable) where T : struct
    {
        if (variable is null)
        {
            throw new ArgumentNullException("The nullifies object must have a non-null value.");
        }

        T returnedValue = (T)variable;
        variable = null;

        return returnedValue;
    }
}