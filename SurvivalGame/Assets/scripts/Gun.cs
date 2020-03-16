using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public string gunName; // 총의 이름
    public float range; //총의 사정 거리
    public float accuracy; //총의 정확도
    public float fireRate; //총의 연사속도 
    public float reloadTime; //총의 재장전 속도 
    
    
    public int damage; // 총의 공격력
    public int reloadBulletCount; //총알의 재장전 개수
    public int currentBulletCount; //현재 탄알집에 남아있는 총알의 개수
    public int maxBulletCount; //최대소유가능총알개수
    public int carryBulletCount; //현재 소유하고있는 총알 개수

    public float retroActionForce; //반동 세기
    public float retroActionFineSightForce; //정조준 반동 세기

    public Vector3 fineSightOriginPos;
    public Animator anim;
    public ParticleSystem muzzleflash;

    public AudioClip fire_Sound;
    

     
}
