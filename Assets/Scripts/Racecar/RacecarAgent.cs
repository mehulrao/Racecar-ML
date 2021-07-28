using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class RacecarAgent : Agent
{

    Drive Drive;
    private Rigidbody rb;
    public GameObject redCone1;
    private Vector3 redCone1Pos;
    public GameObject redCone2;
    private Vector3 redCone2Pos;
    public GameObject redConeFinish;
    private Vector3 redConeFinishPos;
    public GameObject blueCone1;
    private Vector3 blueCone1Pos;
    public GameObject blueCone2;
    private Vector3 blueCone2Pos;
    public GameObject blueConeFinish;
    private Vector3 blueConeFinishPos;
    public GameObject finishLine;
    private Vector3 finishLinePos;
    public override void Initialize()
    {
        this.Drive = GetComponent<Drive>();
        this.Drive.MaxSpeed = 1;
        rb = GetComponent<Rigidbody>();
        redCone1Pos = redCone1.transform.localPosition;
        redCone2Pos = redCone2.transform.localPosition;
        redConeFinishPos = redConeFinish.transform.localPosition;
        blueCone1Pos = blueCone1.transform.localPosition;
        blueCone2Pos = blueCone2.transform.localPosition;
        blueConeFinishPos = blueConeFinish.transform.localPosition;
        finishLinePos = finishLine.transform.localPosition;
    }
    public override void CollectObservations(VectorSensor sensor) {
    // Target and Agent positions
        sensor.AddObservation(this.transform.InverseTransformVector(rb.velocity.normalized));
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "fail") {
            SetReward(-1f);
            EndEpisode();
        }
        else if (other.gameObject.tag == "checkpoint") {
            Debug.Log("Checkpoint");
            AddReward(1f);
        }
        else if (other.gameObject.tag == "end") {
            AddReward(3f);
            EndEpisode();
        }
        else if (other.gameObject.tag == "redFail" || other.gameObject.tag == "blueFail") {
            SetReward(-1f);
            EndEpisode();
        }
    }


    public override void OnActionReceived(ActionBuffers actionBuffers) {
        this.Drive.Angle = Mathf.Clamp(actionBuffers.ContinuousActions[0], -1f, 1f);
        this.Drive.Speed = Mathf.Clamp(actionBuffers.ContinuousActions[1], 0f, 1f);
        AddReward(-1f / MaxStep);
    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Controller.GetJoystick(Controller.Joystick.LEFT).x;
        continuousActionsOut[1] = Controller.GetTrigger(Controller.Trigger.RIGHT) - Controller.GetTrigger(Controller.Trigger.LEFT);
    }
    public override void OnEpisodeBegin() {
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;
        transform.localPosition = new Vector3(0, 0, -39);
        transform.localRotation = Quaternion.identity;
        Vector3 newRedCone1Pos = redCone1Pos;
        newRedCone1Pos.x += Random.Range(-5f, 5f);
        Vector3 newRedCone2Pos = redCone2Pos;
        newRedCone2Pos.x += Random.Range(-5f, 5f);
        Vector3 newBlueCone1Pos = blueCone1Pos;
        newBlueCone1Pos.x += Random.Range(-5f, 5f);
        Vector3 newBlueCone2Pos = blueCone2Pos;
        newBlueCone2Pos.x += Random.Range(-5f, 5f);
        Vector3 newFinishLinePos = finishLinePos;
        newFinishLinePos.x += Random.Range(-5f, 5f);
        redCone1.transform.localPosition = newRedCone1Pos;
        redCone2.transform.localPosition = newRedCone2Pos;
        redConeFinish.transform.localPosition = redConeFinishPos;
        blueCone1.transform.localPosition = newBlueCone1Pos;
        blueCone2.transform.localPosition = newBlueCone2Pos;
        blueConeFinish.transform.localPosition = blueConeFinishPos;
        finishLine.transform.localPosition = newFinishLinePos;
    }
}
