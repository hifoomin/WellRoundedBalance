using MonoMod.Cil;
using UnityEngine;

namespace UltimateCustomRun.Items.Reds
{
    public class ResonanceDisc : ItemBase
    {
        public static int KillRequirement;
        public static int KillDuration;
        public static float BaseDuration;
        public static float BeamDamage;
        public static float BeamProcCoefficient;
        public static float BeamRadius;
        public static float BombDamage;
        public override string Name => ":: Items ::: Reds :: Resonance Disc";
        public override string InternalPickupToken => "laserTurbine";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "Killing <style=cIsDamage>4</style> enemies in <style=cIsUtility>7 seconds</style> charges the Resonance Disc. The disc launches itself toward a target for <style=cIsDamage>300%</style> base damage <style=cStack>(+300% per stack)</style>, piercing all enemies it doesn't kill, and then explodes for <style=cIsDamage>1000%</style> base damage <style=cStack>(+1000% per stack)</style>. Returns to the user, striking all enemies along the way for <style=cIsDamage>300%</style> base damage <style=cStack>(+300% per stack)</style>.";

        public override void Init()
        {
            KillRequirement = ConfigOption(4, "Kills Required", "Vanilla is 4");
            ROSOption("Greens", 0f, 10f, 1f, "3");
            KillDuration = ConfigOption(7, "Kill Duration", "Vanilla is 7");
            ROSOption("Greens", 0f, 10f, 1f, "3");
            BaseDuration = ConfigOption(7f, "Base Duration", "Vanilla is 3");
            ROSOption("Greens", 0f, 10f, 1f, "3");
            BeamDamage = ConfigOption(3f, "Beam Damage", "Vanilla is 3");
            ROSOption("Greens", 0f, 10f, 1f, "3");
            BeamProcCoefficient = ConfigOption(1f, "Beam Proc Coefficient", "Vanilla is 1");
            ROSOption("Greens", 0f, 1f, 0.05f, "3");
            BeamRadius = ConfigOption(1f, "Beam Radius", "Vanilla is 1");
            ROSOption("Greens", 0f, 10f, 1f, "3");
            BombDamage = ConfigOption(10f, "Explosion Damage", "Vanilla is 10");
            ROSOption("Greens", 0f, 30f, 1f, "3");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.LaserTurbine.RechargeState.OnEnter += ChangeRequirement;
            On.EntityStates.LaserTurbine.FireMainBeamState.OnEnter += Changes;
        }

        public static void Changes(On.EntityStates.LaserTurbine.FireMainBeamState.orig_OnEnter orig, EntityStates.LaserTurbine.FireMainBeamState self)
        {
            EntityStates.LaserTurbine.FireMainBeamState.mainBeamDamageCoefficient = BeamDamage;
            EntityStates.LaserTurbine.FireMainBeamState.mainBeamProcCoefficient = BeamProcCoefficient;
            EntityStates.LaserTurbine.FireMainBeamState.mainBeamRadius = BeamRadius;
            EntityStates.LaserTurbine.FireMainBeamState.secondBombDamageCoefficient = BombDamage;
            orig(self);
        }

        public static void ChangeRequirement(On.EntityStates.LaserTurbine.RechargeState.orig_OnEnter orig, EntityStates.LaserTurbine.RechargeState self)
        {
            EntityStates.LaserTurbine.RechargeState.baseDuration = BaseDuration;
            EntityStates.LaserTurbine.RechargeState.killChargeDuration = KillDuration;
            EntityStates.LaserTurbine.RechargeState.killChargesRequired = KillRequirement;
            orig(self);
        }
    }
}