using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 7f;
    [SerializeField] [Tooltip("In degrees")] private float verticalLimit = 20f;
    [SerializeField] private float sensitivityX = 7f;
    [SerializeField] private float sensitivityY = 7f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        //Movement
        transform.position += transform.forward * (Input.GetAxisRaw("Vertical") * Time.deltaTime * speed);
        transform.position += transform.right * (Input.GetAxisRaw("Horizontal") * Time.deltaTime * speed);
        
        transform.position += Vector3.up * ((Input.GetKey(KeyCode.Q) ? 1 : 0) * Time.deltaTime * speed);
        transform.position += Vector3.down * ((Input.GetKey(KeyCode.E) ? 1 : 0) * Time.deltaTime * speed);
        
        //Camera movement
        float tempX = transform.eulerAngles.x - Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivityX;
        float rotY = transform.eulerAngles.y + Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivityY;


        float rotX = transform.eulerAngles.x < 180
            ? Mathf.Min(tempX, 90f - verticalLimit)
            : Mathf.Max(tempX, 270f + verticalLimit);

        transform.rotation = Quaternion.Euler(rotX, rotY, transform.eulerAngles.z);

    }
}
