using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public string handName; // �Ǽ�, ���� �������� ����
    public float range; // ���� ����
    public int damage;
    public float attackDelay;
    public float attackDelayA; // Į�� �ֵη�� �ִϸ��̼� ����� ���� ���� �� �� ������ �����ð�
    public float attackDelayB; // Į�� �ֵη�� ���� ���� ������ ���� �ȵ�

    public Animator animator;
}
