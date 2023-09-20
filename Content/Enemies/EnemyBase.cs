using BepInEx.Configuration;
using EntityStates;
using RoR2.Skills;

namespace WellRoundedBalance.Enemies
{
    public abstract class EnemyBase : SharedBase
    {
        public override ConfigFile Config => Main.WRBEnemyConfig;
        public static List<string> enemyList = new();

        public override void Init()
        {
            base.Init();
            enemyList.Add(Name);
        }

        public SkillDef CreateSkillDef<T>(float cooldown, string esm) where T : EntityState {
            SkillDef skill = ScriptableObject.CreateInstance<SkillDef>();
            skill.baseRechargeInterval = cooldown;
            skill.activationStateMachineName = esm;
            skill.activationState = new(typeof(T));

            ContentAddition.AddSkillDef(skill);
            return skill;
        }

        public void ReplaceSkill(SkillDef skill, GenericSkill slot) {
            slot._skillFamily.variants[0].skillDef = skill;
        }
    }
}