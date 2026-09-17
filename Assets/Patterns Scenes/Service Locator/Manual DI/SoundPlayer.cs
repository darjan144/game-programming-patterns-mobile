using UnityEngine;

namespace ManualDI
{
    public class SoundPlayer
    {
        private readonly IAudioService audioService;

        public SoundPlayer(IAudioService audioService)
        {
            this.audioService = audioService;
        }

        public void PlayEffect(int soundID)
        {
            audioService.PlaySound(soundID);
        }

        public void StopEffect(int soundID)
        {
            audioService.StopSound(soundID);
        }

        public void Silence()
        {
            audioService.StopAllSounds();
        }
    }
}
