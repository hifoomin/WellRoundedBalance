using MonoMod.Cil;
using RoR2;
using RoR2.Orbs;
using System;

namespace WellRoundedBalance.Items.Greens
{
    public class Ukulele : ItemBase
    {
        public override string Name => ":: Items :: Greens :: Ukulele";
        public override string InternalPickupToken => "chainLightning";

        public override string PickupText => "...and his music was electric.";
        public override string DescText => "<style=cIsDamage>25%</style> Chance to fire <style=cIsDamage>chain lightning</style> for <style=cIsDamage>50%</style> TOTAL damage on up to <style=cIsDamage>3 <style=cStack>(+2 per stack)</style></style> targets within <style=cIsDamage>12m</style> <style=cStack>(+4m per stack)</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += Changes;
            ChangeTargetCountBase();
            ChangeRangeBase();
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
                c.Next.Operand = 25f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Ukulele Chance hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt(typeof(Util).GetMethod("CheckRoll", new Type[] { typeof(float), typeof(CharacterMaster) })),
                x => x.MatchBrfalse(out _),
                x => x.MatchLdcR4(0.8f)))
            {
                c.Index += 2;
                c.Next.Operand = 0.5f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Ukulele Total Damage hook");
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
                c.Next.Operand = 2;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Ukulele Target hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdfld<LightningOrb>("range"),
                    x => x.MatchLdcI4(2)))
            {
                c.Index += 1;
                c.Next.Operand = 4f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Ukulele Range hook");
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
                Main.WRBLogger.LogError("Failed to apply Ukulele Proc Coefficient hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.2f),
                x => x.MatchStfld<VoidLightningOrb>("procCoefficient")))
            {
                c.Next.Operand = 0f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Polylute Proc Coefficient hook");
            }
        }

        public static void ChangeTargetCountBase()
        {
            On.RoR2.Orbs.LightningOrb.Begin += (orig, self) =>
            {
                orig(self);
                if (self.lightningType is LightningOrb.LightningType.Ukulele)
                {
                    self.bouncesRemaining = 3;
                }
            };
        }

        public static void ChangeRangeBase()
        {
            On.RoR2.Orbs.LightningOrb.Begin += (orig, self) =>
            {
                orig(self);
                if (self.lightningType is LightningOrb.LightningType.Ukulele)
                {
                    self.range = 12f;
                    // self.canBounceOnSameTarget = :TROLLGE:
                    // im scared of the warning :IL:
                }
            };
        }
    }
}