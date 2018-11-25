using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Necromancy.Empowerments
{
	public class EmpowermentCritChance : Empowerment
    {
        public EmpowermentCritChance(int time, int owner, int maxTime, bool flag, int power) : base(time, owner, maxTime, flag, power) { }

        public override Color Color
        {
            get { return new Color(1f, 0.5f, 0f); }
        }

        public override Point Frame
        {
            get { return new Point(1, 1); }
        }

        public override string Text
        {
            get { return "+" + power / 8 + "%"; }
        }

        public override string EmpDisplayName
        {
            get { return "Critical Strike Chance"; }
        }

        public override void Behavior()
        {
            Player.meleeCrit += power / 8;
            Player.rangedCrit += power / 8;
            Player.magicCrit += power / 8;
            Player.thrownCrit += power / 8;
        }
    }
}