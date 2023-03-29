using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Enemies.Standard
{
    internal class LunarWisp : EnemyBase
    {
        public override string Name => ":: Enemies ::::::::: Lunar Wisp";

        public override void Init()
        {
            base.Init();
        }

        [ConfigField("Should have damage falloff?", "", true)]
        public static bool shouldHaveDamageFalloff;

        [ConfigField("Director Credit Costt", "", 1500)]
        public static int directorCreditCost;

        public override void Hooks()
        {
            IL.EntityStates.LunarWisp.FireLunarGuns.OnFireAuthority += FireLunarGuns_OnFireAuthority;
            Changes();
            // HOPOO GAMESSSS ! !
        }

        private void FireLunarGuns_OnFireAuthority(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdfld(typeof(EntityStates.LunarWisp.FireLunarGuns), "muzzleNameOne")))
            {
                c.Index -= 25;
                c.EmitDelegate<Func<int, int>>((self) =>
                {
                    return shouldHaveDamageFalloff ? 1 : self;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Lunar Wisp Falloff hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdfld(typeof(EntityStates.LunarWisp.FireLunarGuns), "muzzleNameTwo")))
            {
                c.Index -= 25;
                c.EmitDelegate<Func<int, int>>((self) =>
                {
                    return shouldHaveDamageFalloff ? 1 : self;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Lunar Wisp Falloff hook");
            }
        }

        private void Changes()
        {
            var lunarWisp = Utils.Paths.CharacterSpawnCard.cscLunarWisp.Load<CharacterSpawnCard>();
            lunarWisp.directorCreditCost = directorCreditCost;
        }
    }
}