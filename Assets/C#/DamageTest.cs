using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTest : MonoBehaviour
{

   /* [SerializeField]
    private GameObject ParentObj;*/

    [SerializeField]
    private GameObject DamageObj;

    [SerializeField]
    private GameObject PosObj;

    [SerializeField]
    private Vector3 AdjPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            ViewDamage(10);
        }
    }

    private void ViewDamage(int damage)
    {
        GameObject damageObj=Instantiate(DamageObj);
        damageObj.GetComponent<TextMesh>().text=damage.ToString();
        damageObj.transform.position=PosObj.transform.position+AdjPos;
    }
}
