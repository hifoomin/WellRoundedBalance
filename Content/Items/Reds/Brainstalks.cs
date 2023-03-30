using RoR2.Skills;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using static RoR2.MasterSpawnSlotController;

namespace WellRoundedBalance.Items.Reds
{
    public class Brainstalks : ItemBase
    {
        public override string Name => ":: Items ::: Reds :: Brainstalks";
        public override ItemDef InternalPickup => RoR2Content.Items.KillEliteFrenzy;

        public override string PickupText => "Skills have NO cooldowns for a short period after killing an elite.";

        public override string DescText => "Upon killing an elite monster, <style=cIsDamage>enter a frenzy</style> for <style=cIsDamage>4s</style> <style=cStack>(+4s per stack)</style> where <style=cIsUtility>skills have no cooldowns</style>.";

        public static HashSet<SkillDef> forbidden = new();
        public static HashSet<SkillDef> allowed = new();

        public override void Init()
        {
            SkillCatalog.skillsDefined.CallWhenAvailable(() =>
            {
                foreach (var skill in SkillCatalog.allSkillDefs)
                {
                    if (skill.beginSkillCooldownOnSkillEnd)
                    {
                        forbidden.Add(skill);
                    }
                    else if (!skill.mustKeyPress)
                    {
                        allowed.Add(skill);
                    }
                }
            });
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.Skills.SkillDef.OnExecute += SkillDef_OnExecute;

            Changes();
        }

        private void SkillDef_OnExecute(On.RoR2.Skills.SkillDef.orig_OnExecute orig, SkillDef self, GenericSkill skillSlot)
        {
            orig(self, skillSlot);
            if (skillSlot.characterBody.HasBuff(RoR2Content.Buffs.NoCooldowns) && !forbidden.Contains(self))
            {
                if (!allowed.Contains(self) && skillSlot.stateMachine.state.GetType() == self.activationState.stateType)
                {
                    if (skillSlot.stateMachine.CanInterruptState(self.interruptPriority))
                    {
                        forbidden.Add(self);
                        return;
                    }
                    else
                    {
                        allowed.Add(self);
                    }
                }
                skillSlot.RestockSteplike();
            }
        }

        private void Changes()
        {
            var brainstalksVFX = Utils.Paths.GameObject.NoCooldownEffect.Load<GameObject>();
            var pp = brainstalksVFX.transform.GetChild(1).GetChild(0);
            var postProcessVolume = pp.GetComponent<PostProcessVolume>();
            var profile = postProcessVolume.sharedProfile;
            var colorGrading = profile.GetSetting<ColorGrading>();
            colorGrading.saturation.value = 15f;
        }
    }
}