using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Judgment : MonoBehaviour
{
    [SerializeField, Header("攻撃力")]
    public int Power = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            // プレイヤーのスクリプトを取得
            PT_PlayerMovement player =
                other.GetComponent<PT_PlayerMovement>();

            //プレイヤーにダメージを与える
            if (player != null)
            {
                player.Damage(Power);
            }
        }
    }
}
