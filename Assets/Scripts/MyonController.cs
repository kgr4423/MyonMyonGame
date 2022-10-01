using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]

public class MyonController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private SeManager seManager;
    [SerializeField] private BgmManager bgmManager; 
    [SerializeField] private GameObject endText; 
    [SerializeField] private GameObject restartText; 
    [SerializeField] private float moveSpeed;
    [SerializeField] private float chargeDirSpeed;
    [SerializeField] private float chargePowerSpeed;
    [SerializeField] private float maxChargePower;
    [SerializeField] private float playerSize;
    private bool isMoving = false;
    private bool isCharging = false;
    private bool isJumping = false;
    private bool onAir = true;
    private bool isFalling = false;
    private bool isLanding = false;
    private bool isFirstPlayOfLandAnimation = true;
    private bool canEndGame = false;
    private int afterEndCount = 0;
    private float horizontal;
    private float speedY;
    private float chargeDir = 0;
    private float chargePower = 0;
    

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        speedY = rb.velocity.y;

        isMoving = (horizontal != 0);
        onAir = (-1.0f < rb.velocity.y && rb.velocity.y < 1.0f && isJumping);
        isFalling = (rb.velocity.y < -3.0f);

        if(!canEndGame){
            //移動処理
            if(!isCharging && !isJumping && isLanding){
                rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
            }else if(isCharging && !isJumping){
                rb.velocity = new Vector2(0, rb.velocity.y);
            }else{
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);

                //ジャンプ中、キーを押し込んでる方向へ微妙に動くようにする
                rb.velocity += new Vector2(horizontal * 0.005f, 0);
            }

            //移動方向に合わせてプレイヤーの画像を反転させる
            if (isMoving && !isCharging){
                ReverseMovingCharacter();
            }else{
                ReverseChargingCharacter();
            }

            //チャージ処理
            if(Input.GetKey(KeyCode.Space)){
                if(isLanding && !isCharging){
                    ResetChargeState();
                    StartCharge();
                    isCharging = true;
                }
                if(isCharging){
                    ChangeChargeDir();
                    ChargeJumpPower();
                    ChangeCharacterSize();
                }
            }else{
                if(isCharging){
                    isCharging = false;
                    isJumping = true;
                    Jump();
                    ResetCharacterSize();
                }
            }

            //サウンド
            CheckAndPlayLandSE();  

        }else{
            //ゴール地点到着後の処理
            AfterEnd();
        }
        
        if(Input.GetKeyDown(KeyCode.Return)){
            SceneManager.LoadScene("MainScene");
        }
        

        //Animatorの変数に値をセットする
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsCharging", isCharging);
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("OnAir", onAir);
        animator.SetBool("IsFalling", isFalling);
        animator.SetBool("IsLanding", isLanding);
        animator.SetFloat("ChargeDir", chargeDir);
        animator.SetFloat("SpeedY", speedY);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EndArea"))
        {
            canEndGame = true;
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Stage"))
        {
            isJumping = false;
            isLanding = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Stage")){
            isJumping = true;
            isLanding = false;
        }
    }

    private void ReverseMovingCharacter(){
        //移動中、キーの入力方向にキャラクターを向かせる
        Vector3 scale = gameObject.transform.localScale;
        if(horizontal < 0 && scale.x > 0 || horizontal > 0 && scale.x < 0)
        {
            scale.x = -playerSize * Mathf.Sign(scale.x);
        }
        gameObject.transform.localScale = scale;
    }

    private void ReverseChargingCharacter(){
        //チャージ中、キーの入力方向にキャラクターを向かせる
        Vector3 scale = gameObject.transform.localScale;
        if(chargeDir < 0 && scale.x > 0 || chargeDir > 0 && scale.x < 0){
            scale.x = -playerSize * Mathf.Sign(scale.x);
        }
        gameObject.transform.localScale = scale;
    }

    private void StartCharge(){
        //移動中にチャージを始めたら移動方向を初期方向としてチャージを始める
        if(horizontal < 0){
            chargeDir = -79;
        }else if(horizontal > 0){
            chargeDir = 79;
        }
    }

    private void ChargeJumpPower(){
        //チャージパワーはmaxChargePowerまで
        if(chargePower < maxChargePower){
            chargePower += chargePowerSpeed;
        }
    }

    private void ChangeCharacterSize(){
        //チャージパワーが大きくなるにつれてキャラクターが縦につぶれる
        Vector3 scale = gameObject.transform.localScale;
        scale.x = (playerSize + chargePower / 10) * Mathf.Sign(scale.x);
        scale.y = playerSize - chargePower / 20;
        gameObject.transform.localScale = scale;
    }

    private void ChangeChargeDir(){
        //押してるキーの方向にジャンプ方向が向き、何もキーを押してないときはジャンプ方向が真上に向かう
        if(Input.GetKey(KeyCode.D) && Mathf.Abs(chargeDir) < 80){
            chargeDir += chargeDirSpeed;
        }else if(Input.GetKey(KeyCode.A) && Mathf.Abs(chargeDir) < 80){
            chargeDir -= chargeDirSpeed;
        }else if(Mathf.Abs(chargeDir) < 0.5){
            chargeDir = 0;
        }else{
            if(chargeDir > 0){
                chargeDir -= chargeDirSpeed;
            }else if(chargeDir < 0){
                chargeDir += chargeDirSpeed;
            }
        }
    }

    private void Jump()
    {
        //ジャンプが最大値の半分より大きい時SEをならす
        if(chargePower > maxChargePower / 2 && !canEndGame){
            seManager.Play("Jump");
        }

        //チャージ方向の設定
        float powerX = Mathf.Sin(Mathf.PI * chargeDir / 180.0f);
        float powerY = Mathf.Cos(Mathf.PI * chargeDir / 180.0f);
        Vector2 jumpDirX = new Vector2(powerX, 0);
        Vector2 jumpDirY = new Vector2(0, powerY);

        //ジャンプベクトル　＝　チャージ方向　ｘ　チャージパワー
        gameObject.transform.position += new Vector3(0, 0.3f, 0);
        rb.AddForce(jumpDirX * chargePower, ForceMode2D.Impulse);
        rb.AddForce(jumpDirY * chargePower, ForceMode2D.Impulse);
    }

    private void ResetCharacterSize(){
        //何もしないとサイズが片になっていくのでリセットしておく
        Vector3 scale = gameObject.transform.localScale;
        scale.x = playerSize * Mathf.Sign(scale.x);
        scale.y = playerSize;
        gameObject.transform.localScale = scale;
    }

    private void ResetChargeState(){
        //何もしないと次チャージするときに影響が出るのでリセットしておく
        chargeDir = 0;
        chargePower = 0;
    }

    private void CheckAndPlayLandSE(){
        //Landアニメーションに遷移していることを確認し、LandSEをならす
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Myon Land Animation") && isFirstPlayOfLandAnimation){
            seManager.Play("Land");
            isFirstPlayOfLandAnimation = false;
        }else if(animator.GetCurrentAnimatorStateInfo(0).IsName("Myon Idle Animation") 
                || animator.GetCurrentAnimatorStateInfo(0).IsName("Myon Move Animation") || animator.GetCurrentAnimatorStateInfo(0).IsName("Myon Charge0 Animation")){
            isFirstPlayOfLandAnimation = true;
        }
    }

    private void AfterEnd(){
        //ゴール地点到着後の処理

        afterEndCount += 1;

        if(afterEndCount == 1){
            bgmManager.StopBgm();
        }

        if(afterEndCount == 80){
            bgmManager.Play("EndBGM");
            endText.SetActive(true);
            restartText.SetActive(true);
        }
        
        if(afterEndCount > 30){
            if(gameObject.transform.position.x < 6){
                isMoving = true;
                rb.velocity = new Vector2(1, rb.velocity.y);
            }else if(7 < gameObject.transform.position.x){
                isMoving = true;
                rb.velocity = new Vector2(-1, rb.velocity.y);
            }else{
                isMoving = false;
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }

        if(afterEndCount > 80){
            if(afterEndCount % 150 == 0){
                ResetChargeState();
                isCharging = true;
            }
            if(afterEndCount % 150 == 30){
                isCharging = false;
                isJumping = true;
                Jump();
                ResetCharacterSize();
            }
        }

        if(isCharging){
            ChargeJumpPower();
            ChangeCharacterSize();
        }
    }

    //プロパティ（private変数を他クラスから安全に参照する）
    public bool IsCharging{
        get{return isCharging;}
        set{isCharging = value;}
    }

    public float ChargeDir{
        get{return chargeDir;}
        set{chargeDir = value;}
    }
    
    public float ChargePower{
        get{return chargePower;}
        set{chargePower = value;}
    }
    public float MaxChargePower{
        get{return maxChargePower;}
        set{maxChargePower = value;}
    }


}