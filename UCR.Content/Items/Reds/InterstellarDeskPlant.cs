using MonoMod.Cil;
using UnityEngine;

namespace UltimateCustomRun.Items.Reds
{
    public class InterstellarDeskPlant : ItemBase
    {
        public static float PercentHealing;
        public static float Radius;
        public static float StackRadius;
        public override string Name => ":: Items ::: Reds :: Interstellar Deskplant";
        public override string InternalPickupToken => "interstellarDeskPlant";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "On kill, plant a <style=cIsHealing>healing</style> fruit seed that grows into a plant after <style=cIsUtility>5</style> seconds. \n\nThe plant <style=cIsHealing>heals</style> for <style=cIsHealing>" + d(PercentHealing * 2f) + "</style> of <style=cIsHealing>maximum health</style> every second to all allies within <style=cIsHealing>" + Radius + "m</style> <style=cStack>(+" + StackRadius + "m per stack)</style>. Lasts <style=cIsUtility>10</style> seconds.";

        public override void Init()
        {
            PercentHealing = ConfigOption(0.05f, "Percent Healing", "Decimal. Vanilla is 0.05");
            Radius = ConfigOption(10, "Base Radius", "Vanilla is 10");
            StackRadius = ConfigOption(5, "Stack Radius", "Vanilla is 5");
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.DeskPlantController.Awake += ChangeRadius;
            IL.RoR2.DeskPlantController.MainState.OnEnter += ChangeHealing;
        }

        public static void ChangeHealing(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.05f)
            );
            c.Next.Operand = PercentHealing;
        }

        public static void ChangeRadius(On.RoR2.DeskPlantController.orig_Awake orig, RoR2.DeskPlantController self)
        {
            self.healingRadius = Radius;
            self.radiusIncreasePerStack = StackRadius;
            orig(self);
        }
    }
}