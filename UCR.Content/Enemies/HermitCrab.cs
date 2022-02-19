using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;

namespace UltimateCustomRun.Enemies
{
    public class HermitCrab : EnemyBase
    {
        public static bool spdtw;
        public override string Name => ":::: Enemies :: Hermit Crab";

        public override void Init()
        {
            spdtw = ConfigOption(false, "Make Hermit Crab faster?", "Vanilla is false. Recommended Value: true");
            base.Init();
        }

        public override void Hooks()
        {
            Buff();
        }
        public static void Buff()
        {
            var body = Resources.Load<CharacterBody>("prefabs/characterbodies/HermitCrabBody");

            if (spdtw)
            {
                body.baseMoveSpeed = 22f;
            }
        }
    }
}
