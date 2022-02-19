using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    public class GhorsTome : ItemBase
    {
        public static float chance;
        public static int reward;

        public override string Name => ":: Items :: Greens :: Ghors Tome";
        public override string InternalPickupToken => "bonusGoldPackOnKill";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "<style=cIsUtility>" + chance + "%</style> <style=cStack>(+" + chance + "% on stack)</style> chance on kill to drop a treasure worth <style=cIsUtility>$" + reward + "</style>. <style=cIsUtility>Scales over time.</style>";


        public override void Init()
        {
            chance = ConfigOption(4f, "Chance", "Per Stack. Vanilla is 4");
            reward = ConfigOption(25, "Reward", "Vanilla is 25");
            base.Init();
        }

        public override void Hooks()
        {
            ChangeReward();
            IL.RoR2.GlobalEventManager.OnCharacterDeath += ChangeChance;
        }
        public static void ChangeReward()
        {
            var gtc = Resources.Load<GameObject>("Prefabs/NetworkedObjects/BonusMoneyPack").GetComponentInChildren<MoneyPickup>();
            gtc.baseGoldReward = reward;
        }
        public static void ChangeChance(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchBle(out _),
                x => x.MatchLdcR4(4f)
            );
            c.Index += 1;
            c.Next.Operand = chance;
        }
    }
}
