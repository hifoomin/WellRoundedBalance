using MonoMod.Cil;
using R2API;
using RoR2;

namespace WellRoundedBalance.Items.Reds
{
    public class SentientMeatHook : ItemBase
    {
        public override string Name => ":: Items ::: Reds :: Sentient Meat Hook";
        public override string InternalPickupToken => "bounceNearby";

        public override string PickupText => "Chance to hook all nearby enemies.";

        public override string DescText => "<style=cIsDamage>20%</style> <style=cStack>(+20% per stack)</style> chance on hit to <style=cIsDamage>fire homing hooks</style> at up to <style=cIsDamage>5</style> <style=cStack>(+2 per stack)</style> enemies for <style=cIsDamage>100%</style> TOTAL damage.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += Changes;
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcI4(5),
                    x => x.MatchLdloc(13),
                    x => x.MatchLdcI4(5)))
            {
                c.Next.Operand = 3;
                c.Index += 2;
                c.Next.Operand = 2;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Sentient Meat Hook Max Targets hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(30f),
                    x => x.MatchLdloc(66),
                    x => x.MatchLdloc(62)))
            {
                c.Next.Operand = 20f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Sentient Meat Hook Range hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdloc(71),
                    x => x.MatchLdcR4(0.33f)))
            {
                c.Index += 1;
                c.Next.Operand = 0f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Sentient Meat Hook Proc Coefficient hook");
            }
        }
    }
}