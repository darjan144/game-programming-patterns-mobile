using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SingletonPattern
{
    public class GameController : MonoBehaviour
    {

        void Start()
        {
            TestCSharpSingleton();

            TestUnitySingleton();

            TestThreadSafeSingleton();

            TestGenericSingleton();

            TestSceneScopedSingleton();
        }


        void Update()
        {

        }



        private void TestCSharpSingleton()
        {
            SingletonCSharp instance = SingletonCSharp.Instance;

            instance.TestSingleton();

            SingletonCSharp instance2 = SingletonCSharp.Instance;

            instance2.TestSingleton();
        }



        private void TestUnitySingleton()
        {
            SingletonUnity instance = SingletonUnity.Instance;

            instance.TestSingleton();

            SingletonUnity instance2 = SingletonUnity.Instance;

            instance2.TestSingleton();
        }



        private void TestThreadSafeSingleton()
        {
            SingletonThreadSafe instance = SingletonThreadSafe.Instance;

            instance.TestSingleton();

            SingletonThreadSafe instance2 = SingletonThreadSafe.Instance;

            instance2.TestSingleton();
        }



        private void TestGenericSingleton()
        {
            AudioManagerExample instance = AudioManagerExample.Instance;

            instance.TestSingleton();

            AudioManagerExample instance2 = AudioManagerExample.Instance;

            instance2.TestSingleton();
        }



        private void TestSceneScopedSingleton()
        {
            LevelManagerExample instance = LevelManagerExample.Instance;

            instance.TestSingleton();

            LevelManagerExample instance2 = LevelManagerExample.Instance;

            instance2.TestSingleton();
        }
    }
}
