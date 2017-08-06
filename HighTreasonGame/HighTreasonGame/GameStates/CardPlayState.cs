﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame.GameStates
{
    public abstract class CardPlayState : GameState
    {
        public CardPlayState(int _gameId) 
            : base(_gameId)
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

        protected virtual void mainLoop()
        {
            Game game = Game.GetGameFromId(gameId);

            while (true)
            {
                System.Console.WriteLine(game);

                game.CurPlayer.PlayCard(GetType());

                int numPlayersFinished = 0;
                List<Player> players = game.GetPlayers();
                players.ForEach(p => numPlayersFinished += p.MustPass() ? 1 : 0);
                if (numPlayersFinished == players.Count)
                {
                    break;
                }

                game.PassToNextPlayer();
            }

            loopEndLogic(game);

            GotoNextState();
        }

        protected virtual void loopEndLogic(Game game)
        {
            game.GetPlayers().ForEach(p => p.AddHandToSummation());
        }
    }
}