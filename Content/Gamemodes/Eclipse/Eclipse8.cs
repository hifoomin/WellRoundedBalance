using EntityStates.LaserTurbine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Gamemodes.Eclipse
{
    internal class Eclipse8 : GamemodeBase<Eclipse8>
    {
        // look at Mechanic>Bosses>Enrage
        public override string Name => ":: Gamemode : Eclipse 8";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        private void HealthComponent_TakeDamage(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.4f)))
            {
                c.Index++;
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldarg_1);
                c.EmitDelegate<Func<float, HealthComponent, DamageInfo, float>>((orig, healthComponent, damageInfo) =>
                {
                    var isSelfDamage = damageInfo.attacker != healthComponent.body.gameObject;
                    if (!isSelfDamage)
                    {
                        Logger.LogError("not is self damage");
                        return orig;
                    }
                    else
                    {
                        Logger.LogError("is self damage");
                        return 0f;
                    }
                });
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Eclipse 8 hook");
            }
        }
    }
}