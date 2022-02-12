using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Enemies
{
    public static class BighornBison
    {
        public static CharacterBody body;
        public static void Buff()
        {
            body = Resources.Load<CharacterBody>("prefabs/characterbodies/BisonBody");
            body.GetComponent<CharacterDirection>().turnSpeed = 360f;

            On.EntityStates.Bison.Headbutt.OnEnter += (orig,self) =>
            {
                EntityStates.Bison.Headbutt.baseHeadbuttDuration = 1f;
                orig(self);
            };
            On.EntityStates.Bison.Charge.OnEnter += (orig, self) =>
            {
                EntityStates.Bison.Charge.chargeMovementSpeedCoefficient = 12f;
                EntityStates.Bison.Charge.turnSpeed = 99999f;
                EntityStates.Bison.Charge.turnSmoothTime = 0f;
                EntityStates.Bison.Charge.selfStunDuration = 0.5f;
                orig(self);
            };
        }
    }
}
