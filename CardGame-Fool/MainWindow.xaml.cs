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

using CardGame_Fool.Model;

namespace CardGame_Fool;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Dictionary<Suits, char> suitSymbols = new()
        {
            [Suits.Spades] = '♤',
            [Suits.Diamonds] = '♢',
            [Suits.Clubs] = '♧',
            [Suits.Hearts] = '♡'
        };

        BotPlayer player = new("player1");
        BotPlayer player1 = new("player2");

        Fool gameFool = new(player, player1);
        //gameFool.StartGame();

        //foreach (Card item in gameFool._cardsDeck)
        //{
        //    var textBox = new TextBlock
        //    {
        //        FontSize = 11,
        //        Text =
        //            (item.Rank < Ranks.Jack
        //            ? ((int)item.Rank).ToString()
        //            : item.Rank.ToString())
        //            + $" {suitSymbols[item.Suit]}",
        //        Foreground =
        //            (item.Suit == Suits.Diamonds) || (item.Suit == Suits.Hearts)
        //            ? Brushes.Red
        //            : Brushes.Black
        //    };
            
        //    Workspace.Items.Add(textBox);
        //}
    }
}