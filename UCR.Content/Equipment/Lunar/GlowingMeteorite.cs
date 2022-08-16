using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using R2API;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace UltimateCustomRun.Equipment
{
    public class GlowingMeteorite : EquipmentBase
    {
        public override string Name => "::: Equipment ::: Glowing Meteorite";
        public override string InternalPickupToken => "meteor";

        public override bool NewPickup => false;

        public override bool NewDesc => true;

        public override string PickupText => "";

        public override string DescText => "<style=cIsDamage>Rain meteors</style> from the sky, damaging ALL characters for <style=cIsDamage>600% damage per blast</style>. Lasts 20 seconds.";

        public static float Damage;
        public static float Radius;
        public static float Force;
        public static float MinInterval;
        public static float MaxInterval;
        public static int WaveCount;
        public static int FalloffType;

        public override void Init()
        {
            Damage = ConfigOption(6f, "Damage", "Decimal. Vanilla is 6 ");
            Radius = ConfigOption(8f, "Radius", "Vanilla is 8");
            Force = ConfigOption(4000f, "Force", "Vanilla is 4000");
            MinInterval = ConfigOption(0.5f, "Minimum Wave Interval", "Vanilla is 0.5");
            MaxInterval = ConfigOption(1.5f, "Maximum Wave Interval", "Vanilla is 1.5");
            WaveCount = ConfigOption(20, "Wave Count", "Vanilla is 20");
            FalloffType = ConfigOption(2, "Falloff Type", "1 - Sweetspot, 2 - Linear, 3 - None.\nVanilla is 2");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.MeteorStormController.DetonateMeteor += ChangeFalloff;
            On.RoR2.MeteorStormController.DetonateMeteor += Changes;
        }

        private void Changes(On.RoR2.MeteorStormController.orig_DetonateMeteor orig, MeteorStormController self, object meteor)
        {
            self.blastDamageCoefficient = Damage;
            self.blastRadius = Radius;
            self.blastForce = Force;
            self.waveMinInterval = MinInterval;
            self.waveMaxInterval = MaxInterval;
            self.waveCount = WaveCount;
            orig(self, meteor);
        }

        private void ChangeFalloff(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcI4(1),
                x => x.MatchStfld<BlastAttack>("falloffModel")))
            {
                c.Remove();
                var falloff = FalloffType switch
                {
                    1 => BlastAttack.FalloffModel.SweetSpot,
                    2 => BlastAttack.FalloffModel.Linear,
                    3 => BlastAttack.FalloffModel.None,
                    _ => BlastAttack.FalloffModel.Linear,
                };
                c.Emit(OpCodes.Ldc_I4, falloff);
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Glowing Meteorite Falloff hook");
            }
        }
    }
}