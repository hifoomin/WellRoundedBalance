﻿using MonoMod.Cil;
using RoR2.Items;

namespace WellRoundedBalance.Items.Reds
{
    internal class BensRaincoat : ItemBase<BensRaincoat>
    {
        public static BuffDef braincoatSpeed;
        public override string Name => ":: Items ::: Reds :: Bens Raincoat";

        public override ItemDef InternalPickup => DLC1Content.Items.ImmuneToDebuff;

        public override string PickupText => "Prevent debuffs, instead gaining temporary movement speed. Recharges over time.";

        public override string DescText => (passiveMovementSpeedGain > 0 ? "Gain <style=cIsUtility>" + d(passiveMovementSpeedGain) + " movement speed</style>. " : "") +
                                           "Prevents <style=cIsUtility>1 <style=cStack>(+1 per stack)</style></style> <style=cIsDamage>debuff</style> and instead grants <style=cIsUtility>" + d(baseBuffMovementSpeedGain) + "</style>" +
                                           (buffMovementSpeedGainPerStack > 0 ? " <style=cStack>(+" + d(buffMovementSpeedGainPerStack) + " per stack)</style>" : "") +
                                           " <style=cIsUtility>movement speed</style> for <style=cIsUtility>" + buffDuration + "</style> seconds." +
                                           (rechargeTime > 0 ? (rechargeTime != 1 ? " Recharges every <style=cIsUtility>" + rechargeTime + " seconds</style>." : " Recharges every <style=cIsUtility>second</style>.") : "");

        [ConfigField("Passive Movement Speed Gain", "Decimal.", 0.2f)]
        public static float passiveMovementSpeedGain;

        [ConfigField("Base Buff Movement Speed Gain", "Decimal.", 0.3f)]
        public static float baseBuffMovementSpeedGain;

        [ConfigField("Buff Movement Speed Gain Per Stack", "Decimal.", 0f)]
        public static float buffMovementSpeedGainPerStack;

        [ConfigField("Recharge Time", 1f)]
        public static float rechargeTime;

        [ConfigField("Buff Duration", 3f)]
        public static float buffDuration;

        public override void Init()
        {
            var whip = Utils.Paths.Texture2D.texMovespeedBuffIcon.Load<Texture2D>();

            braincoatSpeed = ScriptableObject.CreateInstance<BuffDef>();
            braincoatSpeed.buffColor = new Color32(224, 164, 0, 255);
            braincoatSpeed.isDebuff = false;
            braincoatSpeed.iconSprite = Sprite.Create(whip, new Rect(0f, 0f, (float)whip.width, (float)whip.height), new Vector2(0f, 0f));
            braincoatSpeed.canStack = false;
            braincoatSpeed.isHidden = false;

            braincoatSpeed.name = "Ben's Raincoat Speed Boost";

            ContentAddition.AddBuffDef(braincoatSpeed);

            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.Items.ImmuneToDebuffBehavior.TryApplyOverride += ImmuneToDebuffBehavior_TryApplyOverride;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.Items.ImmuneToDebuffBehavior.TryApplyOverride += ImmuneToDebuffBehavior_TryApplyOverride1;
        }

        private bool ImmuneToDebuffBehavior_TryApplyOverride1(On.RoR2.Items.ImmuneToDebuffBehavior.orig_TryApplyOverride orig, CharacterBody body)
        {
            var hasRaincoat = body.GetComponent<ImmuneToDebuffBehavior>();
            if (hasRaincoat)
            {
                body.AddTimedBuff(braincoatSpeed, buffDuration);
            }
            return orig(body);
        }

        private void ImmuneToDebuffBehavior_TryApplyOverride(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.1f)))
            {
                c.Next.Operand = 0f;
            }
            else
            {
                Logger.LogError("Failed to apply Ben's Raincoat Barrier hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(5f)))
            {
                c.Next.Operand = rechargeTime;
            }
            else
            {
                Logger.LogError("Failed to apply Ben's Raincoat Recharge hook");
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender)
            {
                if (sender.inventory)
                {
                    var stack = sender.inventory.GetItemCount(DLC1Content.Items.ImmuneToDebuff);
                    if (stack > 0)
                    {
                        args.moveSpeedMultAdd += passiveMovementSpeedGain;
                        if (sender.HasBuff(braincoatSpeed))
                        {
                            args.moveSpeedMultAdd += baseBuffMovementSpeedGain + buffMovementSpeedGainPerStack * (stack - 1);
                        }
                    }
                }
            }
        }
    }
}