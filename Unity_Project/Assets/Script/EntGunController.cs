using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntGunController : MonoBehaviour
{
    public bool isAim = false;

    private bool isCharge = false;

    public static bool isEntGunActivated = true;

    private float fireRateValue; //계산할 발사속도 값

    [SerializeField]
    private AudioClip fire_Sound;

    [SerializeField]
    private AudioClip charge_Fire_Sound;

    [SerializeField]
    private AudioClip charge_Alert;

    [SerializeField]
    private GameObject charge_Fire_Effect;

    [SerializeField]
    private GameObject uncharge_Fire_Effect;

    [SerializeField]
    public int chargeGauge = 0;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask layerMask;
    
    

    private RaycastHit hitInfo;

    private AudioSource audioSource;

    private Gun gun;


    private Crosshair crosshair;

    [SerializeField]
    private Status status;

    [SerializeField]
    private int entCount;

    [SerializeField]
    private int max_Ent;

    private GameManager gameManager;

    private PlayerController playerController;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gun = FindObjectOfType<Gun>();
        crosshair = FindObjectOfType<Crosshair>();
        WeaponManager.currentWeaponTr = gun.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = gun.anim;
        gameManager = FindObjectOfType<GameManager>();
        playerController = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        if (isEntGunActivated)
        {
            FireRateCalc();
            BeforeFire();
            BeforeAim();
        }
        
    }

    //발사 속도 제어
    private void FireRateCalc()
    {
        if (fireRateValue > 0)
        {
            fireRateValue -= Time.deltaTime;
            if (fireRateValue < 0)
            {
                fireRateValue = 0;
            }
        }
    }

    //발사 조건
    private void BeforeFire()
    {
        if (Input.GetButton("Fire1") && fireRateValue == 0 && !gameManager.isPause)
        {
            Fire();
        }
    }

    //발사 가능여부 확인
    private void Fire()
    {
        if (!isCharge)
        {
            UnChargeFire();
        }
                                 
        Hit();

        crosshair.FireAnimation();
        
    }

    //미충전발사(대상정지)
    private void UnChargeFire()
    {
        if (isAim)
        {
            gun.anim.SetTrigger("Aim_Fire");
        }
        else
        {
            gun.anim.SetTrigger("Fire");

        }
        audioSource.PlayOneShot(fire_Sound);
        fireRateValue = gun.fireRate;
        gun.muzzleFlash.Play();

    }

    private void ChargeCalc()
    {
        chargeGauge += entCount;
        status.IncreaseEnt(entCount);
        if (chargeGauge >= max_Ent)
        {
            chargeGauge = max_Ent;
            isCharge = true;
            audioSource.PlayOneShot(charge_Alert);

        }
    }

    //충전발사(대상처치)상태
    private void BeforeChargeFire()
    {
        if (hitInfo.transform.tag == "Enemy")
        {
            chargeGauge = 0;
            gun.anim.SetTrigger("Fire");
            audioSource.PlayOneShot(charge_Fire_Sound);
        }
        
    }

    //정조준 조건
    private void BeforeAim()
    {
        if (Input.GetButtonDown("Fire2") && !isCharge && !playerController.isRun)
        {
            Aim();
        }
    }

    private void Aim()
    {
        isAim = !isAim;
        gun.anim.SetBool("Aim", isAim);
        crosshair.AimAnimation(isAim);
    }

    public void CancleAim()
    {
        if (isAim)
        {
            Aim();
        }
    }

    private void Hit()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, gun.range, layerMask))
        {
            if (hitInfo.transform.tag == "Enemy")
            {
                if (!isCharge)
                {
                    if (!hitInfo.transform.GetComponent<EnemyController>().isEnt)
                    {
                        hitInfo.transform.GetComponent<EnemyController>().EntGunAttacked();
                        ChargeCalc();
                        GameObject clone = Instantiate(uncharge_Fire_Effect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                        Destroy(clone, 1.5f);
                    }
                }
                else if (isCharge)
                {
                    chargeGauge = 0;
                    gun.anim.SetTrigger("Fire");
                    audioSource.PlayOneShot(charge_Fire_Sound);
                    StartCoroutine(ReturnCoroutine());   

                }
                

            }

        }
    }

    IEnumerator ReturnCoroutine()
    {
        GameObject clone = Instantiate(charge_Fire_Effect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
        Destroy(clone, 1.5f);
        yield return null;
        if (hitInfo.transform.tag == "Enemy")
        {
            hitInfo.transform.GetComponent<EnemyController>().EntDead();
        }
        yield return null;
        isCharge = false;
        status.EntOut();
        chargeGauge = 0;
    }

    //무기교체
    public void EntGunChange()
    {
        //교체되는 무기 비활성화
        if (WeaponManager.currentWeaponTr != null)
        {
            WeaponManager.currentWeaponTr.gameObject.SetActive(false);
        }

        //교체할 무기의 정보 받기
        WeaponManager.currentWeaponTr = GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = GetComponent<Animator>();


        //교체할 무기 활성화
        gameObject.SetActive(true);

        isEntGunActivated = true;


    }

 
}
