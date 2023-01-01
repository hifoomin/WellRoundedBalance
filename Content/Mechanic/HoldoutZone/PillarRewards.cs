using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace WellRoundedBalance.Mechanic.HoldoutZone
{
    internal class PillarRewards : GlobalBase
    {
        public static float whiteChance = 0f;
        public static float greenChance = 100f;
        public static float redChance = 0f;
        public static float lunarChance = 0f;
        private static Vector3 rewardPositionOffset = new(0f, 3f, 0f);

        public override string Name => "Global :::: Holdout Zone";

        public override void Hooks()
        {
            On.RoR2.MoonBatteryMissionController.OnBatteryCharged += MoonBatteryMissionController_OnBatteryCharged;
        }

        private void MoonBatteryMissionController_OnBatteryCharged(On.RoR2.MoonBatteryMissionController.orig_OnBatteryCharged orig, MoonBatteryMissionController self, HoldoutZoneController holdoutZone)
        {
            orig(self, holdoutZone);
            if (NetworkServer.active)
            {
                var pickupIndex = SelectItem();
                var tier = PickupCatalog.GetPickupDef(pickupIndex).itemTier;
                if (pickupIndex != PickupIndex.none)
                {
                    var pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
                    var playerCount = Run.instance.participatingPlayerCount;

                    if (playerCount != 0 && holdoutZone.transform)
                    {
                        float angle = 360f / playerCount;
                        var vector = Quaternion.AngleAxis(UnityEngine.Random.Range(0, 360), Vector3.up) * (Vector3.up * 40f + Vector3.forward * 5f);
                        var rotation = Quaternion.AngleAxis(angle, Vector3.up);
                        int k = 0;
                        while (k < playerCount)
                        {
                            PickupIndex pickupOverwrite = PickupIndex.none;
                            bool overwritePickup = false;
                            if (tier != ItemTier.Tier3)
                            {
                                float pearlChance = 0.15f;
                                if (Run.instance.bossRewardRng.RangeFloat(0f, 100f) < pearlChance)
                                {
                                    pickupOverwrite = SelectPearl();
                                }
                            }

                            PickupDropletController.CreatePickupDroplet(pickupOverwrite, holdoutZone.transform.position + rewardPositionOffset, vector);

                            k++;
                            vector = rotation * vector;
                        }
                    }
                }
            }
        }

        private static PickupIndex SelectPearl()
        {
            var pearlIndex = PickupCatalog.FindPickupIndex(RoR2Content.Items.Pearl.itemIndex);
            var shinyPearlIndex = PickupCatalog.FindPickupIndex(RoR2Content.Items.ShinyPearl.itemIndex);
            bool pearlAvailable = pearlIndex != PickupIndex.none && Run.instance.IsItemAvailable(RoR2Content.Items.Pearl.itemIndex);
            bool shinyPearlAvailable = shinyPearlIndex != PickupIndex.none && Run.instance.IsItemAvailable(RoR2Content.Items.ShinyPearl.itemIndex);

            var toReturn = PickupIndex.none;
            if (pearlAvailable && shinyPearlAvailable)
            {
                toReturn = pearlIndex;
                if (Run.instance.bossRewardRng.RangeFloat(0f, 100f) <= 20f)
                {
                    toReturn = shinyPearlIndex;
                }
            }
            else
            {
                if (pearlAvailable)
                {
                    toReturn = pearlIndex;
                }
                else if (shinyPearlAvailable)
                {
                    toReturn = shinyPearlIndex;
                }
            }
            return toReturn;
        }

        private static PickupIndex SelectItem()
        {
            List<PickupIndex> list;
            Xoroshiro128Plus bossRewardRng = Run.instance.bossRewardRng;
            PickupIndex selectedPickup = PickupIndex.none;

            float total = whiteChance + greenChance + redChance + lunarChance;

            if (bossRewardRng.RangeFloat(0f, total) <= whiteChance)//drop white
            {
                list = Run.instance.availableTier1DropList;
            }
            else
            {
                total -= whiteChance;
                if (bossRewardRng.RangeFloat(0f, total) <= greenChance)//drop green
                {
                    list = Run.instance.availableTier2DropList;
                }
                else
                {
                    total -= greenChance;
                    if ((bossRewardRng.RangeFloat(0f, total) <= redChance))
                    {
                        list = Run.instance.availableTier3DropList;
                    }
                    else
                    {
                        list = Run.instance.availableLunarCombinedDropList;
                    }
                }
            }
            if (list.Count > 0)
            {
                selectedPickup = bossRewardRng.NextElementUniform(list);
            }
            return selectedPickup;
        }
    }
}