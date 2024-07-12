using R2API.Utils;
using System;

namespace WellRoundedBalance.Gamemodes.Simulacrum
{
    internal class Fog : GamemodeBase<Fog>
    {
        public override string Name => ":: Gamemode :: Simulacrum Fog";

        [ConfigField("Percent Max HP Per Second", "Decimal.", 0.02f)]
        public static float maxHpDamage;

        [ConfigField("Percent Damage Ramping Per Second", "Decimal.", 0.175f)]
        public static float rampingDamage;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var fog = Utils.Paths.GameObject.InfiniteTowerFogDamager.Load<GameObject>().GetComponent<FogDamageController>();
            fog.healthFractionPerSecond = maxHpDamage;
            fog.healthFractionRampCoefficientPerSecond = rampingDamage;
        }
    }
}