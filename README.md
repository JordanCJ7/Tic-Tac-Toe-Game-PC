# Tic-Tac-Toe Game

A modern, feature-rich implementation of the classic Tic-Tac-Toe game built with C# and WPF. This application offers multiple board sizes, game modes, and AI difficulty levels for an enhanced gaming experience.

![Tic-Tac-Toe Game](https://example.com/screenshot.png) <!-- Replace with an actual screenshot -->

## Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Game Modes](#game-modes)
- [AI Difficulty Levels](#ai-difficulty-levels)
- [Board Sizes](#board-sizes)
- [How to Play](#how-to-play)
- [Installation](#installation)
- [Future Enhancements](#future-enhancements)
- [Contributing](#contributing)
- [License](#license)

## Features

- **Multiple Game Modes**: Classic and Timed modes
- **Adjustable Board Sizes**: 3×3, 4×4, 5×5, and 6×6 grids
- **Customizable Win Conditions**: Win conditions automatically adjust based on board size
- **AI Opponent**: Play against the computer with three difficulty levels
- **Time-Based Gameplay**: Timed mode with countdown timer for each move
- **Time-Based Tiebreaker**: In case of a draw in Timed mode, the player who used less total time wins
- **Responsive UI**: Clean and intuitive user interface
- **Game Statistics**: Track time spent by each player in Timed mode

## Tech Stack

- **Framework**: .NET 9.0
- **UI Framework**: Windows Presentation Foundation (WPF)
- **Programming Language**: C#
- **Build Tool**: MSBuild / dotnet CLI
- **Design Pattern**: MVVM (Model-View-ViewModel) architecture
- **Version Control**: Git

## Project Structure

```
Tic-Tac-Toe-Game-PC/
├── TicTacToe/
│   ├── App.xaml                 # Application entry point and resources
│   ├── App.xaml.cs              # Application logic
│   ├── MainWindow.xaml          # Main UI definition
│   ├── MainWindow.xaml.cs       # Game logic and UI interaction
│   └── TicTacToe.csproj         # Project configuration
├── Tic-Tac-Toe-Game-PC.sln      # Solution file
└── README.md                    # Project documentation
```

## Game Modes

### Classic Mode
Traditional Tic-Tac-Toe gameplay where players take turns placing their marks (X or O) on the board. The first player to get their marks in a row (horizontally, vertically, or diagonally) wins.

### Timed Mode
Adds an exciting time constraint to the game:
- Each player has 5 seconds to make a move
- If time runs out, the player's turn is skipped
- In case of a draw, the player who used less total time wins
- Time statistics are displayed at the end of the game

## AI Difficulty Levels

### Easy
The AI makes random moves, ideal for beginners or casual play.

### Medium
Balanced difficulty that's challenging but beatable. The AI has a 50% chance of making the optimal move and a 50% chance of making a random move.

### Hard
The AI uses strategic decision-making to play optimally:
1. Tries to win if possible
2. Blocks the player from winning
3. Takes strategic positions (center, corners) when available
4. Makes intelligent random moves when no better option exists

## Board Sizes

The game offers four different board sizes, each with appropriate win conditions:

| Board Size | Grid | Win Condition |
|------------|------|---------------|
| Classic    | 3×3  | 3 in a row    |
| Extended   | 4×4  | 4 in a row    |
| Advanced   | 5×5  | 4 in a row    |
| Expert     | 6×6  | 5 in a row    |

## How to Play

1. **Start the Game**: Launch the application
2. **Select Game Type**: Choose Player vs Player or Player vs AI
   - If Player vs AI is selected, choose an AI difficulty level
3. **Select Board Size**: Choose from 3×3, 4×4, 5×5, or 6×6
4. **Select Game Mode**: Choose Classic Mode or Timed Mode
5. **Play the Game**: 
   - Players take turns placing their marks on the board
   - In Timed Mode, make your move before the timer runs out
   - The first player to get their marks in a row wins
6. **End of Game**:
   - View the result (win, lose, or draw)
   - View time statistics (in Timed Mode)
   - Choose to play again or return to the main menu

## Installation

### Prerequisites
- Windows OS
- .NET 9.0 SDK or Runtime installed

### Option 1: Download and Run
1. Download the latest release from the [Releases](https://github.com/JordanCJ7/Tic-Tac-Toe-Game-PC/releases) page
2. Extract the ZIP file
3. Run `TicTacToe.exe`

### Option 2: Build from Source
1. Clone the repository:
   ```
   git clone https://github.com/JordanCJ7/Tic-Tac-Toe-Game-PC.git
   ```
2. Navigate to the project directory:
   ```
   cd Tic-Tac-Toe-Game-PC
   ```
3. Build the project:
   ```
   dotnet build
   ```
4. Run the application:
   ```
   dotnet run --project TicTacToe
   ```

## Future Enhancements

- [ ] Online multiplayer support
- [ ] User profiles and statistics tracking
- [ ] Custom themes and board designs
- [ ] Additional game modes (e.g., Blitz, Tournament)
- [ ] Sound effects and animations
- [ ] Replay functionality to review past games
- [ ] Leaderboard for high scores

## Contributing

Contributions are welcome! Feel free to submit issues, feature requests, or pull requests.

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

© 2025 | Created with ❤️ by Janitha Gamage
