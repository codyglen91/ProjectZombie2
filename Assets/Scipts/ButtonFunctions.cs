using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
   public void resume()
    {
        gameManager.instance.StateUnpaused();
    }
    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.StateUnpaused();
    }
    /* Build a button for next level, Add that button to the Win Menu in UnityHierarchy (not VS)*/

    /* Build a button for stat increases, Add that button to the a Menu in UnityHierarchy (not VS)*/

    public void quit()
    {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in the editor
    #else
        Application.Quit(); 
       
#endif
    }
}
