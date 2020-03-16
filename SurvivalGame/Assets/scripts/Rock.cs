using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{

    [SerializeField]
    private int hp; //바위의 체력

    [SerializeField]
    private float destroyTime; // 파편 제거 시간 


    [SerializeField]
    private SphereCollider col; //구체 컬라이더;

    //필요한 게임 오브젝트
    [SerializeField]
    private GameObject go_rock; //일반 바위
    [SerializeField]
    private GameObject go_devris; //깨진 바위
    [SerializeField]
    private GameObject go_effect_prefabs; //채굴 이펙트
    [SerializeField]
    private GameObject go_rock_item_prefab; //돌맹이 아이템

    //돌맹이 아이텝 드롭 개수
    [SerializeField]
    private int count;


    [SerializeField]
    private string strike_Sound;
    [SerializeField]
    private string destroy_Sound;
    

    public void Mining() //채굴
    {
        AkSoundEngine.PostEvent("Rock_CloseAttack", gameObject);
        var clone = Instantiate(go_effect_prefabs, col.bounds.center, Quaternion.identity);
        Destroy(clone, destroyTime);
        hp--;
        if (hp <= 0) //파괴 조건
        {
            Destruction(); //파괴
        }
    }

    private void Destruction()
    {
        AkSoundEngine.PostEvent("Rock_Crash", gameObject);

        col.enabled = false;

        for (int i = 0; i < count; i++)
        {
            Instantiate(go_rock_item_prefab, go_rock.transform.position, Quaternion.identity);
        }
         


        Destroy(go_rock);
        go_devris.SetActive(true);
        Destroy(go_devris, destroyTime);
    }
}
