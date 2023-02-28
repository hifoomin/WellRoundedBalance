using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Greens
{
    internal class Chronobauble : ItemBase
    {
        public static BuffDef slow50;
        public override string Name => ":: Items :: Greens :: Chronobauble";

        public override string InternalPickupToken => "slowOnHit";

        public override string PickupText => "Slow enemies on hit.";

        public override string DescText => "<style=cIsUtility>Slow</style> enemies on hit for <style=cIsUtility>-" + (Mathf.Round(slowPercent * 100f)) + "%</style> <style=cIsUtility>movement speed</style> " +
                                           (baseAttackSpeedReduction > 0 || attackSpeedReductionPerStack > 0 ? "and <style=cIsDamage>-" + d(baseAttackSpeedReduction) + "</style> <style=cStack>(-" + d(attackSpeedReductionPerStack) + " per stack)</style> <style=cIsDamage>attack speed</style> for <style=cIsUtility>" + debuffDuration + "s</style></style>." : ".");

        [ConfigField("Slow Percent", "Decimal.", 1f / 3f)]
        public static float slowPercent;

        [ConfigField("Debuff Duration", "", 5f)]
        public static float debuffDuration;

        [ConfigField("Base Attack Speed Reduction", "Decimal.", 0.15f)]
        public static float baseAttackSpeedReduction;

        [ConfigField("Attack Speed Reduction Per Stack", "Decimal.", 0.05f)]
        public static float attackSpeedReductionPerStack;

        public override void Init()
        {
            var slowIcon = Utils.Paths.Texture2D.texBuffSlow50Icon.Load<Texture2D>();

            slow50 = ScriptableObject.CreateInstance<BuffDef>();
            slow50.isHidden = false;
            slow50.canStack = false;
            slow50.buffColor = new Color32(173, 156, 105, 255);
            slow50.iconSprite = Sprite.Create(slowIcon, new Rect(0f, 0f, (float)slowIcon.width, (float)slowIcon.height), new Vector2(0f, 0f));

            slow50.name = "Chronobauble Slow";

            ContentAddition.AddBuffDef(slow50);

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
                if (sender.HasBuff(slow50) && stack > 0)
                {
                    args.moveSpeedReductionMultAdd += Mathf.Abs(1 - (1 / (1 - slowPercent)));
                    // 1 - (1/(1+0.6)) for actual slow in vanilla
                    args.attackSpeedReductionMultAdd += baseAttackSpeedReduction + attackSpeedReductionPerStack * (stack - 1);
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
                c.Emit<Chronobauble>(OpCodes.Ldsfld, nameof(slow50));
                c.Next.Operand = 5f;
                c.Index += 1;
                c.EmitDelegate<Func<float, float>>((useless) =>
                {
                    return debuffDuration;
                });
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Chronobauble Duration hook");
            }
        }
    }
}