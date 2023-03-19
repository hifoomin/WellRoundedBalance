using MonoMod.Cil;

namespace WellRoundedBalance.Items.Reds
{
    internal class InterstellarDeskPlant : ItemBase
    {
        public override string Name => ":: Items ::: Reds :: Interstellar Desk Plant";

        public override ItemDef InternalPickup => RoR2Content.Items.Plant;

        public override string PickupText => "Plant a healing fruit on kill.";

        public override string DescText => "On kill, plant a <style=cIsHealing>healing</style> fruit seed that grows into a plant after <style=cIsUtility>5</style> seconds. \n\nThe plant <style=cIsHealing>heals</style> for <style=cIsHealing>" + d(percentHealing) + "</style> of <style=cIsHealing>maximum health</style> every second to all allies within <style=cIsHealing>" + baseRange + "m</style> <style=cStack>(+" + rangePerStack + "m per stack)</style>. Lasts <style=cIsUtility>10</style> seconds.";

        [ConfigField("Percent Healing", "Decimal.", 0.06f)]
        public static float percentHealing;

        [ConfigField("Base Range", 12f)]
        public static float baseRange;

        [ConfigField("Range Per Stack", 3f)]
        public static float rangePerStack;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.DeskPlantController.Awake += DeskPlantController_Awake;
            IL.RoR2.DeskPlantController.MainState.OnEnter += MainState_OnEnter;
        }

        private void MainState_OnEnter(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.05f)))
            {
                c.Next.Operand = percentHealing / 2f;
            }
            else
            {
                Logger.LogError("Failed to apply Interstellar Desk Plant Healing hook");
            }
        }

        private void DeskPlantController_Awake(On.RoR2.DeskPlantController.orig_Awake orig, DeskPlantController self)
        {
            self.healingRadius = baseRange;
            self.radiusIncreasePerStack = rangePerStack;
            orig(self);
        }
    }
}