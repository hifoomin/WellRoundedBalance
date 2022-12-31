using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace WellRoundedBalance.Items.Greens
{
    public class GhorsTome : ItemBase
    {
        public override string Name => ":: Items :: Greens :: Ghors Tome";
        public override string InternalPickupToken => "bonusGoldPackOnKill";

        public override string PickupText => "Chance on kill to drop a treasure.";
        public override string DescText => "<style=cIsUtility>7%</style> <style=cStack>(+7% on stack)</style> Chance on kill to drop a treasure worth <style=cIsUtility>$25</style>. <style=cIsUtility>Scales over time.</style>";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            ChangeReward();
            IL.RoR2.GlobalEventManager.OnCharacterDeath += ChangeChance;
        }

        public static void ChangeReward()
        {
            var gtc = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/BonusMoneyPack").GetComponentInChildren<MoneyPickup>();
            gtc.baseGoldReward = 25;
        }

        public static void ChangeChance(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchBle(out _),
                    x => x.MatchLdcR4(4f)))
            {
                c.Index += 1;
                c.Next.Operand = 7f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Ghor's Tome Chance hook");
            }
        }
    }
}