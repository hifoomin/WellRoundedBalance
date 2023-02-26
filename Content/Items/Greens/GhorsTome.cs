using MonoMod.Cil;

namespace WellRoundedBalance.Items.Greens
{
    public class GhorsTome : ItemBase
    {
        public override string Name => ":: Items :: Greens :: Ghors Tome";
        public override string InternalPickupToken => "bonusGoldPackOnKill";

        public override string PickupText => "Chance on kill to drop a treasure.";
        public override string DescText => "<style=cIsUtility>" + goldPackDropChance + "%</style> <style=cStack>(+" + goldPackDropChance + "% on stack)</style> chance on kill to drop a treasure worth <style=cIsUtility>$" + goldPackGoldGain + "</style>. <style=cIsUtility>Scales over time.</style>";

        [ConfigField("Gold Pack Drop Chance", "", 5f)]
        public static float goldPackDropChance;

        [ConfigField("Gold Pack Gold Gain", "", 25)]
        public static int goldPackGoldGain;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
            IL.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
        }

        private void GlobalEventManager_OnCharacterDeath(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchBle(out _),
                    x => x.MatchLdcR4(4f)))
            {
                c.Index += 1;
                c.Next.Operand = goldPackDropChance;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Ghor's Tome Chance hook");
            }
        }

        private void Changes()
        {
            var gtc = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/BonusMoneyPack").GetComponentInChildren<MoneyPickup>();
            gtc.baseGoldReward = goldPackGoldGain;
        }
    }
}