﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    IState currentState;
    [Header("Character_Enemy")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] Animator anim;
    public bool IsDead;

    [Header("Pool")]
    [SerializeField] private GameObject bulletBasePrefab;
    [SerializeField] private Transform firePoint;
    
    public Transform bodyTransform;
    public float fireDelayTime = 3.18f;
    public float enemySpeed;
    public Vector3[] directions = new Vector3[] { Vector3.forward, Vector3.back,  Vector3.left, Vector3.right };
    public float delta;
 
    private float timeToNextFire = 0f;
    public float moveCheckRadius = 0.66f;
    private void OnEnable()
    {
        OnInit();
    }
    protected virtual void OnInit()
    {
        ChangeState(new MoveState());
    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.OnExecute(this);
        }

        Debug.DrawRay(transform.position + transform.forward / delta + transform.up , Vector3.down * 10, Color.yellow);
        if (timeToNextFire > 0)
        {
            timeToNextFire -= Time.deltaTime;
        }
    }

    public virtual void MovePosition(int dirIndex)
    {
        rb.MovePosition(transform.position + directions[dirIndex] * enemySpeed * Time.deltaTime);
    }
    
    public virtual void RotatePosition(int rotIndex)
    {
            // forward , back , left ,right 

        Vector3 dirRotation = Vector3.zero;
        switch(rotIndex)
        {
            case 0:
                dirRotation = transform.position + directions[rotIndex] - transform.position;
                   break;
            case 1:
                dirRotation = transform.position + directions[rotIndex] - transform.position;

                break;
            case 2:
                dirRotation = transform.position + directions[rotIndex] - transform.position;

                break;
            case 3:
                dirRotation = transform.position + directions[rotIndex] - transform.position;

                break;

        }
        transform.rotation = Quaternion.LookRotation(dirRotation);
    }

    public bool isBoxFront()
    {
        return Physics.Raycast(transform.position + transform.forward + transform.up, Vector3.down * 10, out RaycastHit hit, Mathf.Infinity)
            && hit.collider.CompareTag(ConstProperty.borderTag);
    }

    public virtual Quaternion GetRotation(Vector3 rotation)
    {
        return bodyTransform.rotation = Quaternion.LookRotation(rotation);
    }

    public bool CanMoveWithinRadius()
    {
        return !Physics.CheckSphere(transform.position, moveCheckRadius, LayerMask.GetMask(ConstProperty.obstacleLayer));
    }

    public virtual void Fire()
    {
       
        if (timeToNextFire <= 0f ) 
        {
            GameObject enemyBullet = Instantiate(bulletBasePrefab, firePoint.position, Quaternion.identity) as GameObject;
            BulletEnemy bulletrb = enemyBullet.GetComponent<BulletEnemy>();
            Vector3 launchDirection = transform.forward;
            anim.SetTrigger(ConstProperty.shootAnim);
            enemyBullet.transform.rotation = Quaternion.LookRotation(launchDirection);
            bulletrb.thisRigidbody.AddForce(launchDirection * bulletrb.Speed, ForceMode.Impulse);
            timeToNextFire = fireDelayTime;
        }
    }
    public virtual void ChangeState(IState newState)
    {
        if (currentState != null)
        {
            currentState.OnExit(this);
        }
        currentState = newState;
        if (currentState != null)
        {
            currentState.OnEnter(this);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue; 
        Gizmos.DrawWireSphere(transform.position, moveCheckRadius); 
    }

}
