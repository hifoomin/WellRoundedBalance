using System;
using System.Collections.Generic;
using System.Text;

namespace WellRoundedBalance.Elites
{
    internal class Celestine : EliteBase
    {
        public override string Name => "Elites :::: Celestine";
        public static BuffDef CelestineBoost;
        public static Material CelestineOverlay;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.CharacterBody.AffixHauntedBehavior.FixedUpdate += NoMoreInvis;

            CelestineBoost = ScriptableObject.CreateInstance<BuffDef>();
            CelestineBoost.buffColor = Color.cyan;
            CelestineBoost.canStack = false;
            CelestineBoost.isHidden = true;
            CelestineBoost.name = "Celestine Boost";

            ContentAddition.AddBuffDef(CelestineBoost);

            RecalculateStatsAPI.GetStatCoefficients += StatIncrease;
            On.RoR2.CharacterModel.UpdateOverlays += HandleOverlay;

            CelestineOverlay = Utils.Paths.Material.matMoonbatteryGlassOverlay.Load<Material>();
        }

        private static void HandleOverlay(On.RoR2.CharacterModel.orig_UpdateOverlays orig, CharacterModel self)
        {
            orig(self);
            if (self.body && self.body.HasBuff(CelestineBoost))
            {
                self.currentOverlays[self.activeOverlayCount++] = CelestineOverlay;
            }
        }

        private static void StatIncrease(CharacterBody self, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (NetworkServer.active && self.HasBuff(CelestineBoost))
            {
                args.attackSpeedMultAdd += 0.3f;
                args.armorAdd += 20;
            }
        }

        private static void NoMoreInvis(On.RoR2.CharacterBody.AffixHauntedBehavior.orig_FixedUpdate orig, CharacterBody.AffixHauntedBehavior self)
        {
            orig(self);
            if (self.affixHauntedWard)
            {
                BuffWard ward = self.affixHauntedWard.GetComponent<BuffWard>();
                ward.buffDef = CelestineBoost;
            }
        }
    }
}