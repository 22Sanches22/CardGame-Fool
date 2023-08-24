using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CardGameFool.Model;
using CardGameFool.Model.Cards;
using CardGameFool.Model.Players;

namespace CardGameFool.UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly LivePlayer livePlayer = new("Live player");
    private readonly BotPlayer botPlayer = new("Bot player");

    private readonly Deck deck = new();
    private readonly Fool gameFool;

    private readonly Dictionary<Player, PlayerCards> _playersPositions = new();

    private bool _isGameStarted = false;

    public MainWindow()
    {
        InitializeComponent();
        
        SubscribeGeneralPlayersEvents(livePlayer);
        livePlayer.WaitingActionChoiceHandler += LivePlayer_HandleWaitingActionChoice;

        SubscribeGeneralPlayersEvents(botPlayer);

        gameFool = new Fool(livePlayer, botPlayer, deck);
        SubscribeFoolEvents();

        _playersPositions.Add(livePlayer, BottomPositionPlayerCards);
        _playersPositions.Add(botPlayer, TopPositionPlayerCards);

        BottomPositionPlayerCards.CardsMouseDown += BottomPositionCards_MouseDown;
    }

    public static RoutedCommand ChoiceAction { get; } = new(nameof(ChoiceAction), typeof(MainWindow));

    private void SubscribeGeneralPlayersEvents(Player player)
    {
        player.TakedCards += Player_TakedCards;
        player.MakeMoved += Player_UsePlayerCard;
        player.BeatedCard += Player_UsePlayerCard;
    }

    private void SubscribeFoolEvents()
    {
        gameFool.ChangedFirstPlayer += Fool_ChangeActivePlayer;
        gameFool.ClearedСardsOnTable += CardsSlots.Clear;
        gameFool.AddingСardsOnTable += Fool_PutСardInSlot;
    }

    private async void StartGame()
    {
        CardsDeck.Trump = new CardUI(deck.TrumpCard);

        var result = await gameFool.AsyncStartGame();
        MessageBox.Show(result.ToString());
    }

    private void Player_TakedCards(Player player, IEnumerable<Card> addedCards)
    {
        foreach (Card card in addedCards)
        {
            _playersPositions[player].Add(new CardUI(card));
        }

        int slideIndex = _playersPositions[player].Count / Player.DefaultCardsCount - 1;
        _playersPositions[player].ShowCards(slideIndex);
    }

    private void Player_UsePlayerCard(Player player, Card card)
    {
        _playersPositions[player].Remove(card);
    }

    private void LivePlayer_HandleWaitingActionChoice(bool isStartHandling)
    {
        MakeMoveButton.IsEnabled = isStartHandling;
        DiscardCardsButton.IsEnabled = isStartHandling;
        BeatCardButton.IsEnabled = isStartHandling;
        TakeCardButton.IsEnabled = isStartHandling;
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
        livePlayer.SetChosenCard(((CardUI)sender).Card);
    }

    private void ChoiceAction_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        PlayerActions action = Enum.Parse<PlayerActions>((string)e.Parameter);
        livePlayer.SetChosenAction(action);
    }

    private void ChoiceButton_MouseEnter(object sender, MouseEventArgs e)
    {
        object buttonTag = ((Button)sender).Tag;
        ChoiceButtonsTooltip.ChangeValue((string)buttonTag, true);
    }

    private void ChoiceButton_MouseLeave(object sender, MouseEventArgs e)
    {
        ChoiceButtonsTooltip.ChangeValue("...", false);
    }
}