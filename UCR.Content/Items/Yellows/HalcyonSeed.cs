/*
using MonoMod.Cil;
using R2API;
using RoR2;

namespace UltimateCustomRun.Items.Yellows
{
    public class HalcyonSeed : ItemBase
    {
        public static float Health;
        public static float StackHealth;
        public static float Damage;
        public static float StackDamage;

        public override string Name => ":: Items :::: Yellows :: Halcyon Seed";
        public override string InternalPickupToken => "titanGoldDuringTp";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "Summon <style=cIsDamage>Aurelionite</style> during the teleporter event. It has <style=cIsDamage>100% <style=cStack>(+50% per stack)</style> damage</style> and <style=cIsHealing>100% <style=cStack>(+100% per stack)</style> health</style>.";

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