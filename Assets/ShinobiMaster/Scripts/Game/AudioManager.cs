using System.Collections;
using UnityEngine;

namespace Game
{
	public class AudioManager: MonoBehaviour
	{
		public static AudioManager Instance { get; private set; }


		private const string SoundsActiveKey = "sounds_active";
		private const string MusicActiveKey = "music_active";

		public AudioClip[] playerAttackSounds;
		public float attackSoundsVolume;
		public AudioClip playerJumpSound;
		public float playerJumpVolume;
		public AudioClip enemyPreAttackSound;
		public float enemyPreAttackVolume;
		public AudioClip enemyAttackSound;
		public float enemyAttackVolume;
		public AudioClip clickButtonSound;
		public float clickButtonVolume;
		public AudioClip loseSound;
		public float loseSoundVolume;
		public AudioClip winSound;
		public float winSoundVolume;

		[SerializeField] private AudioSource musicAudioSource;
		[SerializeField] private AudioSource slowDownAudioSource;
		[SerializeField] private AudioSource otherAudioSource;
		public float MusicInLobbyVolume;
		public float MusicInGameVolume;
		public float SlowPitch;
		private Coroutine musicVolumeCoroutine;
        // Add these fields to AudioManager
        public AudioClip introTheme;
        public AudioClip bossFightTheme;
        public AudioClip actionPhaseTheme;

        // Optionally, you can add volumes for each theme if needed
        public float introThemeVolume = 1f;
        public float bossFightThemeVolume = 1f;
        public float actionPhaseThemeVolume = 1f;



		public bool SoundsActive
		{
			get => bool.Parse(PlayerPrefs.GetString(SoundsActiveKey, "True"));
			private set
			{
				PlayerPrefs.SetString(SoundsActiveKey, value.ToString());
				PlayerPrefs.Save();
			}
		}
		
		public bool MusicActive
		{
			get => bool.Parse(PlayerPrefs.GetString(MusicActiveKey, "True"));
			private set
			{
				PlayerPrefs.SetString(MusicActiveKey, value.ToString());
				PlayerPrefs.Save();
			}
		}


		private void Awake()
		{
			if (Instance)
			{
				Destroy(gameObject);
				return;
			}
			else
			{
				Instance = this;
			}
		}




		public void PlayPlayerAttackSound()
		{
			this.slowDownAudioSource.PlayOneShot(this.playerAttackSounds[Random.Range(0, this.playerAttackSounds.Length)]
				, this.attackSoundsVolume);
		}
		
		public void PlayPlayerJumpSound()
		{
			this.otherAudioSource.PlayOneShot(this.playerJumpSound, this.playerJumpVolume);
		}
		
		public void PlayEnemyPreAttackSound()
		{
			this.slowDownAudioSource.PlayOneShot(this.enemyPreAttackSound, this.enemyPreAttackVolume);
		}
		
		public void PlayEnemyAttackSound()
		{
			this.slowDownAudioSource.PlayOneShot(this.enemyAttackSound, this.enemyAttackVolume);
		}
		
		public void PlayClickButtonSound()
		{
			this.otherAudioSource.PlayOneShot(this.clickButtonSound, this.clickButtonVolume);
		}
		
		public void PlayLoseSound()
		{
			this.otherAudioSource.PlayOneShot(this.loseSound, this.loseSoundVolume);
		}
		
		public void PlayWinSound()
		{
			this.otherAudioSource.PlayOneShot(this.winSound, this.winSoundVolume);
		}

		public void SetSlowDownAudioSourcePitch(float pitch)
		{
			this.slowDownAudioSource.pitch = pitch;
		}

		public void SetMusicASVolume(float vol)
		{
			this.musicAudioSource.volume = vol;
		}

		public void SetMusicVolumeSmooth(float vol, float time)
		{
			if (this.musicVolumeCoroutine != null)
			{
				StopCoroutine(this.musicVolumeCoroutine);
			}
		
			this.musicVolumeCoroutine = StartCoroutine(SetMusicVolumeSmoothProcess(vol, time));
		}

		private IEnumerator SetMusicVolumeSmoothProcess(float vol, float time)
		{
			var startVol = this.musicAudioSource.volume;

			var startTime = time;

			while (time > 0)
			{
				var lerp = 1 - time / startTime;

				this.musicAudioSource.volume = Mathf.Lerp(startVol, vol, lerp);
			
				time -= Time.deltaTime;

				yield return null;
			}

			this.musicAudioSource.volume = vol;
		}

		public void SetActiveMusic(bool active)
		{
			this.musicAudioSource.mute = !active;

			MusicActive = active;
		}

		public void SetActiveSounds(bool active)
		{
			this.otherAudioSource.mute = !active;
			this.slowDownAudioSource.mute = !active;

			SoundsActive = active;
		}

        // Add these methods to AudioManager
        public void PlayIntroTheme()
        {
            PlayTheme(introTheme, introThemeVolume);
        }

        public void PlayBossFightTheme()
        {
            PlayTheme(bossFightTheme, bossFightThemeVolume);
        }

        public void PlayActionPhaseTheme()
        {
            PlayTheme(actionPhaseTheme, actionPhaseThemeVolume);
        }

        // Helper method to play a theme
        private void PlayTheme(AudioClip themeClip, float volume)
        {
            if (musicAudioSource.clip == themeClip && musicAudioSource.isPlaying)
                return;

            musicAudioSource.Stop();
            musicAudioSource.clip = themeClip;
            musicAudioSource.volume = volume;
            musicAudioSource.Play();
        }
	}
}