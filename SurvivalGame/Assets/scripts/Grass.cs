using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Grass : MonoBehaviour
{
    //풀 체력 (보통은 1)

    [SerializeField]
    private int hp;
    [SerializeField]
    private float destroyTime;
    
    //폭발력 세기
    [SerializeField]
    private float force;

    [SerializeField]
    private GameObject go_hit_effect_prefab;

    [SerializeField]
    private Item item_leaf;
    [SerializeField]
    private int leafCount;
    private Inventory theInventory;


    private Rigidbody[] rigidbodys;
    private BoxCollider[] boxColliders;

    [SerializeField]
    private string hit_Sound;


    void Start()
    {
        rigidbodys = this.transform.GetComponentsInChildren<Rigidbody>();
        boxColliders = this.transform.GetComponentsInChildren<BoxCollider>();
        theInventory = FindObjectOfType<Inventory>();
    }

    

    public void Damage()
    { 
        hp--;
        HIT();

        if(hp <= 0)
        {
            Destruction();
        }
    }

    private void HIT()
    {

        var clone = Instantiate(go_hit_effect_prefab, transform.position + Vector3.up, Quaternion.identity);
        Destroy(clone, destroyTime);
    }

    private void Destruction()
    {

        AkSoundEngine.PostEvent("Branch_Attack", gameObject);


        theInventory.AcquireItem(item_leaf, leafCount);

        for (int i = 0; i < rigidbodys.Length; i++)
        {
            rigidbodys[i].useGravity = true;
            rigidbodys[i].AddExplosionForce(force, transform.position, 1f); //폭발세기, 폭발 위치, 폭발 반경

            boxColliders[i].enabled = true;

        }

        Destroy(this.gameObject, destroyTime);
    }
}
