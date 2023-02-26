using MonoMod.Cil;

namespace WellRoundedBalance.Items.Whites
{
    public class RepulsionArmorPlate : ItemBase
    {
        public override string Name => ":: Items : Whites :: Repulsion Armor Plate";
        public override string InternalPickupToken => "repulsionArmorPlate";

        public override string PickupText => "Receive flat damage reduction from all attacks.";

        public override string DescText => "Reduce all <style=cIsDamage>incoming damage</style> by <style=cIsDamage>" + flatDamageReduction + "<style=cStack> (+" + flatDamageReduction + " per stack)</style></style>. Cannot be reduced below <style=cIsDamage>" + minimumDamage + "</style>.";

        [ConfigField("Flat Damage Reduction", "", 5f)]
        public static float flatDamageReduction;

        [ConfigField("Minimum Damage", "", 8f)]
        public static float minimumDamage;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += Changes;
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(1),
                    x => x.MatchLdloc(out _),
                    x => x.MatchLdcR4(5)))
            {
                c.Index += 2;
                c.Next.Operand = flatDamageReduction;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Repulsion Armor Plate Reduction hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,

               x => x.MatchLdcI4(0),
               x => x.MatchBle(out _),
               x => x.MatchLdcR4(1)))
            {
                c.Index += 2;
                c.Next.Operand = minimumDamage;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Repulsion Armor Plate Minimum hook");
            }
        }
    }
}