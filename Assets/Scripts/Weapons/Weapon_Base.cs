using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Base : MonoBehaviour
{
    private Dictionary<WeaponState, Coroutine> activeStateTimers = new Dictionary<WeaponState, Coroutine>();

    public WeaponStats stats;

    public float currentMagAmmo, currentReserveAmmo, fireRate;

    public float fireTimer { get; protected set; }

    public bool isFiring;

    public enum WeaponState
    {
        Idle,
        Firing,
        Reloading
    }

    public WeaponState currentState
    {
        get { return _currentState; }
        private set
        {
            if(value != _currentState)
            {
                previousState = _currentState;
                _currentState = value;
            }
        }
    }

    //Do not modify these directly, use currentState instead
    private WeaponState _currentState = WeaponState.Idle;
    public WeaponState previousState { get; private set; } = WeaponState.Idle;

    public void SetState(WeaponState inState)
    {
        currentState = inState;

        switch(previousState)
        {
            case WeaponState.Idle:
                ExitIdle();
                break;
            case WeaponState.Firing:
                ExitFiring();
                break;
            case WeaponState.Reloading:
                ExitReloading();
                break;
        }

        switch (currentState)
        {
            case WeaponState.Idle:
                EnterIdle();
                break;
            case WeaponState.Firing:
                EnterFiring();
                break;
            case WeaponState.Reloading:
                EnterReloading();
                break;
        }
    }

    protected virtual void Start()
    {
        currentMagAmmo = stats.maxMagCount;
        currentReserveAmmo = stats.maxReserveCount;
        fireRate = stats.fireRate;
    }

    protected virtual void Awake() { }

    protected virtual void Update()
    {
        switch (currentState)
        {
            case WeaponState.Idle:
                UpdateIdle();
                break;
            case WeaponState.Firing:
                UpdateFiring();
                break;
            case WeaponState.Reloading:
                UpdateReloading();
                break;
        }
    }

    //On Enter functions
    protected virtual void EnterIdle() { }
    protected virtual void EnterFiring() { }
    protected virtual void EnterReloading() { }

    //On Update functions
    protected virtual void UpdateIdle() { }
    protected virtual void UpdateFiring() { }
    protected virtual void UpdateReloading() { }

    //On Exit functions
    protected virtual void ExitIdle() { }
    protected virtual void ExitFiring() { }
    protected virtual void ExitReloading() { }


    public bool SetStateTimer(WeaponState inState, float time)
    {
        if(!activeStateTimers.ContainsKey(inState))
        {
            activeStateTimers.Add(inState, StartCoroutine(StateTimer(inState, time)));
            return true;
        }

        return false;
    }

    private IEnumerator StateTimer(WeaponState inState, float time)
    {
        yield return new WaitForSeconds(time);

        activeStateTimers.Remove(inState);

        SetState(inState);
    }

    protected void CancelStateTimer()
    {
        foreach(KeyValuePair<WeaponState, Coroutine> pair in activeStateTimers)
        {
            StopCoroutine(pair.Value);
        }

        activeStateTimers.Clear();
    }
}
