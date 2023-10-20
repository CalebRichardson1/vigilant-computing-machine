using System.Collections;
using UnityEngine;

public class FarmAnimalController : MonoBehaviour
{
    [SerializeField] int escapeSpeed = 50;

    private MeshRenderer meshRenderer => GetComponent<MeshRenderer>();
    private int riseSpeed = 0;
    private bool isTractored = false;
    void Update()
    {
        if(isTractored)
        {
            transform.Translate(0, riseSpeed * Time.deltaTime, 0);
        }
    }

    public void GetTractored() {
        //create an integer named cubeSpeed with a random range between 1 and 10 (inclusive)
        var randomEscapeChance = Random.Range(1, 11);
        // if speed is greater than 6 (7, 8, 9, or 10)
        if(randomEscapeChance > 6)
        {
            isTractored = true;
            // turn green (Color.green)
            meshRenderer.material.color = Color.green;
            // move up by changing riseSpeed to 5
            riseSpeed = 5;
            //disable gravity on the rigidbody
            GetComponent<Rigidbody>().isKinematic = true;
            // destroy after 5 seconds.
            Destroy(gameObject, 0.90f);
        }

        else
        {
            // turn red
            meshRenderer.material.color = Color.red;
            // add force in a random direction to escape the beam
            Vector3 direction = new(Random.Range(-90, 90), gameObject.transform.position.y + 3, Random.Range(-90, 90)); 
            gameObject.GetComponent<Rigidbody>().AddForce(direction * escapeSpeed * Time.deltaTime, ForceMode.Impulse); 
            //turn back into normal color after 1.5 seconds
            StartCoroutine(AnimalRunColorChange());
        }
    }

    private IEnumerator AnimalRunColorChange()
    {
        yield return new WaitForSeconds(1.5f);
        meshRenderer.material.color = Color.clear;
    }
}
