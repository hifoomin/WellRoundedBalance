using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Enemies
{
    public class Jellyfish : EnemyBase
    {
        public static bool spdtw;
        public override string Name => ":::: Enemies :: Jellyfish";

        public override void Init()
        {
            spdtw = ConfigOption(false, "Make Jellyfish faster?", "Vanilla is false. Recommended Value: true");
            base.Init();
        }

        public override void Hooks()
        {
            Buff();
        }
        public static void Buff()
        {
            var body = Resources.Load<CharacterBody>("prefabs/characterbodies/JellyfishBody");

            if (spdtw)
            {
                body.baseMoveSpeed = 13f;
            }
        }
    }
}
