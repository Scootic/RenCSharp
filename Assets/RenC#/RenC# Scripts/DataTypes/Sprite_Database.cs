using UnityEngine;
using System.Collections.Generic;
namespace RenCSharp
{
    [CreateAssetMenu(menuName = "New Sprite Database")]
    public class Sprite_Database : ScriptableObject
    {
        [SerializeField] private List<Sprite> sprites;
        private Dictionary<string, Sprite> spriteDictionary;
        private void OnValidate()
        {
            Debug.Log("Validating Sprite DB!");
            spriteDictionary = new(); //wipe
            foreach(Sprite s in sprites)
            {
                spriteDictionary.Add(s.name, s);
            }
            string result = "All Sprites in Dict: + \n";
            foreach (KeyValuePair<string, Sprite> kvp in spriteDictionary)
            {
                result += kvp.Key + "\n";
            }
            Debug.Log(result);
        }

        private void OnEnable()
        {
            OnValidate();
        }

        public Dictionary<string, Sprite> Sprites => spriteDictionary;
    }
}
