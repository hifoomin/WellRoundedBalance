using System;

namespace WellRoundedBalance.Artifacts.New
{
    public abstract class ArtifactAddBase<T> : ArtifactAddBase where T : ArtifactAddBase<T>
    {
        public static T instance { get; set; }

        public ArtifactAddBase()
        {
            if (instance != null)
            {
                throw new InvalidOperationException("Singleton class " + typeof(T).Name + " was instantiated twice");
            }
            instance = this as T;
        }
    }
}