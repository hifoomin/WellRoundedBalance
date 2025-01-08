using System;

namespace WellRoundedBalance.Items.Greens
{
    public class RedWhip : ItemBase<RedWhip>
    {
        public override string Name => ":: Items :: Greens :: Red Whip";
        public override ItemDef InternalPickup => RoR2Content.Items.SprintOutOfCombat;

        public override string PickupText => "Move faster out of combat.";
        public override string DescText => (oocReduction > 0 ? "Leaving combat takes <style=cIsUtility>" + oocReduction + "s</style> less. " : "") + "Leaving combat boosts your <style=cIsUtility>movement speed</style> by <style=cIsUtility>" + d(movementSpeedGain) + "</style> <style=cStack>(+" + d(movementSpeedGain) + " per stack)</style>.";

        [ConfigField("Movement Speed Gain", "Decimal.", 0.35f)]
        public static float movementSpeedGain;

        [ConfigField("Out of Combat Timer Reduction", "", 0.5f)]
        public static float oocReduction;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeSpeed;
            IL.RoR2.CharacterBody.UpdateOutOfCombatAndDanger += CharacterBody_FixedUpdate;
        }

        private void CharacterBody_FixedUpdate(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdfld("RoR2.CharacterBody", "outOfCombatStopwatch"),
                x => x.MatchLdcR4(out _)))
            {
                c.Index += 2;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, CharacterBody, float>>((orig, self) =>
                {
                    var inventory = self.inventory;
                    if (inventory)
                    {
                        var stack = inventory.GetItemCount(RoR2Content.Items.SprintOutOfCombat);
                        if (stack > 0)
                        {
                            return orig - oocReduction;
                        }
                    }
                    return orig;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Red Whip Out Of Combat Timer Reduction hook");
            }
        }

        private void ChangeSpeed(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchCallOrCallvirt<CharacterBody>("HasBuff"),
                    x => x.MatchBrfalse(out _),
                    x => x.MatchLdloc(out _),
                    x => x.MatchLdloc(out _),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(0.3f)))
            {
                c.Index += 5;
                c.Next.Operand = movementSpeedGain;
            }
            else
            {
                Logger.LogError("Failed to apply Red Whip Speed hook");
            }
        }
    }
}