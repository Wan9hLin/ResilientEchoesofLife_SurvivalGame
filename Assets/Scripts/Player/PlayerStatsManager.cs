using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerStatsManager : MonoBehaviour
{
    public float currentHp;

    public float maxHp;

    public float currentHunger;
    
    public float maxHunger;

    public float currentWarmth;
    
    public float maxWarmth;
    //hunger
    public float hungerDecreaseAmount;

    public float hungerDecreaseRate;

    private float _hungerDecreaseTimer;
    
    //warm
    public float warmthDecreaseAmount;
    
    public float warmthDecreaseRate;
    
    private float _warmthDecreaseTimer;
    
    //warm / hunger < 0 decrease hp
    public float decreaseHpRate;
    
    public float decreaseHpAmount;
    
    private float _decreaseHpTimer;
    
    private Animator _playerAnimator;

    public static PlayerStatsManager Instance;

    public bool isWarthDecrease;
    //
    public PlayerStatusType _currentPlayerStatusType = PlayerStatusType.Alive;

    //event 
    public event Action<float> OnHealthChanged;
    
    
    public delegate void PlayerOutdoorStatusChanged(bool isOutdoor);
    public event PlayerOutdoorStatusChanged OnPlayerOutdoorStatusChanged;
    private bool _isOutdoor=true;
    private Vector3 _rebornPosition= new Vector3(0,0,0);
    
    private PlayerInput playerInput;

    public bool isfiring;
    public enum PlayerStatusType
    {
        Die,
        Alive
    }
    
    private void Awake()
    {

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHp = maxHp;
        currentHunger = maxHunger;
        currentWarmth = maxWarmth;
        _playerAnimator = GetComponent<Animator>();
        //initial maxHunger,maxWarmth,maxHp to UI
        isWarthDecrease = true;
        playerInput = GetComponent<PlayerInput>();
    }
    
    // Update is called once per frame
    void Update()
    {   
        //if die 
        if (_currentPlayerStatusType == PlayerStatusType.Die) return;
        
        _hungerDecreaseTimer += Time.deltaTime;
        if (_hungerDecreaseTimer >= hungerDecreaseRate)
        {
            DecreaseHunger(hungerDecreaseAmount);
            _hungerDecreaseTimer = 0f;
        }
        
        if (currentHunger == 0 || currentWarmth == 0)
        {   
            _decreaseHpTimer += Time.deltaTime;
            if (_decreaseHpTimer >= decreaseHpRate)
            {
                TakeDamage((int)decreaseHpAmount);
                _decreaseHpTimer = 0f;
            }
        }
        
        _warmthDecreaseTimer += Time.deltaTime;
        if (_warmthDecreaseTimer >= warmthDecreaseRate && isWarthDecrease)
        {
            DecreaseWarmth(warmthDecreaseAmount);
            _warmthDecreaseTimer = 0f;
        }
        
    }
    
    void LateUpdate()
    {
        //save reborn position
        if (!CheckIfPlayerOutIndoor() && _rebornPosition == new Vector3(0, 0, 0)) _rebornPosition = transform.position;
        bool currentIsOutdoor = CheckIfPlayerOutIndoor();
        if (_isOutdoor != currentIsOutdoor)
        {
            _isOutdoor = currentIsOutdoor;
            OnPlayerOutdoorStatusChanged?.Invoke(_isOutdoor);
        }
        
    }
    private bool CheckIfPlayerOutIndoor()
    {
        int indoorLayerMask = LayerMask.GetMask("Wall");

        bool hitTop = Physics.Raycast(transform.position, transform.up, 20f, indoorLayerMask);
        //Debug.DrawRay(transform.position, transform.up * 20f, hitTop ? Color.green : Color.red);

        bool hitForward = Physics.Raycast(transform.position + Vector3.up * 1f, transform.forward, 20f, indoorLayerMask);
        //Debug.DrawRay(transform.position + Vector3.up * 1f, transform.forward * 20f, hitForward ? Color.green : Color.red);

        bool hitBackward = Physics.Raycast(transform.position + Vector3.up * 1f, -transform.forward, 20f, indoorLayerMask);
        //Debug.DrawRay(transform.position + Vector3.up * 1f, -transform.forward * 20f, hitBackward ? Color.green : Color.red);

        bool hitLeft = Physics.Raycast(transform.position + Vector3.up * 1f, -transform.right, 20f, indoorLayerMask);
        //Debug.DrawRay(transform.position + Vector3.up * 1f, -transform.right * 20f, hitLeft ? Color.green : Color.red);

        bool hitRight = Physics.Raycast(transform.position + Vector3.up * 1f, transform.right, 20f, indoorLayerMask);
        //Debug.DrawRay(transform.position + Vector3.up * 1f, transform.right * 20f, hitRight ? Color.green : Color.red);

        return !(hitTop && hitForward && hitBackward && hitLeft && hitRight);
    }
    private void DecreaseHunger(float amount)
    {
        currentHunger -= amount;

        // Check if hunger becomes negative or goes below zero
        if (currentHunger < 0f)
        {
            currentHunger = 0f;
        }

        // Update UI to reflect the new hunger value
       HungerBar.Instance.UpdateHungerBar(currentHunger);
    }
    private void DecreaseWarmth(float amount)
    {
        currentWarmth -= amount;

        // Check if warmth becomes negative or goes below zero
        if (currentWarmth < 0f)
        {
            currentWarmth = 0f;
        }

        // Update UI to reflect the new warmth value
        WarmthBar.Instance.UpdateWarmthBar(currentWarmth);
        
    }
    public void TakeDamage(int damage)
    {
        if(_currentPlayerStatusType == PlayerStatusType.Die)return;

        if (PlayerToolController.Instance.currentItemData.itemName == "Shield")
        {
            Parry parry = Parry.Instance;
            if (parry.GetColliderIsActive())
            {
                parry.ParryEffect();
                return;
            }
            
        }

        // Play animation
        _playerAnimator.CrossFade("TakeDamage", 0.1f);
        AudioManager.instance.PlaySFX("PlayerAttacked");
        // Update values after checking
        currentHp -= damage;
    
        // Check if currentHp exceeds maxHp or becomes less than or equal to zero
        if (currentHp > maxHp)
        {
            currentHp = maxHp;
        }
        else if (currentHp <= 0)
        {
            currentHp = 0;
            Die();
        }

        HealthBar.Instance.UpdateHealthBar(currentHp);
        OnHealthChanged?.Invoke(currentHp);
        
        
    }
    
    public void AddHungerValue(int value)
    {
        //change value
        currentHunger += value;
        if (currentHunger > maxHunger)
        {
            currentHunger = maxHunger;
        }
        //update UI
        HungerBar.Instance.UpdateHungerBar(currentHunger);
    }
    //add effect
    public void AddHpValue(int value)
    {   
        currentHp += value;
        //change value 
        if (currentHp > maxHp)
        {
            currentHp = maxHp;
        }
        //update ui
        HealthBar.Instance.UpdateHealthBar(currentHp);
        OnHealthChanged?.Invoke(currentHp);

    }
    
    public void AddWarmthValue()
    {
        currentWarmth += 10;

        // Check if warmth value exceeds the maximum value
        if (currentWarmth > maxWarmth)
        {
            currentWarmth = maxWarmth;
        }
        //update ui
        WarmthBar.Instance.UpdateWarmthBar(currentWarmth);
        
    }
    public void Die()
    {   
        _currentPlayerStatusType = PlayerStatusType.Die;
        _playerAnimator.CrossFade("Die",0.1f);
        playerInput.enabled = false;
    }
    
    public void Reborn()
    {
        StartCoroutine(DelayToReborn());
    }

    IEnumerator DelayToReborn()
    {
        yield return new WaitForSeconds(1.5f);
        playerInput.enabled = true;
        //reset player data
        currentHp = maxHp;
        currentHunger = maxHunger;
        currentWarmth = maxWarmth;
        HealthBar.Instance.ResetHp();
        WarmthBar.Instance.ResetWarmth();
        HungerBar.Instance.ResetHurger();
        _currentPlayerStatusType = PlayerStatusType.Alive;
        //position 
        _playerAnimator.CrossFade("Idle Walk Run Blend",0.1f);
        if (_rebornPosition != new Vector3(0, 0, 0)) {
            transform.position = _rebornPosition+Vector3.up *1.5f;
            yield return null;
        }

        GameObject rebornPosObj = GameObject.FindWithTag("RebornPoint");
        if (rebornPosObj!=null)
        {
            
            transform.position = rebornPosObj.transform.position+Vector3.up *1.5f;
        }
        else
        {
            transform.position = new Vector3(0, 1.5f, 0);
            Debug.LogWarning("Reborn Position Not Assign");
            
        }
    }
    
}
