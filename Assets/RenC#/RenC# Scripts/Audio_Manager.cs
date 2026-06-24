using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using EXPERIMENTAL;
namespace RenCSharp
{
    /// <summary>
    /// This mf handles audio, 2D, 3D, and BGM.
    /// </summary>
    public sealed class Audio_Manager : MonoBehaviour
    {
        public static Audio_Manager AM; //global variable, every object can see and send messages to the audio manager

        [SerializeField, Range(1, 20)] private int sfxAmount = 20; //dictates how many sound effects we can have at once
        [SerializeField] private GameObject audioObject; //prefab for 3D sfx
        private AudioSource[] sfxSources; //stores 2D sfx
        [SerializeField] private List<AudioSource> sfxList = new(); //stores 3D sfx
        private AudioSource leMusic, newBGM; //stores the background music
        [SerializeField] private List<SFXToken> loopingSFXAddresses = new();

        private int sfxIndex = 0; //Store the current sfx index
        private int realSFXSize; //?
        private bool enteringBGM = false; //used to see if we are currently doing a BGM fade transition.
        private Coroutine bgmRoutine; //stores the current bgm transition coroutine incase we need to stop it preemptively
        private string songAssetGUID; //keeps track of asset guid that'll be passed to save/load

        [Range(0, 1)] private float bgmVolMult = 0.5f, sfxVolMult = 0.5f, esfxVolMult = 0.5f; //volume multipliers
        public float BGMVol => bgmVolMult;
        public float SFXVol => sfxVolMult;
        public float ESFXVol => esfxVolMult;
        public AudioClip CurrentBGM => leMusic.clip;
        public string SongAssetGUID => songAssetGUID;
        public List<SFXToken> GetLoopingSFXGUIDs => loopingSFXAddresses;

        private void InitSFX()
        {
            sfxSources = new AudioSource[sfxAmount]; //set the size of the audiosources array

            leMusic = gameObject.AddComponent<AudioSource>();//create new audio source, make it the bgm
            realSFXSize = 64 - sfxAmount - 2; //64 is hard-set max real voices value, minus 2D sfx amount, minus 2 sources for fading music tracks.

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
                Destroy(gameObject); // game end me
            }

            Event_Bus.AddFloatEvent("BGMVol", ReceiveBGM); //the accursed event_bus!
            Event_Bus.AddFloatEvent("SFXVol", ReceiveSFX);
            Event_Bus.AddFloatEvent("ESFXVol", ReceiveESFX);

            bgmVolMult = PlayerPrefs.GetFloat("BGMVol");
            sfxVolMult = PlayerPrefs.GetFloat("SFXVol");
            esfxVolMult = PlayerPrefs.GetFloat("ESFXVol");

            InitSFX(); // now that the manager is up, initialize all needed audio sources
            DontDestroyOnLoad(gameObject);
        }

        public async Awaitable StopAllESFX()
        {
            for(int i = loopingSFXAddresses.Count - 1; i > -1; i--)
            {
                AsyncOperationHandle aso = Addressables.LoadAssetAsync<AudioClip>(loopingSFXAddresses[i].SFXAddress);
                await aso.Task;

                if(aso.Status == AsyncOperationStatus.Failed)
                {
                    Addressables.Release(aso);
                    continue;
                }

                AudioClip stinker = aso.Result as AudioClip;

                if (loopingSFXAddresses[i].xPos == 0 && loopingSFXAddresses[i].yPos == 0 && loopingSFXAddresses[i].zPos == 0)
                {
                    Stop2DSFX(stinker, true);
                }
                else
                {
                    Stop3DSFX(stinker, true);
                }
                loopingSFXAddresses.RemoveAt(i);
            }
        }

        #region 2DSFX
        /// <summary>
        /// Play a 2D sound effect.
        /// </summary>
        /// <param name="clipToPlay">The clip that will be played by AM</param>
        /// <param name="minRand">the lowest pitch it can randomly be</param>
        /// <param name="maxRand">the highest pitch it can randomly be</param>
        /// <param name="volume">volume multiplier based on settings</param>
        /// <param name="environmental">whether or not to use environmental or regular sound settings</param>
        public void Play2DSFX(AudioClip clipToPlay, float minRand = 1, float maxRand = 1, float volume = 1, bool environmental = false, bool loop = false)
        {
            if (environmental && AlreadyPlaying2DSFX(clipToPlay)) return; //no duped esfx 

            sfxSources[sfxIndex].clip = clipToPlay; //tell the current audio source to load x clip
            if (!environmental)
            {
                sfxSources[sfxIndex].volume = volume * sfxVolMult;
            }
            else
            {
                sfxSources[sfxIndex].volume = 0;
                StartCoroutine(FadeInSFXVolume(sfxSources[sfxIndex], 1, volume, environmental));
            }
            sfxSources[sfxIndex].loop = loop;
            sfxSources[sfxIndex].pitch = Random.Range(minRand, maxRand);
            sfxSources[sfxIndex].Play(); //tell the current audio source to play

            sfxIndex++; //increment to next current clip

            if (sfxIndex >= sfxSources.Length) //check if index goes out of range
            {
                sfxIndex = 0; //reset index if true
            }
        }

        public async Awaitable Play2DSFX(AssetReference sfxToPlay, float minRand = 1, float maxRand = 1, float volume = 1, bool environmental = false, bool loop = false)
        {
            if (sfxToPlay.IsValid())
            {
                AudioClip sfxer = sfxToPlay.OperationHandle.Result as AudioClip;

                if (environmental && loop)
                {
                    SFXToken toAdd = new SFXToken();
                    toAdd.SFXAddress = sfxToPlay.AssetGUID;
                    toAdd.yPos = 0;
                    toAdd.zPos = 0;
                    toAdd.xPos = 0;
                    toAdd.localVolume = volume;
                    loopingSFXAddresses.Add(toAdd);
                }

                Play2DSFX(sfxer, minRand, maxRand, volume, environmental, loop);

                return;
            }

            AsyncOperationHandle sfxHandle = sfxToPlay.LoadAssetAsync<AudioClip>();
            await sfxHandle.Task;

            if (sfxHandle.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError("Failed to load sfx asset: " + sfxToPlay.Asset.name);
                Addressables.Release(sfxHandle);
                return;
            }

            AudioClip sfx = sfxHandle.Result as AudioClip;

            if (environmental && loop)
            {
                SFXToken toAdd = new SFXToken();
                toAdd.SFXAddress = sfxToPlay.AssetGUID;
                toAdd.yPos = 0;
                toAdd.zPos = 0;
                toAdd.xPos = 0;
                toAdd.localVolume = volume;
                loopingSFXAddresses.Add(toAdd);
            }

            Play2DSFX(sfx, minRand, maxRand, volume, environmental, loop);
        }

        public async Awaitable Play2DSFX(string sfxGUID, float minRand = 1, float maxRand = 1, float volume = 1, bool environmental = false, bool loop = false)
        {
            AsyncOperationHandle stinker = Addressables.LoadAssetAsync<AudioClip>(sfxGUID);
            await stinker.Task;

            if(stinker.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError("couldnae load sfx to play at: " + sfxGUID);
                Addressables.Release(stinker);
                return;
            }

            AudioClip man = stinker.Result as AudioClip;

            SFXToken toAdd = new();
            toAdd.xPos = 0;
            toAdd.yPos = 0;
            toAdd.zPos = 0;
            toAdd.SFXAddress = sfxGUID;
            toAdd.localVolume = volume;
            loopingSFXAddresses.Add(toAdd);

            Play2DSFX(man, minRand, maxRand, volume, environmental, loop);
        }


        /// <summary>
        /// Plays a random 2D sfx from an array of sounds, without repeating the previous one.
        /// </summary>
        /// <param name="clips">Array of sound clips that the desired sound will be found from</param>
        /// <param name="randomID">name of integer id, to remember what the prev roll was</param>
        /// <param name="minRand">the lowest pitch it can randomly be</param>
        /// <param name="maxRand">the highest pitch it can randomly be</param>
        /// <param name="volume">volume multiplier based on settings</param>
        /// <param name="environmental">whether or not to use environmental or regular sound settings.</param>
        public void PlayRandom2DSFX(AudioClip[] clips, string randomID, float minRand = 1, float maxRand = 1, float volume = 1, bool environmental = false)
        {
            int randI = RandomHelper.NoRepeatRoll(randomID, clips.Length);
            Play2DSFX(clips[randI], minRand, maxRand, volume, environmental);
        }

        public void Stop2DSFX(AudioClip clipToStop, bool onlyStopOne = true, bool fadeOut = false)
        {
            foreach (AudioSource source in sfxSources)
            {
                if (source.clip = clipToStop)
                {
                    if(!fadeOut)source.Stop();
                    else
                    if (onlyStopOne) return;
                }
            }
        }

        public async Awaitable Stop2DSFX(AssetReference sfxToStop, bool onlyStopOne = true, bool fadeOut = false)
        {
            for(int i = loopingSFXAddresses.Count - 1; i > -1 ;i--)
            {
                if (loopingSFXAddresses[i].SFXAddress == sfxToStop.AssetGUID)
                {
                    loopingSFXAddresses.RemoveAt(i);
                    if(onlyStopOne) break;
                }
            }

            if (sfxToStop.IsValid())
            {
                AudioClip c = sfxToStop.OperationHandle.Result as AudioClip;
                Stop2DSFX(c, onlyStopOne);
                return;
            }

            AsyncOperationHandle handle = sfxToStop.LoadAssetAsync<AudioClip>();
            await handle.Task;

            if(handle.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError("Failed to stop SFX: " + sfxToStop.Asset);
                Addressables.Release(handle);
                return;
            }

            AudioClip sfx = handle.Result as AudioClip;
            Stop2DSFX(sfx, onlyStopOne);
        }

        public bool AlreadyPlaying2DSFX(AudioClip clipToCheck)
        {
            foreach(AudioSource sauce in sfxSources)
            {
                if (sauce.clip == clipToCheck && sauce.isPlaying) return true;
            }
            return false;
        }
        #endregion

        #region 3DSFX
        public void Play3DSFX(AudioSource ThingToPlay, Vector3 position, bool environmental = false, bool loop = false, float vol = 1, float minRand = 1, float maxRand = 1)
        {
            AudioObjectCheck();
            RemoveOldest3DSFXCheck();
            
            GameObject gaming = Object_Pooling.Spawn(audioObject, position, Quaternion.identity); //Quaternion.identity is basically default for Quaternions
            gaming.transform.SetParent(null); //prevent dumbass going away with despawning objects?

            if (gaming.GetComponent<AudioSource>() == null) //if no audio source
            {
                gaming.AddComponent<AudioSource>(); //create an audio source
            }

            AudioSource temp = gaming.GetComponent<AudioSource>();
            sfxList.Add(temp);
            temp = ThingToPlay; //works or nah???
            temp.spatialBlend = 1f;
            temp.volume = vol; //reset volume because object pooling
            temp.loop = loop;
            temp.pitch = Random.Range(minRand, maxRand);
            temp.volume *= environmental ? esfxVolMult : sfxVolMult;
            temp.Play();

            if(!loop) StartCoroutine(BleanUp(temp, ThingToPlay.clip.length));
        }

        public void Play3DSFX(AudioClip clipToPlay, Vector3 position, bool environmental = false, bool loop = false, float vol = 1, float minRand = 1, float maxRand = 1)
        {
            AudioObjectCheck();
            RemoveOldest3DSFXCheck();
            if (environmental && _3DSFXAlreadyPlaying(clipToPlay)) return; //no dupes of esfx. probably not great

            GameObject gaming = Object_Pooling.Spawn(audioObject, position, Quaternion.identity);
            gaming.transform.SetParent(null);

            if(gaming.GetComponent<AudioSource>() == null)
            {
                gaming.AddComponent<AudioSource>();
            }

            AudioSource temp = gaming.GetComponent<AudioSource>();
            sfxList.Add(temp);
            temp.clip = clipToPlay;
            temp.spatialBlend = 1;
            temp.pitch = Random.Range(minRand, maxRand);
            temp.loop = loop;
            if (!environmental)
            {
                temp.volume = vol;
                temp.volume *= sfxVolMult;
            }
            else
            {
                temp.volume = 0;
                StartCoroutine(FadeInSFXVolume(temp, 1, vol, environmental));
            }
            temp.Play();

            if(!loop) StartCoroutine(BleanUp(temp, temp.clip.length));
        }

        public async Awaitable Play3DSFX(AssetReference sfxToPlay, Vector3 position, float minRand = 1, float maxRand = 1, float volume = 1, bool environmental = false, bool loop = false)
        {
            if (sfxToPlay.IsValid())
            {
                AudioClip sfxer = sfxToPlay.OperationHandle.Result as AudioClip;

                if (environmental && loop)
                {
                    SFXToken toAdd = new SFXToken();
                    toAdd.SFXAddress = sfxToPlay.AssetGUID;
                    toAdd.yPos = position.y;
                    toAdd.zPos = position.z;
                    toAdd.xPos = position.x;
                    toAdd.localVolume = volume;
                    loopingSFXAddresses.Add(toAdd);
                }

                Play3DSFX(sfxer, position, environmental, loop, volume, minRand, maxRand);

                return;
            }

            AsyncOperationHandle sfxHandle = sfxToPlay.LoadAssetAsync<AudioClip>();
            await sfxHandle.Task;

            if (sfxHandle.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError("Failed to load sfx asset: " + sfxToPlay.Asset.name);
                Addressables.Release(sfxHandle);
                return;
            }

            AudioClip sfx = sfxHandle.Result as AudioClip;

            if (environmental && loop)
            {
                SFXToken toAdd = new SFXToken();
                toAdd.SFXAddress = sfxToPlay.AssetGUID;
                toAdd.yPos = position.y;
                toAdd.zPos = position.x;
                toAdd.xPos = position.z;
                toAdd.localVolume = volume;
                loopingSFXAddresses.Add(toAdd);
            }

            Play3DSFX(sfx, position, environmental, loop, minRand, maxRand);
        }

        public async Awaitable Play3DSFX(string sfxGUID, Vector3 position, float minRand = 1, float maxRand = 1, float volume = 1, bool environmental = false, bool loop = true)
        {
            AsyncOperationHandle loadSFX = Addressables.LoadAssetAsync<AudioClip>(sfxGUID);
            await loadSFX.Task;

            if(loadSFX.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError("could play sfx at guid: " + sfxGUID);
                Addressables.Release(loadSFX);
                return;
            }

            SFXToken toAdd = new SFXToken();
            toAdd.xPos = position.x;
            toAdd.yPos = position.y;
            toAdd.zPos = position.z;
            toAdd.SFXAddress = sfxGUID;
            toAdd.localVolume = volume;
            loopingSFXAddresses.Add(toAdd);

            AudioClip stinker = loadSFX.Result as AudioClip;
            Play3DSFX(stinker, position, environmental, loop, volume, minRand, maxRand);
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

        void RemoveOldest3DSFXCheck() 
        {
            if (sfxList.Count > realSFXSize)
            {
                AudioSource go = sfxList[0];
                sfxList.RemoveAt(0);
                Object_Pooling.Despawn(go.gameObject);
            }
        }

        bool _3DSFXAlreadyPlaying(AudioClip clipTocheck)
        {
            foreach(AudioSource AS in sfxList)
            {
                if (AS.clip == clipTocheck) return true;
            }
            return false;
        }

        /// <summary>
        /// Removes a 3DSFX from scene
        /// </summary>
        /// <param name="clipToRemove">The type of sound that will be removed</param>
        /// <param name="removeOnlyOne">Remove every instance of this sound, or just the first we find</param>
        public void Stop3DSFX(AudioClip clipToRemove, bool removeOnlyOne = true, bool fadeOut = false)
        {
            for(int i = sfxList.Count - 1; i > -1;i--)
            {
                if (sfxList[i].clip == clipToRemove)
                {
                    AudioSource stinker = sfxList[i];
                    if (!fadeOut)
                    {
                        sfxList.RemoveAt(i);
                        Object_Pooling.Despawn(stinker.gameObject);
                    }
                    else
                    {
                        StartCoroutine(FadeOut3DSFX(stinker, 1));
                    }
                    if (removeOnlyOne) break;
                }
            }
        }

        public async Awaitable Stop3DSFX(AssetReference clipToRemove, bool removeOnlyOne = true, bool fadeOut = false)
        {
            for(int i = loopingSFXAddresses.Count - 1; i > -1; i--)
            {
                if(clipToRemove.AssetGUID == loopingSFXAddresses[i].SFXAddress)
                {
                    loopingSFXAddresses.RemoveAt(i);
                    if (removeOnlyOne) break;
                }
            }

            if (clipToRemove.IsValid())
            {
                AudioClip sfxer = clipToRemove.OperationHandle.Result as AudioClip;
                Stop3DSFX(sfxer, removeOnlyOne, fadeOut);
            }

            AsyncOperationHandle stinker = clipToRemove.LoadAssetAsync<AudioClip>();
            await stinker.Task;

            if(stinker.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError("couldn't find sfx: " + clipToRemove.Asset + " to remove.");
                Addressables.Release(stinker);
                return;
            }

            AudioClip sfx = stinker.Result as AudioClip;
            Stop3DSFX(sfx, removeOnlyOne, fadeOut);
        }

        /// <summary>
        /// Removes a 3DSFX from scene
        /// </summary>
        /// <param name="goToRemove">The gameobject reference of the specific sound you want gone</param>
        public void Stop3DSFX(AudioSource goToRemove)
        {
            sfxList.Remove(goToRemove);
            Object_Pooling.Despawn(goToRemove.gameObject);
        }
        /// <summary>
        /// cleans up a 3d sfx from the sfxList
        /// </summary>
        /// <param name="gaming">The gameobject that's playing the sound that we wish to be gone</param>
        /// <param name="duration">how long it takes to despawn sound obj</param>
        /// <returns>Jack</returns>
        private IEnumerator BleanUp(AudioSource gaming, float duration)
        {
            yield return new WaitForSeconds(duration);
            if (sfxList.Contains(gaming))
            {
                sfxList.Remove(gaming);
                Object_Pooling.Despawn(gaming.gameObject);
            }
        }

        private IEnumerator FadeOut3DSFX(AudioSource gaming, float fadeOutDuration)
        {
            float t = fadeOutDuration;
            float perc;
            float ogVol = gaming.volume;
            while (t >= 0)
            {
                t -= Time.deltaTime;
                perc = t / fadeOutDuration;
                gaming.volume = ogVol * perc;
                yield return null;
            }
            gaming.Stop();
            if (sfxList.Contains(gaming))
            {
                sfxList.Remove(gaming);
                Object_Pooling.Despawn(gaming.gameObject);
            }
        }
        #endregion

        private IEnumerator FadeInSFXVolume(AudioSource source, float fadeDuration, float endBaseVolume, bool environmental)
        {
            float t = 0;
            source.volume = 0;
            while(t <= fadeDuration)
            {
                t += Time.deltaTime;
                float perc = t / fadeDuration;
                source.volume = endBaseVolume * perc * (environmental ? esfxVolMult : sfxVolMult);
                yield return null;
            }
            source.volume = endBaseVolume * (environmental ? esfxVolMult : sfxVolMult);
        }

        #region BGM
        /// <summary>
        /// plays a song.
        /// </summary>
        /// <param name="musicToPlay"></param>
        /// <param name="fadeTime"></param>
        /// <param name="isLooping"></param>
        /// <param name="setSameTime"></param>
        public void PlayBGM(AudioClip musicToPlay, float fadeTime = 5f, bool isLooping = true, bool setSameTime = false)
        {
            if (musicToPlay != null) 
            {
                if (enteringBGM) //bail out of a fade IF we're already doing one, and just do the new one instaed
                {
                    if (newBGM != null) Destroy(newBGM);
                    StopCoroutine(bgmRoutine);
                }
                bgmRoutine = StartCoroutine(PlayBGMPog(musicToPlay, fadeTime, isLooping, setSameTime)); 
            }
            else Debug.Log("You didn't give AM a clip to play bgm! Dumbass!");
        }
        /// <summary>
        /// plays a song from an asset reference. async moment.
        /// </summary>
        /// <param name="musicAsset"></param>
        /// <param name="fadeTime"></param>
        /// <param name="isLooping"></param>
        /// <param name="setSameTime"></param>
        /// <returns></returns>
        public async Awaitable PlayBGM(AssetReference musicAsset, float fadeTime =5f, bool isLooping = true, bool setSameTime = false)
        {
            if (musicAsset.IsValid())
            {
                AudioClip clip = musicAsset.OperationHandle.Result as AudioClip;
                songAssetGUID = musicAsset.AssetGUID;

                if (enteringBGM)
                {
                    if (newBGM != null) Destroy(newBGM);
                    StopCoroutine(bgmRoutine);
                }
                bgmRoutine = StartCoroutine(PlayBGMPogAddressable(clip, fadeTime, isLooping, setSameTime));
                return;
            }

            AsyncOperationHandle songHandle = musicAsset.LoadAssetAsync<AudioClip>();

            await songHandle.Task;

            if(songHandle.Status == AsyncOperationStatus.Failed) 
            { 
                Debug.LogWarning("Failed to load song asset: " + musicAsset.Asset);
                Addressables.Release(songHandle);
                return; 
            }

            AudioClip song = songHandle.Result as AudioClip;
            songAssetGUID = musicAsset.AssetGUID;
            //Addressables.Release(songHandle);

            if (enteringBGM)
            {
                if (newBGM != null) Destroy(newBGM);
                StopCoroutine(bgmRoutine);
            }
            bgmRoutine = StartCoroutine(PlayBGMPogAddressable(song, fadeTime, isLooping, setSameTime));
        }
        /// <summary>
        /// play song from an asset's GUID. async moment.
        /// </summary>
        /// <param name="assetGUID"></param>
        /// <param name="fadeTime"></param>
        /// <param name="isLooping"></param>
        /// <param name="setSameTime"></param>
        /// <returns></returns>
        public async Awaitable PlayBGM(string assetGUID, float fadeTime = 5f, bool isLooping = true, bool setSameTime = false)
        {
            AsyncOperationHandle songHandle = Addressables.LoadAssetAsync<AudioClip>(assetGUID);
            await songHandle.Task;

            if(songHandle.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogWarning("Failed to load song asset (guid): " + assetGUID);
                Addressables.Release(songHandle);
                return;
            }

            AudioClip song = songHandle.Result as AudioClip;
            songAssetGUID = assetGUID;

            if (enteringBGM)
            {
                if (newBGM != null) Destroy(newBGM);
                StopCoroutine(bgmRoutine);
            }
            bgmRoutine = StartCoroutine(PlayBGMPogAddressable(song, fadeTime, isLooping, setSameTime));
        }

        private IEnumerator PlayBGMPog(AudioClip musicToPlay, float fadeTime = 3f, bool isLooping = true, bool setSameTime = false)
        {
            enteringBGM = true;
            newBGM = gameObject.AddComponent<AudioSource>(); //make a new Audio sauce
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
            enteringBGM = false;
        }

        private IEnumerator PlayBGMPogAddressable(AudioClip musicToPlay, float fadeTime = 3f, bool isLooping = true, bool setSameTime = false)
        {
            enteringBGM = true;
            newBGM = gameObject.AddComponent<AudioSource>(); //make a new Audio sauce
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
            if(leMusic.clip != null) Addressables.Release(leMusic.clip);
            Destroy(leMusic);
            //set new sauce where the old sauce was
            leMusic = newBGM;
            enteringBGM = false;
        }

        public bool SameBGM(AudioClip musicToKompare)
        {
            if (leMusic.clip == musicToKompare) return true;
            else return false;
        }
        #endregion

        #region Settings
      
        private void ReceiveBGM(float f)
        {
            bgmVolMult = f;
            if (!enteringBGM && leMusic != null) leMusic.volume = f;
        }

        void ReceiveSFX(float f)
        {
            sfxVolMult = f;
        }

        void ReceiveESFX(float f)
        {
            esfxVolMult = f;
        }
        #endregion
    }
}
