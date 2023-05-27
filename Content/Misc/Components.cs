using System;

namespace WellRoundedBalance.Misc
{
    public class DestroyStuckObject : MonoBehaviour
    {
        public float noMovementThreshold = 0.1f;
        public const int noMovementFrames = 3;
        public Vector3[] previousLocations = new Vector3[noMovementFrames];
        public float timer;
        public float initialDelay = 2.5f;

        private void Awake()
        {
            //For good measure, set the previous locations
            for (int i = 0; i < previousLocations.Length; i++)
            {
                previousLocations[i] = Vector3.zero;
            }
        }

        private void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            for (int i = 0; i < previousLocations.Length - 1; i++)
            {
                previousLocations[i] = previousLocations[i + 1];
            }
            previousLocations[previousLocations.Length - 1] = transform.position;
            //Store the newest vector at the end of the list of vectors
            if (timer >= initialDelay)
            {
                //Check the distances between the points in your previous locations
                //If for the past several updates, there are no movements smaller than the threshold,
                //you can most likely assume that the object is not moving
                for (int i = 0; i < previousLocations.Length - 1; i++)
                {
                    if (Vector3.Distance(previousLocations[i], previousLocations[i + 1]) >= noMovementThreshold)
                    {
                        break;
                    }
                    else
                    {
                        // Main.WRBLogger.LogError("below threshold, destroying");
                        Destroy(gameObject);
                    }
                }
            }
        }
    }

    public static class RecalculateEvent
    {
        public static EventHandler<RecalculateEventArgs> RecalculateBleedCap;
    }

    public class RecalculateEventArgs
    {
        public BleedCap BleedCap;

        public RecalculateEventArgs(BleedCap bleedCap)
        {
            BleedCap = bleedCap;
        }
    }

    public class BleedCap : MonoBehaviour
    {
        public CharacterBody body;
        public int bleedCap = 0;
        public int bleedCapAdd;

        private void Start()
        {
            body = GetComponent<CharacterBody>();
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender == body && sender.GetComponent<BleedCap>())
            {
                bleedCapAdd = 0;

                RecalculateEvent.RecalculateBleedCap?.Invoke(this, new(sender.GetComponent<BleedCap>()));

                bleedCap = bleedCapAdd;
            }
        }

        private void OnDestroy()
        {
            RecalculateStatsAPI.GetStatCoefficients -= RecalculateStatsAPI_GetStatCoefficients;
        }
    }
}