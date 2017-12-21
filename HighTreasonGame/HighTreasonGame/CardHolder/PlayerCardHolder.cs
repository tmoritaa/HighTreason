using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public class PlayerCardHolder : CardHolder
    {
        public Player Owner
        {
            get; protected set;
        }

        public PlayerCardHolder(HolderId id, Player player) : base(id)
        {
            Owner = player;
        }

        // Copy constructor
        public PlayerCardHolder(PlayerCardHolder holder, Player player)
            : base(holder)
        {
            Owner = player;
        }

        public override bool CheckCloneEquality(CardHolder holder)
        {
            bool equal = base.CheckCloneEquality(holder);

            equal &= Owner.Side == ((PlayerCardHolder)holder).Owner.Side;

            return equal;
        }
    }
}
