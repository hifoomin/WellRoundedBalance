using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;

namespace UltimateCustomRun.Enemies
{
    public class ClayTemplar : EnemyBase
    {
        public static bool aitw;
        public static bool m2tw;
        public static bool spdtw;
        public override string Name => ":::: Enemies :: Clay Templar";

        public override void Init()
        {
            aitw = ConfigOption(false, "Make Clay Templar AI smarter?", "Vanilla is false. Recommended Value: true");
            m2tw = ConfigOption(false, "Make Shotgun better?", "Vanilla is false. Recommended Value: true");
            spdtw = ConfigOption(false, "Make Clay Templar faster?", "Vanilla is false. Recommended Value: true");
            base.Init();
        }

        public override void Hooks()
        {
            Buff();
        }
        public static void Buff()
        {
            var master = Resources.Load<CharacterMaster>("prefabs/charactermasters/ClayBruiserMaster");
            var body = Resources.Load<CharacterBody>("prefabs/characterbodies/ClayBruiserBody");

            if (aitw)
            {
                AISkillDriver ai = (from x in master.GetComponents<AISkillDriver>()
                                    where x.customName == "WalkAndShoot"
                                    select x).First();
                ai.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            }

            if (m2tw)
            {
                On.EntityStates.ClayBruiser.Weapon.FireSonicBoom.OnEnter += (orig, self) =>
                {
                    self.baseDuration = 1f;
                    self.fieldOfView = 60f;
                    orig(self);
                };
            }

            if (spdtw)
            {
                body.baseMoveSpeed = 13f;
            }
        }
    }
}
