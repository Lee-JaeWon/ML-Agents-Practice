using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RollerAgent : Agent
{
    // rBody라는 Rigidbody 함수 정의
    Rigidbody rBody;

    void Start()
    {
        // rBody함수는 현재 GameObject의 Rigidbody Component를 참조
        rBody = GetComponent<Rigidbody>();
    }

    // Target이라는 public Transform 함수를 선언하여 차후 Inspector 윈도우에서 지정 
    public Transform Target;
    public override void OnEpisodeBegin()
    {

        // Agent가 플랫폼 외부로 떨어지면(Y 좌표가 0이하가 되면), angularVelocity/velocity=0으로, 위치를 초기 좌표로 리셋
        if (this.transform.localPosition.y < 0)
        {
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
        }

        // Target을 Random.value함수를 활용해서 새로운 무작위 위치에 이동
        Target.localPosition = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target/Agent의 위치 정보 수집
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // Agent의 velocity 정보 수집
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }

    public float forceMultiplier = 10;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {

        // Agent가 Target쪽으로 이동하기 위해 X, Z축으로의 Force를 정의
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * forceMultiplier);

        // Agent와 Target사이의 거리를 측정
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // Target에 도달하는 경우 (거리가 1.42보다 작은 경우) Episode 종료
        if (distanceToTarget < 1.42)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        // 플랫폼 밖으로 나가면 Episode 종료
        if (this.transform.localPosition.y < 0)
        {
            EndEpisode();
        }

    }
    //public override void Heuristic(in ActionBuffers actionsOut)
    //{
    //    var continuousActionsOut = actionsOut.ContinuousActions;
    //    continuousActionsOut[0] = Input.GetAxis("Horizontal");
    //    continuousActionsOut[1] = Input.GetAxis("Vertical");
    //}

}
