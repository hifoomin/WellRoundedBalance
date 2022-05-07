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
            Damage = ConfigOption(5f, "TOTAL Damage", "Decimal. Per Stack. Vanilla is 5");
            Chance = ConfigOption(10f, "Chance", "Vanilla is 10");
            ProcCoefficient = ConfigOption(1f, "Proc Coefficient", "Vanilla is 1");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeChance;
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeDamage;
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeProcCoefficient;
        }

        private void ChangeChance(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<ProcChainMask>("HasProc"),
                x => x.MatchBrtrue(out _),
                x => x.MatchLdcR4(10f)
            );
            c.Index += 2;
            c.Next.Operand = Chance;
        }

        private void ChangeDamage(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<CharacterBody>("get_damage"),
                x => x.MatchLdcR4(5f)
            );
            c.Index += 1;
            c.Next.Operand = Damage;
        }

        private void ChangeProcCoefficient(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchStfld("RoR2.Orbs.GenericDamageOrb", "procChainMask"),
                x => x.MatchDup(),
                x => x.MatchLdcR4(1f)
            );
            c.Index += 2;
            c.Next.Operand = ProcCoefficient;
        }
    }
}