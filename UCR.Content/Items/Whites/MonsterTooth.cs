using MonoMod.Cil;
using RoR2;
using System;

namespace UltimateCustomRun.Items.Whites
{
    public class MonsterTooth : ItemBase
    {
        public static float FlatHealing;
        public static float PercentHealing;

        public override string Name => ":: Items : Whites :: Monster Tooth";
        public override string InternalPickupToken => "tooth";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "Killing an enemy spawns a <style=cIsHealing>Healing orb</style> that heals for <style=cIsHealing>" + FlatHealing + "</style> plus an additional <style=cIsHealing>" + d(PercentHealing) + " <style=cStack>(+" + d(PercentHealing) + " per stack)</style></style> of <style=cIsHealing>maximum health</style>.";

        public override void Init()
        {
            FlatHealing = ConfigOption(8f, "Flat Healing", "Vanilla is 8");
            PercentHealing = ConfigOption(0.02f, "Percent Healing", "Decimal. Per Stack. Vanilla is 0.02");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += ChangeHealing;
        }

        public static void ChangeHealing(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(8f),
                    x => x.MatchStfld<HealthPickup>("flatHealing"),
                    x => x.MatchLdloc(out _),
                    x => x.MatchLdcR4(0.02f)))
            {
                c.Next.Operand = FlatHealing;
                c.Index += 3;
                c.Next.Operand = PercentHealing;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Monster Tooth Healing hook");
            }
            // thanks to RandomlyAwesome!
        }
    }
}