using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Enemies
{
    public class LunarWisp : EnemyBase
    {
        public static bool tw;
        public static CharacterBody body;
        public override string Name => ":::: Enemies :: Lunar Wisp";

        public override void Init()
        {
            tw = ConfigOption(false, "Enable Speed Tweaks?", "Vanilla is false. Recommended Value: true");
            base.Init();
        }

        public override void Hooks()
        {
            if (tw)
            {
                Nerf();
            }
        }
        public static void Nerf()
        {
            body = Resources.Load<CharacterBody>("prefabs/characterbodies/LunarWispBody").GetComponent<CharacterBody>();
            body.baseMoveSpeed = 14f;
            body.acceleration = 8f;
        }
    }
}
