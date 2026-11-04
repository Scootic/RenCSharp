using UnityEngine;
using UnityEngine.SceneManagement;
namespace RenCSharp
{
    public sealed class Simple_Scene_Loader : MonoBehaviour
    {
        public void LoadAnScene(int index)
        {
            SceneManager.LoadScene(index);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
