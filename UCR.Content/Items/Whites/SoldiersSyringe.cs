using MonoMod.Cil;

namespace UltimateCustomRun.Items.Whites
{
    public class SoldiersSyringe : ItemBase
    {
        public static float AttackSpeed;

        public override string Name => ":: Items : Whites :: Soldiers Syringe";
        public override string InternalPickupToken => "syringe";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "Increases <style=cIsDamage>attack speed</style> by <style=cIsDamage>" + d(AttackSpeed) + " <style=cStack>(+" + d(AttackSpeed) + " per stack)</style></style>.";

        public override void Init()
        {
            AttackSpeed = ConfigOption(0.15f, "Attack Speed", "Decimal. Per Stack. Vanilla is 0.15");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeAS;
        }

        public static void ChangeAS(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(0.15f)))
            {
                c.Index += 1;
                c.Next.Operand = AttackSpeed;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Soldier's Syringe Attack Speed hook");
            }
        }
    }
}