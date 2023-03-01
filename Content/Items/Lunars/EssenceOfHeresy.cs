using MonoMod.Cil;

namespace WellRoundedBalance.Items.Lunars
{
    internal class EssenceOfHeresy : ItemBase
    {
        public override string Name => ":: Items ::::: Lunars :: Essence of Heresy";

        public override string InternalPickupToken => "lunarSpecialReplacement";

        public override string PickupText => "Replace your Special Skill with 'Ruin'.";

        public override string DescText => "<style=cIsUtility>Replace your Special Skill</style> with <style=cIsUtility>Ruin</style>. \n\nDealing damage adds a stack of <style=cIsDamage>Ruin</style> for 10 <style=cStack>(+10 per stack)</style> seconds, up to <style=cIsDamage>" + maxDebuffCountPerTarget + "</style> stacks per target. Activating the skill <style=cIsDamage>detonates</style> all Ruin stacks, dealing <style=cIsDamage>300% damage</style> plus <style=cIsDamage>120% damage per stack of Ruin</style>. Recharges after 8 <style=cStack>(+8 per stack)</style> seconds.";

        [ConfigField("Max Debuff Count Per Target", 10)]
        public static int maxDebuffCountPerTarget;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.CharacterBody.AddTimedBuff_BuffDef_float += CharacterBody_AddTimedBuff_BuffDef_float;
        }

        private void CharacterBody_AddTimedBuff_BuffDef_float(On.RoR2.CharacterBody.orig_AddTimedBuff_BuffDef_float orig, CharacterBody self, BuffDef buffDef, float duration)
        {
            var lunarDet = RoR2Content.Buffs.LunarDetonationCharge;
            if (buffDef == lunarDet)
            {
                if (self.GetBuffCount(lunarDet) >= maxDebuffCountPerTarget)
                {
                    return;
                }
            }
            orig(self, buffDef, duration);
        }
    }
}