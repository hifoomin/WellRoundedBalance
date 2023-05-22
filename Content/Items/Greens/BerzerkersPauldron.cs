using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API.Utils;

namespace WellRoundedBalance.Items.Greens
{
    public class BerzerkersPauldron : ItemBase<BerzerkersPauldron>
    {
        public override string Name => ":: Items :: Greens :: Berzerkers Pauldron";
        public override ItemDef InternalPickup => RoR2Content.Items.WarCryOnMultiKill;

        public override string PickupText => "Enter a frenzy after killing " + killCount + " enemies in quick succession.";

        public override string DescText => "<style=cIsDamage>Killing " + killCount + " enemies</style> within <style=cIsDamage>1</style> second sends you into a <style=cIsDamage>frenzy</style> for <style=cIsDamage>" + baseBuffDuration + "s</style> <style=cStack>(+" + buffDurationPerStack + "s per stack)</style>, which increases <style=cIsUtility>movement speed</style> by <style=cIsUtility>50%</style> and <style=cIsDamage>attack speed</style> by <style=cIsDamage>100%</style>.";

        [ConfigField("Kill Count", 3)]
        public static int killCount;

        [ConfigField("Base Buff Duration", 4.5f)]
        public static float baseBuffDuration;

        [ConfigField("Buff Duration Per Stack", 4.5f)]
        public static float buffDurationPerStack;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.AddMultiKill += CharacterBody_AddMultiKill;
        }

        private void CharacterBody_AddMultiKill(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchCallOrCallvirt(typeof(CharacterBody).GetPropertyGetter(nameof(CharacterBody.multiKillCount))),
                    x => x.MatchLdcI4(4)))
            {
                c.Index += 1;
                c.Remove();
                c.Emit(OpCodes.Ldc_I4, killCount);
            }
            else
            {
                Logger.LogError("Failed to apply Berzerker's Pauldron Buff Kill Requirement hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(2f),
                    x => x.MatchLdcR4(4f)))
            {
                c.Next.Operand = baseBuffDuration - buffDurationPerStack;
                c.Index++;
                c.Next.Operand = buffDurationPerStack;
            }
            else
            {
                Logger.LogError("Failed to apply Berzerker's Pauldron Buff Duration hook");
            }
        }
    }
}