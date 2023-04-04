using MonoMod.Cil;
using RoR2.Orbs;
using System;

namespace WellRoundedBalance.Items.Greens
{
    public class Ukulele : ItemBase<Ukulele>
    {
        public override string Name => ":: Items :: Greens :: Ukulele";
        public override ItemDef InternalPickup => RoR2Content.Items.ChainLightning;

        public override string PickupText => "...and his music was electric.";
        public override string DescText => "<style=cIsDamage>" + chance + "%</style> Chance to fire <style=cIsDamage>chain lightning</style> for <style=cIsDamage>" + d(totalDamage) + "</style> TOTAL damage on up to <style=cIsDamage>" + baseMaxTargets + " <style=cStack>(+" + maxTargetsPerStack + " per stack)</style></style> targets within <style=cIsDamage>" + baseRange + "m</style> <style=cStack>(+" + rangePerStack + "m per stack)</style>.";

        [ConfigField("TOTAL Damage", "Decimal.", 0.6f)]
        public static float totalDamage;

        [ConfigField("Chance", 25f)]
        public static float chance;

        [ConfigField("Base Max Targets", 3)]
        public static int baseMaxTargets;

        [ConfigField("Max Targets Per Stack", 2)]
        public static int maxTargetsPerStack;

        [ConfigField("Base Range", 12f)]
        public static float baseRange;

        [ConfigField("Range Per Stack", 4f)]
        public static float rangePerStack;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += Changes;
            Changes();
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdsfld("RoR2.RoR2Content/Items", "ChainLightning"),
                    x => x.MatchCallOrCallvirt<Inventory>("GetItemCount"),
                    x => x.MatchStloc(out _),
                    x => x.MatchLdcR4(25f)))
            {
                c.Index += 3;
                c.Next.Operand = chance;
            }
            else
            {
                Logger.LogError("Failed to apply Ukulele Chance hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt(typeof(Util).GetMethod("CheckRoll", new Type[] { typeof(float), typeof(CharacterMaster) })),
                x => x.MatchBrfalse(out _),
                x => x.MatchLdcR4(0.8f)))
            {
                c.Index += 2;
                c.Next.Operand = totalDamage;
            }
            else
            {
                Logger.LogError("Failed to apply Ukulele Total Damage hook");
            }
            // oh wow util.checkroll is stupid why tf are there two methods named the same
            // thank you harb :)

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchStfld<LightningOrb>("isCrit"),
                    x => x.MatchLdloc(out _),
                    x => x.MatchLdcI4(2)))
            {
                c.Index += 2;
                c.Next.Operand = maxTargetsPerStack;
            }
            else
            {
                Logger.LogError("Failed to apply Ukulele Target hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdfld<LightningOrb>("range"),
                    x => x.MatchLdcI4(2)))
            {
                c.Index += 1;
                c.Next.Operand = rangePerStack;
            }
            else
            {
                Logger.LogError("Failed to apply Ukulele Range hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdflda<LightningOrb>("procChainMask"),
                    x => x.MatchLdcI4(3),
                    x => x.MatchCallOrCallvirt<ProcChainMask>("AddProc"),
                    x => x.MatchLdloc(out _),
                    x => x.MatchLdcR4(0.2f)))
            {
                c.Index += 4;
                c.Next.Operand = 0f;
            }
            else
            {
                Logger.LogError("Failed to apply Ukulele Proc Coefficient hook");
            }
        }

        private void Changes()
        {
            On.RoR2.Orbs.LightningOrb.Begin += (orig, self) =>
            {
                orig(self);
                if (self.lightningType is LightningOrb.LightningType.Ukulele)
                {
                    self.bouncesRemaining = baseMaxTargets;
                    self.range = baseRange;
                }
            };
        }
    }
}