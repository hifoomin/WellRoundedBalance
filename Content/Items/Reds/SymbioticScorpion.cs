using MonoMod.Cil;

namespace WellRoundedBalance.Items.Reds
{
    public class SymbioticScorpion : ItemBase
    {
        public override string Name => ":: Items ::: Reds :: Symbiotic Scorpion";
        public override string InternalPickupToken => "permanentDebuffOnHit";

        public override string PickupText => "Chance to permanently reduce armor on hit.";

        public override string DescText => "Gain a <style=cIsDamage>60%</style> chance on hit to reduce <style=cIsDamage>armor</style> by <style=cIsDamage>2</style> <style=cStack>(+2 per stack)</style> <style=cIsDamage>permanently</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += ChangeChance;
        }

        public static void ChangeChance(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdsfld("RoR2.DLC1Content/Items", "PermanentDebuffOnHit"),
                    x => x.MatchCallOrCallvirt<RoR2.Inventory>("GetItemCount"),
                    x => x.MatchStloc(out _),
                    x => x.MatchLdcI4(0),
                    x => x.MatchStloc(out _),
                    x => x.MatchLdcI4(0),
                    x => x.MatchStloc(out _),
                    x => x.MatchBr(out _),
                    x => x.MatchLdcR4(100f)))
            {
                c.Index += 8;
                c.Next.Operand = 60f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Symbiotic Scorpion Chance hook");
            }
        }
    }
}