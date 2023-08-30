using System;
using System.Collections.Generic;
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

    public event Action<IList<PlayerActions>>? StartWaitActionChoice;
    public event Action? EndWaitActionChoice;

    public override async Task<Card> AsyncWaitCardChoice()
    {
        _isWaitingCard = true;
        await AsyncWait(() => _chosenCard is null);
        _isWaitingCard = false;
        
        return Nulling(ref _chosenCard);
    }

    public override async Task<PlayerActions> AsyncWaitActionСhoice(IList<PlayerActions> allowedActions)
    {
        StartWaitActionChoice?.Invoke(allowedActions);

        await AsyncWait(() =>
        _chosenAction is null
        || !allowedActions.Contains((PlayerActions)_chosenAction)
        || !PlayerActionsValidator.IsValidAction((PlayerActions)_chosenAction, _cards));

        EndWaitActionChoice?.Invoke();

        return Nulling(ref _chosenAction);
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
    private static T Nulling<T>(ref T? variable) where T : struct
    {
        if (variable is null)
        {
            throw new ArgumentNullException("The nulling variable must have a non-null value.");
        }

        T returnedValue = (T)variable;
        variable = null;

        return returnedValue;
    }
}