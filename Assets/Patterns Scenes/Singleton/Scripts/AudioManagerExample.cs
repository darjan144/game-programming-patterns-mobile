using UnityEngine;

namespace SingletonPattern
{
    public class AudioManagerExample : SingletonGeneric<AudioManagerExample>
    {
        private float volume = 0.8f;

        public void TestSingleton()
        {
            Debug.Log($"Hello this is AudioManagerExample (Generic Singleton), volume is: {volume}");
        }
    }
}
