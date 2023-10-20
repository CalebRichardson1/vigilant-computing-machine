using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TroopSpawner : MonoBehaviour
{
    [SerializeField] private Troop[] troopsToSpawn = new Troop[5];
    [SerializeField] private float cooldownTimer = 2.5f;
    [SerializeField, Range(5f, 100f)] private float spawnSize = 5f;
    [SerializeField] private Color gizmosColor;

    private bool canSpawn = true;
    private Color currentUnitColor;
    private Dictionary<string, int> nameDictionary = new Dictionary<string, int>();
    private Dictionary<string, Color> colorDictionary = new Dictionary<string, Color>();
    private List<GameObject> spawnedUnits = new List<GameObject>();


    public void MoveTroop(int index, Vector3 pos)
    {
        var validTroops = spawnedUnits.FindAll(x => x.GetComponent<TroopTracker>().troopType == troopsToSpawn[index].troopType);
        StartCoroutine(TroopMovingTimer(validTroops, pos));
    }

    public void SpawnTroop(int index)
    {
        if(!canSpawn)
        {
            print("Spawn Cooldown is active");
            return;
        } 
        
        Troop troop = troopsToSpawn[index];
        //get the total amount to spawn (if the troop has a exact amount to spawn or if the troop has a random amount)
        int unitAmountToSpawn;
        if(troop.randomAmountToSpawn) unitAmountToSpawn = Random.Range(troop.minAmountToSpawn, 
                                                        troop.maxAmountToSpawn + 1);
        else unitAmountToSpawn = troop.amountToSpawn;

        int count = 0;
        //spawn the amount of units defined
        for (int i = 0; i < unitAmountToSpawn; i++)
        {
            count++;
            var x = Random.Range(transform.position.x - spawnSize / 2, transform.position.x + spawnSize / 2);
            var z = Random.Range(transform.position.z - spawnSize / 2, transform.position.z + spawnSize / 2);

            //instantiate the troop base gameobject in a random pos in the spawn zone
            GameObject troopGameObject = Instantiate(troop.unitBasePrefab, new Vector3(x, 2f, z), Quaternion.identity);  
           
           //add the unit to the spawnedUnits list
            spawnedUnits.Add(troopGameObject);

            //color and name logic
            UpdateTroop(troop, troopGameObject);   
        }
        print(count);
    }

    private void UpdateTroop(Troop troop, GameObject troopGameObject)
    {
        //get random name from troop at index
        string unitName = troop.unitNames[Random.Range(0, troop.unitNames.Length)];
        //see if the name is in the name dictionary
        if(nameDictionary.ContainsKey(unitName))
        {
            //add 1 to the dictionary int value
            nameDictionary[unitName] = nameDictionary[unitName] + 1;
            //add the name int value to the gameObject's name
            string updatedUnitName = unitName + nameDictionary[unitName];

            //change gameobject name to the randomly selected name
            troopGameObject.name = updatedUnitName;
        } 
        else
        {
            //add it to the name dictionary
            nameDictionary.Add(unitName, 1);

            //select a color random color not in the colorDictionary
            GetRandomColor(troop);
            //add that color to the dictionary
            colorDictionary.Add(unitName, currentUnitColor);
            print(unitName + " is the color " + currentUnitColor);

            //change gameobject name to the randomly selected name  
            troopGameObject.name = unitName;
        }

        //change gameobject material from the color dictionary
        troopGameObject.GetComponent<Renderer>().material.color = colorDictionary[unitName];
        
        //add the troops type component for moving and set it to the SO's troop type
        troopGameObject.AddComponent<TroopTracker>().troopType = troop.troopType;

        //add a rigidbody to the gameObject
        troopGameObject.AddComponent<Rigidbody>();

        StartCoroutine(TroopSpawnTimer());
    }

    private IEnumerator TroopSpawnTimer()
    {
        canSpawn = false;
        yield return new WaitForSeconds(cooldownTimer);
        canSpawn = true;
    }

    private IEnumerator TroopMovingTimer(List<GameObject> troopGameObjects, Vector3 pos)
    {
        foreach (GameObject troop in troopGameObjects)
        {
            troop.transform.position = pos; 
            troop.transform.localRotation = Quaternion.identity;
            yield return new WaitForEndOfFrame();
        }
    }

    private void GetRandomColor(Troop troop)
    {
        int randomColorIndex = Random.Range(0, troop.unitColors.Length);
        Color randomColor = troop.unitColors[randomColorIndex];

        bool containsValue = false;

        foreach(Color color in colorDictionary.Values)
        {
            if(EqualityComparer<Color>.Default.Equals(color, randomColor))
            {
                containsValue = true;
                break;
            }
        }
        
        if(containsValue) GetRandomColor(troop);
        else {
            currentUnitColor = troop.unitColors[randomColorIndex];
        }
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = gizmosColor;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), new Vector3(spawnSize, 1f, spawnSize));
    }

}
