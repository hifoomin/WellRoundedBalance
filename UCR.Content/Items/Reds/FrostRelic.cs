using RoR2;
using UnityEngine;

namespace UltimateCustomRun
{
    public static class FrostRelic
    {
        public static void Changes()
        {
            var f = Resources.Load<GameObject>("prefabs/networkedobjects/IcicleAura");
            var fi = f.GetComponent<IcicleAuraController>();
            fi.baseIcicleAttackInterval = Main.FrostRelicAS.Value;
            // (1 / interval) = aps
            fi.icicleBaseRadius = Main.FrostRelicBaseRadius.Value;
            // 6 + max icicles * radius per icicle
            fi.icicleRadiusPerIcicle = Main.FrostRelicRadiusPerKill.Value;
            fi.icicleDamageCoefficientPerTick = Main.FrostRelicDamage.Value;
            // damage coeff * (1 / interval) = dps
            fi.icicleDuration = Main.FrostRelicDuration.Value;
            fi.icicleProcCoefficientPerTick = Main.FrostRelicProcCo.Value;
            // proc coeff * (1 / interval) = pps
            fi.baseIcicleMax = Main.FrostRelicMax.Value;
            // max icicles
            fi.icicleMaxPerStack = Main.FrostRelicMaxStack.Value;
            // per stack
            var fb = f.GetComponent<BuffWard>();
            fb.interval = Main.FrostRelicAS.Value;
        }
    }
}
