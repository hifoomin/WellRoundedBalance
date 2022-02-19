using RoR2;
using UnityEngine;
using System.Linq;

namespace UltimateCustomRun.Enemies.Bosses
{
    public class TwistedScavengerGuraGura : EnemyBase
    {
        public static bool itw;
        public override string Name => ":::: Enemies ::: Twisted Scavenger Guragura";

        public override void Init()
        {
            itw = ConfigOption(false, "Remove Bands?", "Vanilla is false. Recommended Value: True");
            base.Init();
        }

        public override void Hooks()
        {
            if (itw)
            {
                Nerf();
            }
        }
        public static void Nerf()
        {
            var master = Resources.Load<CharacterMaster>("prefabs/charactermasters/ScavLunar4Master");
            var inv = (from x in master.GetComponents<GivePickupsOnStart>()
                       where x.enabled == true
                       select x).First();
            inv.itemInfos[5].count = 0;
            inv.itemInfos[6].count = 0;
        }
    }
}
