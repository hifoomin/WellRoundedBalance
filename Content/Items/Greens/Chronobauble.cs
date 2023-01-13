using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Greens
{
    internal class Chronobauble : ItemBase
    {
        public static BuffDef slow40;
        public override string Name => ":: Items :: Greens :: Chronobauble";

        public override string InternalPickupToken => "slowOnHit";

        public override string PickupText => "Slow enemies on hit.";

        public override string DescText => "<style=cIsUtility>Slow</style> enemies on hit for <style=cIsUtility>-40%</style> <style=cStack>(+10% per stack)</style> <style=cIsUtility>movement speed</style> and <style=cIsDamage>-10%</style> <style=cStack>(+5% per stack)</style> <style=cIsDamage>attack speed</style> for <style=cIsUtility>5s</style></style>.";

        public override void Init()
        {
            var slowIcon = Utils.Paths.Texture2D.texBuffSlow50Icon.Load<Texture2D>();

            slow40 = ScriptableObject.CreateInstance<BuffDef>();
            slow40.isHidden = false;
            slow40.canStack = false;
            slow40.buffColor = new Color32(173, 156, 105, 255);
            slow40.iconSprite = Sprite.Create(slowIcon, new Rect(0f, 0f, (float)slowIcon.width, (float)slowIcon.height), new Vector2(0f, 0f));
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.SlowOnHit);
                if (sender.HasBuff(slow40))
                {
                    args.moveSpeedReductionMultAdd += Util.ConvertAmplificationPercentageIntoReductionPercentage(40f + 10f * (stack - 1));
                    args.baseAttackSpeedAdd -= 0.1f + 0.05f * (stack - 1);
                }
            }
        }

        private void GlobalEventManager_OnHitEnemy(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld("RoR2.RoR2Content/Buffs", "Slow60"),
                x => x.MatchLdcR4(2f)))
            {
                c.Remove();
                c.Emit<Chronobauble>(OpCodes.Ldsfld, nameof(slow40));
                c.Index += 1;
                c.Next.Operand = 5f;
                c.Index += 2;
                c.EmitDelegate<Func<float, float>>((useless) =>
                {
                    return 1f;
                });
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Chronobauble Duration hook");
            }
        }
    }
}