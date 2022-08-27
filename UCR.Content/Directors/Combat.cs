/*
using RoR2;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System.Reflection;
using static RoR2.CombatDirector;
using System.Collections.Generic;
using System.Linq;
using System;
using R2API;
using static R2API.DirectorAPI;
using UnityEngine.AddressableAssets;
using BepInEx.Configuration;

namespace UltimateCustomRun.Directors
{
    public class Combat : DirectorBase
    {
        public override string Name => "Directors : Combat";
        public static ConfigEntry<float> cscBeetle { get; set; }
        public static ConfigEntry<float> cscBeetleGuard { get; set; }
        public static ConfigEntry<float> cscBeetleQueen { get; set; }
        public static ConfigEntry<float> cscBell { get; set; }
        public static ConfigEntry<float> cscBison { get; set; }
        public static ConfigEntry<float> cscClayBoss { get; set; }
        public static ConfigEntry<float> cscClayBruiser { get; set; }
        public static ConfigEntry<float> cscElectricWorm { get; set; }
        public static ConfigEntry<float> cscGolem { get; set; }
        public static ConfigEntry<float> cscGrandparent { get; set; }
        public static ConfigEntry<float> cscGravekeeper { get; set; }
        public static ConfigEntry<float> cscGreaterWisp { get; set; }
        public static ConfigEntry<float> cscHermitCrab { get; set; }
        public static ConfigEntry<float> cscImp { get; set; }
        public static ConfigEntry<float> cscImpBoss { get; set; }
        public static ConfigEntry<float> cscJellyfish { get; set; }
        public static ConfigEntry<float> cscLemurian { get; set; }
        public static ConfigEntry<float> cscLemurianBruiser { get; set; }
        public static ConfigEntry<float> cscLunarExploder { get; set; }
        public static ConfigEntry<float> cscLunarGolem { get; set; }
        public static ConfigEntry<float> cscLunarWisp { get; set; }
        public static ConfigEntry<float> cscMagmaWorm { get; set; }
        public static ConfigEntry<float> cscMiniMushroom { get; set; }
        public static ConfigEntry<float> cscNullifier { get; set; }
        public static ConfigEntry<float> cscParent { get; set; }
        public static ConfigEntry<float> cscRoboBallBoss { get; set; }
        public static ConfigEntry<float> cscRoboBallMini { get; set; }
        public static ConfigEntry<float> cscScav { get; set; }
        public static ConfigEntry<float> cscTitanBlackBeach { get; set; }
        public static ConfigEntry<float> cscTitanDampCave { get; set; }
        public static ConfigEntry<float> cscTitanGolemPlains { get; set; }
        public static ConfigEntry<float> cscTitanGooLake { get; set; }
        public static ConfigEntry<float> cscVagrant { get; set; }
        public static ConfigEntry<float> cscVulture { get; set; }
        public static ConfigEntry<float> cscLesserWisp { get; set; }
        public static ConfigEntry<float> cscAcidLarva { get; set; }
        public static ConfigEntry<float> cscClayGrenadier { get; set; }
        public static ConfigEntry<float> cscFlyingVermin { get; set; }
        public static ConfigEntry<float> cscFlyingVerminSnowy { get; set; }
        public static ConfigEntry<float> cscGeepBody { get; set; }
        public static ConfigEntry<float> cscGipBody { get; set; }
        public static ConfigEntry<float> cscGupBody { get; set; }
        public static ConfigEntry<float> cscMegaConstruct { get; set; }
        public static ConfigEntry<float> cscMinorConstruct { get; set; }
        public static ConfigEntry<float> cscVermin { get; set; }
        public static ConfigEntry<float> cscVerminSnowy { get; set; }
        public static ConfigEntry<float> cscVoidBarnacle { get; set; }
        public static ConfigEntry<float> cscVoidJailer { get; set; }
        public static ConfigEntry<float> cscVoidMegaCrab { get; set; }
        public static ConfigEntry<float> cscVoidInfestor { get; set; }

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Config();
        }

        private void Config()
        {
            foreach (string key in Monsters)
            {
                // MonsterToBind[key] = Main.UCRDConfig.Bind<float>(MonsterToGroup[key], NormalizedName[key],);
            }
        }

        static Combat()
        {
            Dictionary<string, ConfigEntry<float>> dict = new();
            Dictionary<string, ConfigEntry<int>> dictS = new();
            MonsterToBind = dict;

            MonsterToGroup = new Dictionary<string, string>
            {
            }
        }

        private static readonly List<string> Monsters = new()
        {
            "cscBeetle",
            "cscBeetleGuard",
            "cscBeetleQueen",
            "cscBell",
            "cscBison",
            "cscClayBoss",
            "cscClayBruiser",
            "cscElectricWorm",
            "cscGolem",
            "cscGrandparent",
            "cscGravekeeper",
            "cscGreaterWisp",
            "cscHermitCrab",
            "cscImp",
            "cscImpBoss",
            "cscJellyfish",
            "cscLemurian",
            "cscLemurianBruiser",
            "cscLunarExploder",
            "cscLunarGolem",
            "cscLunarWisp",
            "cscMagmaWorm",
            "cscMiniMushroom",
            "cscNullifier",
            "cscParent",
            "cscRoboBallBoss",
            "cscRoboBallMini",
            "cscScav",
            "cscTitanBlackBeach",
            "cscTitanDampCave",
            "cscTitanGolemPlains",
            "cscTitanGooLake",
            "cscVagrant",
            "cscVulture",
            "cscLesserWisp",
            "cscAcidLarva",
            "cscClayGrenadier",
            "cscFlyingVermin",
            "cscFlyingVerminSnowy",
            "cscGeepBody",
            "cscGipBody",
            "cscGupBody",
            "cscMegaConstruct",
            "cscMinorConstruct",
            "cscVermin",
            "cscVerminSnowy",
            "cscVoidBarnacle",
            "cscVoidJailer",
            "cscVoidMegaCrab",
            "cscVoidInfestor"
        };

        private static readonly Dictionary<string, string> NormalizedName = new()
        {
            {
                "cscBeetle",
                "Beetle"
            },
            {
                "cscBeetleGuard",
                "Beetle Guard"
            },
            {
                "cscBeetleQueen",
                "Beetle Queen"
            },
            {
                "cscBell",
                "Brass Contraption"
            },
            {
                "cscBison",
                "Bighorn Bison"
            },
            {
                "cscClayBoss",
                "Clay Dunestrider"
            },
            {
                "cscClayBruiser",
                "Clay Templar"
            },
            {
                "cscElectricWorm",
                "Overloading Worm"
            },
            {
                "cscGolem",
                "Stone Golem"
            },
            {
                "cscGrandparent",
                "Grandparent"
            },
            {
                "cscGravekeeper",
                "Grovetender"
            },
            {
                "cscGreaterWisp",
                "Greater Wisp"
            },
            {
                "cscHermitCrab",
                "Hermit Crab"
            },
            {
                "cscImp",
                "Imp"
            },
            {
                "cscImpBoss",
                "Imp Overlord"
            },
            {
                "cscJellyfish",
                "Jellyfish"
            },
            {
                "cscLemurian",
                "Lemurian"
            },
            {
                "cscLemurianBruiser",
                "Elder Lemurian"
            },
            {
                "cscLunarExploder",
                "Lunar Exploder"
            },
            {
                "cscLunarGolem",
                "Lunar Golem"
            },
            {
                "cscLunarWisp",
                "Lunar Wisp"
            },
            {
                "cscMagmaWorm",
                "Magma Worm"
            },
            {
                "cscMiniMushroom",
                "Mini Mushrum"
            },
            {
                "cscNullifier",
                "Void Reaver"
            },
            {
                "cscParent",
                "Parent"
            },
            {
                "cscRoboBallBoss",
                "Solus Control Unit"
            },
            {
                "cscRoboBallMini",
                "Solus Probe"
            },
            {
                "cscScav",
                "Scavenger"
            },
            {
                "cscTitanBlackBeach",
                "Stone Titan : Distant Roost"
            },
            {
                "cscTitanDampCave",
                "Stone Titan :::: Abyssal Depths"
            },
            {
                "cscTitanGolemPlains",
                "Stone Titan : Titanic Plains"
            },
            {
                "cscTitanGooLake",
                "Stone Titan :: Abandoned Aqueduct"
            },
            {
                "cscVagrant",
                "Wandering Vagrant"
            },
            {
                "cscVulture",
                "Alloy Vulture"
            },
            {
                "cscLesserWisp",
                "Lesser Wisp"
            },
            {
                "cscAcidLarva",
                "Larva"
            },
            {
                "cscClayGrenadier",
                "Clay Apothecary"
            },
            {
                "cscFlyingVermin",
                "Blind Pest"
            },
            {
                "cscFlyingVerminSnowy",
                "Blind Pest (Snowy)"
            },
            {
                "cscGeepBody",
                "Geep"
            },
            {
                "cscGipBody",
                "Gip"
            },
            {
                "cscGupBody",
                "Gup"
            },
            {
                "cscMegaConstruct",
                "Xi Construct"
            },
            {
                "cscMinorConstruct",
                "Alpha Construct"
            },
            {
                "cscVermin",
                "Blind Vermin"
            },
            {
                "cscVerminSnowy",
                "Blind Vermin (Snowy)"
            },
            {
                "cscVoidBarnacle",
                "Void Barnacle"
            },
            {
                "cscVoidJailer",
                "Void Jailer"
            },
            {
                "cscVoidMegaCrab",
                "Void Devastator"
            },
            {
                "cscVoidInfestor",
                "Void Infestor"
            }
        };

        public static Dictionary<string, ConfigEntry<float>> MonsterToBind;
        public static Dictionary<string, string> MonsterToGroup;
    }
}
*/