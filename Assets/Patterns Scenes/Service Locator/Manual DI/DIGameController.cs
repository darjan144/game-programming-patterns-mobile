using UnityEngine;

namespace ManualDI
{
    public class DIGameController : MonoBehaviour
    {
        private SoundPlayer soundPlayer;

        void Start()
        {
            IAudioService audioService = new ConsoleAudioService();

            soundPlayer = new SoundPlayer(audioService);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                soundPlayer.PlayEffect(23);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                soundPlayer.StopEffect(23);
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                soundPlayer.Silence();
            }
        }
    }
}
