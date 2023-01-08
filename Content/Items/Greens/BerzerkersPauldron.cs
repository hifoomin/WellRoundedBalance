using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using R2API.Utils;
using RoR2;
using System;

namespace WellRoundedBalance.Items.Greens
{
    public class BerzerkersPauldron : ItemBase
    {
        public override string Name => ":: Items :: Greens :: Berzerkers Pauldron";
        public override string InternalPickupToken => "warCryOnMultiKill";

        public override string PickupText => "Enter a frenzy after killing 3 enemies in quick succession.";

        public override string DescText => "<style=cIsDamage>Killing 3 enemies</style> within <style=cIsDamage>1</style> second sends you into a <style=cIsDamage>frenzy</style> for <style=cIsDamage>6s</style> <style=cStack>(+4s per stack)</style>, which increases <style=cIsUtility>movement speed</style> by <style=cIsUtility>50%</style> and <style=cIsDamage>attack speed</style> by <style=cIsDamage>100%</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.AddMultiKill += Changes;
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchCallOrCallvirt(typeof(CharacterBody).GetPropertyGetter(nameof(CharacterBody.multiKillCount))),
                    x => x.MatchLdcI4(4)))
            {
                c.Index += 1;
                c.Remove();
                c.Emit(OpCodes.Ldc_I4, 3);
                /* c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<int, int>>((useless) =>
                {
                    return KillRequirement;
                });
                */
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Berzerker's Pauldron Buff Kill Requirement hook");
            }
        }
    }
}