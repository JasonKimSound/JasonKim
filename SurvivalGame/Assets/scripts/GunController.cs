using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    //활성화여부
    public static bool isActivate = false;

    //현재 장착된 총
    [SerializeField]
    private Gun currentGun;
    
    //연사 속도
    private float currentFireRate; 


    //상태 변수들
    private bool isReload = false; //재장전중인지 아닌지

    [HideInInspector] //인스펙터창에서숨기기
    public bool isFineSightMode = false; //러닝때 파인사이트취소하려고


    //본래 포지션 값,,정조준해제
    
    
    private Vector3 originPos; // 기본값은 (0, 0, 0)


    //효과음 재생
    [SerializeField]
    private string fire_Sound;
    //private AudioSource audioSource;


    //필요한 컴포넌트
    [SerializeField]
    private Camera theCam;
    private Crosshair theCrosshair;


    //피격 이펙트
    [SerializeField]
    private GameObject hit_effect_prefab;

    //레이저 충돌 정보 받아옴
    RaycastHit hitInfo;


    void Start()
    {
        originPos = Vector3.zero;
        theCrosshair = FindObjectOfType<Crosshair>();



    }
    // Update is called once per frame
    void Update()
    {
        if (isActivate)
        {
            if (!Inventory.inventoryActivated)
            {
                GunFireRateCalc();
                TryFire();
                TryReload();
                TryFineSight();
            }
        }
        
    }



    private void GunFireRateCalc() //연사속도 재계산
    {
        if (currentFireRate > 0)
        {
            currentFireRate -= Time.deltaTime; // time.deltatime = 1/60 즉 1초에 1 감소
        }
    }

    private void TryFire() //발사 시도
    {
        if (Input.GetButton("Fire1") && currentFireRate <= 0 && !isReload)
        {
            Fire();

            AkSoundEngine.PostEvent("Gun_Fire", gameObject);

        }
    }
    private void Fire() //발사 전 계산
    {
        if (!isReload)
        {
            if (currentGun.currentBulletCount > 0)
                Shoot();
            else
            {
                CancelFineSight();
                StartCoroutine(Reload());
            }
        }

    }



    private void Shoot() //발사 후 계산
    {
        theCrosshair.FireAnimation(); //크로스헤어 애니메이션 실행
        currentGun.currentBulletCount--; //현재 총알 개수 1개 하락
        currentFireRate = currentGun.fireRate; //연사 속도 재계산
        SoundManager.instance.PlaySE(fire_Sound);
        //PlaySE(currentGun.fire_Sound); //총소리 재생
        currentGun.muzzleflash.Play(); //머즐 플래시 재생
        Hit(); //맞춘곳을 반환해서 이펙트 생성
        StopAllCoroutines();
        StartCoroutine(RetroActionCoroutione());
    }

    private void Hit() //위치 생성
    {
        if(Physics.Raycast(
            theCam.transform.position, 
            theCam.transform.forward + new Vector3(Random.Range(-theCrosshair.GetAcuraccy() - currentGun.accuracy, theCrosshair.GetAcuraccy() + currentGun.accuracy),
            Random.Range(-theCrosshair.GetAcuraccy() - currentGun.accuracy, theCrosshair.GetAcuraccy() + currentGun.accuracy),
            0), 
            out hitInfo, 
            currentGun.range))
        {
            GameObject clone = Instantiate(hit_effect_prefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(clone, 2.0f);
                //hitInfo.point -> 레이가 충돌한 곳의 월드 좌표..... Quaternio.LookRotation(hitInfo.normal)) -> 충돌체의 표면이 바라보는 방향
        }
    } 

    private void TryReload() //재장전 시도
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReload && currentGun.currentBulletCount < currentGun.reloadBulletCount)
        {
            CancelFineSight();
            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload() //재장전
    {
        isReload = true;

        AkSoundEngine.PostEvent("Gun_Load", gameObject);

        if (currentGun.carryBulletCount > 0)
        {
            currentGun.anim.SetTrigger("Reload");


            yield return new WaitForSeconds(currentGun.reloadTime);


            currentGun.carryBulletCount += currentGun.currentBulletCount;
            //currentGun.currentBulletCount = 0;


            if (currentGun.carryBulletCount >= currentGun.currentBulletCount)
            {
                if (currentGun.carryBulletCount < currentGun.reloadBulletCount)
                {
                    currentGun.currentBulletCount = currentGun.carryBulletCount;
                    currentGun.carryBulletCount = 0;
                }
                else
                {
                    currentGun.currentBulletCount = currentGun.reloadBulletCount;
                    currentGun.carryBulletCount -= currentGun.reloadBulletCount;
                }



            }
            else
            {
                currentGun.currentBulletCount = currentGun.carryBulletCount;
                currentGun.carryBulletCount = 0;
            }
            //9 0

        }
        else
        {
            Debug.Log("소유한 총알이 없습니다.");
        }
        isReload = false;

    }

    public void CancelReload()
    {
        if (isReload)
        {
            StopAllCoroutines();
            isReload = false;
        }
            
    }

    private void TryFineSight() //정조준 시도
    {
        if (Input.GetButtonDown("Fire2") && !isReload)
        {
            FineSight();
        }
    }

    public void CancelFineSight() //리로드 하기 전에 정조준 모드 취소 스탑 올코루틴 때문에 리로드가 false로 안바뀌어서ㅜ ㅜ.....정조준 취소
    {
        if (isFineSightMode)
            FineSight();
        
    }

   

    private void FineSight() //정조준 로직가동
    {
        isFineSightMode = !isFineSightMode;
        currentGun.anim.SetBool("FineSightMode", isFineSightMode);
        theCrosshair.FineSightAnimation(isFineSightMode);
        if (isFineSightMode)
        {
            StopAllCoroutines();
            StartCoroutine(FineSightActivateCoroutine());
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(FineSightDeactivateCoroutine());
        }
    } 

    IEnumerator FineSightActivateCoroutine() //정조준 활성화
    {
        while(currentGun.transform.localPosition != currentGun.fineSightOriginPos)
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.2f);
            yield return null; // 1frame 대기
        }
    }

    IEnumerator FineSightDeactivateCoroutine() //정조준 비활성화
    {
        while (currentGun.transform.localPosition != originPos)
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.2f);
            yield return null; // 1frame 대기
        }
    }

    IEnumerator RetroActionCoroutione() //반동 코루틴
    {
        Vector3 retroback = new Vector3(currentGun.retroActionForce, originPos.y, originPos.z); //정조준 안했을 때의 최대반동
        Vector3 recoilActionRetroBack = new Vector3(currentGun.retroActionFineSightForce, currentGun.fineSightOriginPos.y, currentGun.fineSightOriginPos.z); //정조준 했을 때의 최대반동

        if (!isFineSightMode)
        {
            currentGun.transform.localPosition = originPos;

            while(currentGun.transform.localPosition.x <= currentGun.retroActionForce -0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, retroback, 0.4f);
                yield return null;
            }

            while(currentGun.transform.localPosition != originPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.1f);
                yield return null;

            }
        }
        else
        {
            currentGun.transform.localPosition = currentGun.fineSightOriginPos;

            while (currentGun.transform.localPosition.x <= currentGun.retroActionFineSightForce - 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, recoilActionRetroBack, 0.4f);
                yield return null;
            }

            while (currentGun.transform.localPosition != currentGun.fineSightOriginPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.1f);
                yield return null;

            }
        }
         
    }


    public Gun GetGun() //Gun 스크립트 받아오기
    {
        return currentGun;
    }

    public bool GetFineSightMode()
    {
        return isFineSightMode;
    }

    public void GunChange(Gun _gun)
    {
        if(WeaponManager.currentWeapon != null)
        {
            WeaponManager.currentWeapon.gameObject.SetActive(false);
        }

        currentGun = _gun;
        WeaponManager.currentWeapon = currentGun.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentGun.anim;

        currentGun.transform.localPosition = Vector3.zero;
        currentGun.gameObject.SetActive(true);
        isActivate = true;
    }


}
