using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PT_PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 3;
    public float gravity = -9.18f;
    public float jumpHeight = 3f;
    [SerializeField, Header("体力")]
    public int HP = 50;
    public Slider slider;//ＨＰバー用のスライダー

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    [SerializeField, Header("ダメージ数の表示")]
    private GameObject DamageObj;

    [SerializeField]
    private Transform PosObj;

    [SerializeField]
    private Vector3 AdjPos;

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // ── Input ────────────────────────────────────────────────────────────
        float x        = 0f;
        float z        = 0f;
        bool  sprint   = false;
        bool  jumpDown = false;

#if ENABLE_INPUT_SYSTEM
        // New Input System (Unity 6 / package com.unity.inputsystem)
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.wKey.isPressed) z  =  1f;
            if (kb.sKey.isPressed) z  = -1f;
            if (kb.aKey.isPressed) x  = -1f;
            if (kb.dKey.isPressed) x  =  1f;

            sprint   = kb.leftShiftKey.isPressed;
            jumpDown = kb.spaceKey.wasPressedThisFrame;
        }
#elif ENABLE_LEGACY_INPUT_MANAGER
        // Legacy Input Manager (Unity 2022 and earlier)
        x        = Input.GetAxis("Horizontal");
        z        = Input.GetAxis("Vertical");
        sprint   = Input.GetKey(KeyCode.LeftShift);
        jumpDown = Input.GetButtonDown("Jump");
#endif
        // ─────────────────────────────────────────────────────────────────────

        speed = (sprint && isGrounded) ? 10f : 5f;

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        if (jumpDown && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void Damage(int damage)//ダメージ判定
    {
        HP-= damage;
        if(slider !=null)
        {
            slider.value=HP;//ＨＰバー
        }

        
        ViewDamage(damage);//ダメージを受けた数字を表示
    }

    private void ViewDamage(int damage)
    {
        GameObject damageObj = Instantiate(DamageObj);
        damageObj.GetComponent<TextMesh>().text = damage.ToString()+" Hit";
        damageObj.transform.position = PosObj.transform.position + AdjPos;
        // 生成時の向きを固定
        damageObj.transform.rotation = Camera.main.transform.rotation;
    }
}
