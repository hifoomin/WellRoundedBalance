using RoR2;
using UnityEngine;
using System.Linq;

namespace UltimateCustomRun.Enemies.Bosses
{
    public class TwistedScavengerKipKip : EnemyBase
    {
        public static bool itw;
        public override string Name => ":::: Enemies ::: Twisted Scavenger Kipkip";

        public override void Init()
        {
            itw = ConfigOption(false, "Reduce amount of Monster Teeth and Medkit?", "Vanilla is false. Recommended Value: True");
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
            var master = Resources.Load<CharacterMaster>("prefabs/charactermasters/ScavLunar1Master");
            var inv = (from x in master.GetComponents<GivePickupsOnStart>()
                      where x.enabled == true
                      select x).First();
            inv.itemInfos[0].count = 1;
            inv.itemInfos[2].count = 2;
        }
    }
}
