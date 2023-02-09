using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Yellows
{
    public class ChargedPerforator : ItemBase
    {
        public override string Name => ":: Items :::: Yellows :: Charged Perforator";
        public override string InternalPickupToken => "lightningStrikeOnHit";

        public override string PickupText => "Chance on hit to call down a lightning strike.";

        public override string DescText => "<style=cIsDamage>10%</style> chance on hit to call down a lightning strike, dealing <style=cIsDamage>400%</style> <style=cStack>(+400% per stack)</style> TOTAL damage.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += Changes;
            Changes();
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
               x => x.MatchCallOrCallvirt<CharacterBody>("get_damage"),
               x => x.MatchLdcR4(5f)))
            {
                c.Index += 2;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, CharacterBody, float>>((useless, self) =>
                {
                    return 4f + 1.5f * (self.inventory.GetItemCount(RoR2Content.Items.LightningStrikeOnHit) - 1);
                });
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Charged Perforator Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
               x => x.MatchStfld("RoR2.Orbs.GenericDamageOrb", "procChainMask"),
               x => x.MatchDup(),
               x => x.MatchLdcR4(1f)))
            {
                c.Index += 2;
                c.Next.Operand = 0f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Charged Perforator Proc Coefficient hook");
            }
        }

        private void Changes()
        {
            LanguageAPI.Add("ITEM_lightningStrikeOnHit_NAME".ToUpper(), "Charged Peripherator");
        }
    }
}