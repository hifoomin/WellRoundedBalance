namespace WellRoundedBalance.Items.Lunars
{
    public class HooksOfHeresy : ItemBase<HooksOfHeresy>
    {
        public override string Name => ":: Items ::::: Lunars :: Hooks of Heresy";
        public override ItemDef InternalPickup => RoR2Content.Items.LunarSecondaryReplacement;

        public override string PickupText => "Replace your Secondary Skill with 'Slicing Maelstrom'.";

        public override string DescText => "<style=cIsUtility>Replace your Secondary Skill </style> with <style=cIsUtility>Slicing Maelstrom</style>.\n\nCharge up a projectile that deals <style=cIsDamage>" /* min damage * 0.25 * 5 */ + "325%" + "-500% damage per second</style> to nearby enemies, exploding after <style=cIsUtility>3</style> seconds to deal <style=cIsDamage>260%-400% damage</style> and <style=cIsDamage>root</style> enemies for <style=cIsUtility>3</style> <style=cStack>(+3 per stack)</style> seconds. Recharges after 5 <style=cStack>(+5 per stack)</style> seconds.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.Mage.Weapon.BaseThrowBombState.OnEnter += BaseThrowBombState_OnEnter;
            Changes();
        }

        private void BaseThrowBombState_OnEnter(On.EntityStates.Mage.Weapon.BaseThrowBombState.orig_OnEnter orig, EntityStates.Mage.Weapon.BaseThrowBombState self)
        {
            if (self is EntityStates.GlobalSkills.LunarNeedle.ThrowLunarSecondary)
            {
                self.minDamageCoefficient = 2f;
                self.maxDamageCoefficient = 3f;
            }
            orig(self);
        }

        private void Changes()
        {
            LanguageAPI.Add("SKILL_LUNAR_SECONDARY_REPLACEMENT_DESCRIPTION", "Charge up a projectile that deals <style=cIsDamage>" /* min damage * 0.25 * 5 */ + "325%" + "-500% damage per second</style> to nearby enemies, exploding after <style=cIsUtility>3</style> seconds to deal <style=cIsDamage>260%-400% damage</style> and <style=cIsDamage>root</style> enemies for <style=cIsUtility>3</style> <style=cStack>(+3 per stack)</style> seconds. Recharges after 5 <style=cStack>(+5 per stack)</style> seconds.");
        }
    }
}