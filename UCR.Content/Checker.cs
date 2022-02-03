using System;
namespace UltimateCustomRun
{
    public abstract class Based<T> : Based where T : Based<T>
    {
        public static T instance { get; set; }
        public Based()
        {
            if (instance != null)
            {
                throw new InvalidOperationException("Singleton class " + typeof(T).Name + " was instantiated twice");
            }
            instance = this as T;
        }
    }
}
