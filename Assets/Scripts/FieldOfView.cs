﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Range(0, 360)] public float viewAngle = 30;
    public float viewRadius = 5;
    public float coroutineDelay = .2f;

    //LayerMasks allow for raycasts to choose what to and not to register
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    private List<Transform> visibleTargets = new List<Transform>();
    public List<Transform> getVisibleTargets() { return visibleTargets; }

    void Start() {
        StartCoroutine("FindTargetsWithDelay", coroutineDelay);
    }

    private IEnumerator FindTargetsWithDelay(float delay){
        while (true){
            yield return new WaitForSeconds(delay); //only coroutine every delay seconds
            findVisibleTargets();
        }
    }


    //for every target (via an array), lock on em
    private void findVisibleTargets(){
        visibleTargets.Clear(); //reset list every time so no dupes
        //cast a sphere over player, store everything inside col
        Collider[] targetsInView = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        foreach(Collider col in targetsInView){
            Transform target = col.transform; //get the targets locatoin
            Vector3 directionToTarget = (target.position - transform.position).normalized; //direction vector of where bloke is
            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle/2){ //if the FOV is within bounds, /2 cause left right
                //do the ray 
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if(!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask)){ //if, from character at given angle and distance, it DOESNT collide with obstacleMask
                    visibleTargets.Add(target);
                    //Debug.Log("hit a cunt");
                }
            }
        }

    }

    
    //return direction vector given a specific angle
    public Vector3 directionGivenAngle(float angle, bool isGlobal){
        if(!isGlobal){
            angle+=transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle*Mathf.Deg2Rad));
    }

}