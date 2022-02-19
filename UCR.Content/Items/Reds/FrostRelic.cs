using RoR2;
using UnityEngine;

namespace UltimateCustomRun
{
    public class FrostRelic : ItemBase
    {
        public static float aspd;
        public static float baseradius;
        public static float radiusperkill;
        public static float damageperaspd;
        public static float duration;
        public static float procco;
        public static int max;
        public static int maxperstack;
        public static bool guide;
        public override string Name => ":: Items ::: Reds :: Frost Relic";
        public override string InternalPickupToken => "icicle";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "Killing an enemy surrounds you with an <style=cIsDamage>ice storm</style> that deals <style=cIsDamage>" + d(damageperaspd / aspd) + " damage per second</style> and <style=cIsUtility>slows</style> enemies by <style=cIsUtility>80%</style> for <style=cIsUtility>1.5s</style>. The storm <style=cIsDamage>grows with every kill</style>, increasing its radius by <style=cIsDamage>" + radiusperkill + "m</style>. Stacks up to <style=cIsDamage>" + (baseradius + radiusperkill * max) + "m</style> <style=cStack>(+" + radiusperkill * maxperstack + "m per stack)</style>.";
        public override void Init()
        {
            aspd = ConfigOption(0.25f, "Attack Interval", "Vanilla is 0.25");
            baseradius = ConfigOption(6f, "Base Radius", "Vanilla is 6");
            radiusperkill = ConfigOption(2f, "Icicle Radius", "Per Icicle. Vanilla is 2");
            damageperaspd = ConfigOption(3f, "Damage Per Tick", "Decimal. Vanilla is 3");
            duration = ConfigOption(5f, "Duration", "Vanilla is 5");
            procco = ConfigOption(0.2f, "Proc Coefficient Per Tick", "Vanilal is 0.2");
            max = ConfigOption(6, "Maximum Icicles Amount", "Vanilla is 6");
            maxperstack = ConfigOption(6, "Stack Maximum Icicles Amount", "Per Stack. Vanilla is 6");
            guide = ConfigOption(true, "Formulas", "1 / Attack Interval = Attacks per Second\nBase Radius * (Base Icicle Cap + Icicle Cap) = Max Radius\nDamage / Attack Interval = DPS\nProc Coefficient / Attack Interval = PPS");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }
        public static void Changes()
        {
            var f = Resources.Load<GameObject>("prefabs/networkedobjects/IcicleAura");
            var fi = f.GetComponent<IcicleAuraController>();
            fi.baseIcicleAttackInterval = aspd;
            // (1 / interval) = aps
            fi.icicleBaseRadius = baseradius;
            // 6 + max icicles * radius per icicle
            fi.icicleRadiusPerIcicle = radiusperkill;
            fi.icicleDamageCoefficientPerTick = damageperaspd;
            // damage coeff * (1 / interval) = dps
            fi.icicleDuration = duration;
            fi.icicleProcCoefficientPerTick = procco;
            // proc coeff * (1 / interval) = pps
            fi.baseIcicleMax = max;
            // max icicles
            fi.icicleMaxPerStack = maxperstack;
            // per stack
            var fb = f.GetComponent<BuffWard>();
            fb.interval = aspd;
        }
    }
}
