using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    //필요한 컴포넌트
    [SerializeField]
    private GunController theGunController;
    private Gun currentGun;

    //필요하면 허드 호출, 필요 없으면 허드 비활성화
    [SerializeField]
    private GameObject go_BulletHud;


    //총알 개수 텍스트에 반영
    [SerializeField]
    private Text[] text_Bullet;


    void Update()
    {
        CheckBullet();
    }

    private void CheckBullet()
    {
        currentGun = theGunController.GetGun(); //건컨트롤러스크립트에서 Gun스크립트 정보 받아오기
        text_Bullet[0].text = currentGun.carryBulletCount.ToString();
        text_Bullet[1].text = currentGun.reloadBulletCount.ToString();
        text_Bullet[2].text = currentGun.currentBulletCount.ToString();
    }
}
