using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Greens
{
    public class Infusion : ItemBase
    {
        // ////////////
        //
        // Thanks to Borbo
        //
        // ///////////////

        public override string Name => ":: Items :: Greens :: Infusion";
        public override string InternalPickupToken => "infusion";

        public override string PickupText => "Killing an enemy permanently increases your maximum health.";

        public override string DescText => "Killing an enemy increases your <style=cIsHealing>health permanently</style> by <style=cIsHealing>1</style> <style=cStack>(+1 per stack)</style>, up to a <style=cIsHealing>maximum</style> of <style=cIsHealing>" + baseMaximumHealthCap + " <style=cStack>(+" + ((baseMaximumHealthCap * (1 + levelBonusMultiplier * (2 - 1))) - baseMaximumHealthCap) + " per level per stack)</style> health</style>.";

        [ConfigField("Base Maximum Health Cap", "Formula for maximum health gain: Base Maximum Health Cap * (1 + Level Bonus Multiplier * (Current Level - 1)) * Infusion", 30f)]
        public static float baseMaximumHealthCap;

        [ConfigField("Level Bonus Multiplier", "Formula for maximum health gain: Base Maximum Health Cap * (1 + Level Bonus * (Current Level - 1)) * Infusion", 0.25f)]
        public static float levelBonusMultiplier;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += ChangeBehavior;
        }

        public static void ChangeBehavior(ILContext il)
        {
            ILCursor c = new(il);

            //int bodyLoc = 17;
            int countLoc = 43;
            int capLoc = 63;

            c.GotoNext(MoveType.After,
                x => x.MatchLdsfld("RoR2.RoR2Content/Items", "Infusion"),
                x => x.MatchCallOrCallvirt<Inventory>(nameof(Inventory.GetItemCount)),
                x => x.MatchStloc(out countLoc)
                );
            c.GotoNext(MoveType.Before,
                x => x.MatchStloc(out capLoc)
                );
            c.Emit(OpCodes.Ldloc, countLoc);
            c.Emit(OpCodes.Ldloc, 15);
            // Ldloc here is infusionOrb.target = Util.FindBodyMainHurtBox(attackerBody);
            c.EmitDelegate<Func<int, int, CharacterBody, int>>((currentInfusionCap, infusionCount, body) =>
            {
                float newInfusionCap = 100 * infusionCount;

                if (body != null)
                {
                    float levelBonus = 1 + levelBonusMultiplier * (body.level - 1);

                    newInfusionCap = baseMaximumHealthCap * levelBonus * infusionCount;
                }

                return (int)newInfusionCap;
            });
        }
    }
}