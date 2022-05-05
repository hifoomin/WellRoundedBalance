using MonoMod.Cil;

namespace UltimateCustomRun.Items.Reds
{
    public class SymbioticScorpion : ItemBase
    {
        public static float Chance;
        public static float ArmorDecrease;
        public override string Name => ":: Items ::: Reds :: Symbiotic Scorpion";
        public override string InternalPickupToken => "permanentDebuffOnHit";
        public override bool NewPickup => true;

        public override string PickupText => (Chance < 100f ? "Chance to permanently" : "Permanently") +
                                             " reduce armor on hit.";

        public override string DescText => "<style=cIsDamage>" + d(Chance) + "</style> chance on hit to reduce <style=cIsDamage>armor</style> by <style=cIsDamage>" + ArmorDecrease + "</style> <style=cStack>(+" + ArmorDecrease + " per stack)</style> <style=cIsDamage>permanently</style>.";

        public override void Init()
        {
            Chance = ConfigOption(1f, "Debuff Chance", "Decimal. Vanilla is 1");
            ArmorDecrease = ConfigOption(2f, "Debuff Armor Decrease", "Per Debuff. Per Stack. Vanilla is 2");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += ChangeChance;
            IL.RoR2.CharacterBody.RecalculateStats += ChangeArmor;
        }

        public static void ChangeChance(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdsfld<RoR2.DLC1Content>("PermanentDebuffOnHit"),
                x => x.MatchCallOrCallvirt<RoR2.Inventory>("GetItemCount"),
                x => x.MatchStloc(out _),
                x => x.MatchLdcI4(0),
                x => x.MatchStloc(out _),
                x => x.MatchLdcI4(0),
                x => x.MatchStloc(out _),
                x => x.MatchBr(out _),
                x => x.MatchLdcR4(100f)
            );
            c.Index += 8;
            c.Next.Operand = Chance;
        }

        public static void ChangeArmor(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("get_armor"),
                x => x.MatchLdloc(out _),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(2f)
            );
            c.Index += 3;
            c.Next.Operand = ArmorDecrease;
        }
    }
}