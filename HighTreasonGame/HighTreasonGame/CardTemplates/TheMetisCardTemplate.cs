using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    [CardTemplateAttribute]
    public class TheMetisCardTemplate : CardTemplate
    {
        public TheMetisCardTemplate()
            : base("The Metis", 4, Player.PlayerSide.Defense)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(
                new CardEffectPair(
                    (Game game, Player choosingPlayer, ChoiceHandler choiceHandler) =>
                    {
                        BoardChoices boardChoices;

                        choiceHandler.ChooseCards(
                            game.Discards.Cards,
                            (Dictionary<Card, int> selected) => { return true; },
                            (List<Card> remainingChoices, Dictionary<Card, int> selected) => { return remainingChoices; },
                            (Dictionary<Card, int> selected, bool isDone) => { return selected.Count == 1; },
                            false,
                            game,
                            choosingPlayer,
                            CardInfo.JurySelectionInfos[0].Description,
                            out boardChoices);

                        if (boardChoices.NotCancelled)
                        {                            
                            Card card = boardChoices.SelectedCards.Keys.First();

                            choiceHandler.ChooseCardEffect(card, game, choosingPlayer, "Select Jury Selection event to play", out boardChoices.PlayInfo);

                            if (boardChoices.NotCancelled)
                            {
                                int idx = boardChoices.PlayInfo.eventIdx;
                                boardChoices.PlayInfo.resultBoardChoice = card.Template.SelectionEvents[idx].CardChoice(game, choosingPlayer, choiceHandler);

                                boardChoices.NotCancelled = boardChoices.PlayInfo.resultBoardChoice.NotCancelled;
                            }
                        }

                        return boardChoices;
                    },
                    (Game game, Player choosingPlayer, BoardChoices boardChoices) =>
                    {
                        Card card = boardChoices.SelectedCards.Keys.First();
                        int idx = boardChoices.PlayInfo.eventIdx;
                        card.Template.SelectionEvents[idx].CardEffect(game, choosingPlayer, boardChoices.PlayInfo.resultBoardChoice);
                    },
                    (Game game, Player choosingPlayer) =>
                    {
                        return choosingPlayer.Hand.Cards.Count == 3;
                    }));
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        List<AspectTrack> ats = findAspectTracksWithProp(game, Property.English, Property.French);
                        ats.ForEach(t => t.AddToValue(t.Properties.Contains(Property.English) ? 2 : 3));
                    }));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    (Game game, Player choosingPlayer, ChoiceHandler choiceHandler) =>
                    {
                        List<BoardObject> choices = game.FindBO(
                            (BoardObject bo) =>
                            {
                                return 
                                    bo.Properties.Contains(Property.Aspect) 
                                    && bo.Properties.Contains(Property.Track) 
                                    && !bo.Properties.Contains(Property.GovWorker) 
                                    && !bo.Properties.Contains(Property.Merchant);
                            });

                        BoardChoices boardChoices;
                        choiceHandler.ChooseBoardObjects(
                            choices,
                            (Dictionary<BoardObject, int> selected) => { return true; },
                            (List<BoardObject> remainingChoices, Dictionary<BoardObject, int> selected) =>
                            {
                                return remainingChoices.Where(obj => !selected.ContainsKey(obj)).ToList();
                            },
                            (Dictionary<BoardObject, int> selected) => { return selected.Keys.Count == 3; },
                            game,
                            choosingPlayer,
                            CardInfo.SummationInfos[0].Description,
                            out boardChoices);

                        return boardChoices;
                    },
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        choices.SelectedObjs.Keys.Cast<AspectTrack>().ToList().ForEach(t => t.AddToValue(-2));

                        findAspectTracksWithProp(game, Property.GovWorker, Property.Merchant).ForEach(t => t.AddToValue(1));
                    }));
        }
    }
}
