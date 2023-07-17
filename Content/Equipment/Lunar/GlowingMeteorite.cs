using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Equipment.Lunar
{
    internal class GlowingMeteorite : EquipmentBase<GlowingMeteorite>
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

        [ConfigField("Cylinderify", "Make the hitbox a cylinder that go up to the sky rather than just the sphere.", true)]
        public static bool cylinder;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.MeteorStormController.DetonateMeteor += MeteorStormController_DetonateMeteor;
            On.RoR2.MeteorStormController.DetonateMeteor += MeteorStormController_DetonateMeteor1;
            IL.RoR2.BlastAttack.CollectHits += BlastAttack_CollectHits;
        }

        private void BlastAttack_CollectHits(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(x => x.MatchCallOrCallvirt<Physics>(nameof(Physics.OverlapSphere))))
            {
                c.Remove();
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<Vector3, float, int, BlastAttack, Collider[]>>((position, radius, mask, self) =>
                {
                    if (cylinder && self.inflictor.GetComponent<MeteorStormController>() != null) return Physics.OverlapCapsule(position, position + new Vector3(0, 2000, 0), radius, mask);
                    else return Physics.OverlapSphere(position, radius, mask);
                });
            }
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