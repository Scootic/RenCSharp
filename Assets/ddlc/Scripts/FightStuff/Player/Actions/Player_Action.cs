using UnityEngine;
using UnityEngine.UI;
using System.Collections;
namespace RenCSharp.Combat.Player
{
    public abstract class Player_Action : MonoBehaviour
    {
        [Header("Visuals")]
        [SerializeField] protected Sprite ogImage;
        [SerializeField] protected Sprite selectedImage;
        [SerializeField] protected Image myImage;

        public virtual void OnActionSelect()
        {
            myImage.sprite = selectedImage;
        }

        public virtual void OnActionDeselect() 
        {
            myImage.sprite = ogImage;
        }

        public abstract IEnumerator ActionResult();
    }
}
