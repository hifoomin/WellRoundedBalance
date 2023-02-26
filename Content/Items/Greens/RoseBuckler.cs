using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Greens
{
    public class RoseBuckler : ItemBase
    {
        public override string Name => ":: Items :: Greens :: Rose Buckler";
        public override string InternalPickupToken => "sprintArmor";

        public override string PickupText => "Reduce incoming damage while sprinting.";

        public override string DescText => "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>" + baseArmorGain + "</style> <style=cStack>(+" + armorGainPerStack + " per stack)</style> while <style=cIsUtility>sprinting</style>.";

        [ConfigField("Base Armor Gain", "", 25)]
        public static int baseArmorGain;

        [ConfigField("Armor Gain Per Stack", "", 15)]
        public static int armorGainPerStack;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeArmor;
        }

        public static void ChangeArmor(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchCallOrCallvirt<CharacterBody>("get_armor"),
                    x => x.MatchLdloc(out _),
                    x => x.MatchLdcI4(30)))
            {
                c.Index += 2;
                c.Remove();
                c.Emit(OpCodes.Ldc_I4, armorGainPerStack);
                c.Index += 1;
                c.EmitDelegate<Func<int, int>>((useless) =>
                {
                    return armorGainPerStack + (baseArmorGain - armorGainPerStack);
                });
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Rose Buckler Armor hook");
            }
        }
    }
}