using MonoMod.Cil;
using R2API;
using RoR2;

namespace UltimateCustomRun.Items.Whites
{
    public class RepulsionArmorPlate : ItemBase
    {
        public static float Armor;
        public static bool StackArmor;
        public static float FlatReduction;
        public static float MinimumDamage;

        public override string Name => ":: Items : Whites :: Repulsion Armor Plate";
        public override string InternalPickupToken => "repulsionArmorPlate";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => (Armor != 0f ? "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>" + Armor + "</style> " +
                                           (StackArmor ? "<style=cStack>(+" + Armor + " per stack)</style>. " : "") : "") +
                                           (FlatReduction != 0f ? "Reduce all <style=cIsDamage>incoming damage</style> by <style=cIsDamage>" + FlatReduction + "<style=cStack> (+" + FlatReduction + " per stack)</style></style>. Cannot be reduced below <style=cIsDamage>" + MinimumDamage + "</style>." : "");

        public override void Init()
        {
            FlatReduction = ConfigOption(5f, "Flat Damage Reduction", "Per Stack. Vanilla is 5");
            MinimumDamage = ConfigOption(1f, "Minimum Damage", "Vanilla is 1");
            Armor = ConfigOption(0f, "Armor", "Vanilla is 0");
            StackArmor = ConfigOption(false, "Stack Armor?", "Vanilla is false");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += ChangeReduction;
            IL.RoR2.HealthComponent.TakeDamage += ChangeMinimum;
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
        }

        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.ArmorPlate);
                if (stack > 0)
                {
                    args.armorAdd += (StackArmor ? Armor * stack : Armor);
                }
            }
        }

        public static void ChangeReduction(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(1),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(5)
            );
            c.Index += 2;
            c.Next.Operand = FlatReduction;
        }

        public static void ChangeMinimum(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                //x => x.MatchLdflda<HealthComponent>("itemCounts"),
                //x => x.MatchLdfld<HealthComponent>("armorPlate"),
                x => x.MatchLdcI4(0),
                x => x.MatchBle(out _),
                x => x.MatchLdcR4(1)
            );
            c.Index += 2;
            c.Next.Operand = MinimumDamage;
        }
    }
}