using System;
using System.Threading.Tasks;

using CardGameFool.Model.Cards;

namespace CardGameFool.Model.Players;

public class BotPlayer : Player
{
    public BotPlayer(string name) : base(name)
    { }

    public override async Task<Card> AsyncWaitChoiceCard()
    {
        await Task.Delay(1);
        return _cards[0];
    }

    public override async Task<PlayerActions> AsyncWaitActionСhoice(PlayerActions[] allowedActions)
    {
        await Task.Delay(1);
        return allowedActions[0];
    }
}