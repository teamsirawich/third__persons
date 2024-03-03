using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckWinner : MonoBehaviour
 {
    public static CheckWinner instance;

    public Camera defaultCamera;
    public Camera winnerCamera;
    public bool isWinner = false;

    public Transform target;
    public float smoothSpeed = 0.1f;
    public Transform playerRotation;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        defaultCamera.enabled = true;
        winnerCamera.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isWinner)
        {
            defaultCamera.enabled = false;
            winnerCamera.enabled = true;
        }
    }

    private void LateUpdate()
    {
        if (target != null && isWinner)
        {
            // Calculate the desired position for the camera
            Vector3 desiredPosition = new Vector3(target.position.x-0.25f, target.position.y+1.3f, target.position.z + 1.25f);

            // Smoothly move the camera towards the desired position
            Vector3 smoothedPosition = Vector3.Lerp(winnerCamera.transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            winnerCamera.transform.position = smoothedPosition;

            playerRotation.LookAt(new Vector3(playerRotation.position.x, playerRotation.position.y, winnerCamera.transform.position.z));
        }

        
    }
    private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player") && PlayerController.instance.groundedPlayer)
            {
                isWinner = true;
            }
        }

}
