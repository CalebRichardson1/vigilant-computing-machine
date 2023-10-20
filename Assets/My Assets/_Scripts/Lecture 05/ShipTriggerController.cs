using System.Collections;
using UnityEngine;

public class ShipTractorController : MonoBehaviour
{
    public bool IsTractoring {get; private set;}
    [SerializeField, Range(5f, 10f)] private float tractorDetectRange = 8f; 
    [SerializeField, Range(0.1f, 30f)] private float tractorCooldown = 3f; 
    [SerializeField] private LayerMask canBeTracked;
    [SerializeField] Transform shipBodyTransform;
    [SerializeField] GameObject tractorBeamGPX;
    private PlayerShipInput shipInput;

    private void Start() {
        tractorBeamGPX.SetActive(false);
        shipInput = GetComponent<PlayerShipInput>();
    }

    public void AttemptTractorBeam()
    {
        IsTractoring = true;

        //stop player movement and velocity
        shipInput.SetCanMove(false);

        //set the trackerbeam is true for visual feedback
        tractorBeamGPX.SetActive(true);

        //get all animals in the tracker beam range
        Collider[] trackedObjects = Physics.OverlapBox(new Vector3(transform.position.x, transform.position.y / 2, transform.position.z), 
                           new Vector3(1.5f, tractorDetectRange, 1.5f), Quaternion.identity, canBeTracked);
        foreach (Collider animal in trackedObjects)
        {
            //access animal controller
            animal.GetComponent<FarmAnimalController>().GetTractored();
        }
        //cooldown on tracker beam
        StartCoroutine(TrackerCooldown());
    }

    private IEnumerator TrackerCooldown()
    {
        //stop player input for starting a new tracker beam
        // shipInput.SetTrackerBeam(false);
        yield return new WaitForSeconds(tractorCooldown);
        // shipInput.SetTrackerBeam(true);
        //disable tracker beam
        tractorBeamGPX.SetActive(false);
        //allow movement
        shipInput.SetCanMove(true);
        IsTractoring = false;
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(new Vector3(shipBodyTransform.position.x, shipBodyTransform.position.y / 2, shipBodyTransform.position.z),  
                            new Vector3(1.5f, tractorDetectRange, 1.5f));
    }
}
