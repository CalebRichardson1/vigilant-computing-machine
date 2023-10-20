using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WreckingBallController : MonoBehaviour
{
    [SerializeField] private float returnDelay = 1f;
    [SerializeField] private float launchForce = 30;
    [SerializeField] private float returnIntervalInSeconds = 2;
    [SerializeField] private Transform ballStart;
    [SerializeField] private Transform ballTransform;
    [SerializeField] private Rigidbody ballRb;

    [SerializeField] private AnimationCurve curve;

    public bool ThrownBall {get; private set;}

    private void Start() {
        ballRb.isKinematic = true;
    }

    public void ThrowBall(){
        if(ThrownBall) return;
        ballTransform.position = ballStart.position;
        ballTransform.rotation = ballStart.rotation;
        Launch();
    }

    private void Launch(){
        ThrownBall = true;
        StartCoroutine(Return());
        ballRb.isKinematic = false;
        ballRb.AddForce(ballStart.forward * launchForce, ForceMode.Impulse);
    }

    private IEnumerator Return(){
        yield return new WaitForSeconds(returnDelay);
        ballRb.isKinematic = true;

        float counter = 0;

        Vector3 startPosition = ballTransform.position;
        Quaternion startRotation = ballTransform.rotation;

        while(counter < 1){
            counter += Time.deltaTime / returnIntervalInSeconds;

            ballTransform.position = Vector3.Lerp(startPosition, ballStart.position, curve.Evaluate(counter));
            ballTransform.rotation = Quaternion.Lerp(startRotation, ballStart.rotation, curve.Evaluate(counter));

            yield return new WaitForEndOfFrame();
        }

        ThrownBall = false;
    }
}
