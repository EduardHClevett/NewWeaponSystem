using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType { Pistol, SMG, AssaultRifle, SniperRifle };
public enum ReloadType { Magazine, Insertion };
public enum FireMode { FullAuto, SemiAuto };

public class WeaponData : MonoBehaviour
{
    [SerializeField] WeaponDataObject data;

    [Header("Gun Details")]
    public WeaponType weaponType;
    public ReloadType reloadType;
    public FireMode fireMode;

    public float range = 100f;
    
    public int currentReserve = 120;

    public int maxReserve;

    public int currentMag;
    
    public bool hasLastFire = false;


    public int weaponCost = 1000;

    [SerializeField]
    private int shotsFired;

    [Header("Animation Details")]
    public Vector3 aimPos;
    public float aimSpeed = 8f;

    [Header("External References")]
    public Camera cam;
    public WeaponManagerOld weaponManager;
    private Weapon weaponClass;
    public PlayerInputs input;

    [Header("Internal References")]
    public Transform muzzlePoint;
    public ParticleSystem muzzleFlash;
    public AudioSource gunshotSfx;


    private float fireTimer;
    [SerializeField]
    private bool isReloading = false;
    private bool isEnabled = true;
    private Vector3 originalPos;
    private Quaternion originalRot;

    [SerializeField]
    private float playerFOV;
    private float fovAdjust;
    [SerializeField]
    private bool isAiming = false;

    public bool IsEnabled
    {
        get { return isEnabled; }
        set { isEnabled = value; }
    }

    void Awake()
    {
        input = new PlayerInputs();

        if (fireMode == FireMode.SemiAuto)
            input.InGame.Shoot.started += _ => Fire();
        else if (fireMode == FireMode.FullAuto)
            input.InGame.Shoot.performed += _ => Fire();

        input.InGame.Reload.performed += _ => StartCoroutine(StartReload());

        input.InGame.Aim.performed += _ => StartADS();
        input.InGame.Aim.canceled += _ => StopADS();
    }

    void OnEnable()
    {
        input.Enable();
    }
    void OnDisable()
    {
        input.Disable();
    }

    void Start()
    {
        weaponManager = GetComponentInParent<WeaponManagerOld>();
        weaponClass = (Weapon)System.Enum.Parse(typeof(Weapon), data.weaponName);

        cam = Camera.main;

        playerFOV = cam.fieldOfView;
        fovAdjust = cam.fieldOfView - (cam.fieldOfView / 5);


        InitAmmo();

        originalPos = transform.localPosition;
        originalRot = transform.localRotation;

        if (!weaponManager.HasWeapon(weaponClass))
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {

        maxReserve = data.magCapacity * data.extraMags;

        shotsFired = data.magCapacity - currentMag;

        if (fireTimer < 1 / data.fireRate)
        {
            fireTimer += Time.deltaTime;
        }


        if (currentMag == 0)
            StartCoroutine(StartReload());

        if (isAiming)
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fovAdjust, 0.5f);
        else
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, playerFOV, 0.5f);
    }

    void DrawHitRay()
    {
        Debug.DrawRay(muzzlePoint.position, CalculateSpread(data.spread, muzzlePoint), Color.green, 10f);
    }

    Vector3 CalculateSpread(float inaccuracy, Transform trans)
    {
        if (isAiming) inaccuracy /= 2;

        return Vector3.Lerp(trans.TransformDirection(Vector3.forward * range), Random.onUnitSphere, inaccuracy);
    }

    IEnumerator DisableFire(float time = 0.3f)
    {
        isEnabled = false;

        yield return new WaitForSeconds(time);
        isEnabled = true;

        yield break;
    }

    void Fire()
    {
        if (fireTimer <  1 / data.fireRate || !isEnabled || weaponManager.pauseFire || isReloading) return;

        if (currentMag <= 0)
        {
            StartCoroutine(DisableFire());

            shotsFired = data.magCapacity;
            return;
        }

        RaycastHit hit;

        for (int i = 0; i < data.pellets; i++)
        {
            if(gunshotSfx != null)
                gunshotSfx.PlayOneShot(gunshotSfx.clip);

            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
            {
                IEntity entity = hit.transform.GetComponent<IEntity>();

                if (entity != null)
                {
                    entity.TakeDamage(data.damage);
                }
            }
        }

        if(muzzleFlash != null)
            muzzleFlash.Play();

        currentMag--;

        fireTimer = 0f;
    }

    void StartADS()
    {
        isAiming = true;
        transform.localPosition = aimPos;
    }
    void StopADS()
    {
        isAiming = false;
        transform.localPosition = originalPos;
    }

    IEnumerator StartReload()
    {
        if (isReloading || currentMag >= data.magCapacity || currentReserve <= 0)
        {
            yield break;
        }

        isReloading = true;


        yield return new WaitForSeconds(data.reloadTime);

        if ((currentReserve < data.magCapacity) && ((currentMag + currentReserve) <= data.magCapacity))
        {
            currentMag += currentReserve;
            currentReserve = 0;
        }
        else
        {
            currentMag = data.magCapacity;
            currentReserve -= shotsFired;
        }

        isReloading = false;
        
        shotsFired = 0;
    }

    public void DrawObj()
    {
        StartCoroutine(PrepareWeapon());
    }

    public void InitAmmo()
    {
        currentReserve = data.startReserves;
        currentMag = data.magCapacity;
    }

    IEnumerator PrepareWeapon()
    {
        yield return new WaitForEndOfFrame();

        isEnabled = true;

        yield break;
    }

    public void UnloadObj()
    {
        isReloading = false;
        transform.localRotation = originalRot;
        isEnabled = false;

        gameObject.SetActive(false);
    }
}
