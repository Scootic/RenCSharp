using UnityEngine;
using UnityEngine.UI;
namespace RenCSharp.Sequences
{
    [CreateAssetMenu(menuName = "New Poem")]
    public class Poem : ScriptableObject
    {
        [SerializeField] private Font poemFont;
        [SerializeField, TextArea(4, 9)] private string poemText;

        public string PoemText;
        public Font PoemFont;
    }
}
