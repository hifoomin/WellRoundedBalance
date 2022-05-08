using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Items.Whites
{
    public class PowerElixir : ItemBase
    {
        public static float Healing;

        public override string Name => ":: Items : Whites :: Power Elixir";
        public override string InternalPickupToken => "healingPotion";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "Taking damage to below <style=cIsHealth>25% health</style> <style=cIsUtility>consumes</style> this item, <style=cIsHealing>healing</style> you for <style=cIsHealing>" + d(Healing) + "</style> of <style=cIsHealing>maximum health</style>.";

        public override void Init()
        {
            Healing = ConfigOption(0.75f, "Healing", "Decimal. Vanilla is 0.75");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.UpdateLastHitTime += ChangeHealing;
        }

        public static void ChangeHealing(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.75f)
            );
            c.Next.Operand = Healing;
        }
    }
}