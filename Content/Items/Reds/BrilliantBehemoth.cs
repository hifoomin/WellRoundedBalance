using MonoMod.Cil;

namespace WellRoundedBalance.Items.Reds
{
    public class BrilliantBehemoth : ItemBase<BrilliantBehemoth>
    {
        public override string Name => ":: Items ::: Reds :: Brilliant Behemoth";
        public override ItemDef InternalPickup => RoR2Content.Items.Behemoth;

        public override string PickupText => "All your attacks explode!";

        public override string DescText => "All your <style=cIsDamage>attacks explode</style> in a <style=cIsDamage>4m</style> <style=cStack>(+2.5m per stack)</style> radius for a bonus <style=cIsDamage>" + d(totalDamage) + "</style> TOTAL damage to nearby enemies.";

        [ConfigField("TOTAL Damage", "Decimal.", 0.55f)]
        public static float totalDamage;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitAllProcess += GlobalEventManager_OnHitAllProcess;
        }

        private void GlobalEventManager_OnHitAllProcess(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.6f)))
            {
                c.Next.Operand = totalDamage;
            }
            else
            {
                Logger.LogError("Failed to apply Brilliant Behemoth Damage hook");
            }
        }
    }
}