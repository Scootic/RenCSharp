using UnityEngine;
using UnityEngine.SceneManagement;
namespace EXPERIMENTAL
{
    public class Simple_Scene_Loader : MonoBehaviour
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
