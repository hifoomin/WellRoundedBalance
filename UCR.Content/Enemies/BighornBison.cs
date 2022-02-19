using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Enemies
{
    public class BighornBison : EnemyBase
    {
        public static CharacterBody body;
        public static float aspd;
        public static bool makezased;
        public override string Name => ":::: Enemies :: Bighorn Bison";

        public override void Init()
        {
            aspd = ConfigOption(2.5f, "Headbutt Duration", "Vanilla is 2.5, Recommended Value: 1");
            makezased = ConfigOption(false, "Enable Charge Tweaks and AI Tweaks?", "Vanilla is false. Recommended Value: true");
            base.Init();
        }

        public override void Hooks()
        {
            Buff();
        }

        public static void Buff()
        {
            body = Resources.Load<CharacterBody>("prefabs/characterbodies/BisonBody");

            On.EntityStates.Bison.Headbutt.OnEnter += (orig,self) =>
            {
                EntityStates.Bison.Headbutt.baseHeadbuttDuration = aspd;
                orig(self);
            };

            if (makezased)
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
