using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace WellRoundedBalance.Items.Reds
{
    public class FrostRelic : ItemBase
    {
        public override string Name => ":: Items ::: Reds :: Frost Relic";
        public override string InternalPickupToken => "icicle";

        public override string PickupText => "Killing enemies surrounds you with an ice storm.";

        public override string DescText => "Killing an enemy surrounds you with an <style=cIsDamage>ice storm</style> that deals <style=cIsDamage>900% damage per second</style> and <style=cIsUtility>slows</style> enemies by <style=cIsUtility>80%</style> for <style=cIsUtility>1.5s</style>. The storm <style=cIsDamage>grows with every kill</style>, increasing its radius by <style=cIsDamage>2m</style>. Stacks up to <style=cIsDamage>18m</style> <style=cStack>(+12m per stack)</style>.";

        // slows arent accurate in ror2
        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        public static void Changes()
        {
            var f = LegacyResourcesAPI.Load<GameObject>("prefabs/networkedobjects/IcicleAura");
            var fi = f.GetComponent<IcicleAuraController>();
            fi.icicleDamageCoefficientPerTick = 2.25f;
            fi.icicleProcCoefficientPerTick = 0f;
        }
    }
}