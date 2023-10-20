using UnityEngine;

public class PlayerShipInput : MonoBehaviour
{
    private ShipCamera shipCam;
    private ShipMovement shipMove;
    private WreckingBallController ballController;
    private ShipTractorController tractorController;

    public bool CanMove{get; private set;}
    // Start is called before the first frame update
    void Start()
    {
        CanMove = true;
        shipCam = GetComponent<ShipCamera>();
        shipMove = GetComponent<ShipMovement>();
        ballController = GetComponent<WreckingBallController>();
        tractorController = GetComponent<ShipTractorController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse1)) shipCam.ZoomOut();
        if(Input.GetKeyUp(KeyCode.Mouse1)) shipCam.DefaultZoom();
        if(Input.GetKeyDown(KeyCode.Mouse0)) ballController.ThrowBall();
    }

    // runs every 0.02 seconds
    void FixedUpdate() {
        if(CanMove) shipMove.Move(Input.GetAxis("Vertical"));
        shipMove.Rotate(Input.GetAxis("Horizontal"));
    }

    public void SetCanMove(bool state)
    {
        CanMove = state;
    }
}
