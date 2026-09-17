using System;
using UnityEngine;

namespace SingletonPattern
{
    public class SingletonThreadSafe
    {
        private static readonly Lazy<SingletonThreadSafe> instance =
            new Lazy<SingletonThreadSafe>(() => new SingletonThreadSafe());

        private float randomNumber;

        public static SingletonThreadSafe Instance => instance.Value;

        private SingletonThreadSafe()
        {
            randomNumber = UnityEngine.Random.Range(0f, 1f);
        }

        public void TestSingleton()
        {
            Debug.Log($"Hello this is ThreadSafe Singleton, my random number is: {randomNumber}");
        }
    }
}
