using UnityEngine;
namespace Aurora
{
    public sealed class AudioFeedback : MonoBehaviour
    {
        static AudioSource output;
        void Awake() { output = gameObject.AddComponent<AudioSource>(); output.volume = .35f; output.spatialBlend = 0; }
        public static void Play(string clip)
        { if (output != null) { var sound = Resources.Load<AudioClip>("Audio/" + clip); if (sound != null) output.PlayOneShot(sound); } }
    }
}
