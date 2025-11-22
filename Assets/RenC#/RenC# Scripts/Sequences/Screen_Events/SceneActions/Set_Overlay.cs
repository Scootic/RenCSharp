using UnityEngine;
using UnityEngine.UI;
namespace RenCSharp.Sequences
{
    public class Set_Overlay : Screen_Event
    {
        [SerializeField] private Sprite imageToSet;

        public override void DoShit()
        {
            GameObject.Find("Overlay").GetComponent<Image>().sprite = imageToSet;
        }

        public override string ToString()
        {
            return "Set Overlay Image";
        }
    }
}
