using BepInEx.Configuration;
using System;
using UnityEngine;

namespace WellRoundedBalance.Mechanics.Monsters
{
    internal class BetterScaling
    {
        public static ConfigEntry<bool> enable { get; set; }

        public static void Init()
        {
            enable = Main.WRBMechanicConfig.Bind(":: Mechanics ::::::::: Monster Health and Armor Adjustment", "Enable?", true, "Vanilla is false");
            if (enable.Value)
            {
                RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            }
        }

        private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.teamComponent.teamIndex != TeamIndex.Player)
            {
                args.armorAdd += 150 - 150 / Mathf.Pow(1 + 0.055f, Run.instance.stageClearCount);
                args.healthMultAdd *= 0.7f; // no inferno check intentionally
            }
        }
    }
}