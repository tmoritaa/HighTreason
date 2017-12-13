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
            if (game.StartState == this.StateType)
            {
                game.Board.Juries.RemoveRange(0, 6);
            }

            curPlayer = game.GetPlayerOfSide(Player.PlayerSide.Prosecution);

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
            EvidenceTrack guiltTrack = game.Board.GetGuiltTrack();
            int modValue = guiltTrack.Value - 2;

            if (modValue > 0)
            {
                List<Jury> unlockedJuries = game.Board.Juries.FindAll(j => !j.SwayTrack.IsLocked);
                unlockedJuries.ForEach(j => j.SwayTrack.AddToValue(modValue));
            }
        }

        private void handleInsanityTrackEffect(Game game)
        {
            EvidenceTrack insanityTrack = game.Board.GetInsanityTrack();
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

            EvidenceTrack guiltTrack = game.Board.GetGuiltTrack();

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

                if (curPlayer.Side == Player.PlayerSide.Prosecution)
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
                    Jury usedJury = performJuryForDeliberation(lockedJuries, curPlayer);

                    usedJuries.Add(usedJury);
                }
                else
                {
                    playerSidePassed[curPlayer.Side] = true;
                }

                if (!playerSidePassed.ContainsKey(game.GetOtherPlayer(curPlayer).Side))
                {
                    passToNextPlayer();
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

        private Jury performJuryForDeliberation(List<Jury> juries, Player curPlayer)
        {
            Jury usedJury;
            while (true)
            {
                usedJury = chooseJuryChoice(juries, curPlayer, "Select Jury for Deliberation");
                FileLogger.Instance.Log(curPlayer + " chose " + usedJury + " for deliberation");

                int modValue = (curPlayer.Side == Player.PlayerSide.Prosecution) ? 1 : -1;
                List<BoardObject> choices = game.FindBO(
                    (BoardObject htgo) =>
                    {
                        return (htgo.Properties.Contains(Property.Track) &&
                        ((htgo.Properties.Contains(Property.Jury) && htgo.Properties.Contains(Property.Sway))
                        || (htgo.Properties.Contains(Property.Aspect)))
                        && (htgo.Properties.Contains(usedJury.Aspects[0].Aspect) || htgo.Properties.Contains(usedJury.Aspects[1].Aspect) || htgo.Properties.Contains(usedJury.Aspects[2].Aspect)))
                        && ((Track)htgo).CanModifyByAction(modValue);
                    });

                BoardChoices boardChoices;
                curPlayer.ChoiceHandler.ChooseBoardObjects(choices,
                    HTUtility.GenActionValidateChoicesFunc(usedJury.ActionPoints, usedJury),
                    HTUtility.GenActionFilterChoicesFunc(usedJury.ActionPoints, usedJury),
                    HTUtility.GenActionChoicesCompleteFunc(usedJury.ActionPoints, usedJury),
                    game,
                    curPlayer,
                    "Select usage for " + usedJury.ActionPoints + " deliberation points",
                    out boardChoices);

                if (boardChoices.NotCancelled)
                {
                    string str = "";
                    foreach (BoardObject bo in boardChoices.SelectedObjs.Keys)
                    {
                        str += bo + " modified by " + modValue * boardChoices.SelectedObjs[bo] + "\n";
                        if (bo.GetType() == typeof(AspectTrack))
                        {
                            ((AspectTrack)bo).ModTrackByAction(modValue * boardChoices.SelectedObjs[bo]);
                        }
                        else
                        {
                            ((Track)bo).AddToValue(modValue * boardChoices.SelectedObjs[bo]);
                        }
                    }
                    FileLogger.Instance.Log(str);

                    break;
                }
            }

            return usedJury;
        }
    }
}
