using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Necromancy.Empowerments
{
	public class EmpowermentMoveSpeed : Empowerment
    {
        public EmpowermentMoveSpeed(int time, int owner, int maxTime, bool flag, int power) : base(time, owner, maxTime, flag, power) { }

        public override Color Color
        {
            get { return new Color(0f, 1f, 0.5f); }
        }

        public override Point Frame
        {
            get { return new Point(5, 1); }
        }

        public override string Text
        {
            get { return "+" + power / 4 + "%"; }
        }

        public override string EmpDisplayName
        {
            get { return "Movement Speed"; }
        }

        public override void Behavior()
        {
            Player.moveSpeed += power / 400f;
        }
    }
}