using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;

namespace UltimateCustomRun.Enemies
{
    public class StoneGolem : EnemyBase
    {
        public static CharacterMaster master;
        public static CharacterBody body;
        public static CharacterBody bodybase;

        public static float aspd;
        public static float ldmg;
        public static float ldur;
        public static bool tw;
        public override string Name => ":::: Enemies :: Stone Golem";

        public override void Init()
        {
            aspd = ConfigOption(2.5f, "Headbutt Duration", "Vanilla is 2.5. Recommended Value: 1");
            ldmg = ConfigOption(2.5f, "Laser Damage", "Vanilla is 3.125 with Damage Tweaks and 2.5 without. Recommended Value: 2.5");
            ldur = ConfigOption(3f, "Laser Charge Up Duration", "Vanilla is 3. Recommended Value: 1.8");
            tw = ConfigOption(false, "Enable Damage Tweaks and AI Tweaks?", "Vanilla is false. Recommended Value: true");
            base.Init();
        }

        public override void Hooks()
        {
            Berf();
        }
        public static void Berf()
        {
            master = Resources.Load<CharacterMaster>("prefabs/charactermasters/GolemMaster").GetComponent<CharacterMaster>();
            body = Resources.Load<CharacterBody>("prefabs/characterbodies/GolemBody").GetComponent<CharacterBody>();
            bodybase = Resources.Load<CharacterBody>("prefabs/characterbodies/GolemBody");
            if (tw)
            {
                body.baseDamage = 16f;

                AISkillDriver ai = (from x in master.GetComponents<AISkillDriver>()
                                    where x.skillSlot == SkillSlot.Secondary
                                    select x).First();
                ai.selectionRequiresAimTarget = true;
                ai.activationRequiresAimTargetLoS = true;
                ai.activationRequiresAimConfirmation = true;

                On.EntityStates.GolemMonster.ClapState.OnEnter += (orig, self) =>
                {
                    EntityStates.GolemMonster.ClapState.damageCoefficient = 3.5f;
                    EntityStates.GolemMonster.ClapState.radius = 6f;
                    EntityStates.GolemMonster.ClapState.duration = 1.7f;
                    orig(self);
                };

            }

            On.EntityStates.GolemMonster.ChargeLaser.OnEnter += (orig, self) =>
            {
                EntityStates.GolemMonster.ChargeLaser.baseDuration = ldur;
                orig(self);
            };

            On.EntityStates.GolemMonster.FireLaser.OnEnter += (orig, self) =>
            {
                EntityStates.GolemMonster.FireLaser.damageCoefficient = ldmg;
                // 3.125 to match vanilla
                // 2.5 for 20% nerf
                orig(self);
            };

        }
    }
}
