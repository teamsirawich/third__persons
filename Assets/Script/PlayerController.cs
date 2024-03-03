using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    
   
    [SerializeField] private float playerSpeed = 5f;
    [SerializeField] private Camera followCamera;

    [SerializeField] private float rotationSpeed = 10f;

    private Vector3 playerVelocity;
    [SerializeField] private float gravityValue = -13f;

    public bool groundedPlayer;
    [SerializeField] private float jumpHeight = 0.5f;

    public Animator animator;

    public static PlayerController instance;
    public bool isDead;

    public ParticleSystem damageParticle;
    public ParticleSystem deadParticle;


    private void Awake()
    {
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        damageParticle.Stop();
        deadParticle.Stop();
        controller = GetComponent<CharacterController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (CheckWinner.instance.isWinner || isDead)
        {
            case true:
                animator.SetBool("Victory", CheckWinner.instance.isWinner);
                fixGravityWhenPlayerDead();
                if (isDead) 
                animator.SetBool("Dead", true);
                break;
            case false:
                Movement();
                break;
        }

    }
    void Movement()
    {
        groundedPlayer = controller.isGrounded;
        if(controller.isGrounded && playerVelocity.y < 2)
        {
            playerVelocity.y = -1f;
        }


        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementInput = Quaternion.Euler(0,followCamera.transform.eulerAngles.y,0)*new Vector3(horizontalInput,0,verticalInput);

        Vector3 movementDirection = movementInput.normalized;
       

        controller.Move(movementDirection*playerSpeed*Time.deltaTime);
        if(movementDirection != Vector3.zero)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(movementDirection,Vector3.up); 
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation ,rotationSpeed*Time.deltaTime);
        }

        

       

        if(Input.GetButtonDown("Jump")&& groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -1.0f * gravityValue);
            animator.SetTrigger("jumping");
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity*Time.deltaTime);


        animator.SetFloat("Speed", Mathf.Abs(movementDirection.x) + Mathf.Abs(movementDirection.z));
        animator.SetBool("Ground", controller.isGrounded);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BoxDamage"))
        {
            ShowDamageParticles();
            isDead = true;
        }
    }
    private void ShowDamageParticles()
    {
        ToggleSlowMotion();
        damageParticle.Play();
        deadParticle.Play();
        StartCoroutine(delaySlow());
    }
    void ToggleSlowMotion()
    {
        Time.timeScale = 0.2f;

    }

    IEnumerator delaySlow()
    {
        yield return new WaitForSeconds(1f);
        Time.timeScale = 0.25f;
    }
    void fixGravityWhenPlayerDead()
    {
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }


}
