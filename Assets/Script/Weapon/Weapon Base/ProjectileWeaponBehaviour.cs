using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeaponBehaviour : MonoBehaviour
{

    public WeaponTableObject weaponData;
    protected Vector3 direction;
    public float destroyAfterSeconds;

    //Current Stats
    protected float currentDamage;
    protected int currentPierce;
    protected float currentSpeed;
    protected float currentCooldownDuration;

    void Awake()
    {
        currentDamage = weaponData.Dame;
        currentPierce = weaponData.Pierce;
        currentSpeed = weaponData.Speed;
        currentCooldownDuration = weaponData.CooldownDuration;
    }

    public float GetCurrentDame()
    {
        return currentDamage *= FindObjectOfType<PlayerStats>().CurrentMight;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

    public void DirectionChecker(Vector3 dir)
    {
        direction = dir;
        float dirx = direction.x;
        float diry = direction.y;

        Vector3 scale = transform.localScale;
        Vector3 rotation = transform.rotation.eulerAngles;

        if (dirx < 0 && diry == 0) //Left
        {
            scale.y = scale.y * -1;
        }
        else if (dirx == 0 && diry < 0) //Down
        {
            scale.y = scale.y * -1;
            rotation.z = 0f;
        }
        else if (dirx == 0 && diry > 0) //Up
        {
            rotation.z = 0f;
        }
        else if (dirx > 0 && diry > 0)
        {
            rotation.z = -45f;
        }
        else if (dirx > 0 && diry < 0)
        {
            rotation.z = -135f;
        }
        else if (dirx < 0 && diry > 0)
        {
            scale.y = scale.y * -1;
            rotation.z = -135f;
        }
        else if (dirx < 0 && diry < 0)
        {
            scale.y = scale.y * -1;
            rotation.z = -45f;
        }


        transform.localScale = scale;
        transform.rotation = Quaternion.Euler(rotation);
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Enemy"))
        {
            EnemyStats enemyStats = other.GetComponent<EnemyStats>();
            enemyStats.TakeDamage(GetCurrentDame(), transform.position);
            ReducePierce();
        }
        else if (other.CompareTag("Prop"))
        {
            if (other.gameObject.TryGetComponent(out BreakableProp breakableProp))
            {
                breakableProp.TakeDamage(GetCurrentDame());
                ReducePierce();
            }
        }

    }

    void ReducePierce()
    {
        currentPierce--;
        if (currentPierce <= 0)
        {
            Destroy(gameObject);
        }
    }

}
