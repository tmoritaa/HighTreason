# High Treason PC Port
A PC port written in Unity of the board game High Treason The Trial of Louis Riel published by Victory Point Games.

# Directories
Each branch has three directories:
* HighTreasonGame - Implementation of game logic. Compiles to dll to be used by other projects
* HighTreasonUnity - UI implementation
* HighTreasonConsole - Command line implementation. Only used in the MCTS branch for testing AI

# Branches
Currently the project has two branches, master and MCTS.
* master - All rules and cards implemented, as well as UI to allow users to play both sides. An AI that makes random moves is also implemented, but not integrated with the UI implementation.
* MCTS - Initial implementation of MCTS implemented. However due to the game logic design, currently uses too much memory and time to finish a single game. Problem will be described a bit more later.

# MCTS
The MCTS branch currently has a cheating MCTS implementation. The problem is however, due to the game logic design, a single node has the potential to have up to 7000 potential actions it can perform, which results in some nodes being massive in memory consumption. Game logic should be rewritten s.t. choices that require multiple targets are broken down to multiple single target choices, each getting its own node.
