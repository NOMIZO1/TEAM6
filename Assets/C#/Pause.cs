using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    [SerializeField]
    private GameObject インベントリ;
    private GameObject Instance;
    private bool isOpen=false;
    // Start is called before the first frame update
    void Start()
    {
        インベントリ.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Tab))
        {
            isOpen = !isOpen;
            インベントリ.SetActive(isOpen);

            if(isOpen )
            {
                Time.timeScale= 0;
            }

            else
            {
                インベントリ.SetActive(false);
                Time.timeScale = 1f;
            }
        }

        
    }
}
