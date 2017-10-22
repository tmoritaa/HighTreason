using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame.GameStates
{
    public class DelibrationState : GameState
    {
        public DelibrationState(Game _game) 
            : base(GameStateType.Deliberation, _game)
        {}

        public override void StartState()
        {
            game.CurPlayer = game.GetPlayerOfSide(Player.PlayerSide.Prosecution);

            if (game.NotifyStateStart != null)
            {
                game.NotifyStateStart();
            }

            mainLogic();
        }

        public override void GotoNextState()
        {
            game.SignifyEndGame();
        }

        private void handleGuiltTrackEffect(Game game)
        {
            EvidenceTrack guiltTrack = game.GetGuiltTrack();
            int modValue = guiltTrack.Value - 2;

            if (modValue > 0)
            {
                List<Jury> unlockedJuries = game.Board.Juries.FindAll(j => !j.SwayTrack.IsLocked);
                unlockedJuries.ForEach(j => j.SwayTrack.AddToValue(modValue));
            }
        }

        private void handleInsanityTrackEffect(Game game)
        {
            EvidenceTrack insanityTrack = game.GetInsanityTrack();
            int modValue = (insanityTrack.Value - 1);

            if (modValue > 0)
            {
                modValue *= -1;
                game.Board.AspectTracks.ForEach(t => t.AddToValue(modValue));
            }
        }

        private void mainLogic()
        {
            if (game.NotifyStartOfTurn != null)
            {
                game.NotifyStartOfTurn();
            }

            // Reveal all jury aspects.
            foreach (Jury jury in game.Board.Juries)
            {
                jury.Aspects.ForEach(a => a.Reveal());
            }

            EvidenceTrack guiltTrack = game.GetGuiltTrack();

            if (guiltTrack.Value < 2)
            {
                if (game.NotifyGameEnd != null)
                {
                    game.NotifyGameEnd(Player.PlayerSide.Defense, true, 0);
                }
                
                goto GameEnd;
            }

            handleGuiltTrackEffect(game);
            handleInsanityTrackEffect(game);

            game.Board.Juries.ForEach(j => j.RevealAllTraits());

            List<Jury> usedJuries = new List<Jury>();

            Dictionary<Player.PlayerSide, bool> playerSidePassed = new Dictionary<Player.PlayerSide, bool>();
            while (playerSidePassed.Count < 2)
            {
                List<Jury> lockedJuries;

                if (game.CurPlayer.Side == Player.PlayerSide.Prosecution)
                {
                    lockedJuries = game.Board.Juries.FindAll(j => j.SwayTrack.IsLocked && j.SwayTrack.Value == j.SwayTrack.MaxValue);
                }
                else
                {
                    lockedJuries = game.Board.Juries.FindAll(j => j.SwayTrack.IsLocked && j.SwayTrack.Value == j.SwayTrack.MinValue);
                }

                lockedJuries = lockedJuries.Except(usedJuries).ToList();

                if (lockedJuries.Count > 0)
                {
                    Jury usedJury = game.CurPlayer.PerformJuryForDeliberation(lockedJuries);

                    usedJuries.Add(usedJury);
                }
                else
                {
                    playerSidePassed[game.CurPlayer.Side] = true;
                }

                if (!playerSidePassed.ContainsKey(game.GetOtherPlayer().Side))
                {
                    game.PassToNextPlayer();
                }
            }

            // Calculate winning player.
            int totalScore = 0;
            foreach (Jury jury in game.Board.Juries)
            {
                int score = jury.CalculateGuiltScore();
                totalScore += score;
            }

            Player.PlayerSide winningPlayer = totalScore >= GameConstants.PROSECUTION_SCORE_THRESHOLD ? Player.PlayerSide.Prosecution : Player.PlayerSide.Defense;

            if (game.NotifyGameEnd != null)
            {
                game.NotifyGameEnd(winningPlayer, false, totalScore);
            }

            GameEnd:
            {
                GotoNextState();
            }
        }
    }
}
