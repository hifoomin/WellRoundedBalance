using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;

namespace UltimateCustomRun.Enemies
{
    public class BeetleGuard : EnemyBase
    {
        public static bool aitw;
        public static bool sundtw;
        public static bool spdtw;
        public override string Name => ":::: Enemies :: Beetle Guard";

        public override void Init()
        {
            aitw = ConfigOption(false, "Make Beetle Guard AI smarter?", "Vanilla is false. Recommended Value: true");
            sundtw = ConfigOption(false, "Make Beetle Guard spammier?", "Vanilla is false. Recommended Value: true");
            spdtw = ConfigOption(false, "Make Beetle Guard faster?", "Vanilla is false. Recommended Value: true");
            base.Init();
        }

        public override void Hooks()
        {
            Buff();
        }
        public static void Buff()
        {
            var master = Resources.Load<CharacterMaster>("prefabs/charactermasters/BeetleGuardMaster");
            var body = Resources.Load<CharacterBody>("prefabs/characterbodies/BeetleGuardBody");

            if (aitw)
            {
                AISkillDriver ai = (from x in master.GetComponents<AISkillDriver>()
                                    where x.customName == "FireSunder"
                                    select x).First();
                ai.maxDistance = 80f;
            }

            if (sundtw)
            {
                On.EntityStates.BeetleGuardMonster.FireSunder.OnEnter += (orig, self) =>
                {
                    EntityStates.BeetleGuardMonster.FireSunder.baseDuration = 1.7f;
                    orig(self);
                };
            }

            if (spdtw)
            {
                body.baseMoveSpeed = 22f;
                On.EntityStates.BeetleGuardMonster.GroundSlam.OnEnter += (orig, self) =>
                {
                    EntityStates.BeetleGuardMonster.GroundSlam.baseDuration = 2.4f;
                    orig(self);
                };
            }
        }
    }
}
