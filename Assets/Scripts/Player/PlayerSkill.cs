using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    private Animator _playerAnimator;
    public PlayerStatus currentStatus;
    private PlayerToolController _playerToolController;
    [SerializeField] private float attackRate;
    private float attackTimer;
    private GameObject previousObject=null;
    [SerializeField] private float distanceToGrab;
    private GameObject _tempPickUpObk;
    public List<GameObject> throwObj;
    public Transform throwPoint;
    public GameObject shoveledSoil;
    public GameObject effect;
    public enum PlayerStatus
    {
        Movement,Construction,Collect,Attack,TakeDamage,OpenBag,Idle
    }
    public static PlayerSkill Instance;
    private Coroutine _dropCoroutine;

    private GameObject _tempShoveledSoid;
    private void Awake()
    {
        Instance = this;
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        _playerAnimator = GetComponent<Animator>();
        _playerToolController = PlayerToolController.Instance;
    }

    // Update is called once per frame
    void Update()
    {   
        attackTimer += Time.deltaTime;
        if(currentStatus==PlayerStatus.OpenBag)return;
        
        // raycast interactable things    
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, distanceToGrab))
        {
            GameObject hitObject = raycastHit.transform.gameObject;
            if (hitObject.CompareTag("Interactable"))
            {
                if (hitObject != previousObject)
                {
                    RemoveOutline(previousObject); 
                    AddOutline(hitObject); 
                    previousObject = hitObject; 
                }
            }
            else
            {
                RemoveOutline(previousObject); 
                previousObject = null; 
            }
        }
        else
        {
            RemoveOutline(previousObject); 
            previousObject = null; 
        }

        
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
        
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            Skill();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (_playerToolController.currentToolType == PlayerToolController.PlayerTool.Food ||
                _playerToolController.currentToolType == PlayerToolController.PlayerTool.Resource)
            {
                Debug.Log("throw");
                AudioManager.instance.PlaySFX("DropItem");
                _playerAnimator.CrossFade("Drop",0.1f);
            }
        }
        
    }

    private void Attack()
    {   
        if(attackTimer < attackRate)return;
        PlayerToolController.PlayerTool currentTool = _playerToolController.GetPlayerToolType();
        
        switch (currentTool)
        {
           case PlayerToolController.PlayerTool.Hammer:
                _playerAnimator.CrossFade("Hammer_attack", 0.1f);
               break;
           case PlayerToolController.PlayerTool.Shovel:
               
                _playerAnimator.CrossFade("Axe_attack", 0.1f);
               break;
           case PlayerToolController.PlayerTool.Spear:
                //spear attack
               _playerAnimator.CrossFade("Spear_attack", 0.1f);
               break;
           case PlayerToolController.PlayerTool.Resource:
                
                _playerAnimator.CrossFade("Punch_attack", 0.1f);
               break;
           case PlayerToolController.PlayerTool.Axe:
                
                _playerAnimator.CrossFade("Axe_attack", 0.1f);
               break;
           case PlayerToolController.PlayerTool.Water:
                
                _playerAnimator.CrossFade("Axe_attack", 0.1f);
               break;
           case PlayerToolController.PlayerTool.Punch:
               
                _playerAnimator.CrossFade("Punch_attack", 0.1f);
               break;
           case PlayerToolController.PlayerTool.Food:
               
                _playerAnimator.CrossFade("Punch_attack", 0.1f);
               break;
           case PlayerToolController.PlayerTool.Shield:
               
               _playerAnimator.CrossFade("Shield", 0.1f);
               break;
           
           default:
               // 处理未知工具类型的情况
               break;
        }

        attackTimer = 0f;

        //_playerAnimator.CrossFade("Pick_up",0.1f);
    }
    
    //special skill with E key
    private void Skill()
    {   
        //pick up
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, distanceToGrab))
        {
            GameObject hitObject = raycastHit.transform.gameObject;
            if (hitObject.GetComponent<PickableItem>()!= null)
            {   
                Debug.Log(hitObject.GetComponent<PickableItem>().item);
                _tempPickUpObk = hitObject;
                StartCoroutine(SmoothLookAtCoroutine());
                _playerAnimator.CrossFade("Pick_up",0.1f);
                return;
            }
            // else
            // {
            //     Debug.Log("Not pickable");
            // }
            
            
            //open door 
            if (hitObject.name=="Door")
            {
                AudioManager.instance.PlaySFX("Door");
                OpenOrCloseDoor(hitObject);
                return;
            }
            
            //plant 
            if (hitObject.name == "Soil(Clone)" && _playerToolController.GetPlayerToolType()== PlayerToolController.PlayerTool.Food )
            {
                _tempShoveledSoid = hitObject;
                if (_playerToolController.currentItemData.name == "Carrot" ||
                    _playerToolController.currentItemData.name == "Potato" ||
                    _playerToolController.currentItemData.name == "Pumpkin")
                {
                     _playerAnimator.CrossFade("Plant_seed_start",0.1f);
                     return;
                }
            }
            _tempShoveledSoid = null;
        
            
        }
        Debug.Log("active tool skill");
        
        //tool skill 
        PlayerToolController.PlayerTool currentTool = _playerToolController.GetPlayerToolType();
        switch (currentTool)
        {
            case PlayerToolController.PlayerTool.Hammer:
                AudioManager.instance.PlaySFX("HammerBuild");
                _playerAnimator.CrossFade("Build_hammer",0.1f);
                break;
            case PlayerToolController.PlayerTool.Food:
                _playerAnimator.CrossFade("Eating",0.1f);
                Debug.Log("eat");
                break;
            case PlayerToolController.PlayerTool.Shovel:
               
                _playerAnimator.CrossFade("Shoveling",0.1f);
                break;
            case PlayerToolController.PlayerTool.Water:
                
                _playerAnimator.CrossFade("Watering",0.1f);
                break;
            
        }
    }

    public void PickUp()
    {
        if (_tempPickUpObk)
        {
            //play sound
            AudioManager.instance.PlaySFX("PickUp");
            InventoryManager bag = InventoryManager.instance;
             bag.AddItem(bag.GetLastNotNullIndex(),_tempPickUpObk.GetComponent<PickableItem>().item);
             Destroy(_tempPickUpObk);
        }
       
    }
    
    public void Eat()
    {
        //sound 
        AudioManager.instance.PlaySFX("Eat");
        InventoryManager bagManager = InventoryManager.instance;
        Item consumeFood = InventoryManager.instance.GetCurrentItemData(PickTableContentManager.Instance.currentIndex);
        Debug.Log(consumeFood);
        bagManager.ConsumedItem(PickTableContentManager.Instance.currentIndex,1);

        if (consumeFood.addHpAmount > 0)
        {
            PlayerStatsManager.Instance.AddHpValue(consumeFood.addHpAmount);
        }
        else
        {
            PlayerStatsManager.Instance.TakeDamage(-consumeFood.addHpAmount);
        }
        
        PlayerStatsManager.Instance.AddHungerValue(consumeFood.addHungerAmount);

    }

    public void Drop()
    {
        foreach (var obj in throwObj)
        {
            if (obj.GetComponent<PickableItem>().item.name == _playerToolController.currentItemData.itemName)
            {   
                // Vector3 mousePosition = Input.mousePosition;
                // mousePosition.z = Camera.main.nearClipPlane;
                // Vector3 mouseDirection = Camera.main.ScreenToWorldPoint(mousePosition) - transform.position;
                // mouseDirection.Normalize(); 
                
                GameObject throwable = Instantiate(obj, throwPoint.position, Quaternion.identity);
                Rigidbody rb = throwable.GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * 5f, ForceMode.Impulse);
                InventoryManager.instance.ConsumedItem(PickTableContentManager.Instance.currentIndex,1);
                
            }
        }
    }

    public void Water()
    {
        Debug.Log("start water");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, distanceToGrab))
        {
            AudioManager.instance.PlaySFX("Watering");
            GameObject hitObject = raycastHit.transform.gameObject;
            hitObject.GetComponentInChildren<CropBehaviour>().WaterToGrow();
        }
    }

    public void ShovelGround()
    {   
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 5f))
        {   
            CropBehaviour crop = hit.transform.GetComponentInChildren<CropBehaviour>();
            //if planted
            if (crop || hit.transform.GetComponent<ChooseSeed>())
            {
                AudioManager.instance.PlaySFX("Digging");
                if (crop.cropState == CropBehaviour.CropState.Harvestable)
                {
                    var position = hit.transform.position;
                    //plant effect
                    Destroy(Instantiate(effect, position, Quaternion.identity),0.5f);
                    //instantiate harvest obj
                    GameObject obj1= Instantiate(hit.transform.gameObject.GetComponentInChildren<CropBehaviour>().harvestObj, position+new Vector3(0,0.5f,0),
                        quaternion.identity);
                    GameObject obj2 = Instantiate(hit.transform.gameObject.GetComponentInChildren<CropBehaviour>().harvestObj, position+new Vector3(0,0.2f,0),
                        quaternion.identity);
                    //45 degree force
                    Vector3 forceDirection = new Vector3(1f, 1f, 0f); 
                    forceDirection.Normalize(); 
                    obj1.GetComponent<Rigidbody>().AddForce(forceDirection * 20f, ForceMode.Force);
                    obj2.GetComponent<Rigidbody>().AddForce(forceDirection * 10f, ForceMode.Force);
                    //create soil
                    Instantiate(shoveledSoil,position, Quaternion.identity);
                    //destroy planted soil 
                    Destroy(hit.transform.gameObject);
                    
                }else if (crop.cropState == CropBehaviour.CropState.Withered)
                {
                    Debug.Log("die");
                    var position = hit.transform.position;
                    //plant effect
                    Destroy(Instantiate(effect, position, Quaternion.identity),0.5f);
                    //create soil
                    Instantiate(shoveledSoil,position, Quaternion.identity);
                    //destroy
                    Destroy(hit.transform.gameObject);
                }
            }
            else
            {
                AudioManager.instance.PlaySFX("Digging");
                //shoveled land
                Grid.Instance.GetXYZ(transform.position,out int x,out int z,out float y);
                //create effect
                Destroy(Instantiate(effect, Grid.Instance.GetWorldPosition(x, z) + new Vector3(0.8f, 0, 0.8f), Quaternion.identity),0.5f);
                //create soil
                Instantiate(shoveledSoil,Grid.Instance.GetWorldPosition(x,z)+new Vector3(0.8f,0,0.8f), Quaternion.identity);
            }

           
        }
       
       
    }

    public void StartPlanting()
    {   
        //consumed the item from bag 
        InventoryManager.instance.ConsumedItem(PickTableContentManager.Instance.currentIndex, 1);
        List<GameObject> soildList= _tempShoveledSoid.GetComponent<ChooseSeed>().soildList;
        switch (_playerToolController.currentItemData.itemName)
        {
            case "Carrot":
                Instantiate(soildList[0], _tempShoveledSoid.transform.position, Quaternion.identity);
                break;
                
            case "Potato":
                Instantiate(soildList[1], _tempShoveledSoid.transform.position, Quaternion.identity);
                break;
            
            case "Pumpkin":
                Instantiate(soildList[2], _tempShoveledSoid.transform.position, Quaternion.identity);
                break;
        }
        Destroy(_tempShoveledSoid);
        _tempShoveledSoid = null;
    }
    
    void AddOutline(GameObject obj)
    {
        Outline outline = obj.GetComponent<Outline>();
        if (outline == null)
        {
            outline = obj.AddComponent<Outline>();
        }

        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = Color.yellow;
        outline.OutlineWidth = 5f;
    }
    
    void RemoveOutline(GameObject obj)
    {
        if (obj != null)
        {
            Outline outline = obj.GetComponent<Outline>();
            if (outline != null)
            {
                Destroy(outline);
            }
        }
    }

    public void SetAttackStatus()
    {
        currentStatus = PlayerStatus.Attack;
    }
    
    public void SetIdleStatus()
    {
        currentStatus = PlayerStatus.Idle;
    }
    
    public void OpenOrCloseDoor(GameObject door)
    {
        StartCoroutine(SmoothRotate(door));
    }
    private IEnumerator SmoothRotate(GameObject door)
    {
        Quaternion startRotation = door.transform.rotation;
        Quaternion targetRotation;

        float angleDifference = Quaternion.Angle(startRotation, Quaternion.Euler(0, 90, 0));
        if (angleDifference < 1)
        {
            targetRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            targetRotation = Quaternion.Euler(0, 90, 0);
        }

        float duration = 1.0f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            door.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

            yield return null;
        }
    }
    private IEnumerator SmoothLookAtCoroutine()
    {
        while (true)
        {
            if (_tempPickUpObk != null)
            {
                // 计算目标方向
                Vector3 targetDirection = _tempPickUpObk.transform.position - transform.position;
                targetDirection.y = 0f; // 可以根据需要设置 Y 轴的值，如果需要保持平面朝向，将其设为0

                if (targetDirection != Vector3.zero)
                {
                    // 计算目标旋转
                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

                    // 使用 Slerp 进行平滑旋转
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 8f * Time.deltaTime);
                }
            }

            yield return null;
        }
    }
    
    
}
