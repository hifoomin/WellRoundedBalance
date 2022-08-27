using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UltimateCustomRun.Items.Yellows
{
    public class MiredUrn : ItemBase
    {
        public static int MaxTargets;
        public static float Range;
        public static float Damage;
        public static float Interval;
        public static float Healing;
        public static float ProcCoefficient;

        public override string Name => ":: Items :::: Yellows :: Mired Urn";
        public override string InternalPickupToken => "siphonOnLowHealth";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "While in combat, the nearest " + MaxTargets + " <style=cStack>(+" + MaxTargets + " per stack)</style> enemies to you within <style=cIsDamage>" + Range + "m</style> will be 'tethered' to you, <style=cIsUtility>slowing</style> them down by <style=cIsUtility>-" + Util.ConvertAmplificationPercentageIntoReductionPercentage(50f) + "%</style>, and <style=cIsHealing>healing</style> you for <style=cIsHealing>100%</style> of the damage dealt.";

        public override void Init()
        {
            MaxTargets = ConfigOption(1, "Max TARgets", "Per Stack. Vanilla is 1");
            Range = ConfigOption(13f, "Radius", "Vanilla is 13");
            Damage = ConfigOption(1f, "Damage", "Decimal. Vanilla is 1");
            Interval = ConfigOption(0.25f, "Interval", "Vanilla is 0.5");
            //Healing = ConfigOption(1f, "Healing from Damage Multiplier", "Decimal. Vanilla is 1");
            ProcCoefficient = ConfigOption(0f, "Proc Coefficient", "Vanilla is 0");
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.SiphonNearbyController.Awake += Changes;
            IL.RoR2.SiphonNearbyController.Tick += ChangeProcCoeff;
        }

        private void ChangeProcCoeff(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.0f),
                x => x.MatchStfld<DamageInfo>("procCoefficient")))
            {
                c.Next.Operand = ProcCoefficient;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Mired Urn Proc Coefficient hook");
            }
        }

        private void Changes(On.RoR2.SiphonNearbyController.orig_Awake orig, SiphonNearbyController self)
        {
            self.damagePerSecondCoefficient = Damage;
            self.maxTargets = MaxTargets;
            self.radius = Range;
            self.tickRate = 1 / Interval;
            orig(self);
        }
    }
}