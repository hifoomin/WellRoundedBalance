using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace WellRoundedBalance.Items.Greens
{
    public class WillOTheWisp : ItemBase
    {
        public override string Name => ":: Items :: Greens :: Will O The Wisp";
        public override string InternalPickupToken => "explodeOnDeath";

        public override string PickupText => "Detonate enemies on kill.";

        public override string DescText => "On killing an enemy, spawn a <style=cIsDamage>lava pillar</style> in a <style=cIsDamage>12m</style> <style=cStack>(+2.4m per stack)</style> radius for <style=cIsDamage>350%</style> <style=cStack>(+280% per stack)</style> base damage.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += Changes;
            ChangeProc();
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
               x => x.MatchLdloc(55),
               x => x.MatchLdcR4(2000f)))
            {
                c.Index += 1;
                c.Next.Operand = 0f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Will o' The Wisp Knockback hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcI4(2),
                x => x.MatchStfld("RoR2.DelayBlast", "falloffModel")))
            {
                c.Next.Operand = 0;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Will o' The Wisp Falloff hook");
            }
        }

        public static void ChangeProc()
        {
            var w = LegacyResourcesAPI.Load<GameObject>("prefabs/networkedobjects/WilloWispDelay").GetComponent<RoR2.DelayBlast>();
            w.procCoefficient = 0f;
        }
    }
}