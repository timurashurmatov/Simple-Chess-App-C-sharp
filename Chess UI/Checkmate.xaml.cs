using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Chess_UI
{
    /// <summary>
    /// Interaction logic for Checkmate.xaml
    /// </summary>
    public partial class Checkmate : Window
    {
        public Checkmate(bool isWhite)
        {
            InitializeComponent();

            string winner = isWhite ? "WHITE" : "BLACK";
            Message.Content = $" GAME OVER\n{winner} WINS!";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
