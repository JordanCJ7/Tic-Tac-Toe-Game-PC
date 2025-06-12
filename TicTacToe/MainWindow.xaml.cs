using System.Windows;
using System.Windows.Controls;

namespace TicTacToe
{
    public partial class MainWindow : Window
    {
        // Ensure MainWindow.xaml exists and is linked to this code-behind file.
        // The partial keyword and correct XAML linkage are required for InitializeComponent to be generated.
        // Make sure MainWindow.xaml has x:Class="TicTacToe.MainWindow" and UniformGrid x:Name="GameGrid".
        private char[,] board = new char[3, 3];
        private char currentPlayer = 'X';
        private bool gameEnded = false;

        public MainWindow()
        {
            InitializeComponent();
            // GameGrid is initialized from XAML
            InitializeGameBoard();
            InitializeBoard();
        }

        private void InitializeGameBoard()
        {
            GameGrid.Children.Clear();
            for (int i = 0; i < 9; i++)
            {
                var button = new Button { FontSize = 32, Tag = i };
                button.Click += Button_Click;
                GameGrid.Children.Add(button);
            }
        }

        private void InitializeBoard()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    board[i, j] = ' ';
            currentPlayer = 'X';
            gameEnded = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (gameEnded) return;
            if (sender is not Button button || button.Tag is not int index)
                return;
            int row = index / 3;
            int col = index % 3;
            if (board[row, col] != ' ')
                return;
            board[row, col] = currentPlayer;
            button.Content = currentPlayer.ToString();

            if (CheckWin())
            {
                MessageBox.Show($"Player {currentPlayer} wins!");
                gameEnded = true;
            }
            else if (IsBoardFull())
            {
                MessageBox.Show("It's a draw!");
                gameEnded = true;
            }
            else
            {
                currentPlayer = currentPlayer == 'X' ? 'O' : 'X';
            }
        }

        private bool CheckWin()
        {
            for (int i = 0; i < 3; i++)
            {
                if (board[i, 0] == currentPlayer && board[i, 1] == currentPlayer && board[i, 2] == currentPlayer)
                    return true;
                if (board[0, i] == currentPlayer && board[1, i] == currentPlayer && board[2, i] == currentPlayer)
                    return true;
            }
            if (board[0, 0] == currentPlayer && board[1, 1] == currentPlayer && board[2, 2] == currentPlayer)
                return true;
            if (board[0, 2] == currentPlayer && board[1, 1] == currentPlayer && board[2, 0] == currentPlayer)
                return true;
            return false;
        }

        private bool IsBoardFull()
        {
            foreach (char c in board)
                if (c == ' ')
                    return false;
            return true;
        }

        // Optional: Add a method to reset the game
        private void ResetGame()
        {
            InitializeBoard();
            foreach (Button btn in GameGrid.Children)
                btn.Content = string.Empty;
        }
    }
}
