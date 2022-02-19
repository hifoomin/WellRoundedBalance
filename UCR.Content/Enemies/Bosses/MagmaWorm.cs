using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;

namespace UltimateCustomRun.Enemies.Bosses
{
    public class MagmaWorm : EnemyBase
    {
        public static bool aitw;
        public static bool speedtw;
        public override string Name => ":::: Enemies ::: Magma Worm";

        public override void Init()
        {
            aitw = ConfigOption(false, "Make Magma Worm AI smarter?", "Vanilla is false. Recommended Value: True");
            speedtw = ConfigOption(false, "Make Magma Worm faster?", "Vanilla is false. Recommended Value: True");
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

            if (aitw)
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

            if (speedtw)
            {
                body.baseMoveSpeed = 35f;
                body.acceleration = 500f;
            }
        }
    }
}
