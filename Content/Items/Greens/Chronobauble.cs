using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Greens
{
    internal class Chronobauble : ItemBase<Chronobauble>
    {
        public static BuffDef slow50;
        public override string Name => ":: Items :: Greens :: Chronobauble";

        public override ItemDef InternalPickup => RoR2Content.Items.SlowOnHit;

        public override string PickupText => "Slow enemies on hit.";

        public override string DescText => "<style=cIsUtility>Slow</style> enemies on hit" +
            StackDesc(movementDebuff, movementDebuffStack, init => $" for <style=cIsUtility>-{d(init)}</style>{{Stack}} <style=cIsUtility>movement speed</style>", d) +
            StackDesc(attackSpeedDebuff, attackSpeedDebuffStack, init => $"{((movementDebuff != 0 || movementDebuffStack != 0) ? " and" : "")} <style=cIsDamage>{d(init)}</style>{{Stack}} <style=cIsDamage>attack speed</style>", d) +
            StackDesc(duration, durationStack, init => $" for <style=cIsUtility>{s(init, "second")}</style>{{Stack}}.", stack => s(stack, "second"));

        [ConfigField("Duration", "Decimal.", 5f)]
        public static float duration;

        [ConfigField("Duration per Stack", "Decimal.", 0f)]
        public static float durationStack;

        [ConfigField("Duration is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float durationIsHyperbolic;

        [ConfigField("Movement Debuff", "Decimal.", 0.33f)]
        public static float movementDebuff;

        [ConfigField("Movement Debuff per Stack", "Decimal.", 0f)]
        public static float movementDebuffStack;

        [ConfigField("Movement Debuff is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float movementDebuffIsHyperbolic;

        [ConfigField("Attack Speed Debuff", "Decimal.", 0.15f)]
        public static float attackSpeedDebuff;

        [ConfigField("Attack Speed Debuff per Stack", "Decimal.", 0.05f)]
        public static float attackSpeedDebuffStack;

        [ConfigField("Attack Speed Debuff is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 1f)]
        public static float attackSpeedDebuffIsHyperbolic;

        public override void Init()
        {
            var slowIcon = Utils.Paths.Texture2D.texBuffSlow50Icon.Load<Texture2D>();

            slow50 = ScriptableObject.CreateInstance<BuffDef>();
            slow50.isHidden = false;
            slow50.canStack = true;
            slow50.buffColor = new Color32(173, 156, 105, 255);
            slow50.iconSprite = Sprite.Create(slowIcon, new Rect(0f, 0f, slowIcon.width, slowIcon.height), new Vector2(0f, 0f));

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
                if (sender.HasBuff(slow50))
                {
                    var stack = sender.GetBuffCount(slow50);
                    args.moveSpeedReductionMultAdd += Mathf.Abs(1 - (1 / (1 + StackAmount(movementDebuff, movementDebuffStack, stack, movementDebuffIsHyperbolic))));
                    // 1 - (1/(1+0.6)) for actual slow in vanilla
                    args.attackSpeedReductionMultAdd += StackAmount(attackSpeedDebuff, attackSpeedDebuffStack, stack, attackSpeedDebuffIsHyperbolic);
                }
            }
        }

        private void GlobalEventManager_OnHitEnemy(ILContext il)
        {
            ILCursor c = new(il);
            int stack = GetItemLoc(c, nameof(RoR2Content.Items.SlowOnHit));
            int body = -1;
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdloc(out body), x => x.MatchLdsfld(typeof(RoR2Content.Buffs), nameof(RoR2Content.Buffs.Slow60))))
            {
                c.Emit(OpCodes.Pop);
                c.EmitDelegate(() => Buffs.Useless.chronobaubleUseless);
            }
            else Logger.LogError("Failed to apply Chronobauble Buff Type hook");
            if (c.TryGotoNext(MoveType.After, x => x.MatchCallOrCallvirt<CharacterBody>(nameof(CharacterBody.AddTimedBuff))))
            {
                c.Emit(OpCodes.Ldloc, body);
                c.Emit(OpCodes.Ldloc, stack);
                c.EmitDelegate<Action<CharacterBody, int>>((body, stack) =>
                {
                    float time = StackAmount(duration, durationStack, stack, durationIsHyperbolic);
                    body.ClearTimedBuffs(slow50);
                    for (var i = 0; i < stack; i++) body.AddTimedBuff(slow50, time, stack);
                });
            }
            else Logger.LogError("Failed to apply Chronobauble Duration hook");
        }
    }
}