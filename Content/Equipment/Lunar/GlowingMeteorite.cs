using MonoMod.Cil;
using System;
using UnityEngine.Rendering.PostProcessing;

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

        [ConfigField("Minimum Wave Interval", "", 1f)]
        public static float minimumWaveInterval;

        [ConfigField("Maximum Wave Interval", "", 1.5f)]
        public static float maximumWaveInterval;

        [ConfigField("Duration", "", 30)]
        public static int duration;

        [ConfigField("Cylinderify", "Make the hitbox a Cylinder?", true)]
        public static bool cylinder;

        [ConfigField("Cylinder Height", "Cylinder Height in Unity units", 10f)]
        public static float inches;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.MeteorStormController.DetonateMeteor += MeteorStormController_DetonateMeteor;
            On.RoR2.MeteorStormController.DetonateMeteor += MeteorStormController_DetonateMeteor1;
            IL.RoR2.BlastAttack.CollectHits += BlastAttack_CollectHits;
            IL.RoR2.MeteorStormController.FixedUpdate += MeteorStormController_FixedUpdate;
            Changes();
        }

        private void MeteorStormController_FixedUpdate(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdfld<MeteorStormController>("blastRadius")))
            {
                c.Index++;
                c.EmitDelegate<Func<float, float>>((useless) =>
                {
                    return radius;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Meteorite VFX Scale hook");
            }
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
                    // Main.WRBLogger.LogError("cylinder: " + cylinder);
                    // Main.WRBLogger.LogError("inflictor: " + self.inflictor);
                    // Main.WRBLogger.LogError("meteor storm controller: " + self.inflictor.GetComponent<MeteorStormController>());
                    var inflictor = self.inflictor;
                    if (inflictor)
                    {
                        var meteorStormController = self.inflictor.GetComponent<MeteorStormController>();
                        if (cylinder && meteorStormController)
                        {
                            return Physics.OverlapCapsule(position, position + new Vector3(0f, inches, 0f), radius, mask);
                        }
                    }

                    return Physics.OverlapSphere(position, radius, mask);
                });
            }
            else
            {
                Logger.LogError("Failed to apply Meteorite Hitbox hook");
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

        private void Changes()
        {
            var meteorStorm = Utils.Paths.GameObject.MeteorStorm.Load<GameObject>();
            var ppIn = meteorStorm.transform.GetChild(0).GetComponent<PostProcessVolume>().sharedProfile;
            var ppOut = meteorStorm.transform.GetChild(1).GetComponent<PostProcessVolume>().sharedProfile;

            var cgIn = ppIn.GetSetting<ColorGrading>();
            cgIn.contrast.value = 30f;
            cgIn.postExposure.value = 0.25f;
            cgIn.colorFilter.value = new Color32(255, 171, 204, 255);
            cgIn.gain.value.z = 0.7f;
            cgIn.tint.value = -15f;
            cgIn.colorFilter.overrideState = true;
            cgIn.colorFilter.value = new Color32(154, 160, 255, 255);
            cgIn.mixerRedOutRedIn.value = 140f;

            var cgOut = ppOut.GetSetting<ColorGrading>();
            cgOut.contrast.value = 30f;
            cgOut.postExposure.value = 0.25f;
            cgOut.colorFilter.value = new Color32(255, 171, 204, 255);
            cgOut.gain.value.z = 0.7f;
            cgOut.tint.value = -15f;
            cgOut.colorFilter.overrideState = true;
            cgOut.colorFilter.value = new Color32(154, 160, 255, 255);
            cgOut.mixerRedOutRedIn.value = 140f;
        }
    }
}