/*
using MonoMod.Cil;
using R2API;
using RoR2;

namespace UltimateCustomRun.Items.Yellows
{
    public class LittleDisciple : ItemBase
    {
        public static float Damage;
        public static float ProcCoefficient;
        public static float Range;
        public static float RechargeTime;

        public override string Name => ":: Items :::: Yellows :: Little Disciple";
        public override string InternalPickupToken => "sprintWisp";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "Fire a <style=cIsDamage>tracking wisp</style> for <style=cIsDamage>300% <style=cStack>(+300% per stack)</style> damage</style>. Fires every <style=cIsUtility>1.6</style> seconds while sprinting. Fire rate increases with <style=cIsUtility>movement speed</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
           //
        }
    }
}
*/