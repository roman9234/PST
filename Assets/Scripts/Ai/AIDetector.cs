using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDetector : MonoBehaviour
{
    [Range(1, 15)]
    [SerializeField]
    private float viewRadius = 11;
    [SerializeField]
    private float detectionCheckDelay = 0.1f; //����� �������� �� ����������� ������ ����
    [SerializeField]
    private Transform target = null; // �������� ����
    [SerializeField]
    private LayerMask playerLayerMask; //������� �� ������� ������ ���� ������ ����� ���� ���������� ��� ����
    [SerializeField]
    private LayerMask visibilityLayer; // ������� �������� ���������

    [field: SerializeField] //���������� ������� ����

    public bool TargetVisible { get; private set; }

    public Transform Target
    {
        get => target;
        set
        {
            target = value;
            TargetVisible = false;
        }
    }

    private void Update()
    {
        if (Target != null)
            TargetVisible = CheckTargetVisibile();
    }

    private bool CheckTargetVisibile()
    {
        // ������� ���� ���� ����
        var result = Physics2D.Raycast(transform.position, Target.position - transform.position, viewRadius, visibilityLayer);
        if (result.collider != null) 
        {
            return (playerLayerMask & (1 << result.collider.gameObject.layer)) != 0; //������� �������� � ������� ������� (����������)
        }
        return false;
    }

    private void Start()
    {
        StartCoroutine(DetectionCoroutline());
    }

    private void DetectTarget() //�������� ���� �� ���� /���������/�������
    {
        if (target == null)
            CheckIfPlayerInRange();
        else if (target != null)
            DetectIfOutRange();
    }


    /*
     ���� ����� ��������� (����), �� �����, ��� ������ ������� ������, ���� �� �����
     */
    private void DetectIfOutRange() 
    {
        if (Target == null || Target.gameObject.activeSelf == false || Vector2.Distance(transform.position, Target.position) > viewRadius)
        {
            Target = null;
            //TargetVisible = false; //���
        }
    }

    private void CheckIfPlayerInRange()
    {
        Collider2D collision = Physics2D.OverlapCircle(transform.position, viewRadius, playerLayerMask); //��������� ���� �� ��������� � ������� ���� ������ ���������
        if (collision != null)
        {
            Target = collision.transform;
            //TargetVisible = true; //���
        }
    }
    IEnumerator DetectionCoroutline() //���� � ��������� � 0.1 �������
    {
        yield return new WaitForSeconds(detectionCheckDelay);
        DetectTarget();
        StartCoroutine(DetectionCoroutline());
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, viewRadius); //������ ��������� ���������
    }

}
