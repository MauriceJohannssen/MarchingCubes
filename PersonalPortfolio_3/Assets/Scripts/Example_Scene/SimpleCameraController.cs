using UnityEngine;
using System.Collections;
 
public class SimpleCameraController : MonoBehaviour 
{
    void Start()
    {
        transform.rotation = Quaternion.identity;
    }
    void Update()
    {
        float movementSpeed = 5.0f;
        if (Input.GetKey(KeyCode.LeftShift)) movementSpeed = 10.0f;
        
        Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        transform.Translate(Time.deltaTime * movementSpeed * movement);

        Vector3 rotation = new Vector3(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"), 0);
        transform.eulerAngles +=  Time.deltaTime * 50.0f * rotation;
        Mathf.Clamp(transform.eulerAngles.x, -50, 80);
    }
}