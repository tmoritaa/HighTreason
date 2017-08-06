using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame.GameStates
{
    public class JurySelectionState : GameState
    {
        public JurySelectionState(int gameId)
            : base(gameId)
        {}

        public override void StartState()
        {
            Game game = Game.GetGameFromId(gameId);

            foreach (Player.PlayerSide side in new Player.PlayerSide[] { Player.PlayerSide.Prosecution, Player.PlayerSide.Defense })
            {
                game.GetPlayerOfSide(side).SetupHand(game.Deck.DealCards(GameConstants.NUM_HAND_SIZE));
            }

            game.CurPlayer = game.GetPlayerOfSide(Player.PlayerSide.Prosecution);

            mainLoop();
        }

        public override void GotoNextState()
        {
            Game.GetGameFromId(gameId).GotoStateAndStart(StateType.TrialInChief);
        }

        private void mainLoop()
        {
            Game game = Game.GetGameFromId(gameId);
            while (true)
            {
                game.CurPlayer.PlayCard(StateType.JurySelection);

                int numPlayersFinished = 0;
                List<Player> players = game.GetPlayers();
                players.ForEach(p => numPlayersFinished += p.MustPass() ? 1 : 0);
                if (numPlayersFinished == players.Count)
                {
                    break;
                }

                game.PassToNextPlayer();
            }

            game.GetPlayers().ForEach(p => p.AddHandToSummation());
            game.ShuffleDiscardBackToDeck();

            GotoNextState();
        }
    }
}
