using MonoMod.Cil;

namespace WellRoundedBalance.Items.Reds
{
    public class SentientMeatHook : ItemBase<SentientMeatHook>
    {
        public override string Name => ":: Items ::: Reds :: Sentient Meat Hook";
        public override ItemDef InternalPickup => RoR2Content.Items.BounceNearby;

        public override string PickupText => "Chance to hook all nearby enemies.";

        public override string DescText => "<style=cIsDamage>20%</style> <style=cStack>(+20% per stack)</style> chance on hit to <style=cIsDamage>fire homing hooks</style> at up to <style=cIsDamage>" + baseMaxTargets + "</style> <style=cStack>(+" + maxTargetsPerStack + " per stack)</style> enemies for <style=cIsDamage>100%</style> TOTAL damage.";

        [ConfigField("Base Max Targets", 5)]
        public static int baseMaxTargets;

        [ConfigField("Max Targets Per Stack", 2)]
        public static int maxTargetsPerStack;

        [ConfigField("Range", 20f)]
        public static float range;

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
                c.Next.Operand = baseMaxTargets - maxTargetsPerStack;
                c.Index += 2;
                c.Next.Operand = maxTargetsPerStack;
            }
            else
            {
                Logger.LogError("Failed to apply Sentient Meat Hook Max Targets hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(30f),
                    x => x.MatchLdloc(66),
                    x => x.MatchLdloc(62)))
            {
                c.Next.Operand = range;
            }
            else
            {
                Logger.LogError("Failed to apply Sentient Meat Hook Range hook");
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
                Logger.LogError("Failed to apply Sentient Meat Hook Proc Coefficient hook");
            }
        }
    }
}