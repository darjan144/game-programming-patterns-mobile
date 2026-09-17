using UnityEngine;

namespace SingletonPattern
{
    public class LevelManagerExample : SingletonSceneScoped<LevelManagerExample>
    {
        private int currentLevel = 1;

        public void TestSingleton()
        {
            Debug.Log($"Hello this is LevelManagerExample (Scene-Scoped Singleton), level is: {currentLevel}");
        }
    }
}
