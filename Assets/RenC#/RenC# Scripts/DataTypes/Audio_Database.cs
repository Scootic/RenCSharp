using UnityEngine;
using System.Collections.Generic;
namespace RenCSharp
{
    [CreateAssetMenu(menuName = "New Audio Database")]
    public class Audio_Database : ScriptableObject
    {
        [SerializeField] private List<AudioClip> sounds;
        [SerializeField] private bool debugValidation = false;
        private Dictionary<string, AudioClip> soundDictionary;
        private void OnValidate()
        {
            if(debugValidation) Debug.Log("Validating Audio DB!");
            soundDictionary = new();
            foreach (AudioClip a in sounds)
            {
                soundDictionary.Add(a.name, a);
            }
            if (debugValidation)
            {
                string result = "All Sounds in Dict: + \n";
                foreach (KeyValuePair<string, AudioClip> kvp in soundDictionary)
                {
                    result += kvp.Key + "\n";
                }
                Debug.Log(result);
            }
        }

        private void OnEnable()
        {
            OnValidate();
        }
        public Dictionary<string, AudioClip> Sounds => soundDictionary;
    }
}
