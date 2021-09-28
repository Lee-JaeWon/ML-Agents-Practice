using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RollerAgent : Agent
{
    // rBody��� Rigidbody �Լ� ����
    Rigidbody rBody;

    void Start()
    {
        // rBody�Լ��� ���� GameObject�� Rigidbody Component�� ����
        rBody = GetComponent<Rigidbody>();
    }

    // Target�̶�� public Transform �Լ��� �����Ͽ� ���� Inspector �����쿡�� ���� 
    public Transform Target;
    public override void OnEpisodeBegin()
    {

        // Agent�� �÷��� �ܺη� ��������(Y ��ǥ�� 0���ϰ� �Ǹ�), angularVelocity/velocity=0����, ��ġ�� �ʱ� ��ǥ�� ����
        if (this.transform.localPosition.y < 0)
        {
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
        }

        // Target�� Random.value�Լ��� Ȱ���ؼ� ���ο� ������ ��ġ�� �̵�
        Target.localPosition = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target/Agent�� ��ġ ���� ����
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // Agent�� velocity ���� ����
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }

    public float forceMultiplier = 10;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {

        // Agent�� Target������ �̵��ϱ� ���� X, Z�������� Force�� ����
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * forceMultiplier);

        // Agent�� Target������ �Ÿ��� ����
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // Target�� �����ϴ� ��� (�Ÿ��� 1.42���� ���� ���) Episode ����
        if (distanceToTarget < 1.42)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        // �÷��� ������ ������ Episode ����
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
