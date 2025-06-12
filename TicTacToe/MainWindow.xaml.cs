using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace TicTacToe
{
    public partial class MainWindow : Window
    {
        private char[,] board = null!;
        private char currentPlayer = 'X';
        private bool gameEnded = false;
        private int boardSize;
        private int winLength;
        private bool isAIMode = false;
        private enum AIDifficulty { Easy, Medium, Hard }
        private AIDifficulty currentAIDifficulty = AIDifficulty.Easy;
        private Random random = new Random();
        private char aiPlayer = 'O'; // AI is always O, human is always X

        public MainWindow()
        {
            InitializeComponent();
            
            // Explicitly set initial visibility
            StartScreen.Visibility = Visibility.Visible;
            GameScreen.Visibility = Visibility.Collapsed;
            
            // Add event handlers for radio buttons
            AIModeRadio.Checked += GameModeRadio_Checked;
            PlayerModeRadio.Checked += GameModeRadio_Checked;
            
            // Set default values
            isAIMode = false;
        }
        
        private void GameModeRadio_Checked(object sender, RoutedEventArgs e)
        {
            isAIMode = sender == AIModeRadio;
            AIDifficultyPanel.Visibility = isAIMode ? Visibility.Visible : Visibility.Collapsed;
        }

        #region Game Setup Methods

        private void Board3x3_Click(object sender, RoutedEventArgs e)
        {
            StartGame(3, 3); // 3x3 board with 3 in a row to win
        }

        private void Board4x4_Click(object sender, RoutedEventArgs e)
        {
            StartGame(4, 4); // 4x4 board with 4 in a row to win
        }

        private void Board5x5_Click(object sender, RoutedEventArgs e)
        {
            StartGame(5, 4); // 5x5 board with 4 in a row to win
        }

        private void Board6x6_Click(object sender, RoutedEventArgs e)
        {
            StartGame(6, 5); // 6x6 board with 5 in a row to win
        }

        private void StartGame(int size, int winCondition)
        {
            boardSize = size;
            winLength = winCondition;
            board = new char[boardSize, boardSize];
            
            // Set AI difficulty if in AI mode
            if (isAIMode)
            {
                if (MediumAIRadio.IsChecked == true)
                    currentAIDifficulty = AIDifficulty.Medium;
                else if (HardAIRadio.IsChecked == true)
                    currentAIDifficulty = AIDifficulty.Hard;
                else
                    currentAIDifficulty = AIDifficulty.Easy;
            }
            
            // Initialize the game
            InitializeGameBoard();
            InitializeBoard();
            
            // Update UI
            string playerInfo = isAIMode ? "Player vs AI" : "Player X's Turn";
            GameInfo.Text = playerInfo;
            StartScreen.Visibility = Visibility.Collapsed;
            GameScreen.Visibility = Visibility.Visible;
        }

        private void InitializeGameBoard()
        {
            // Set up UniformGrid dimensions
            GameGrid.Rows = boardSize;
            GameGrid.Columns = boardSize;
            GameGrid.Children.Clear();
            
            // Calculate button size (smaller for larger boards)
            double buttonSize = 300.0 / boardSize;
            
            // Create buttons
            for (int i = 0; i < boardSize * boardSize; i++)
            {
                var button = new Button { 
                    FontSize = 32 / (boardSize / 3.0), 
                    Tag = i,
                    Width = buttonSize,
                    Height = buttonSize,
                    Margin = new Thickness(2)
                };
                button.Click += Button_Click;
                GameGrid.Children.Add(button);
            }
        }

        private void InitializeBoard()
        {
            for (int i = 0; i < boardSize; i++)
                for (int j = 0; j < boardSize; j++)
                    board[i, j] = ' ';
            currentPlayer = 'X';
            gameEnded = false;
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            ResetGame();
        }

        private void ChangeSize_Click(object sender, RoutedEventArgs e)
        {
            GameScreen.Visibility = Visibility.Collapsed;
            StartScreen.Visibility = Visibility.Visible;
        }

        #endregion

        #region Game Logic Methods
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (gameEnded) return;
            if (sender is not Button button || button.Tag is not int index)
                return;
                
            int row = index / boardSize;
            int col = index % boardSize;
            
            if (board[row, col] != ' ')
                return;
                
            // In AI mode, only allow player to make moves as 'X'
            if (isAIMode && currentPlayer != 'X')
                return;
                
            board[row, col] = currentPlayer;
            button.Content = currentPlayer.ToString();
            
            // Set color based on player
            button.Foreground = currentPlayer == 'X' ? 
                new SolidColorBrush(Colors.DarkBlue) : 
                new SolidColorBrush(Colors.DarkRed);

            if (CheckWin(row, col))
            {
                string winner = isAIMode ? (currentPlayer == 'X' ? "You win!" : "AI wins!") : $"Player {currentPlayer} wins!";
                GameInfo.Text = winner;
                MessageBox.Show(winner);
                gameEnded = true;
            }
            else if (IsBoardFull())
            {
                GameInfo.Text = "It's a draw!";
                MessageBox.Show("It's a draw!");
                gameEnded = true;
            }
            else
            {
                currentPlayer = currentPlayer == 'X' ? 'O' : 'X';
                
                if (isAIMode && currentPlayer == 'O' && !gameEnded)
                {
                    GameInfo.Text = "AI is thinking...";
                    
                    // Use a small delay to give a thinking effect for the AI
                    var timer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromMilliseconds(500)
                    };
                    timer.Tick += (s, args) =>
                    {
                        MakeAIMove();
                        timer.Stop();
                    };
                    timer.Start();
                }
                else
                {
                    GameInfo.Text = isAIMode ? "Your Turn" : $"Player {currentPlayer}'s Turn";
                }
            }
        }

        private bool CheckWin(int lastMoveRow, int lastMoveCol)
        {
            // We only need to check lines that include the last move
            return CheckHorizontal(lastMoveRow) || 
                   CheckVertical(lastMoveCol) || 
                   CheckDiagonals(lastMoveRow, lastMoveCol);
        }

        private bool CheckHorizontal(int row)
        {
            for (int startCol = 0; startCol <= boardSize - winLength; startCol++)
            {
                bool win = true;
                for (int i = 0; i < winLength; i++)
                {
                    if (board[row, startCol + i] != currentPlayer)
                    {
                        win = false;
                        break;
                    }
                }
                if (win) return true;
            }
            return false;
        }

        private bool CheckVertical(int col)
        {
            for (int startRow = 0; startRow <= boardSize - winLength; startRow++)
            {
                bool win = true;
                for (int i = 0; i < winLength; i++)
                {
                    if (board[startRow + i, col] != currentPlayer)
                    {
                        win = false;
                        break;
                    }
                }
                if (win) return true;
            }
            return false;
        }

        private bool CheckDiagonals(int row, int col)
        {
            // Check diagonal (top-left to bottom-right)
            for (int offset = -Math.Min(row, col); offset <= Math.Min(boardSize - 1 - row, boardSize - 1 - col) - (winLength - 1); offset++)
            {
                bool win = true;
                for (int i = 0; i < winLength; i++)
                {
                    if (row + offset + i >= boardSize || col + offset + i >= boardSize || 
                        board[row + offset + i, col + offset + i] != currentPlayer)
                    {
                        win = false;
                        break;
                    }
                }
                if (win) return true;
            }

            // Check anti-diagonal (top-right to bottom-left)
            for (int offset = -Math.Min(row, boardSize - 1 - col); offset <= Math.Min(boardSize - 1 - row, col) - (winLength - 1); offset++)
            {
                bool win = true;
                for (int i = 0; i < winLength; i++)
                {
                    if (row + offset + i >= boardSize || col - offset - i < 0 || 
                        board[row + offset + i, col - offset - i] != currentPlayer)
                    {
                        win = false;
                        break;
                    }
                }
                if (win) return true;
            }

            return false;
        }

        private bool IsBoardFull()
        {
            for (int i = 0; i < boardSize; i++)
                for (int j = 0; j < boardSize; j++)
                    if (board[i, j] == ' ')
                        return false;
            return true;
        }

        private void ResetGame()
        {
            InitializeBoard();
            foreach (Button btn in GameGrid.Children)
                btn.Content = string.Empty;
            GameInfo.Text = isAIMode ? "Your Turn" : "Player X's Turn";
        }

        #endregion

        #region AI Logic Methods

        private void MakeAIMove()
        {
            if (gameEnded) return;

            (int row, int col) = GetAIMove();
            
            // Make the move
            board[row, col] = aiPlayer;
            
            // Find the button for this position
            int index = row * boardSize + col;
            var button = GameGrid.Children.Cast<Button>().FirstOrDefault(b => b.Tag is int tag && tag == index);
            
            if (button != null)
            {
                button.Content = aiPlayer.ToString();
                button.Foreground = new SolidColorBrush(Colors.DarkRed);
                
                if (CheckWin(row, col))
                {
                    GameInfo.Text = "AI wins!";
                    MessageBox.Show("AI wins!");
                    gameEnded = true;
                }
                else if (IsBoardFull())
                {
                    GameInfo.Text = "It's a draw!";
                    MessageBox.Show("It's a draw!");
                    gameEnded = true;
                }
                else
                {
                    currentPlayer = 'X'; // Switch back to human player
                    GameInfo.Text = "Your Turn";
                }
            }
        }

        private (int row, int col) GetAIMove()
        {
            switch (currentAIDifficulty)
            {
                case AIDifficulty.Hard:
                    return GetHardAIMove();
                case AIDifficulty.Medium:
                    return GetMediumAIMove();
                case AIDifficulty.Easy:
                default:
                    return GetEasyAIMove();
            }
        }

        private (int row, int col) GetEasyAIMove()
        {
            // Easy AI: Make random moves
            var emptyCells = new List<(int row, int col)>();
            
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (board[i, j] == ' ')
                    {
                        emptyCells.Add((i, j));
                    }
                }
            }
            
            if (emptyCells.Count > 0)
            {
                int randomIndex = random.Next(emptyCells.Count);
                return emptyCells[randomIndex];
            }
            
            // Should never happen if called correctly
            return (0, 0);
        }

        private (int row, int col) GetMediumAIMove()
        {
            // Medium AI: Block opponent's winning moves and try to win
            
            // First check if AI can win in the next move
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (board[i, j] == ' ')
                    {
                        // Try this move
                        board[i, j] = aiPlayer;
                        
                        if (CheckWin(i, j))
                        {
                            // Undo the move
                            board[i, j] = ' ';
                            return (i, j);
                        }
                        
                        // Undo the move
                        board[i, j] = ' ';
                    }
                }
            }
            
            // Then check if opponent can win in the next move and block
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (board[i, j] == ' ')
                    {
                        // Try this move for the opponent
                        board[i, j] = 'X';
                        
                        if (CheckWin(i, j))
                        {
                            // Undo the move
                            board[i, j] = ' ';
                            return (i, j);
                        }
                        
                        // Undo the move
                        board[i, j] = ' ';
                    }
                }
            }
            
            // 50% chance to make a strategic move, 50% random
            if (random.Next(2) == 0)
            {
                // Try to play in the center or corners first
                var preferredMoves = new List<(int row, int col)>();
                
                // Center
                int center = boardSize / 2;
                if (boardSize % 2 == 1 && board[center, center] == ' ')
                {
                    return (center, center);
                }
                
                // Corners
                var corners = new List<(int row, int col)>
                {
                    (0, 0),
                    (0, boardSize - 1),
                    (boardSize - 1, 0),
                    (boardSize - 1, boardSize - 1)
                };
                
                foreach (var corner in corners)
                {
                    if (board[corner.row, corner.col] == ' ')
                    {
                        preferredMoves.Add(corner);
                    }
                }
                
                if (preferredMoves.Count > 0)
                {
                    int randomIndex = random.Next(preferredMoves.Count);
                    return preferredMoves[randomIndex];
                }
            }
            
            // Otherwise make a random move
            return GetEasyAIMove();
        }

        private (int row, int col) GetHardAIMove()
        {
            // For 3x3 board use minimax
            if (boardSize == 3)
            {
                return MinimaxMove();
            }
            
            // For larger boards, we'll use a smarter version of medium difficulty
            // to avoid long computation times
            
            // First check if AI can win in the next move
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (board[i, j] == ' ')
                    {
                        // Try this move
                        board[i, j] = aiPlayer;
                        
                        if (CheckWin(i, j))
                        {
                            // Undo the move
                            board[i, j] = ' ';
                            return (i, j);
                        }
                        
                        // Undo the move
                        board[i, j] = ' ';
                    }
                }
            }
            
            // Then check if opponent can win in the next move and block
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (board[i, j] == ' ')
                    {
                        // Try this move for the opponent
                        board[i, j] = 'X';
                        
                        if (CheckWin(i, j))
                        {
                            // Undo the move
                            board[i, j] = ' ';
                            return (i, j);
                        }
                        
                        // Undo the move
                        board[i, j] = ' ';
                    }
                }
            }
            
            // Look for fork opportunities (creating two threats to win)
            var forkMove = FindForkMove(aiPlayer);
            if (forkMove.Item1 != -1)
            {
                return (forkMove.Item1, forkMove.Item2);
            }
            
            // Block opponent's fork opportunities
            forkMove = FindForkMove('X');
            if (forkMove.Item1 != -1)
            {
                return (forkMove.Item1, forkMove.Item2);
            }
            
            // Center
            int center = boardSize / 2;
            if (boardSize % 2 == 1 && board[center, center] == ' ')
            {
                return (center, center);
            }
            
            // Opposite corner
            for (int i = 0; i < boardSize; i += boardSize - 1)
            {
                for (int j = 0; j < boardSize; j += boardSize - 1)
                {
                    if (board[i, j] == 'X' && board[boardSize - 1 - i, boardSize - 1 - j] == ' ')
                    {
                        return (boardSize - 1 - i, boardSize - 1 - j);
                    }
                }
            }
            
            // Empty corner
            for (int i = 0; i < boardSize; i += boardSize - 1)
            {
                for (int j = 0; j < boardSize; j += boardSize - 1)
                {
                    if (board[i, j] == ' ')
                    {
                        return (i, j);
                    }
                }
            }
            
            // Empty edge
            for (int i = 0; i < boardSize; i++)
            {
                if (board[i, 0] == ' ') return (i, 0);
                if (board[i, boardSize - 1] == ' ') return (i, boardSize - 1);
                if (board[0, i] == ' ') return (0, i);
                if (board[boardSize - 1, i] == ' ') return (boardSize - 1, i);
            }
            
            // If all else fails, make a random move
            return GetEasyAIMove();
        }

        private (int, int) FindForkMove(char player)
        {
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (board[i, j] == ' ')
                    {
                        int winningLines = 0;
                        board[i, j] = player;
                        
                        // Check if this move creates a fork (two potential winning lines)
                        // Horizontal
                        for (int startCol = 0; startCol <= boardSize - winLength; startCol++)
                        {
                            int playerCount = 0;
                            int emptyCount = 0;
                            
                            for (int k = 0; k < winLength; k++)
                            {
                                if (board[i, startCol + k] == player) playerCount++;
                                else if (board[i, startCol + k] == ' ') emptyCount++;
                            }
                            
                            if (playerCount == winLength - 1 && emptyCount == 1)
                                winningLines++;
                        }
                        
                        // Vertical
                        for (int startRow = 0; startRow <= boardSize - winLength; startRow++)
                        {
                            int playerCount = 0;
                            int emptyCount = 0;
                            
                            for (int k = 0; k < winLength; k++)
                            {
                                if (board[startRow + k, j] == player) playerCount++;
                                else if (board[startRow + k, j] == ' ') emptyCount++;
                            }
                            
                            if (playerCount == winLength - 1 && emptyCount == 1)
                                winningLines++;
                        }
                        
                        // Diagonal (top-left to bottom-right)
                        if (i - j >= -(boardSize - winLength) && i - j <= boardSize - winLength)
                        {
                            for (int offset = -Math.Min(i, j); offset <= Math.Min(boardSize - 1 - i, boardSize - 1 - j) - (winLength - 1); offset++)
                            {
                                int playerCount = 0;
                                int emptyCount = 0;
                                
                                for (int k = 0; k < winLength; k++)
                                {
                                    if (i + offset + k < boardSize && j + offset + k < boardSize)
                                    {
                                        if (board[i + offset + k, j + offset + k] == player) playerCount++;
                                        else if (board[i + offset + k, j + offset + k] == ' ') emptyCount++;
                                    }
                                }
                                
                                if (playerCount == winLength - 1 && emptyCount == 1)
                                    winningLines++;
                            }
                        }
                        
                        // Anti-diagonal (top-right to bottom-left)
                        if (i + j >= winLength - 1 && i + j <= 2 * boardSize - winLength - 1)
                        {
                            for (int offset = -Math.Min(i, boardSize - 1 - j); offset <= Math.Min(boardSize - 1 - i, j) - (winLength - 1); offset++)
                            {
                                int playerCount = 0;
                                int emptyCount = 0;
                                
                                for (int k = 0; k < winLength; k++)
                                {
                                    if (i + offset + k < boardSize && j - offset - k >= 0)
                                    {
                                        if (board[i + offset + k, j - offset - k] == player) playerCount++;
                                        else if (board[i + offset + k, j - offset - k] == ' ') emptyCount++;
                                    }
                                }
                                
                                if (playerCount == winLength - 1 && emptyCount == 1)
                                    winningLines++;
                            }
                        }
                        
                        board[i, j] = ' ';
                        
                        if (winningLines >= 2)
                            return (i, j);
                    }
                }
            }
            
            return (-1, -1);
        }

        private (int row, int col) MinimaxMove()
        {
            int bestScore = int.MinValue;
            (int row, int col) move = (-1, -1);
            
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (board[i, j] == ' ')
                    {
                        board[i, j] = aiPlayer;
                        int score = Minimax(board, 0, false);
                        board[i, j] = ' ';
                        
                        if (score > bestScore)
                        {
                            bestScore = score;
                            move = (i, j);
                        }
                    }
                }
            }
            
            return move;
        }

        private int Minimax(char[,] board, int depth, bool isMaximizing)
        {
            // Check for terminal states
            char winner = CheckWinner();
            
            if (winner == aiPlayer)
                return 10 - depth;
            else if (winner == 'X')
                return depth - 10;
            else if (IsBoardFull())
                return 0;
            
            if (isMaximizing)
            {
                int bestScore = int.MinValue;
                
                for (int i = 0; i < boardSize; i++)
                {
                    for (int j = 0; j < boardSize; j++)
                    {
                        if (board[i, j] == ' ')
                        {
                            board[i, j] = aiPlayer;
                            int score = Minimax(board, depth + 1, false);
                            board[i, j] = ' ';
                            bestScore = Math.Max(score, bestScore);
                        }
                    }
                }
                
                return bestScore;
            }
            else
            {
                int bestScore = int.MaxValue;
                
                for (int i = 0; i < boardSize; i++)
                {
                    for (int j = 0; j < boardSize; j++)
                    {
                        if (board[i, j] == ' ')
                        {
                            board[i, j] = 'X';
                            int score = Minimax(board, depth + 1, true);
                            board[i, j] = ' ';
                            bestScore = Math.Min(score, bestScore);
                        }
                    }
                }
                
                return bestScore;
            }
        }

        private char CheckWinner()
        {
            // Check rows
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j <= boardSize - winLength; j++)
                {
                    bool win = true;
                    char first = board[i, j];
                    
                    if (first == ' ') continue;
                    
                    for (int k = 1; k < winLength; k++)
                    {
                        if (board[i, j + k] != first)
                        {
                            win = false;
                            break;
                        }
                    }
                    
                    if (win) return first;
                }
            }
            
            // Check columns
            for (int j = 0; j < boardSize; j++)
            {
                for (int i = 0; i <= boardSize - winLength; i++)
                {
                    bool win = true;
                    char first = board[i, j];
                    
                    if (first == ' ') continue;
                    
                    for (int k = 1; k < winLength; k++)
                    {
                        if (board[i + k, j] != first)
                        {
                            win = false;
                            break;
                        }
                    }
                    
                    if (win) return first;
                }
            }
            
            // Check diagonals
            for (int i = 0; i <= boardSize - winLength; i++)
            {
                for (int j = 0; j <= boardSize - winLength; j++)
                {
                    // Diagonal (top-left to bottom-right)
                    bool win = true;
                    char first = board[i, j];
                    
                    if (first != ' ')
                    {
                        for (int k = 1; k < winLength; k++)
                        {
                            if (board[i + k, j + k] != first)
                            {
                                win = false;
                                break;
                            }
                        }
                        
                        if (win) return first;
                    }
                    
                    // Diagonal (top-right to bottom-left)
                    win = true;
                    first = board[i, j + winLength - 1];
                    
                    if (first != ' ')
                    {
                        for (int k = 1; k < winLength; k++)
                        {
                            if (board[i + k, j + winLength - 1 - k] != first)
                            {
                                win = false;
                                break;
                            }
                        }
                        
                        if (win) return first;
                    }
                }
            }
            
            return ' ';
        }

        #endregion
    }
}
