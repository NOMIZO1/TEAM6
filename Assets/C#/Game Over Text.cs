using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverText : MonoBehaviour
{
    [SerializeField]
    GameObject gameover;

    [SerializeField]
    CanvasGroup CG;
    // Start is called before the first frame update
    void Start()
    {
        gameover.SetActive(true);
        CG.alpha = 0;
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(0.5f);

        float time = 0f;
        float duration = 4f;

        while(time<duration)
        {
            time += Time.deltaTime;
            CG.alpha=time/duration;
            yield return null;
        }

        CG.alpha = 1;
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }
}
