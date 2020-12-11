using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class RollerAgent : Agent
{
    public Transform target;
    Rigidbody rBody;

    public override void Initialize()
    {
        this.rBody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        //RollerAgentが床から落下している時
        if(this.transform.position.y < 0)
        {
            //RollerAgentの位置と速度をリセット
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.position = new Vector3(0.0f, 0.5f, 0.0f);
        }
        //Targetの位置リセット
        target.position = new Vector3(
            Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
    }

    //観察取得時に呼ばれる
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target.position); //TargetのXYZ座標
        sensor.AddObservation(this.transform.position); //RollerAgentのXYZ座標
        sensor.AddObservation(rBody.velocity.x); //RollerAgentのX速度
        sensor.AddObservation(rBody.velocity.z); //RollerAgentのZ速度
    }

    //行動実行時に呼ばれる
    public override void OnActionReceived(float[] vectorAction)
    {
        //RollerAgentに力を加える
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        rBody.AddForce(controlSignal * 10);

        //RollerAgentがTargetの位置に到着したとき
        float distanceToTarget = Vector3.Distance(
            this.transform.position, target.position);
        if(distanceToTarget < 1.42f)
        {
            AddReward(1.0f);
            EndEpisode();
        }
        //RollerAgentが床から落下したとき
        if(this.transform.position.y < 0)
        {
            EndEpisode();
        }
    }

    //ヒューリスティックモードの行動決定時に呼ばれる
    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }
}
