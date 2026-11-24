
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    [Header("ˆÚ“®æ‚ÌScene–¼")]
    [SerializeField] private string nextSceneName;

    // Button‚©‚çŒÄ‚Ño‚·
    public void ChangeScene()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogWarning("ˆÚ“®æ‚ÌScene–¼‚ªİ’è‚³‚ê‚Ä‚¢‚Ü‚¹‚ñI");
            return;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}
