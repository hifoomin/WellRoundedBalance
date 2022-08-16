using MonoMod.Cil;

namespace UltimateCustomRun.Items.Whites
{
    public class StunGrenade : ItemBase
    {
        public static float Chance;
        public static float Duration;

        public override string Name => ":: Items : Whites :: Stun Grenade";
        public override string InternalPickupToken => "stunChanceOnHit";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "<style=cIsUtility>" + Chance + "%</style> <style=cStack>(+" + Chance + "% on stack)</style> Chance on hit to <style=cIsUtility>stun</style> enemies for <style=cIsUtility>" + Duration + " seconds</style>.";

        public override void Init()
        {
            Chance = ConfigOption(5f, "Chance", "Vanilla is 5");
            Duration = ConfigOption(2f, "Stun Duration", "Vanilla is 2");
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.SetStateOnHurt.OnTakeDamageServer += ChangeChance;
            IL.RoR2.SetStateOnHurt.OnTakeDamageServer += ChangeDuration;
        }

        private void ChangeDuration(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(2f),
                x => x.MatchCallOrCallvirt<RoR2.SetStateOnHurt>("SetStun")))
            {
                c.Next.Operand = Duration;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Stun Grenade Duration hook");
            }
        }

        private void ChangeChance(On.RoR2.SetStateOnHurt.orig_OnTakeDamageServer orig, RoR2.SetStateOnHurt self, RoR2.DamageReport damageReport)
        {
            RoR2.SetStateOnHurt.stunChanceOnHitBaseChancePercent = Chance;
            orig(self, damageReport);
        }
    }
}