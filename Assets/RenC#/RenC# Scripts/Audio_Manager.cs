using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RenCSharp
{
    public sealed class Audio_Manager : MonoBehaviour
    {
        public static Audio_Manager AM; //global variable, every object can see and send messages to the audio manager

        [SerializeField, Range(1, 20)] private int sfxAmount = 20; //dictates how many sound effects we can have at once
        [SerializeField] private GameObject audioObject; //prefab for 3D sfx
        private AudioSource[] sfxSources; //stores 2D sfx
        [SerializeField] private List<GameObject> sfxList = new List<GameObject>(); //stores 3D sfx
        private AudioSource leMusic; //stores the background music

        private int sfxIndex = 0; //Store the current sfx index

        [Range(0, 1)] private float bgmVolMult = 0.5f, sfxVolMult = 0.5f, esfxVolMult = 0.5f; //volume multipliers

        private void InitSFX()
        {
            sfxSources = new AudioSource[sfxAmount]; //set the size of the audiosources array

            leMusic = gameObject.AddComponent<AudioSource>();//create new audio source, make it the bgm

            for (int i = 0; i < sfxAmount; i++) //for size of array, add a new audio source, and put it into the correct index of the array
            {
                sfxSources[i] = gameObject.AddComponent<AudioSource>();
            }
        }

        private void Awake() //is called before Start method, at start of the game
        {
            if (AM == null) //does the audiomanager exist?
            {
                AM = this; //if it doesn't exist, make me manager
            }
            else if (AM != this) // if it does exist but not me
            {
                Destroy(this); // game end me
            }

            InitSFX(); // now that the manager is up, initialize all needed audio sources
            DontDestroyOnLoad(gameObject);
        }
        #region 2DSFX
        public void Play2DSFX(AudioClip clipToPlay, float volume = 1, bool environmental = false)
        {
            sfxSources[sfxIndex].clip = clipToPlay; //tell the current audio source to load x clip
            sfxSources[sfxIndex].volume = volume * (environmental ? esfxVolMult : sfxVolMult);
            sfxSources[sfxIndex].Play(); //tell the current audio source to play

            sfxIndex++; //increment to next current clip

            if (sfxIndex >= sfxSources.Length) //check if index goes out of range
            {
                sfxIndex = 0; //reset index if true
            }
        }

        public void Stop2DSFX(AudioClip clipToStop, bool onlyStopOne = true)
        {
            foreach (AudioSource source in sfxSources)
            {
                if (source.clip = clipToStop)
                {
                    source.Stop();
                    if (onlyStopOne) return;
                }
            }
        }
        #endregion

        #region 3DSFX
        public void Play3DSFX(AudioSource ThingToPlay, Vector3 position, bool environmental, bool loop)
        {
            AudioObjectCheck();
            
            GameObject gaming = Instantiate(audioObject, position, Quaternion.identity); //Quaternion.identity is basically default for Quaternions
            gaming.transform.SetParent(null); //prevent dumbass going away with despawning objects?
            sfxList.Add(gaming);

            if (gaming.GetComponent<AudioSource>() == null) //if no audio source
            {
                gaming.AddComponent<AudioSource>(); //create an audio source
            }

            AudioSource temp = gaming.GetComponent<AudioSource>();
            temp = ThingToPlay; //works or nah???
            temp.spatialBlend = 1f;
            temp.volume = 1; //reset volume because object pooling
            temp.loop = loop;
            temp.volume *= environmental ? esfxVolMult : sfxVolMult;
            temp.Play();

            if(!loop) StartCoroutine(BleanUp(gaming, ThingToPlay.clip.length));
        }

        public void Play3DSFX(AudioClip clipToPlay, Vector3 position, bool environmental, bool loop)
        {
            AudioObjectCheck();

            GameObject gaming = Instantiate(audioObject, position, Quaternion.identity);
            gaming.transform.SetParent(null);
            sfxList.Add(gaming);

            if(gaming.GetComponent<AudioSource>() == null)
            {
                gaming.AddComponent<AudioSource>();
            }

            AudioSource temp = gaming.GetComponent<AudioSource>();
            temp.clip = clipToPlay;
            temp.spatialBlend = 1;
            temp.volume = 1;
            temp.volume *= environmental ? esfxVolMult : sfxVolMult;
            temp.loop = loop;
            temp.Play();

            if(!loop) StartCoroutine(BleanUp(gaming, temp.clip.length));
        }

        void AudioObjectCheck()
        {
            if(audioObject == null)
            {
                audioObject = new GameObject();
                audioObject.name = "DefaultAudioObj";
                audioObject.AddComponent<AudioSource>();
            }
        }

        public void Stop3DSFX(AudioClip clipToRemove, bool removeOnlyOne = true)
        {
            foreach (GameObject go in sfxList)
            {
                if (go.GetComponent<AudioSource>().clip == clipToRemove)
                {
                    sfxList.Remove(go);
                    Destroy(go);
                    if (removeOnlyOne) return;
                }
            }
        }

        public void Stop3DSFX(GameObject goToRemove)
        {
            sfxList.Remove(goToRemove);
            Destroy(goToRemove);
        }

        private IEnumerator BleanUp(GameObject gaming, float duration)
        {
            yield return new WaitForSeconds(duration);
            if (sfxList.Contains(gaming))
            {
                sfxList.Remove(gaming);
                Destroy(gaming);
            }
        }
        #endregion

        #region BGM
        //only exists so a coroutine can be called by another script
        public void PlayBGM(AudioClip musicToPlay, float fadeTime = 5f, bool isLooping = true, bool setSameTime = false)
        {
            if (musicToPlay != null) StartCoroutine(PlayBGMPog(musicToPlay, fadeTime, isLooping, setSameTime));
            else Debug.Log("You didn't give AM a clip to play bgm! Dumbass!");
        }

        private IEnumerator PlayBGMPog(AudioClip musicToPlay, float fadeTime = 3f, bool isLooping = true, bool setSameTime = false)
        {
            AudioSource newBGM = gameObject.AddComponent<AudioSource>(); //make a new Audio sauce
            newBGM.clip = musicToPlay; //Init the new sauce, based on passed in values
            newBGM.volume = 0;
            newBGM.loop = isLooping;
            newBGM.Play();

            if (setSameTime) newBGM.time = leMusic.time;
            float t = 0; //shorthand for time, starting at 0

            while (t < fadeTime)
            {
                //increase t by amount of time passed between frames
                t += Time.deltaTime;
                //calc percent of time that has passed, based on fadeTime
                float perc = t / fadeTime;
                //fade the musics out/in
                leMusic.volume = Mathf.Lerp(bgmVolMult, 0, perc);
                newBGM.volume = Mathf.Lerp(0, bgmVolMult, perc);
                //yield the frame, then continue
                yield return null;
            }
            //destroy unneeded audio sauce
            Destroy(leMusic);
            //set new sauce where the old sauce was
            leMusic = newBGM;
        }

        public bool SameBGM(AudioClip musicToKompare)
        {
            if (leMusic.clip == musicToKompare) return true;
            else return false;
        }
        #endregion

    }
}
