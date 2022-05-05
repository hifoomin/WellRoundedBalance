/*
using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using RoR2.Orbs;
using MonoMod.Cil;

namespace UltimateCustomRun.Global
{
    public class CombatDangerTimers : GlobalBase
    {
        public static float ood;
        public static float ooc;
        public override string Name => ": Global ::: Health";

        public override void Init()
        {
            ood = ConfigOption(7f, "Out of Danger Timer", "Used for Shield Items and Cautious Slug. Vanilla is 7");
            ooc = ConfigOption(5f, "Out of Combat Timer", "Used for Red Whip. Vanilla is 5");
            base.Init();
        }
        public override void Hooks()
        {
            IL.RoR2.CharacterBody.FixedUpdate += ChangeCombatDelay;
            IL.RoR2.CharacterBody.FixedUpdate += ChangeCombatDelay;
        }
        public static void ChangeDangerDelay(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdfld<CharacterBody>("outOfDangerStopwatch"),
                x => x.MatchLdcR4(7f)
            );
            c.Index += 1;
            c.Next.Operand = ooc;
        }
        public static void ChangeCombatDelay(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
               x => x.MatchLdfld<CharacterBody>("outOfCombatStopwatch"),
               x => x.MatchLdcR4(5f)
            );
            c.Index += 1;
            c.Next.Operand = ood;
        }
    }
}
*/