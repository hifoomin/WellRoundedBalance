namespace WellRoundedBalance.Gamemodes.Eclipse
{
    internal class PredictionUtils
    {
        public static Ray PredictAimray(Ray aimRay, TeamIndex attackerTeam, float maxTargetAngle, float projectileSpeed, HurtBox targetHurtBox)
        {
            bool skip = false;
            if (Run.instance)
            {
                if (Run.instance.selectedDifficulty <= DifficultyIndex.Eclipse2)
                    skip = true;
            }

            bool hasHurtbox = false;
            if (!skip)
            {
                if (targetHurtBox == null)
                {
                    targetHurtBox = AcquireTarget(aimRay, attackerTeam, maxTargetAngle);
                }
                hasHurtbox = targetHurtBox && targetHurtBox.healthComponent && targetHurtBox.healthComponent.body && targetHurtBox.healthComponent.body.characterMotor;
            }

            if (!skip && hasHurtbox && projectileSpeed > 0f)
            {
                CharacterBody targetBody = targetHurtBox.healthComponent.body;
                Vector3 targetPosition = targetHurtBox.transform.position;
                Vector3 targetVelocity = targetBody.characterMotor.velocity;

                if (targetVelocity.sqrMagnitude > 0f && !(targetBody && targetBody.hasCloakBuff))   //Dont bother predicting stationary targets
                {
                    //A very simplified way of estimating, won't be 100% accurate.
                    Vector3 currentDistance = targetPosition - aimRay.origin;
                    float timeToImpact = currentDistance.magnitude / projectileSpeed;

                    //Vertical movenent isn't predicted well by this, so just use the target's current Y
                    Vector3 lateralVelocity = new Vector3(targetVelocity.x, 0f, targetVelocity.z);
                    Vector3 futurePosition = targetPosition + lateralVelocity * timeToImpact;

                    //Only attempt prediction if player is jumping upwards.
                    //Predicting downwards movement leads to groundshots.
                    if (targetBody.characterMotor && !targetBody.characterMotor.isGrounded && targetVelocity.y > 0f)
                    {
                        //point + vt + 0.5at^2
                        float futureY = targetPosition.y + targetVelocity.y * timeToImpact;
                        futureY += 0.5f * Physics.gravity.y * timeToImpact * timeToImpact;
                        futurePosition.y = futureY;
                    }

                    Ray newAimray = new Ray
                    {
                        origin = aimRay.origin,
                        direction = (futurePosition - aimRay.origin).normalized
                    };

                    float angleBetweenVectors = Vector3.Angle(aimRay.direction, newAimray.direction);
                    if (angleBetweenVectors <= maxTargetAngle)
                    {
                        return newAimray;
                    }
                }
            }

            return aimRay;
        }

        public static Ray PredictAimrayPS(Ray aimRay, TeamIndex attackerTeam, float maxTargetAngle, GameObject projectilePrefab, HurtBox targetHurtBox)
        {
            float speed = -1f;
            if (projectilePrefab)
            {
                ProjectileSimple ps = projectilePrefab.GetComponent<ProjectileSimple>();
                if (ps)
                {
                    speed = ps.desiredForwardSpeed;

                    if (attackerTeam != TeamIndex.Player)
                    {
                        if (ps.rigidbody && !ps.rigidbody.useGravity)
                        {
                            speed = Main.GetProjectileSimpleModifiers(speed);
                        }
                    }
                }
            }

            if (speed <= 0f)
            {
                Debug.LogError("AccurateEnemies: Could not get speed of ProjectileSimple.");
                return aimRay;
            }

            return speed > 0f ? PredictAimray(aimRay, attackerTeam, maxTargetAngle, speed, targetHurtBox) : aimRay;
        }

        public static Ray PredictAimrayPCC(Ray aimRay, TeamIndex attackerTeam, float maxTargetAngle, GameObject projectilePrefab, HurtBox targetHurtBox)
        {
            float speed = -1f;
            if (projectilePrefab)
            {
                ProjectileCharacterController pcc = projectilePrefab.GetComponent<ProjectileCharacterController>();
                if (pcc)
                {
                    speed = pcc.velocity;
                }
            }

            if (speed <= 0f)
            {
                Debug.LogError("AccurateEnemies: Could not get speed of ProjectileCharacterController.");
                return aimRay;
            }

            return PredictAimray(aimRay, attackerTeam, maxTargetAngle, speed, targetHurtBox);
        }

        public static HurtBox AcquireTarget(Ray aimRay, TeamIndex attackerTeam, float maxTargetAngle)
        {
            BullseyeSearch search = new BullseyeSearch();

            search.teamMaskFilter = TeamMask.allButNeutral;
            search.teamMaskFilter.RemoveTeam(attackerTeam);

            search.filterByLoS = true;
            search.searchOrigin = aimRay.origin;
            search.sortMode = BullseyeSearch.SortMode.Angle;
            search.maxDistanceFilter = 200f;
            search.maxAngleFilter = maxTargetAngle;
            search.searchDirection = aimRay.direction;
            search.RefreshCandidates();

            HurtBox targetHurtBox = search.GetResults().FirstOrDefault();

            return targetHurtBox;
        }

        public static HurtBox GetMasterAITargetHurtbox(CharacterMaster cm)
        {
            if (cm && cm.aiComponents.Length > 0)
            {
                foreach (BaseAI ai in cm.aiComponents)
                {
                    if (ai.currentEnemy != null && ai.currentEnemy.bestHurtBox != null)
                    {
                        return ai.currentEnemy.bestHurtBox;
                    }
                }
            }
            return null;
        }
    }
}