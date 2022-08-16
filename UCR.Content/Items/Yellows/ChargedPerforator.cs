using MonoMod.Cil;
using RoR2;

namespace UltimateCustomRun.Items.Yellows
{
    public class ChargedPerforator : ItemBase
    {
        public static float Damage;
        public static float Chance;
        public static float ProcCoefficient;

        public override string Name => ":: Items :::: Yellows :: Charged Perforator";
        public override string InternalPickupToken => "lightningStrikeOnHit";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "<style=cIsDamage>" + Chance + "%</style> chance on hit to call down a lightning strike, dealing <style=cIsDamage>" + d(Damage) + "</style> <style=cStack>(+" + d(Damage) + " per stack)</style> TOTAL damage.";

        public override void Init()
        {
            Damage = ConfigOption(5f, "Damage", "Decimal. Per Stack. Vanilla is 5");
            Chance = ConfigOption(10f, "Chance", "Vanilla is 10");
            ProcCoefficient = ConfigOption(1f, "Proc Coefficient", "Vanilla is 1");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += Changes;
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<ProcChainMask>("HasProc"),
                x => x.MatchBrtrue(out _),
                x => x.MatchLdcR4(10f)))
            {
                c.Index += 2;
                c.Next.Operand = Chance;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Charged Perforator Chance hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
               x => x.MatchCallOrCallvirt<CharacterBody>("get_damage"),
               x => x.MatchLdcR4(5f)))
            {
                c.Index += 1;
                c.Next.Operand = Damage;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Charged Perforator Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
               x => x.MatchStfld("RoR2.Orbs.GenericDamageOrb", "procChainMask"),
               x => x.MatchDup(),
               x => x.MatchLdcR4(1f)))
            {
                c.Index += 2;
                c.Next.Operand = ProcCoefficient;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Charged Perforator Proc Coefficient hook");
            }
        }
    }
}