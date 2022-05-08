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

        public override string DescText => "Killing an enemy spawns a <style=cIsHealing>Healing orb</style> that heals for <style=cIsHealing>8</style> plus an additional <style=cIsHealing>2% <style=cStack>(+2% per stack)</style></style> of <style=cIsHealing>maximum health</style>.";

        public override void Init()
        {
            //FlatHealing = ConfigOption(8f, "Flat Healing", "Vanilla is 8");
            //PercentHealing = ConfigOption(0.02f, "Percent Healing", "Decimal. Per Stack. Vanilla is 0.02");

            base.Init();
        }

        public override void Hooks()
        {
            // IL.RoR2.GlobalEventManager.OnCharacterDeath += ChangeHealing;
        }

        public static void ChangeHealing(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchStfld<HealthPickup>(nameof(HealthPickup.flatHealing))
            );
            c.Index--;
            c.EmitDelegate<Func<float, float>>((vanilla) => { return FlatHealing; });
            c.Index += 2;
            c.EmitDelegate<Func<float, float>>((vanilla) => { return PercentHealing; });
            // thanks to RandomlyAwesome!
            // PLEASE HELP TO FIX
        }
    }
}