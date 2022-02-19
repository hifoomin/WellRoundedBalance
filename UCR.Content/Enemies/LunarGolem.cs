using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;

namespace UltimateCustomRun.Enemies
{
    public class LunarGolem : EnemyBase
    {
        public static bool spdtw;
        public override string Name => ":::: Enemies :: Lunar Golem";

        public override void Init()
        {
            spdtw = ConfigOption(false, "Make Lunar Golem faster?", "Vanilla is false. Recommended Value: true");
            base.Init();
        }

        public override void Hooks()
        {
            Buff();
        }
        public static void Buff()
        {
            var body = Resources.Load<CharacterBody>("prefabs/characterbodies/LunarGolemBody");

            if (spdtw)
            {
                body.baseMoveSpeed = 16f;
            }
        }
    }
}
