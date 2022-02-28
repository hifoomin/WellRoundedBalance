using MonoMod.Cil;
using RoR2;
using RoR2.Orbs;
using System;

namespace UltimateCustomRun.Items.Greens
{
    public class Ukulele : ItemBase
    {
        public static float Damage;
        public static float Chance;
        public static int Targets;
        public static int StackTargets;
        public static float Radius;
        public static int StackRadius;
        public static float ProcCoefficient;

        public override string Name => ":: Items :: Greens :: Ukulele";
        public override string InternalPickupToken => "chainLightning";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "<style=cIsDamage>" + Chance + "%</style> Chance to fire <style=cIsDamage>chain lightning</style> for <style=cIsDamage>" + d(Damage) + "</style> TOTAL damage on up to <style=cIsDamage>" + Targets + " <style=cStack>(+" + StackTargets + " per stack)</style></style> Targets within <style=cIsDamage>" + Radius + "m</style> <style=cStack>(+" + StackRadius + "m per stack)</style>.";

        public override void Init()
        {
            Damage = ConfigOption(0.8f, "Total Damage", "Decimal. Vanilla is 0.8");
            Chance = ConfigOption(25f, "Chance", "Vanilla is 25");
            ProcCoefficient = ConfigOption(0.2f, "Proc Coefficient", "Decimal. Vanilla is 0.2");
            Radius = ConfigOption(20f, "Base Range", "Vanilla is 20");
            StackRadius = ConfigOption(2, "Stack Range", "Per Stack. Vanilla is 2");
            Targets = ConfigOption(3, "Base Max Targets", "Vanilla is 3");
            StackTargets = ConfigOption(2, "Stack Max Targets", "Per Stack. Vanilla is 2");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeChance;
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeDamage;
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeProcCo;
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeRangeStack;
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeTargetCountStack;
            ChangeTargetCountBase();
            ChangeRangeBase();
        }

        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt(typeof(Util).GetMethod("CheckRoll", new Type[] { typeof(float), typeof(CharacterMaster) })),
                x => x.MatchBrfalse(out _),
                x => x.MatchLdcR4(0.8f)
            );
            c.Index += 2;
            c.Next.Operand = Damage;
            // oh wow util.checkroll is stupid why tf are there two methods named the same
            // thank you harb :)
        }

        public static void ChangeChance(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdsfld("RoR2.RoR2Content/Items", "ChainLightning"),
                x => x.MatchCallOrCallvirt<Inventory>("GetItemCount"),
                x => x.MatchStloc(out _),
                x => x.MatchLdcR4(25f)
            );
            c.Index += 3;
            c.Next.Operand = Chance;
        }

        public static void ChangeTargetCountStack(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchStfld<LightningOrb>("isCrit"),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcI4(2)
            );
            c.Index += 2;
            c.Next.Operand = StackTargets;
        }

        public static void ChangeTargetCountBase()
        {
            On.RoR2.Orbs.LightningOrb.Begin += (orig, self) =>
            {
                orig(self);
                if (self.lightningType is LightningOrb.LightningType.Ukulele)
                {
                    self.bouncesRemaining = Targets;
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
                    self.range = Radius;
                    // self.canBounceOnSameTarget = :TROLLGE:
                    // im scared of the warning :IL:
                }
            };
        }

        public static void ChangeRangeStack(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdfld<LightningOrb>("range"),
                x => x.MatchLdcI4(2)
            );
            c.Index += 1;
            c.Next.Operand = StackRadius;
        }

        public static void ChangeProcCo(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdflda<LightningOrb>("procChainMask"),
                x => x.MatchLdcI4(3),
                x => x.MatchCallOrCallvirt<ProcChainMask>("AddProc"),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(0.2f)

            );
            c.Index += 4;
            c.Next.Operand = ProcCoefficient;
        }
    }
}