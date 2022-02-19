using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;

namespace UltimateCustomRun.Enemies
{
    public class GreaterWisp : EnemyBase
    {
        public static CharacterMaster master;
        public static bool tw;
        public static float dur;
        public override string Name => ":::: Enemies :: Greater Wisp";

        public override void Init()
        {
            dur = ConfigOption(2f, "Cannon Charge Time", "Vanilla is 2. Recommended Value: 1.25");
            tw = ConfigOption(false, "Enable Damage Tweaks and AI Tweaks?", "Vanilla is false. Recommended Value: true");
            base.Init();
        }

        public override void Hooks()
        {
            Buff();
        }
        public static void Buff()
        {
            master = Resources.Load<CharacterMaster>("prefabs/charactermasters/GreaterWispMaster").GetComponent<CharacterMaster>();
            if (tw)
            {
                AISkillDriver ai = (from x in master.GetComponents<AISkillDriver>()
                                    where x.minDistance == 15f
                                    select x).First();
                ai.movementType = AISkillDriver.MovementType.StrafeMovetarget;
                ai.maxDistance = 100f;
            }

            On.EntityStates.GreaterWispMonster.ChargeCannons.OnEnter += (orig, self) =>
            {
                self.baseDuration = 1.25f;
                orig(self);
            };
        }
    }
}
