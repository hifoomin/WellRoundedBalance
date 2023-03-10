using MonoMod.Cil;
using RoR2.Orbs;
using System;

namespace WellRoundedBalance.Items.VoidGreens
{
    public class Polylute : ItemBase
    {
        public override string Name => ":: Items :::::: Voids :: Polylute";
        public override string InternalPickupToken => "chainLightningVoid";

        public override string PickupText => "Chance to repeatedly strike a single enemy with lightning. <style=cIsVoid>Corrupts all Ukuleles</style>.";
        public override string DescText => "<style=cIsDamage>" + chance + "%</style> chance to fire <style=cIsDamage>lightning</style> for <style=cIsDamage>" + d(totalDamage) + "</style> TOTAL damage up to <style=cIsDamage>" + strikeCount + "<style=cStack> (+" + strikeCountPerStack + " per stack)</style></style> times. <style=cIsVoid>Corrupts all Ukuleles</style>.";

        [ConfigField("TOTAL Damage", "Decimal.", 0.4f)]
        public static float totalDamage;

        [ConfigField("Chance", "", 25f)]
        public static float chance;

        [ConfigField("Base Strike Count", "", 2)]
        public static int strikeCount;

        [ConfigField("Strike Count Per Stack", "", 2)]
        public static int strikeCountPerStack;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
        }

        private void GlobalEventManager_OnHitEnemy(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchCallOrCallvirt(typeof(Util).GetMethod("CheckRoll",
                        new Type[] { typeof(float), typeof(CharacterMaster) })),
                    x => x.MatchBrfalse(out _),
                    x => x.MatchLdcR4(0.6f)))
            {
                c.Index += 2;
                c.Next.Operand = totalDamage;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Polylute Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchMul(),
                x => x.MatchStfld(typeof(VoidLightningOrb), "totalStrikes")))
            {
                c.Index += 4;
                c.EmitDelegate<Func<int, int>>((self) =>
                {
                    int newStrikes;
                    if (self > 3)
                    {
                        newStrikes = self - self / 3 + 1;
                    }
                    else
                    {
                        newStrikes = self;
                    }
                    return newStrikes;
                });
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Polylute Hit Count hook");
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

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdsfld("RoR2.DLC1Content/Items", "ChainLightningVoid"),
                    x => x.MatchCallOrCallvirt<Inventory>("GetItemCount"),
                    x => x.MatchStloc(out _),
                    x => x.MatchLdcR4(25f)))
            {
                c.Index += 3;
                c.Next.Operand = chance;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Polylute Chance hook");
            }
        }
    }
}