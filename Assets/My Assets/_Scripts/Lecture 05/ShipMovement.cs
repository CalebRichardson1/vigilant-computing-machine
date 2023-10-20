using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float rotSpeed = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Move(float zAxis) 
    {
        rb.AddRelativeForce(0, 0, zAxis * speed);
    }

    public void Rotate(float yAxis)
    {
        rb.AddTorque(0, yAxis * rotSpeed, 0);
    }
}
