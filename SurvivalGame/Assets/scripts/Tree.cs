using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    //깎일 나무 조각들
    [SerializeField]
    private GameObject[] go_treePieces;
    [SerializeField]
    private GameObject go_treeCenter;
    
    //통나무
    [SerializeField]
    private GameObject go_Log_Prefabs;    


    //쓰러질 때 랜덤으로 가해질 힘의 세기
    [SerializeField]
    private float force;
    //자식 트리
    [SerializeField]
    private GameObject go_childTree;
    
    //부모트리 파괴되면 캡슐콜라이더 제거
    [SerializeField]
    private CapsuleCollider parentCol;
    
    //자식 트리 쓰러질 때 필요한 컴포넌트 활성화 및 중력 활성화
    [SerializeField]
    private CapsuleCollider childCol;
    [SerializeField]
    private Rigidbody childRigid;

    //도끼 파편
    [SerializeField]
    private GameObject go_hit_effect_prefab;

    //파편 제거 시간
    [SerializeField]
    private float debrisDestroyTime;

    //나무 제거 시간
    [SerializeField]
    private float destroyTime;


    //필요한 사운드
    [SerializeField]
    private string chop_Sound;
    [SerializeField]
    private string falldown_Sound;
    [SerializeField]
    private string logChange_Sound;


    public void Chop(Vector3 _pos, float _angleY)
    {
        HIT(_pos);
        AngleCalc(_angleY);


        if (CheckTreePieces())
        {
            return;
        }
        
        FallDownTree();


    }

    //적중이펙트
    private void HIT(Vector3 _pos)
    {

        AkSoundEngine.PostEvent("Chooping_Log", gameObject);


        GameObject clone = Instantiate(go_hit_effect_prefab, _pos, Quaternion.Euler(Vector3.zero)); //Quaternion.Identity = Quaternion.Euler(Vector3.zero)와 같다;;
        Destroy(clone, debrisDestroyTime);
    }

    private void AngleCalc(float _angleY)
    {
        Debug.Log(_angleY);
        if (0 <= _angleY && 70 > _angleY)
            DestroyPiece(2);
        else if (70 <= _angleY && 140 > _angleY)
            DestroyPiece(3);
        else if (140 <= _angleY && 210 > _angleY)
            DestroyPiece(4);
        else if (210 <= _angleY && 280 > _angleY)
            DestroyPiece(0);
        else if (280 <= _angleY && 360 > _angleY)
            DestroyPiece(1);

    }

    private void DestroyPiece(int _num)
    {
        if(go_treePieces[_num].gameObject != null)
        {
            GameObject clone = Instantiate(go_hit_effect_prefab, go_treePieces[_num].transform.position, Quaternion.Euler(Vector3.zero)); //Quaternion.Identity = Quaternion.Euler(Vector3.zero)와 같다;;
            Destroy(clone, debrisDestroyTime);
            Destroy(go_treePieces[_num].gameObject);
        }
    }
    
    private bool CheckTreePieces()
    {
        for (int i = 0; i < go_treePieces.Length; i++)
        {
            if(go_treePieces[i].gameObject != null)
            {
                Debug.Log(go_treePieces[i]);
                return true;
            }
        }
        return false;
    }

    private void FallDownTree()
    {
        SoundManager.instance.PlaySE(falldown_Sound);

        Destroy(go_treeCenter);
        parentCol.enabled = false;
        childCol.enabled = true;
        childRigid.useGravity = true;

        childRigid.AddForce(Random.Range(-force, force), 0, Random.Range(-force, force));


        StartCoroutine(LogCoroutine());

    }
    
    IEnumerator LogCoroutine()
    {

        yield return new WaitForSeconds(destroyTime);
        SoundManager.instance.PlaySE(logChange_Sound);
        Destroy(go_childTree);

        Instantiate(go_Log_Prefabs, go_childTree.transform.position + (go_childTree.transform.up * 3f), Quaternion.LookRotation(go_childTree.transform.up));
        Instantiate(go_Log_Prefabs, go_childTree.transform.position + (go_childTree.transform.up * 6f), Quaternion.LookRotation(go_childTree.transform.up));
        Instantiate(go_Log_Prefabs, go_childTree.transform.position + (go_childTree.transform.up * 9f), Quaternion.LookRotation(go_childTree.transform.up));
    }


    public Vector3 GetTreeCenterPosition()
    {
        return go_treeCenter.transform.position;
    }

}
