using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    static class GhorsTome
    {
        public static void ChangeReward()
        {
            var gtc = Resources.Load<GameObject>("Prefabs/NetworkedObjects/BonusMoneyPack").GetComponentInChildren<MoneyPickup>();
            gtc.baseGoldReward = Main.GhorsTomeReward.Value;
        }
        public static void ChangeChance(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchBle(out _),
                x => x.MatchLdcR4(4f)
            );
            c.Index += 1;
            c.Next.Operand = Main.GhorsTomeChance.Value;
        }
    }
}
