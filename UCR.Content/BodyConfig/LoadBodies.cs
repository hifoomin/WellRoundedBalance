using UnityEngine;

namespace UltimateCustomRun.Survivors
{
    public class LoadBodies
    {
        public static void Initialize()
        {
            On.RoR2.BodyCatalog.Init += (orig) =>
            {
                orig();
            };
        }

        public static GameObject AlloyVulture;
        public static Base AlloyVultureStats;
        public static GameObject Beetle;
        public static Base BeetleStats;
        public static GameObject BeetleGuard;
        public static Base BeetleGuardStats;
        public static GameObject BighornBison;
        public static Base BighornBisonStats;
        public static GameObject BrassContraption;
        public static Base BrassContraptionStats;
        public static GameObject ClayTemplar;
        public static Base ClayTemplarStats;
        public static GameObject ElderLemurian;
        public static Base ElderLemurianStats;
        public static GameObject GreaterWisp;
        public static Base GreaterWispStats;
        public static GameObject HermitCrab;
        public static Base HermitCrabStats;
        public static GameObject Imp;
        public static Base ImpStats;
        public static GameObject Jellyfish;
        public static Base JellyfishStats;
        public static GameObject Lemurian;
        public static Base LemurianStats;
        public static GameObject LesserWisp;
        public static Base LesserWispStats;
        public static GameObject LunarExploder;
        public static Base LunarExploderStats;
        public static GameObject LunarGolem;
        public static Base LunarGolemStats;
        public static GameObject LunarWisp;
        public static Base LunarWispStats;
        public static GameObject MiniMushrum;
        public static Base MiniMushrumStats;
        public static GameObject Parent;
        public static Base ParentStats;
        public static GameObject SolusProbe;
        public static Base SolusProbeStats;
        public static GameObject StoneGolem;
        public static Base StoneGolemStats;
        public static GameObject VoidReaver;
        public static Base VoidReaverStats;
    }
}