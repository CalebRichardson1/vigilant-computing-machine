//Caleb Richardson, Interactive Scripting Lecture 3 Assignment, 9/5/2023

#region Summary + Goals
/// <summary>
/// A object spawner that it customizable in the inspector, and spawns objects at a interval
/// 
/// Required Goals:
/// Change the Color of each spawned object
/// Change how many objects are spawned
/// Change the time between spawns
/// Change the size of the spawn area
/// Add physics to the spawned object
/// Reset the spawn logic on a key press
/// 
/// Personal Goals:
/// Create a custom editor that changes based on what options are chosen in the inspector
/// Can create basic objects but also custom objects
/// Option to change the objects scale
/// Holds multiple objects that can be spawned, each with different spawn chance
/// More color options: Random color, designate a random between 2 hues, or just a color
/// Apply forces to the physics once objects are spawned, with force options: Random direction and strength, random direction but designated strength, designated direction and strength
/// Have spawn checks to make sure the objects don't spawn in walls/other objects
/// Clean and readable code with included notes
/// </summary>
#endregion

//IMPORTANT: This script uses ShapeObject.cs for values and settings for the objects, I would recommend looking at ShapeObject.cs first then come back to this script after
//IMPORTANT: I uploaded a demonstration on YouTube: https://www.youtube.com/watch?v=KcpgkjVf-rs&t=6s - I also give a overview for how this spawner works if you want a visual element to how this script works
//IMPORTANT: Also these scripts are uploaded to my GitHub Page for editing and improving purposes: https://github.com/CalebRichardson1/ObjectSpawner.git

using System.Collections.Generic;
using UnityEngine;
using System.Collections;


#if UNITY_EDITOR //for full editor breakdown look at ShapeObject.cs
using UnityEditor;
#endif

public class ObjectSpawner : MonoBehaviour
{
    #region Project To-Do
    //custom editor that reacts to changes in the inspector
    //shape type enum, can create custom objects when desinated in the inspector
    //spawner options: spawn time for each object, list for objects with different chances, spawn range + visualization w/ gizmos, spawn objects on the y + x and z or just x and z axis -
    // - spawn limit/limitless spawning, physic objects
    //objects options: scale, color, add rigidbody,  
    #endregion
    [SerializeField] List<ShapeObject> shapeObjectsToSpawn; //list of our shape object scriptable objects
    [SerializeField] Color gizmosColor; //set the gizmos color from the inspector
    public bool spawnEndless = false; //boolean to mark if the spawner should spawn endlessly
    [SerializeField] int maxObjectsToSpawn = 5; //max objects to spawn if endless spawning is off
    [SerializeField] float timeBetweenSpawns = 0.5f; //the time for each spawn to take
    public bool GetRandomObjectsOnYAxis = false; //allows spawning on the y axis
    public bool constrainedScale = false; //locks the spawn area into being a equal square
    [SerializeField] float spawnRangeX; //if not constrained these are the sizes of the spawn range axis
    [SerializeField] float spawnRangeY;
    [SerializeField] float spawnRangeZ;

    public bool usePhysics = true; //if this spawner should use physics when spawning objects
    public bool randomForce = false; //apply a random force when a objected is spawned
    public bool randomStrength = false; //apply random strength from 1-100
    [SerializeField] Vector3 forceDirection; //the direction that the force is applied
    [SerializeField] float forceStrengthMin; //if random strength is false, these are the minimum and maximum strength values that are used
    [SerializeField] float forceStrengthMax;

    private const string ANIMAL = "Animal";
    int counter = 0; //used to keep track of what spawn the spawner is at
    //bool canReset; //used to be able to reset the counter when the spawn counter reaches the max object spawned
    GameObject gameObj; //reference to the current object spawned

    private void Start() {
        if(shapeObjectsToSpawn.Count == 0) //guard to make sure there are objects in the shape object list
        {
            print("No Shape Objects defined in spawn list."); //print is a short hand way of typing Debug.Log()
        } 

        else RollObjectSpawnChance(GetRandomObject()); //this starts the spawn loop
    }

    // private void Update() {
    //     if(Input.GetKeyDown(KeyCode.Space) && canReset) //if we get a spacebar input and we reached the max object count we reset the counter and start the spawn loop again
    //     {
    //         counter = 0;
    //         RollObjectSpawnChance(GetRandomObject());
    //         canReset = false;
    //     }
    // }

    //spawn timer
    IEnumerator SpawnTimer()
    {
        yield return new WaitForSeconds(timeBetweenSpawns);
        RollObjectSpawnChance(GetRandomObject());
    }

    private int GetRandomObject()
    {
        var randomObject = Random.Range(0, shapeObjectsToSpawn.Count - 1); //we get a object from the list putting the max as the list.Count - 1 because the list starts at 0 instead of 1
        return randomObject; //we return that objects number
    }

    private void RollObjectSpawnChance(int objNum)
    {
        var rollNumber = Random.Range(0, 101); //we do 101 because the second number of Random.Range is exclusive
        if(shapeObjectsToSpawn[objNum].chanceToSpawn > rollNumber) //if the roll number is less then the chance to spawn we spawn the object
        {
            //print("Spawned " + shapeObjectsToSpawn[objNum].name);
            CreateObject(shapeObjectsToSpawn[objNum]); //we pass the ShapeObject from the selected object into a method called CreateObject
        }
        else //else we reroll to get a different object + roll number
        {
            //print("Rerolling");
            RollObjectSpawnChance(GetRandomObject());
            //reroll
        }
    }

    private void CreateObject(ShapeObject obj) //this takes our selected object a creates it in the scene
    {
        switch (obj.shape) //switch statement on the ShapeObject's shape enum
        {
            case ObjectShape.Cube:
                gameObj = GameObject.CreatePrimitive(PrimitiveType.Cube); //if the shape is not a custom mesh, we use .CreatePrimitive
                break;
            case ObjectShape.Sphere:
                gameObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                break;
            case ObjectShape.Capsule:
                gameObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                break;
            case ObjectShape.Quad:
                gameObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
                break;
            case ObjectShape.Plane:
                gameObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
                break;
            case ObjectShape.Cylinder:
                gameObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);                
                break;
            //when creating a object we need a mesh filter with reference to the mesh, mesh renderer, and mesh collider
            case ObjectShape.Custom:
                gameObj = Instantiate(obj.prefabReference); //spawn the custom object with the prefab
                MeshCollider meshCollider = gameObj.AddComponent<MeshCollider>(); //add a mesh collider to try to get the collider close to the mesh shape
                meshCollider.convex = true; //set the convex to true so that dynamic rigidbody physics are applied
                gameObj.layer = LayerMask.NameToLayer(ANIMAL);
                gameObj.AddComponent<FarmAnimalController>();
                break;
        }

        //set the new GameObject's name to the ShapeObject's name
        gameObj.name = obj.name;
        //if we have randomScale = true we scale the GameObject
        if(obj.randomScale) ScaleObject(gameObj, obj);
        //Color the GameObject with the ShapeObject color settings
        ColorObject(gameObj, obj);
        //We Randomize the location of the GameObject
        RandomizeObjectLocation(gameObj);
        //if this spawner is using physics then apply physics to the GameObject
        if(usePhysics) ApplyPhysics(gameObj);

        //if our spawner isn't endless then check if we are at the max shape count with a method called SpawnCounter()
        if(!spawnEndless) SpawnCounter();
        //if we are endless then we go straight into the spawnTimer
        else StartCoroutine(SpawnTimer());
    }

    private void SpawnCounter()
    {
        if(counter < maxObjectsToSpawn) //if our counter is less then the max we spawn another object
        {
            counter++; //short hand for counter += 1;
            StartCoroutine(SpawnTimer()); //start the spawn timer
        }
        else 
        {
            print("Reached the Max Total Objects, the total count is " + counter); //print the counter count to the console
            //canReset = true; //set canReset to true so that we can start the counter again
        }
    }

    //physics
    private void ApplyPhysics(GameObject gameObj)
    {
        Vector3 direction; //the direction the physics force will be applied
        float strength; //the strength of the force

        //add a rigidbody to the gameObject while also getting a reference to it
        Rigidbody objRB = gameObj.AddComponent<Rigidbody>(); 
        objRB.freezeRotation = true;
        //if we are using random force in the spawner then get a direction
        if(randomForce) direction = new Vector3(Random.Range(-90, 90), Random.Range(0, 90), Random.Range(-90, 90)); 
        //else we set the direction to the forceDirection we set in the inspector
        else direction = forceDirection; 
        //get random strength from 1 to 100
        if(randomStrength) strength = Random.Range(1, 100);
        //else get a random strength from the min and max set in the inspector
        else strength = Random.Range(forceStrengthMin, forceStrengthMax);

        //finally apply the force to the object with the ForceMode.Impulse to make it instantly shoot in that direction
        objRB.AddForce(direction * strength * Time.deltaTime, ForceMode.Impulse); 
        //we also use Time.deltaTime because the physics are applied every frame, so we multiply by deltaTime so the force is applied correctly regardless of the framerate
    }

    //spread out in square
    private void RandomizeObjectLocation(GameObject gameObj)
    {
        //had chatgpt help me out with this one, never used a custom spawner with a square spawn area like this one

        Vector3 center = transform.position; //get the center point of the square
        float xHalf = spawnRangeX / 2; //get the sizes of half the axis sizes
        float yHalf = spawnRangeY / 2; 
        float zHalf = spawnRangeZ / 2;

        gameObj.transform.localRotation = new Quaternion(gameObj.transform.localRotation.x, Random.Range(-90f, 90f), gameObj.transform.localRotation.z, Random.Range(-90f, 90f));

        if(constrainedScale) //if we are constrained use only the spawnRangeX as the parameters
        {
            var x = Random.Range(center.x - xHalf,  center.x + xHalf); //get a random x pos
            var z = Random.Range(center.z - xHalf, center.z + xHalf); //get a random z pos
            if(GetRandomObjectsOnYAxis) //if we are using the y axis then get a random y pos
            {
                var y = Random.Range(1f, center.y + xHalf);
                gameObj.transform.position = new Vector3(x, y, z); //set the GameObject's pos = to the randomized pos
            }

            else gameObj.transform.position = new Vector3(x, 1f, z); //if we aren't using the y axis, then set the GameObjects x and z to the randomized positions while keeping the y at 1
        }
        
        else //else we use the spawnRangeX, Y, and Z values, assigned in the inspector
        {
            var x = Random.Range(center.x - xHalf,  center.x + xHalf);
            var z = Random.Range(center.z - zHalf, center.z + zHalf);
            if(GetRandomObjectsOnYAxis)
            {
                var y = Random.Range(1f, center.y + yHalf);
                gameObj.transform.position = new Vector3(x, y, z);
            }
            else gameObj.transform.position = new Vector3(x, 1f, z);
        }
    }

    private void ScaleObject(GameObject gameObj, ShapeObject obj)
    {
        //apply a random scale to the GameObject using the scaleMin and scaleMax defined in the ShapeObject
        var randomScale = Random.Range(obj.scaleMin, obj.scaleMax);
        //apply the randomized scale to the GameObject
        gameObj.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
    }

    private void ColorObject(GameObject gameObj, ShapeObject obj)
    {
        if(obj.randomColor) //if the ShapeObject uses a random color we set the GameObject material to a randomColor 
        {
            gameObj.GetComponent<MeshRenderer>().material.color = Random.ColorHSV();
        }
        else if(obj.betweenHues) //if we are using a random color between 2 hues then we convert the 2 RGB colors to HSV with the Color.RGBToHSV() Method
        {
            //RGB = Red, Green, Blue - HSV = Hue, Saturation, Value
            Color.RGBToHSV(obj.hue1, out float hueOne, out _, out _); //we do out _ because we aren't intersted in saturation or value from the color
            Color.RGBToHSV(obj.hue2, out float hueTwo, out _, out _);
            gameObj.GetComponent<MeshRenderer>().material.color = Random.ColorHSV(hueOne, hueTwo); //we set the GameObject material to a randomColor between the 2 hues
        }
        else if(obj.shape == ObjectShape.Custom) return; //if we are using a custom object we return to use the prefabs color
        else  gameObj.GetComponent<MeshRenderer>().material.color = obj.shapeColor; //else we just set the color to the color defined in the ShapeObject
    }

    #region  Gizmos
    private void OnDrawGizmosSelected() //Unity method that draws gizmos when the spawner object is selected 
    {
        Gizmos.color = gizmosColor; //setting the gizmos color to the color selected in inspector
        //if else statement tell gizmos how to draw a wire cube depending if we include the y axis or not in the spawn box
        if(GetRandomObjectsOnYAxis) Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y + spawnRangeY / 2, transform.position.z), new Vector3(spawnRangeX, spawnRangeY, spawnRangeZ));
        else Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), new Vector3(spawnRangeX, 1f, spawnRangeZ));
    }
    #endregion
}

public enum ObjectShape //In computer programming, an enumerated type is a data type consisting of a set of named values - Wikipedia
{
    Cube,
    Sphere,
    Capsule,
    Quad,
    Plane,
    Cylinder,
    Custom
}

#region Editor //See ShapeObject.cs for a full editor breakdown
#if UNITY_EDITOR
[CustomEditor(typeof(ObjectSpawner))]
public class ObjectSpawnerEditor : Editor
{
    #region Serialized Properties
    SerializedProperty shapeObjectsToSpawn;
    SerializedProperty gizmosColor;
    SerializedProperty spawnEndless;
    SerializedProperty maxObjectsToSpawn;
    SerializedProperty timeBetweenSpawns;
    SerializedProperty GetRandomObjectsOnYAxis;
    SerializedProperty constrainedScale;
    SerializedProperty spawnRangeX;
    SerializedProperty spawnRangeY;
    SerializedProperty spawnRangeZ;
    SerializedProperty usePhysics;
    SerializedProperty randomForce;
    SerializedProperty randomStrength;
    SerializedProperty forceDirection;
    SerializedProperty forceStrengthMin;
    SerializedProperty forceStrengthMax;
    #endregion
    bool spawnerAreaGroup = false;
    bool gizmosSettingsGroup = false;
    bool spawnerSettingsGroup = false;
    bool physicsObtionsGroup = false;

    private void OnEnable() 
    {
        shapeObjectsToSpawn = serializedObject.FindProperty("shapeObjectsToSpawn");

        gizmosColor = serializedObject.FindProperty("gizmosColor");

        spawnEndless = serializedObject.FindProperty("spawnEndless");
        maxObjectsToSpawn = serializedObject.FindProperty("maxObjectsToSpawn");
        timeBetweenSpawns = serializedObject.FindProperty("timeBetweenSpawns");

        GetRandomObjectsOnYAxis = serializedObject.FindProperty("GetRandomObjectsOnYAxis");
        constrainedScale = serializedObject.FindProperty("constrainedScale");
        spawnRangeX = serializedObject.FindProperty("spawnRangeX");
        spawnRangeY = serializedObject.FindProperty("spawnRangeY");
        spawnRangeZ = serializedObject.FindProperty("spawnRangeZ");
        
        usePhysics = serializedObject.FindProperty("usePhysics");
        randomForce = serializedObject.FindProperty("randomForce");
        randomStrength = serializedObject.FindProperty("randomStrength");
        forceDirection = serializedObject.FindProperty("forceDirection");
        forceStrengthMin = serializedObject.FindProperty("forceStrengthMin");
        forceStrengthMax = serializedObject.FindProperty("forceStrengthMax");
    }

    public override void OnInspectorGUI()
    {
        ObjectSpawner objSpawner = (ObjectSpawner)target;

        serializedObject.Update();

        EditorGUILayout.PropertyField(shapeObjectsToSpawn);
        EditorGUILayout.Space();
        
        spawnerAreaGroup = EditorGUILayout.BeginFoldoutHeaderGroup(spawnerAreaGroup, "Spawner Area Settings");
        if(spawnerAreaGroup)
        {
            EditorGUILayout.PropertyField(GetRandomObjectsOnYAxis);

            EditorGUILayout.PropertyField(constrainedScale);
            if(!objSpawner.constrainedScale)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("X Axis");
                EditorGUILayout.Slider(spawnRangeX, 1f, 100f, GUIContent.none, GUILayout.MaxWidth(150));
                if(objSpawner.GetRandomObjectsOnYAxis)
                {
                    GUILayout.Label("Y Axis");
                    EditorGUILayout.Slider(spawnRangeY, 1f, 100f, GUIContent.none, GUILayout.MaxWidth(150));
                }
                GUILayout.Label("Z Axis");
                EditorGUILayout.Slider(spawnRangeZ, 1f, 100f, GUIContent.none, GUILayout.MaxWidth(150));
                EditorGUILayout.EndHorizontal();
            }

            else
            {
                GUILayout.Label("Spawn Area Scale");
                EditorGUILayout.Slider(spawnRangeX, 1f, 100f, GUIContent.none);
                if(objSpawner.GetRandomObjectsOnYAxis && spawnRangeY.floatValue != spawnRangeX.floatValue) spawnRangeY.floatValue = spawnRangeX.floatValue;
                if(spawnRangeZ.floatValue != spawnRangeX.floatValue) spawnRangeZ.floatValue = spawnRangeX.floatValue;
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        
        EditorGUILayout.Space(5f);

        gizmosSettingsGroup = EditorGUILayout.BeginFoldoutHeaderGroup(gizmosSettingsGroup, "Gizmos Settings");
        if(gizmosSettingsGroup)
        {
            EditorGUILayout.PropertyField(gizmosColor); 
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space(5f);

        spawnerSettingsGroup = EditorGUILayout.BeginFoldoutHeaderGroup(spawnerSettingsGroup, "Spawner Settings");
        if(spawnerSettingsGroup)
        {
            EditorGUILayout.PropertyField(spawnEndless);
            if(!objSpawner.spawnEndless)
            {
                EditorGUILayout.PropertyField(maxObjectsToSpawn);
            }
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Time To Spawn Objects");
            EditorGUILayout.Slider(timeBetweenSpawns, 0.1f, 999f, GUIContent.none, GUILayout.MaxWidth(150));
            EditorGUILayout.EndHorizontal();

        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space(5f);

        physicsObtionsGroup = EditorGUILayout.BeginFoldoutHeaderGroup(physicsObtionsGroup, "Physics Options");
        if(physicsObtionsGroup)
        {
            EditorGUILayout.PropertyField(usePhysics);
            if(objSpawner.usePhysics)
            {
                EditorGUILayout.PropertyField(randomForce);
                if(!objSpawner.randomForce) EditorGUILayout.PropertyField(forceDirection);

                EditorGUILayout.PropertyField(randomStrength);
                if(!objSpawner.randomStrength) 
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Minimum Strength");
                    EditorGUILayout.Slider(forceStrengthMin, 1f, 100f, GUIContent.none, GUILayout.MaxWidth(150));
                    GUILayout.Label("Maximum Strength");
                    EditorGUILayout.Slider(forceStrengthMax, forceStrengthMin.floatValue, 100f, GUIContent.none, GUILayout.MaxWidth(150));
                    if(forceStrengthMax.floatValue < forceStrengthMin.floatValue) forceStrengthMax.floatValue = forceStrengthMin.floatValue;
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        
       
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
#endregion