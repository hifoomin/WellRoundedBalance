using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Enemies.Bosses
{
    public class MagmaWorm : EnemyBase
    {
        public static bool AITweaks;
        public static bool SpeedTweaks;
        public override string Name => ":::: Enemies ::: Magma Worm";

        public override void Init()
        {
            AITweaks = ConfigOption(false, "Make Magma Worm AI smarter?", "Vanilla is false.\nRecommended Value: True");
            SpeedTweaks = ConfigOption(false, "Make Magma Worm faster?", "Vanilla is false.\nRecommended Value: True");
            base.Init();
        }

        public override void Hooks()
        {
            Buff();
        }

        public static void Buff()
        {
            var master = Resources.Load<CharacterMaster>("prefabs/charactermasters/MagmaWormMaster").GetComponent<CharacterMaster>();
            var body = Resources.Load<CharacterBody>("prefabs/characterbodies/MagmaWormBody");

            if (AITweaks)
            {
                On.EntityStates.MagmaWorm.SwitchStance.OnEnter += (orig, self) =>
                {
                    EntityStates.MagmaWorm.SwitchStance.leapStanceSpring = 1.25f;
                    EntityStates.MagmaWorm.SwitchStance.leapingDuration = 3f;
                    EntityStates.MagmaWorm.SwitchStance.leapStanceSpeedMultiplier = 1.5f;
                    EntityStates.MagmaWorm.SwitchStance.groundStanceSpring = 2f;
                    EntityStates.MagmaWorm.SwitchStance.groundStanceDamping = 2f;
                    orig(self);
                };
            }

            if (SpeedTweaks)
            {
                body.baseMoveSpeed = 35f;
                body.acceleration = 500f;
            }
        }
    }
}