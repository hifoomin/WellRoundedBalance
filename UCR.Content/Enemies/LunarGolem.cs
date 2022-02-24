using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Enemies
{
    public class LunarGolem : EnemyBase
    {
        public static bool SpeedTweaks;
        public override string Name => ":::: Enemies :: Lunar Golem";

        public override void Init()
        {
            SpeedTweaks = ConfigOption(false, "Make Lunar Golem faster?", "Vanilla is false.\nRecommended Value: true");
            base.Init();
        }

        public override void Hooks()
        {
            Buff();
        }

        public static void Buff()
        {
            var body = Resources.Load<CharacterBody>("prefabs/characterbodies/LunarGolemBody");

            if (SpeedTweaks)
            {
                body.baseMoveSpeed = 16f;
            }
        }
    }
}