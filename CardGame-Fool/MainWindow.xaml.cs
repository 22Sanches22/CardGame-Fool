using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using CardGame_Fool.GameFool;

namespace CardGame_Fool;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        IPlayer player = new BotPlayer("player1");
        IPlayer player1 = new BotPlayer("player2");

        Fool gameFool = new();
        gameFool.StartGame(player, player1);

        foreach (Card item in gameFool._cardsDeck)
        {
            var textBox = new TextBlock
            {
                FontSize = 11,
                Text =
                    (item.Rank < Ranks.Jack
                    ? ((int)item.Rank).ToString()
                    : item.Rank.ToString())
                    + $" {Fool.s_symbolsOfSuits[item.Suit]}",
                Foreground =
                    (item.Suit == Suits.Diamonds) || (item.Suit == Suits.Hearts)
                    ? Brushes.Red
                    : Brushes.Black
            };
            
            Workspace.Items.Add(textBox);
        }
    }
}