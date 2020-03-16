using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CloseWeaponController : MonoBehaviour
{       // 미완성 클래스 = 추상 클래스.
            //미완성 클래스이기 때문에 컴포넌트로 추가할 수 없을 뿐더러, Update문이 실행되지 않는다.



    //현재 장착된 핸드형 타입 무기
    [SerializeField]
    protected CloseWeapon currentCloseWeapon; // currentCloseWeapon scripts 선언

    //공격중???
    protected bool isAttack;
    protected bool isSwing;


    //레이저 충돌 정보 받아옴
    protected RaycastHit hitInfo;

    //필요한 컴포넌트
    private PlayerController thePlayerController;

    void Start()
    {
        thePlayerController = FindObjectOfType<PlayerController>();
    }




    protected void TryAttack() // 공격시도
    {
        if (!Inventory.inventoryActivated)
        {
            if (Input.GetButton("Fire1"))
            {

                if (!isAttack)
                {
                    AkSoundEngine.PostEvent("Axe_Attack", gameObject);

                    if (CheckObject())
                    {
                        if (currentCloseWeapon.isAxe && hitInfo.transform.tag == "Tree")
                        {

                            StartCoroutine(thePlayerController.TreeLookCoroutine(hitInfo.transform.GetComponent<Tree>().GetTreeCenterPosition()));
                            StartCoroutine(AttackCoroutine("Chop", currentCloseWeapon.workDelayA, currentCloseWeapon.workDelayB, currentCloseWeapon.workDelay));
                            return;
                        }

                    }

                    StartCoroutine(AttackCoroutine("Attack", currentCloseWeapon.attackDelayA, currentCloseWeapon.attackDelayB, currentCloseWeapon.attackDelay));

                }
            }
        }
    }

    protected IEnumerator AttackCoroutine(string swingType, float _delayA, float _delayB, float _delayC) // 공격 동작
    {
        isAttack = true; //새로운 메소드에서 트루로 바꾸어서 중복 실행 방지
        currentCloseWeapon.anim.SetTrigger(swingType); //커런트핸드의 애니메이터에서 setTrigger의 상태변수 "attack" 발동
        yield return new WaitForSeconds(_delayA);
        isSwing = true;

        //공격 활성화 시점
        StartCoroutine(HitCoroutine());

        yield return new WaitForSeconds(_delayB);
        isSwing = false;

        yield return new WaitForSeconds(_delayC - _delayA - _delayB);
        isAttack = false; // 끝나면 다시 false로 바꾸어서 공격다시 할 수있게 만들기
    }

    // 추상 = 미완성, 자식객체에서 완성하겠다.
    protected abstract IEnumerator HitCoroutine(); // 공격활성화 시점
    /*{            
        while (isSwing) // isSwing이 false가 될 때까지
        {
            if (CheckObject())
            {
                isSwing = false; // 충돌체가 있다면 중복 공격되지 않게 와일문을 빠져나오도록 swing을 false로 바꿔줌ㅁ
                Debug.Log(hitInfo.transform.name);
            }
            yield return null;
        }
    }*/

    protected bool CheckObject() // 충돌체가 있는지 검사
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, currentCloseWeapon.range))
        {
            return true;
        }
        return false;
    }

    //완성된 함수이지만 추가 편집이 가능한 함수
    public virtual void CloseWeaponChange(CloseWeapon _closeWeapon)
    {
        if (WeaponManager.currentWeapon != null)
        {
            WeaponManager.currentWeapon.gameObject.SetActive(false);
        }

        currentCloseWeapon = _closeWeapon;
        WeaponManager.currentWeapon = currentCloseWeapon.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentCloseWeapon.anim;

        currentCloseWeapon.transform.localPosition = Vector3.zero;
        currentCloseWeapon.gameObject.SetActive(true);
    }
}
