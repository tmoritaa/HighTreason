using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame.GameStates
{
    public class DeliberationState : GameState
    {
        public Dictionary<Player.PlayerSide, bool> playerSidePassed = new Dictionary<Player.PlayerSide, bool>();
        public BoardChoices juryChoice = null;

        private class PrecalcSubstate : GameSubState
        {
            bool skipToGameEnd = false;

            public PrecalcSubstate(GameState _parent) : base(_parent)
            {
            }

            public override void PreRun(Game game, Player curPlayer)
            {
                game.Board.Juries.ForEach(j => j.RevealAllTraits());

                EvidenceTrack guiltTrack = game.Board.GetGuiltTrack();

                if (guiltTrack.Value < 2)
                {
                    skipToGameEnd = true;
                }

                handleGuiltTrackEffect(game);
                handleInsanityTrackEffect(game);
            }

            public override Action RequestAction(Game game, Player curPlayer)
            {
                return null;
            }

            public override void HandleRequestAction(Action action)
            {
                // Do nothing.
            }

            public override void RunRest(Game game, Player curPlayer)
            {
            }

            public override void PrepareNextSubstate()
            {
                if (skipToGameEnd)
                {
                    parentState.SetNextSubstate(typeof(GameEndSubstate));
                }
                else
                {
                    parentState.SetNextSubstate(typeof(DeliberationJuryChoiceSubstate));
                }
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
        }

        private class DeliberationJuryChoiceSubstate : GameSubState
        {
            private List<Jury> usedJuries = new List<Jury>();

            public DeliberationJuryChoiceSubstate(GameState _parent) : base(_parent)
            {
            }

            public override void PreRun(Game game, Player curPlayer)
            {
                ((DeliberationState)parentState).juryChoice = null;

                if (game.NotifyStartOfTurn != null)
                {
                    game.NotifyStartOfTurn();
                }
            }

            public override Action RequestAction(Game game, Player curPlayer)
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
                    return
                    new Action(
                        ChoiceHandler.ChoiceType.BoardObjects,
                        curPlayer.ChoiceHandler,
                        lockedJuries.Cast<BoardObject>().ToList(),
                        (Func<Dictionary<BoardObject, int>, bool>)((Dictionary<BoardObject, int> selected) => { return true; }),
                        (Func<List<BoardObject>, Dictionary<BoardObject, int>, List<BoardObject>>)((List<BoardObject> remainingChoices, Dictionary<BoardObject, int> selected) =>
                        {
                            return remainingChoices.Where(obj => !selected.ContainsKey(obj)).ToList();
                        }),
                        (Func<Dictionary<BoardObject, int>, bool>)((Dictionary<BoardObject, int> selected) => { return selected.Keys.Count == 1; }),
                        game,
                        curPlayer,
                        "Select Jury for Deliberation");
                }
                else
                {
                    ((DeliberationState)parentState).playerSidePassed[curPlayer.Side] = true;
                    return null;
                }
            }

            public override void HandleRequestAction(Action action)
            {
                if (action != null)
                {
                    ((DeliberationState)parentState).juryChoice = (BoardChoices)action.ChoiceResult;
                }
            }

            public override void RunRest(Game game, Player curPlayer)
            {
                BoardChoices boardChoices = ((DeliberationState)parentState).juryChoice;
                if (boardChoices != null && boardChoices.NotCancelled)
                {
                    Jury usedJury = (Jury)boardChoices.SelectedObjs.Keys.First();
                    usedJuries.Add(usedJury);
                    FileLogger.Instance.Log(curPlayer + " chose " + usedJury + " for deliberation");
                }
                else if (boardChoices == null)
                {
                    if (!((DeliberationState)parentState).playerSidePassed.ContainsKey(game.GetOtherPlayer(curPlayer).Side))
                    {
                        parentState.PassToNextPlayer();
                    }
                }
            }

            public override void PrepareNextSubstate()
            {
                if (((DeliberationState)parentState).playerSidePassed.Count >= 2)
                {
                    parentState.SetNextSubstate(typeof(GameEndSubstate));
                }

                BoardChoices boardChoices = ((DeliberationState)parentState).juryChoice;
                if (boardChoices != null && boardChoices.NotCancelled)
                {
                    parentState.SetNextSubstate(typeof(DeliberationActionSelectSubstate));
                }
            }
        }

        private class DeliberationActionSelectSubstate : GameSubState
        {
            private BoardChoices boardChoices;

            public DeliberationActionSelectSubstate(GameState _parent) : base(_parent)
            {}

            public override void PreRun(Game game, Player curPlayer)
            {
                // Do nothing.
            }

            public override Action RequestAction(Game game, Player curPlayer)
            {
                Jury usedJury = (Jury)((DeliberationState)parentState).juryChoice.SelectedObjs.Keys.First();

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

                return new Action(
                    ChoiceHandler.ChoiceType.BoardObjects,
                    curPlayer.ChoiceHandler,
                    choices,
                    HTUtility.GenActionValidateChoicesFunc(usedJury.ActionPoints, usedJury),
                    HTUtility.GenActionFilterChoicesFunc(usedJury.ActionPoints, usedJury),
                    HTUtility.GenActionChoicesCompleteFunc(usedJury.ActionPoints, usedJury),
                    game,
                    curPlayer,
                    "Select usage for " + usedJury.ActionPoints + " deliberation points");
            }

            public override void HandleRequestAction(Action action)
            {
                boardChoices = (BoardChoices)action.ChoiceResult;
            }

            public override void RunRest(Game game, Player curPlayer)
            {
                if (boardChoices.NotCancelled)
                {
                    int modValue = (curPlayer.Side == Player.PlayerSide.Prosecution) ? 1 : -1;

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

                    if (!((DeliberationState)parentState).playerSidePassed.ContainsKey(game.GetOtherPlayer(curPlayer).Side))
                    {
                        parentState.PassToNextPlayer();
                    }
                }
            }

            public override void PrepareNextSubstate()
            {
                parentState.SetNextSubstate(typeof(DeliberationJuryChoiceSubstate));
            }
        }

        private class GameEndSubstate : GameSubState
        {
            public GameEndSubstate(GameState parent) : base(parent)
            { }

            public override void PreRun(Game game, Player curPlayer)
            {
                EvidenceTrack guiltTrack = game.Board.GetGuiltTrack();

                if (guiltTrack.Value < 2)
                {
                    if (game.NotifyGameEnd != null)
                    {
                        game.NotifyGameEnd(Player.PlayerSide.Defense, true, 0);
                    }

                    return;
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
            }

            public override void HandleRequestAction(Action action)
            {
                // Do nothing.
            }

            public override Action RequestAction(Game game, Player curPlayer)
            {
                return null;
            }

            public override void RunRest(Game game, Player curPlayer)
            {
                parentState.SignifyStateEnd();
            }

            public override void PrepareNextSubstate()
            {
                // Do nothing.
            }
        }

        public DeliberationState(Game _game) 
            : base(GameStateType.Deliberation, _game)
        {}

        public override void InitState()
        {
            base.InitState();

            if (game.StartState == this.StateType)
            {
                game.Board.Juries.RemoveRange(0, 6);
            }

            curPlayer = game.GetPlayerOfSide(Player.PlayerSide.Prosecution);

            CurSubstate = substates[typeof(PrecalcSubstate)];

            if (game.NotifyStateStart != null)
            {
                game.NotifyStateStart();
            }
        }

        public override void GotoNextState()
        {
            game.SignifyEndGame();
        }

        protected override void initSubStates(GameState parent)
        {
            substates.Add(typeof(PrecalcSubstate), new PrecalcSubstate(this));
            substates.Add(typeof(DeliberationJuryChoiceSubstate), new DeliberationJuryChoiceSubstate(this));
            substates.Add(typeof(DeliberationActionSelectSubstate), new DeliberationActionSelectSubstate(this));
            substates.Add(typeof(GameEndSubstate), new GameEndSubstate(this));
        }
    }
}
