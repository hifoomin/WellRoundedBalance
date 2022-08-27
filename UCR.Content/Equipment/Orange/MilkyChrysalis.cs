using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Equipment
{
    public class MilkyChrysalis : EquipmentBase
    {
        public override string Name => "::: Equipment :: Milky Chrysalis";
        public override string InternalPickupToken => "jetpack";

        public override bool NewPickup => false;

        public override bool NewDesc => true;

        public override string PickupText => "";

        public override string DescText => "Sprout wings and <style=cIsUtility>fly for " + Duration + " seconds</style>. Gain <style=cIsUtility>+" + d(SpeedBuff) + " movement speed</style> for the duration.";

        public static float Duration;
        public static float SpeedBuff;
        public static float GlideSpeed;
        public static float FlySpeed;
        public static float BoostSpeed;
        public static float BoostCooldown;

        public override void Init()
        {
            Duration = ConfigOption(15f, "Duration", "Vanilla is 15");
            SpeedBuff = ConfigOption(0.2f, "Movement Speed Boost", "Decimal. Vanilla is 0.2");
            GlideSpeed = ConfigOption(-5f, "Glide Speed", "Vanilla is -5");
            FlySpeed = ConfigOption(15f, "Initial Height Boost", "Vanilla is 15");
            BoostSpeed = ConfigOption(3f, "Boost Speed", "Vanilla is 3");
            BoostCooldown = ConfigOption(0.5f, "Boost Cooldown", "Vanilla is 0.5");
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.JetpackController.Start += Changes;
            IL.RoR2.JetpackController.StartFlight += ChangeFlySpeed;
            IL.RoR2.JetpackController.FixedUpdate += ChangeGlideSpeed;
            IL.RoR2.CharacterBody.RecalculateStats += ChangeSpeed;
        }

        private void ChangeSpeed(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                // x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("HasBuff"),
                x => x.MatchBrfalse(out _),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(0.2f)))
            {
                c.Index += 2;
                c.Next.Operand = SpeedBuff;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Milky Chrysalis Speed Buff hook");
            }
        }

        private void ChangeGlideSpeed(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(-5f)))
            {
                c.Next.Operand = GlideSpeed;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Milky Chrysalis Glide Speed hook");
            }
        }

        private void ChangeFlySpeed(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(15f)))
            {
                c.Next.Operand = FlySpeed;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Milky Chrysalis Flight Speed hook");
            }
        }

        private void Changes(On.RoR2.JetpackController.orig_Start orig, JetpackController self)
        {
            self.duration = Duration;
            Debug.Log("boost cooldown is " + self.boostCooldown);
            Debug.Log("boost speed mult is " + self.boostSpeedMultiplier);
            orig(self);
        }
    }
}