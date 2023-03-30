using MonoMod.Cil;

namespace WellRoundedBalance.Items.Yellows
{
    public class MiredUrn : ItemBase
    {
        public override string Name => ":: Items :::: Yellows :: Mired Urn";
        public override ItemDef InternalPickup => RoR2Content.Items.SiphonOnLowHealth;

        public override string PickupText => "Siphon health from nearby enemies while in combat.";

        public override string DescText => "While in combat, the nearest <style=cIsDamage>1</style> <style=cStack>(+1 per stack)</style> enemies to you within <style=cIsDamage>13m</style> will be '<style=cIsDamage>tethered</style>' to you, dealing <style=cIsDamage>100%</style> damage per second, applying a <style=cIsUtility>33.3% slow</style>, and <style=cIsHealing>healing</style> you for <style=cIsHealing>100%</style> of the damage dealt.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
        }
    }
}