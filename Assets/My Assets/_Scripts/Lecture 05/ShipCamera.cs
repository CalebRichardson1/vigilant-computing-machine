using UnityEngine;

public class ShipCamera : MonoBehaviour
{
    private float startingFOV = 60;
    private Camera cam;
    private PlayerShipInput playerShipInput => GetComponent<PlayerShipInput>();

    void Start()
    {
        cam = gameObject.transform.GetChild(0).GetComponent<Camera>();
        startingFOV = cam.fieldOfView;
    }

    public void ZoomOut() {
        cam.fieldOfView = startingFOV + 35;
        playerShipInput.SetCanMove(false);
        // playerShipInput.SetTrackerBeam(false);
    }

    public void DefaultZoom() {
        cam.fieldOfView = startingFOV;
        playerShipInput.SetCanMove(true);
        // playerShipInput.SetTrackerBeam(true);
    }
}
