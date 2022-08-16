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
        public static float OODangerTimer;
        public static float OOCombatTimer;
        public override string Name => ": Global ::: Health";

        public override void Init()
        {
            OODangerTimer = ConfigOption(7f, "Out of Danger Timer", "Used for Shield Items, Cautious Slug and Oddly-shaped Opal. Vanilla is 7");
            OOCombatTimer = ConfigOption(5f, "Out of Combat Timer", "Used for Red Whip. Vanilla is 5");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.FixedUpdate += Changes;
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(7f)))
            {
                c.Next.Operand = OODangerTimer;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Out Of Danger hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
               x => x.MatchLdfld<CharacterBody>("outOfCombatStopwatch"),
               x => x.MatchLdcR4(5f)))
            {
                c.Index += 1;
                c.Next.Operand = OOCombatTimer;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Out Of Combat hook");
            }
        }
    }
}