using UnityEngine;

namespace ManualDI
{
    public class ConsoleAudioService : IAudioService
    {
        public void PlaySound(int soundID)
        {
            Debug.Log($"[DI] Sound {soundID} has started");
        }

        public void StopSound(int soundID)
        {
            Debug.Log($"[DI] Sound {soundID} has stopped");
        }

        public void StopAllSounds()
        {
            Debug.Log("[DI] All sounds have stopped");
        }
    }
}
