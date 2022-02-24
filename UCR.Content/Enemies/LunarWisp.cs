using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Enemies
{
    public class LunarWisp : EnemyBase
    {
        public static bool Tweaks;
        public override string Name => ":::: Enemies :: Lunar Wisp";

        public override void Init()
        {
            Tweaks = ConfigOption(false, "Enable Speed Tweaks?", "Vanilla is false.\nRecommended Value: true");
            base.Init();
        }

        public override void Hooks()
        {
            if (Tweaks)
            {
                Nerf();
            }
        }

        public static void Nerf()
        {
            var body = Resources.Load<CharacterBody>("prefabs/characterbodies/LunarWispBody").GetComponent<CharacterBody>();
            body.baseMoveSpeed = 14f;
            body.acceleration = 8f;
        }
    }
}