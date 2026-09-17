using UnityEngine;

namespace ManualDI
{
    public interface IAudioService
    {
        void PlaySound(int soundID);
        void StopSound(int soundID);
        void StopAllSounds();
    }
}
