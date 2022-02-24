using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Enemies
{
    public class BighornBison : EnemyBase
    {
        public static float Duration;
        public static bool Tweaks;
        public override string Name => ":::: Enemies :: Bighorn Bison";

        public override void Init()
        {
            Duration = ConfigOption(2.5f, "Headbutt Duration", "Vanilla is 2.5,\nRecommended Value: 1");
            Tweaks = ConfigOption(false, "Enable Charge Tweaks and AI Tweaks?", "Vanilla is false.\nRecommended Value: true");
            base.Init();
        }

        public override void Hooks()
        {
            Buff();
        }

        public static void Buff()
        {
            var body = Resources.Load<CharacterBody>("prefabs/characterbodies/BisonBody");

            On.EntityStates.Bison.Headbutt.OnEnter += (orig, self) =>
            {
                EntityStates.Bison.Headbutt.baseHeadbuttDuration = Duration;
                orig(self);
            };

            if (Tweaks)
            {
                body.GetComponent<CharacterDirection>().turnSpeed = 360f;
                On.EntityStates.Bison.Charge.OnEnter += (orig, self) =>
                {
                    EntityStates.Bison.Charge.chargeMovementSpeedCoefficient = 12f;
                    EntityStates.Bison.Charge.turnSpeed = 99999f;
                    EntityStates.Bison.Charge.turnSmoothTime = 0f;
                    EntityStates.Bison.Charge.selfStunDuration = 0.5f;
                    orig(self);
                };
            }
        }
    }
}