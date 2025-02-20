using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public string handName; // 맨손, 무기 장착상태 구분
    public float range; // 공격 범위
    public int damage;
    public float attackDelay;
    public float attackDelayA; // 칼을 휘두루는 애니메이션 재생후 실제 딜이 들어갈 때 까지의 지연시간
    public float attackDelayB; // 칼을 휘두루고 팔을 빼면 공격이 들어가면 안됨

    public Animator animator;
}
