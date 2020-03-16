using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeController : CloseWeaponController
{
    //활성화여부
    public static bool isActivate = false;



    // Update is called once per frame
    void Update()
    {
        if (isActivate)
        {
            TryAttack(); //공격 시도
        }
    }



    protected override IEnumerator HitCoroutine()
    {

        while (isSwing) // isSwing이 false가 될 때까지
        {
            if (CheckObject())
            {
                if(hitInfo.transform.tag == "Grass")
                {
                    hitInfo.transform.GetComponent<Grass>().Damage();
                }
                else if(hitInfo.transform.tag == "Tree")
                {
                    hitInfo.transform.GetComponent<Tree>().Chop(hitInfo.point, transform.eulerAngles.y);
                }
                isSwing = false; // 충돌체가 있다면 중복 공격되지 않게 와일문을 빠져나오도록 swing을 false로 바꿔줌ㅁ
                Debug.Log(hitInfo.transform.name);
            }
            yield return null;
        }
    }

    public override void CloseWeaponChange(CloseWeapon _closeWeapon)
    {
        base.CloseWeaponChange(_closeWeapon);
        isActivate = true;
    }
}
