using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

using CardGameFool.Model;
using CardGameFool.Model.Cards;
using CardGameFool.Model.Players;

namespace CardGameFool.UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Dictionary<Player, PlayerCards> _playersPositions = new();

    public MainWindow()
    {
        InitializeComponent();
    }

    private void StartGame()
    {
        LivePlayer player1 = new("live player");
        BotPlayer player2 = new("Bot");

        player1.TakedСardsFromDeck += ShowPlayerCards;
        player2.TakedСardsFromDeck += ShowPlayerCards;

        _playersPositions.Add(player1, BottomPositionPlayerCards);
        _playersPositions.Add(player2, TopPositionPlayerCards);

        BottomPositionPlayerCards.CardsMouseDown += BottomPositionCards_MouseDown;

        Deck deck = new();
        CardsDeck.TrumpCard = new CardUI(deck.TrumpCard);

        Fool gameFool = new(player1, player2, deck);
        gameFool.IdentifiedFirstPlayer += ChangeActivePlayer;

        GameResults gameResult = gameFool.StartGame();
    }

    private void ShowPlayerCards(Player player)
    {
        PlayerCards cardsUI = _playersPositions[player];

        Card[] cards = player.Cards;

        for (int i = 0; i < player.CardsCount; i++)
        {
            cardsUI.AddCard(new CardUI(cards[i]), i);
        }
    }

    private void ChangeActivePlayer(Player player)
    {
        BottomPositionPlayerCards.IsActive = false;
        TopPositionPlayerCards.IsActive = false;

        _playersPositions[player].IsActive = true;
    }


    private void PauseButton_Click(object sender, RoutedEventArgs e)
    {
        StartGame();
    }

    private void BottomPositionCards_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (!BottomPositionPlayerCards.IsActive)
        {
            return;
        }

        CardUI card = (CardUI)sender;

        BottomPositionPlayerCards.RemoveCard(card);
        BottomPositionPlayerCards.IsActive = false;

        CardsSlots.PutCard(card, CardsSlots.LastFreeAttackingSlotIndex, SlotTypes.Attacking);
    }
}