using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CardGameFool.Model.Cards;
using CardGameFool.Model.Game;
using CardGameFool.Model.Players;

namespace CardGameFool.UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly LivePlayer _livePlayer = new("Live player");
    private readonly BotPlayer _botPlayer = new("Bot player");

    private readonly Deck _deck = new();
    private readonly CardsTable _cardsTable = new();
    private readonly Fool _gameFool;

    private readonly Dictionary<Player, PlayerCards> _playersPositions = new();

    private bool _isGameStarted = false;

    public MainWindow()
    {
        InitializeComponent();

        SubscribeGeneralPlayersEvents(_livePlayer);
        _livePlayer.StartWaitActionChoice += (actions) => SetEnableStatusToChoiceActionButtons(true, actions);
        _livePlayer.EndWaitActionChoice += () => SetEnableStatusToChoiceActionButtons(false);

        SubscribeGeneralPlayersEvents(_botPlayer);

        _deck.GetedTopCard += () => CardsDeck.Count--;

        _cardsTable.Cleared += CardsSlots.Clear;
        _cardsTable.AddingСard += Fool_PutСardInSlot;

        _gameFool = new Fool(_livePlayer, _botPlayer, _deck, _cardsTable);
        _gameFool.ChangeActivePlayer += Fool_ChangeActivePlayer;

        _playersPositions.Add(_livePlayer, BottomPositionPlayerCards);
        _playersPositions.Add(_botPlayer, TopPositionPlayerCards);

        BottomPositionPlayerCards.CardsMouseDown += BottomPositionCards_MouseDown;
    }

    private void SubscribeGeneralPlayersEvents(Player player)
    {
        player.TakedCards += Player_TakedCards;
        player.MakeMoved += Player_UsePlayerCard;
        player.BeatedCard += Player_UsePlayerCard;
    }

    private async void StartGame()
    {
        CardsDeck.Trump = new CardUI(_deck.TrumpCard);

        //try
        {
            Results result = await _gameFool.AsyncStartGame();
            MessageBox.Show(result.ToString());
        }
        //catch (Exception ex)
        {
            //MessageBox.Show(ex.Message + '\n' + ex.StackTrace);
        }
        
    }

    private void Player_TakedCards(Player player, IEnumerable<Card> addedCards)
    {
        foreach (Card card in addedCards)
        {
            _playersPositions[player].Add(new CardUI(card));
        }

        _playersPositions[player].ShowCards();
    }

    private void Player_UsePlayerCard(Player player, Card card)
    {
        _playersPositions[player].Remove(card);
    }

    private void SetEnableStatusToChoiceActionButtons(bool isEnabled, ICollection<PlayerActions>? actions = null)
    {
        foreach (UIElement item in PlayersActions.Users)
        {
            if (actions is null || actions.Contains(PlayersActions.GetAction(item)))
            {
                item.IsEnabled = isEnabled;
            }
        }
    }

    private void Fool_ChangeActivePlayer(Player player)
    {
        BottomPositionPlayerCards.IsActive = false;
        TopPositionPlayerCards.IsActive = false;

        _playersPositions[player].IsActive = true;
    }

    private void Fool_PutСardInSlot(Card card, PlayerActions action)
    {
        if (action == PlayerActions.MakeMove)
        {
            CardsSlots.Put(new CardUI(card), CardsSlots.LastFreeAttackingSlotIndex, SlotTypes.Attacking);

            return;
        }
        else if (action == PlayerActions.BeatCard)
        {
            CardsSlots.Put(new CardUI(card), CardsSlots.LastFreeDefenseSlotIndex, SlotTypes.Defense);

            return;
        }

        throw new InvalidOperationException("Incorrect action in this context.");
    }

    private void PauseButton_Click(object sender, RoutedEventArgs e)
    {
        if (_isGameStarted)
        {
            return;
        }

        _isGameStarted = true;

        StartGame(); 
    }

    private void BottomPositionCards_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _livePlayer.SetChosenCard(((CardUI)sender).Card);
    }

    private void ChoiceActionButton_Click(object sender, RoutedEventArgs e)
    {
        PlayerActions action = PlayersActions.GetAction((Button)sender);
        _livePlayer.SetChosenAction(action);
    }

    private void ChoiceActionButton_MouseEnter(object sender, MouseEventArgs e)
    {
        PlayerActions action = PlayersActions.GetAction((Button)sender);
        ChoiceButtonsTooltip.ChangeValue(PlayersActions.ToNormalizeString(action), true);
    }

    private void ChoiceActionButton_MouseLeave(object sender, MouseEventArgs e)
    {
        ChoiceButtonsTooltip.ChangeValue("...", false);
    }
}