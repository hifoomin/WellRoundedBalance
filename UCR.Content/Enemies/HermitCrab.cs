using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Enemies
{
    public class HermitCrab : EnemyBase
    {
        public static bool SpeedTweaks;
        public override string Name => ":::: Enemies :: Hermit Crab";

        public override void Init()
        {
            SpeedTweaks = ConfigOption(false, "Make Hermit Crab faster?", "Vanilla is false.\nRecommended Value: true");
            base.Init();
        }

        public override void Hooks()
        {
            Buff();
        }

        public static void Buff()
        {
            var body = Resources.Load<CharacterBody>("prefabs/characterbodies/HermitCrabBody");

            if (SpeedTweaks)
            {
                body.baseMoveSpeed = 22f;
            }
        }
    }
}