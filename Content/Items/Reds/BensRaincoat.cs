using MonoMod.Cil;
using UnityEngine;

namespace WellRoundedBalance.Items.Reds
{
    internal class BensRaincoat : ItemBase
    {
        public static BuffDef braincoatSpeed;
        public override string Name => ":: Items ::: Reds :: Bens Raincoat";

        public override string InternalPickupToken => "immuneToDebuff";

        public override string PickupText => "Prevent debuffs, instead gaining temporary movement speed. Recharges over time.";

        public override string DescText => "Gain <style=cIsUtility>20% movement speed</style>. Prevents <style=cIsUtility>1 <style=cStack>(+1 per stack)</style></style> <style=cIsDamage>debuff</style> and instead grants <style=cIsUtility>30%</style> <style=cStack>(+10% per stack)</style> <style=cIsUtility>movement speed</style> for <style=cIsUtility>3</style> seconds. Recharges every <style=cIsUtility>second</style>.";

        public override void Init()
        {
            var whip = Utils.Paths.Texture2D.texMoveSpeedIcon.Load<Texture2D>();

            braincoatSpeed = ScriptableObject.CreateInstance<BuffDef>();
            braincoatSpeed.buffColor = new Color32(224, 164, 0, 255);
            braincoatSpeed.isDebuff = false;
            braincoatSpeed.iconSprite = Sprite.Create(whip, new Rect(0f, 0f, (float)whip.width, (float)whip.height), new Vector2(0f, 0f));
            braincoatSpeed.canStack = false;

            ContentAddition.AddBuffDef(braincoatSpeed);

            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.Items.ImmuneToDebuffBehavior.TryApplyOverride += Changes;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.Items.ImmuneToDebuffBehavior.OnDisable += ImmuneToDebuffBehavior_OnDisable;
        }

        private void ImmuneToDebuffBehavior_OnDisable(On.RoR2.Items.ImmuneToDebuffBehavior.orig_OnDisable orig, RoR2.Items.ImmuneToDebuffBehavior self)
        {
            if (self.body)
            {
                self.body.AddTimedBuff(braincoatSpeed, 3f);
            }
            orig(self);
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
                        args.moveSpeedMultAdd += 0.2f;
                        if (sender.HasBuff(braincoatSpeed))
                        {
                            args.moveSpeedMultAdd += 0.3f + 0.15f * (stack - 1);
                        }
                    }
                }
            }
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.1f)))
            {
                c.Next.Operand = 0f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Ben's Raincoat Barrier hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(5f)))
            {
                c.Next.Operand = 1f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Ben's Raincoat Recharge hook");
            }
        }
    }
}