using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Equipment.Lunar
{
    internal class GlowingMeteorite : EquipmentBase
    {
        public override string Name => ":: Equipment ::: Glowing Meteorite";

        public override EquipmentDef InternalPickup => RoR2Content.Equipment.Meteor;

        public override string PickupText => "Rain meteors from the sky, <color=#FF7F7F>hurting both enemies and allies.</color>";

        public override string DescText => "<style=cIsDamage>Rain meteors</style> from the sky, damaging ALL characters for <style=cIsDamage>" + d(damage) + " damage per blast</style>. Lasts " + duration + " seconds.";

        [ConfigField("Cooldown", "", 60f)]
        public static float cooldown;

        [ConfigField("Damage", "Decimal.", 3.8f)]
        public static float damage;

        [ConfigField("Radius", "", 9f)]
        public static float radius;

        [ConfigField("Knockback Strength", "", 6000f)]
        public static float knockbackStrength;

        [ConfigField("Minimum Wave Interval", "", 0.5f)]
        public static float minimumWaveInterval;

        [ConfigField("Maximum Wave Interval", "", 1f)]
        public static float maximumWaveInterval;

        [ConfigField("Duration", "", 30)]
        public static int duration;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.MeteorStormController.DetonateMeteor += MeteorStormController_DetonateMeteor;
            On.RoR2.MeteorStormController.DetonateMeteor += MeteorStormController_DetonateMeteor1;
        }

        private void MeteorStormController_DetonateMeteor1(On.RoR2.MeteorStormController.orig_DetonateMeteor orig, MeteorStormController self, object meteor)
        {
            self.blastDamageCoefficient = damage;
            self.blastRadius = radius;
            self.blastForce = knockbackStrength;
            self.waveMinInterval = minimumWaveInterval;
            self.waveMaxInterval = maximumWaveInterval;
            self.waveCount = duration;
            orig(self, meteor);
        }

        private void MeteorStormController_DetonateMeteor(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcI4(1),
                x => x.MatchStfld(typeof(BlastAttack), "falloffModel")))
            {
                c.Index += 1;
                c.EmitDelegate<Func<int, int>>((useless) =>
                {
                    return 0;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Glowing Meteorite Falloff hook");
            }
        }
    }
}