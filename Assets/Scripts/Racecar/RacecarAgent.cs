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
    public override void Initialize()
    {
        this.Drive = GetComponent<Drive>();
        this.Drive.MaxSpeed = 1;
        rb = GetComponent<Rigidbody>();
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
    }
}
