using EntityEngineV4.Engine;
using Microsoft.Xna.Framework.Audio;

namespace EntityEngineV4.Components
{
    public class Sound : Component
    {
        /// <summary>
        /// The sound to process
        /// </summary>
        public SoundEffectInstance SoundEffect;

        /// <summary>
        /// The volume of the sound
        /// </summary>
        public float Volume
        {
            get { return SoundEffect.Volume; }
            set { SoundEffect.Volume = value; }
        }

        /// <summary>
        /// The pitch of the sound
        /// </summary>
        public float Pitch
        {
            get { return SoundEffect.Pitch; }
            set { SoundEffect.Pitch = value; }
        }

        /// <summary>
        /// The panning of the sound
        /// </summary>
        public float Pan
        {
            get { return SoundEffect.Pan; }
            set { SoundEffect.Pan = value; }
        }

        /// <summary>
        /// If the track should loop or not.
        /// </summary>
        public bool Loop
        {
            get { return SoundEffect.IsLooped; }
            set { SoundEffect.IsLooped = value; }
        }

        public bool IsPlaying { get; private set; }

        public bool IsPaused { get; private set; }

        public Sound(Node e, string name)
            : base(e, name)
        {
        }

        public Sound(Node e, string name, SoundEffect _sound)
            : base(e, name)
        {
            SoundEffect = _sound.CreateInstance();
            Volume = 1.0f;
            Pitch = 0.0f;
            Pan = 0.0f;
            Loop = false;
        }

        /// <summary>
        /// Plays this instance.
        /// </summary>
        public void Play()
        {
            SoundEffect.Play();
            IsPlaying = true;
            IsPaused = false;
        }

        /// <summary>
        /// Pauses this instance.
        /// </summary>
        public void Pause()
        {
            SoundEffect.Pause();
            IsPlaying = false;
            IsPaused = true;
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            SoundEffect.Stop();
            IsPlaying = false;
            IsPaused = false;
        }

        public void LoadSound(string location)
        {
            SoundEffect = EntityGame.Self.Content.Load<SoundEffect>(location).CreateInstance();
        }
    }
}