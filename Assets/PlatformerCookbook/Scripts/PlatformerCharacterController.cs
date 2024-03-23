using System.Collections;
using System.Collections.Generic;
using PlatformerCookbook.Scripts;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlatformerCharacterController : MonoBehaviour
{
    [Header("Components")] 
    public RuntimeAnimatorController animatorController;
    [Header("Common")]
    public float Speed = 5;
    public float Gravity = 9.8f;
    public float JumpHeight = 1;
    [Header("DoubleJump")]
    public bool DoubleJumpEnabled;
    [Header("Dash")]
    public bool DashEnabled;
    public float DashSpeed = 20;
    public float DashTime = 0.3f;
    [Header("Rebound(Walljump)")]
    public bool ReboundEnabled;
    public float ReboundAngle = 30f;
    public float ReboundSpeed = 5;
    public float ReboundTimeout = 1;

    private Animator _animator;
    private CharacterController _characterController;

    private Vector3 playerVelocity;
    private Vector3 playerOrientation;

    private bool doubleJumpPerformed = false;
    private bool dashLeftPrepared = false;
    private bool dashLeftPerformed = false;
    private bool dashRightPrepared = false;
    private bool dashRightPerformed = false;
    private float dashLeftTimer = 0.0f;
    private float dashRightTimer = 0.0f;
    private float doubleTapTimer => Time.deltaTime * 10;//0.16f;~

    private float reboundTreshold = 0.1f;
    private float lastReboundTime = 0.0f;
    private float lastCollisionTime = 0.0f;
    private bool reboundPerformed = false;
    private Vector3 lastCollisionPoint;
    private Vector3 platformLastPosition = Vector3.zero;
    private Vector3 platformPosition = Vector3.zero;
    private Rigidbody platform;

    /// Controls
    private bool A;
    private bool D;
    private bool W;
    private bool ADown;
    private bool DDown;
    private bool WDown;
    private bool WUp;
    ///
    private void Start()
    {
        InitCharacterController();
        InitAnimator();
    }

    private void InitCharacterController()
    {
        _characterController = GetComponent<CharacterController>();
        playerOrientation = Vector3.up * 90; // Start rotation;
    }

    private void InitAnimator()
    {
        if (transform.childCount == 0) return;
        var child = transform.GetChild(0).gameObject;

        _animator = child.GetComponent<Animator>();
        if (!_animator)
            _animator = child.AddComponent<Animator>();

        _animator.runtimeAnimatorController = animatorController;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            A = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            D = true;
        }
        if (Input.GetKey(KeyCode.W))
        {
            W = true;
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            WUp = true;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            WDown = true;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            ADown = true;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            DDown = true;
        }

    }
    void FixedUpdate()
    {
        //Character is on the collider
        var grounded = _characterController.isGrounded;

        //Rebound timer reset
        if (!grounded) {
            platform = null;
            if (!reboundPerformed) {
                lastReboundTime = Time.time;
            }
        }
        //Reset velocity and jumps on landing
        if (grounded && _characterController.velocity.y < 0)
        {
            playerVelocity.y = 0f;
            playerVelocity.x = 0f;
            doubleJumpPerformed = false;
            reboundPerformed = false;
        }
        //Reset x-axis velocity if not moving
        if (grounded &&!D && !A && !dashLeftPerformed && !dashRightPerformed)
        {
            playerVelocity.x = 0;
        }
        //Moving
        if (A)//Move left
        {
            playerOrientation = Vector3.up * 270;
            //Ground movement
            if (grounded && !dashLeftPerformed && !dashRightPerformed && !reboundPerformed)
            {
                playerVelocity.x = -Speed;
            }//"In air" movement
            else if(!dashLeftPerformed && !dashRightPerformed && !reboundPerformed)
            {
                playerVelocity.x = Mathf.Clamp(playerVelocity.x - Speed * Time.fixedDeltaTime, -Speed, -Speed * 0.3f);
            }
            _animator?.SetBool("isRunning", true);
        }
        else if (D)//Move right
        {
            playerOrientation = Vector3.up * 90;
            //Ground movement
            if (grounded && !dashLeftPerformed && !dashRightPerformed && !reboundPerformed)
            {
                playerVelocity.x = Speed;
            }//"In air" movement
            else if (!dashLeftPerformed && !dashRightPerformed && !reboundPerformed)
            {
                playerVelocity.x = Mathf.Clamp(playerVelocity.x + Speed * Time.fixedDeltaTime, Speed * 0.3f, Speed);
            }
            _animator?.SetBool("isRunning", true);
        }//Disable Running animation
        else
        {
            _animator?.SetBool("isRunning", false);
        }
        //Rotate character
        transform.eulerAngles = Vector3.MoveTowards(transform.eulerAngles, playerOrientation, 360 * 2 * Time.fixedDeltaTime);
        //Jump, Double jump, and Rebound
        if (WDown)
        {
            //Rebound
            if (ReboundEnabled && !grounded && (Time.time - lastCollisionTime) < reboundTreshold && (lastReboundTime==0 ||(Time.time-lastReboundTime)<ReboundTimeout))
            {
                var reboundSide = (lastCollisionPoint - transform.position).x;
                var reboundMul = 0;
                if (reboundSide <= -0.15f) {
                    reboundMul = 1;
                }
                if (reboundSide >= 0.15f)
                {
                    reboundMul = -1;
                }
                
                var reboundVector = new Vector2(Mathf.Cos(Mathf.Deg2Rad * ReboundAngle), Mathf.Sin(Mathf.Deg2Rad * ReboundAngle));
                playerVelocity.x = ReboundSpeed * reboundVector.x * reboundMul;
                playerVelocity.y = Mathf.Sqrt(reboundVector.y * ReboundSpeed * -2 * -Gravity);
                _animator?.SetBool("isRunning", false);
                _animator?.SetTrigger("Jump");
                reboundPerformed = true;
                lastReboundTime = Time.time;
                playerOrientation = Vector3.up * (reboundSide > 0 ? 270 : 90);
            }//Jump
            else if (grounded || (DoubleJumpEnabled && !doubleJumpPerformed))
            {
                playerVelocity.y = Mathf.Sqrt(JumpHeight * -3.0f * -Gravity);
                _animator?.SetBool("isRunning", false);
                _animator?.SetTrigger("Jump");
            }            
            //Reset Doublejump (Fix inifinity jumps)
            if (!grounded)
            {
                doubleJumpPerformed = true;
            }
        }
        //Dash
        //Left Dash timer
        dashLeftTimer -= Time.fixedDeltaTime;
        if (dashLeftTimer < 0 && grounded)
        {
            dashLeftPrepared = false;
        }
        //Right Dash timer
        dashRightTimer -= Time.fixedDeltaTime;
        if (dashRightTimer < 0 && grounded)
        {
            dashRightPrepared = false;
        }

        if (!W && !WDown && !WUp && !DDown && ADown && DashEnabled && !dashLeftPerformed && dashLeftPrepared && dashLeftTimer > 0)
        {
            playerVelocity.x -= DashSpeed;
            playerVelocity.y = 0;
            _animator?.SetBool("isRunning", false);
            _animator?.SetTrigger("Dash");
            dashLeftPrepared = false;
            dashLeftPerformed = true;
            dashLeftTimer = DashTime;
        }

        if (!W && !WDown && !WUp && !ADown && DDown && DashEnabled && !dashRightPerformed && dashRightPrepared && dashRightTimer > 0)
        {
            playerVelocity.x += DashSpeed;
            playerVelocity.y = 0;
            _animator?.SetBool("isRunning", false);
            _animator?.SetTrigger("Dash");
            dashRightPrepared = false;
            dashRightPerformed = true;
            dashRightTimer = DashTime;
        }

        if (!W && !WDown && !WUp && !DDown && ADown && DashEnabled && !dashLeftPrepared && !dashLeftPerformed)
        {
            dashLeftTimer = doubleTapTimer;
            dashLeftPrepared = true;
            dashRightPrepared = false;
            dashLeftPerformed = false;
            dashRightPerformed = false;
        }

        if (!W && !WDown && !WUp && !ADown && DDown && DashEnabled && !dashRightPrepared && !dashRightPerformed)
        {
            dashRightTimer = doubleTapTimer;
            dashRightPrepared = true;
            dashLeftPrepared = false;
            dashLeftPerformed = false;
            dashRightPerformed = false;
        }
        //Reset completed Dash state
        if (dashRightPerformed && dashRightTimer <= 0 || dashLeftPerformed && dashLeftTimer <= 0)
        {
            dashRightPerformed = false;
            dashLeftPerformed = false;
            dashRightPrepared = false;
            dashLeftPrepared = false;
            playerVelocity.x = 0;
        }
        //Reset failed Dash
        if (dashRightPrepared && dashRightTimer <= 0 || dashLeftPrepared && dashLeftTimer <= 0)
        {
            dashRightPerformed = false;
            dashLeftPerformed = false;
            dashRightPrepared = false;
            dashLeftPrepared = false;
        }
        //Apply gravity
        if (!dashRightPerformed && !dashLeftPerformed)
        {
            playerVelocity.y += -Gravity * Time.fixedDeltaTime;
        }
        //Calc fixed update velocity
        var velo = playerVelocity * Time.fixedDeltaTime;
        //Update platform last position
        if (platform) {
            platformLastPosition = platformPosition;
            platformPosition = platform.position;
        }
        //Apply platform velocity
        if (grounded && platform)
        {
            var platformVelo = (platformPosition - platformLastPosition);     
            
            velo += platformVelo;                        
        }
        //Move character
        _characterController.Move(velo);
        //Reset keys state
        A = false;
        D = false;
        W = false;
        ADown = false;
        DDown = false;
        WDown = false;
        WUp = false;
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        lastCollisionTime = Time.time;
        lastCollisionPoint = hit.point;
        
        playerVelocity.y = Mathf.Min(0, playerVelocity.y);
        Rigidbody body = hit.collider.attachedRigidbody;
        
        if (body == null || hit.point.y> transform.position.y+_characterController.height*0.5f)// transform.position.y)
        {
            platform = null;
            return;
        }
        
        if (body != platform)
        {
            platform = null;
        }

        if (!platform)
        {
            platform = body;
            platformLastPosition = body.position;
            platformPosition = body.position;
        }
    }

    public void SetEnabledDoubleJump(bool isEnabled=true) => SetExtraAbility(ExtraAbility.DoubleJump, isEnabled);
    public void SetEnabledDash(bool isEnabled=true) => SetExtraAbility(ExtraAbility.Dash, isEnabled);
    public void SetEnabledRebound(bool isEnabled=true) => SetExtraAbility(ExtraAbility.Rebound, isEnabled);
    
    private void SetExtraAbility(ExtraAbility extraAbility, bool setEnabled)
    {
        switch (extraAbility)
        {
            case ExtraAbility.DoubleJump:
                DoubleJumpEnabled = setEnabled;
                break;
            case ExtraAbility.Dash:
                DashEnabled = setEnabled;
                break;
            case ExtraAbility.Rebound:
                ReboundEnabled = setEnabled;
                break;
        }
    }
}
