using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Necromancy.Empowerments
{
	public class EmpowermentDodge : Empowerment
    {
        public EmpowermentDodge(int time, int owner, int maxTime, bool flag, int power) : base(time, owner, maxTime, flag, power) { }

        public override Color Color
        {
            get { return new Color(1f, 1f, 0f); }
        }

        public override Point Frame
        {
            get { return new Point(2, 1); }
        }

        public override string Text
        {
            get { return "+" + power / 10 + "%"; }
        }

        public override string EmpDisplayName
        {
            get { return "Dodge"; }
        }

        public override void Behavior()
        {
            Player.GetModPlayer<NecromancyPlayer>().dodgeChance += power / 1000f;
        }
    }
}