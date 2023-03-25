using System;

namespace WellRoundedBalance.Artifacts.Vanilla
{
    public abstract class ArtifactEditBase<T> : ArtifactEditBase where T : ArtifactEditBase<T>
    {
        public static T instance { get; set; }

        public ArtifactEditBase()
        {
            if (instance != null)
            {
                throw new InvalidOperationException("Singleton class " + typeof(T).Name + " was instantiated twice");
            }
            instance = this as T;
        }
    }
}