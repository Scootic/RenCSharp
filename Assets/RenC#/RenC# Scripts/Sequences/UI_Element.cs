using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace RenCSharp.Sequences
{
    /// <summary>
    /// So you don't have to crawl through gameobject transforms to set information to be what you want.
    /// </summary>
    public class UI_Element : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI[] texts;
        [SerializeField] private Image[] images;

        public TextMeshProUGUI[] Texts => texts;
        public Image[] Images => images;
    }
}
