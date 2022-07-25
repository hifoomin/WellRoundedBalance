using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Items.Whites
{
    public class RollOfPennies : ItemBase
    {
        public static int GoldGain;

        public override string Name => ":: Items : Whites :: Roll of Pennies";
        public override string InternalPickupToken => "goldOnHurt";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "Gain <style=cIsUtility>" + GoldGain + " <style=cStack>(+" + GoldGain + " per stack)</style> gold</style> on <style=cIsDamage>taking damage</style> from an enemy. <style=cIsUtility>Scales over time.</style>";

        public override void Init()
        {
            GoldGain = ConfigOption(3, "Gold Gain", "Per Stack. Vanilla is 3");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += ChangeGoldGain;
        }

        public static void ChangeGoldGain(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcI4(3),
                x => x.MatchStloc(out _),
                x => x.MatchNewobj("RoR2.Orbs.GoldOrb")
            );
            c.Next.Operand = GoldGain;
        }
    }
}