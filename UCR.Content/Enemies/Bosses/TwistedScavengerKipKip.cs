using RoR2;
using System.Linq;
using UnityEngine;

namespace UltimateCustomRun.Enemies.Bosses
{
    public class TwistedScavengerKipKip : EnemyBase
    {
        public static bool ItemTweaks;
        public override string Name => ":::: Enemies ::: Twisted Scavenger Kipkip";

        public override void Init()
        {
            ItemTweaks = ConfigOption(false, "Reduce amount of Monster Teeth and Medkit?", "Vanilla is false.\nRecommended Value: True");
            base.Init();
        }

        public override void Hooks()
        {
            if (ItemTweaks)
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