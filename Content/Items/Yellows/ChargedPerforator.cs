using MonoMod.Cil;
using RoR2;

namespace WellRoundedBalance.Items.Yellows
{
    public class ChargedPerforator : ItemBase
    {
        public override string Name => ":: Items :::: Yellows :: Charged Perforator";
        public override string InternalPickupToken => "lightningStrikeOnHit";

        public override string PickupText => "Chance on hit to call down a lightning strike.";

        public override string DescText => "<style=cIsDamage>10%</style> chance on hit to call down a lightning strike, dealing <style=cIsDamage>400%</style> <style=cStack>(+400% per stack)</style> TOTAL damage.";

        public override void Init()
        {
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
               x => x.MatchCallOrCallvirt<CharacterBody>("get_damage"),
               x => x.MatchLdcR4(5f)))
            {
                c.Index += 1;
                c.Next.Operand = 4f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Charged Perforator Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
               x => x.MatchStfld("RoR2.Orbs.GenericDamageOrb", "procChainMask"),
               x => x.MatchDup(),
               x => x.MatchLdcR4(1f)))
            {
                c.Index += 2;
                c.Next.Operand = 0f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Charged Perforator Proc Coefficient hook");
            }
        }
    }
}