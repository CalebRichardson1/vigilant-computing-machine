using UnityEngine;

public class TroopController : MonoBehaviour
{
    [SerializeField] TroopSpawner troopSpawner; //would make this not as coupled together in a real game

    [Range(0, 4)] private int selectedUnit = 0;

    private Camera mainCamera;
    private void Start() {
        mainCamera = Camera.main;
    }
    private void Update() { //would make this better in a actual game
        //getting input for the numbers from the keyboard to spawn troops based on which one is assigned to that key
        if(Input.GetKeyDown(KeyCode.Alpha1)) selectedUnit = 0;
        if(Input.GetKeyDown(KeyCode.Alpha2)) selectedUnit = 1;
        if(Input.GetKeyDown(KeyCode.Alpha3)) selectedUnit = 2;
        if(Input.GetKeyDown(KeyCode.Alpha4)) selectedUnit = 3;
        if(Input.GetKeyDown(KeyCode.Alpha5)) selectedUnit = 4;

        if(Input.GetKeyDown(KeyCode.Tab)) troopSpawner.SpawnTroop(selectedUnit);

        if(Input.GetKeyDown(KeyCode.Mouse0)) troopSpawner.MoveTroop(selectedUnit, GetMousePosition());

    }

    private Vector3 GetMousePosition()
    {
        Ray camToGroundRay = mainCamera.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(camToGroundRay, out RaycastHit hit))
        {
            return hit.point;
        }

        else return Vector3.zero;
    }
}
