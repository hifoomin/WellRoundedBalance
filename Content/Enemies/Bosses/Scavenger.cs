using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Enemies.Bosses
{
    internal class Scavenger : EnemyBase<Scavenger>
    {
        public override string Name => "::: Bosses ::::: Scavenger";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        private void Changes()
        {
            var grove1 = Utils.Paths.DirectorCardCategorySelection.dccsRootJungleMonsters.Load<DirectorCardCategorySelection>();
            Array.Resize(ref grove1.categories[3].cards, 0);

            var grove2 = Utils.Paths.DirectorCardCategorySelection.dccsRootJungleMonstersDLC1.Load<DirectorCardCategorySelection>();
            Array.Resize(ref grove2.categories[3].cards, 0);
        }
    }
}