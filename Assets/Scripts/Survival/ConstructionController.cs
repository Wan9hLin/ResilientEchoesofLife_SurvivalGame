using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ConstructionController : MonoBehaviour
{
    public List<GameObject> construction;

    public static ConstructionController Instance;

    public ConstructionType currentType;
    
    public float height;

    private Animator _playerAnimator;

    public GameObject buildUI;

    public GameObject effect;
    public enum WallDirection
    {
        Up,
        Down,
        Left,
        Right
    }
    
    public enum ConstructionType
    {
        Floor,Wall,Roof,Window,Door,Stair,StoneBase,CampFire
    }
    
    private void Awake()
    {
        Instance = this;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1) && PlayerToolController.Instance.currentToolType == PlayerToolController.PlayerTool.Hammer)
        {
            OpenMenu();
        }
    }

    public void OpenMenu()
    {
        buildUI.SetActive(true);
    }
    public void BuildPreview()
    {
        
    }
    
    public void Build()
    {   
        //build ui
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 10f))
        {
            AudioManager.instance.PlaySFX("ObjectDrop");
            switch (currentType)
            {
                case ConstructionType.Floor:
                    BuildFloor(raycastHit);
                    break;
                case ConstructionType.Wall:
                    BuildWall(raycastHit);
                    break;
                case ConstructionType.Roof:
                    BuildRoof(raycastHit);
                    break;
                case ConstructionType.Door:
                    BuildWall(raycastHit);
                    break;
                case ConstructionType.Window:
                    BuildWall(raycastHit);
                    break;
                case ConstructionType.Stair:
                    BuildStair(raycastHit);
                    break;
                case ConstructionType.StoneBase:
                    BuildStoneBase(raycastHit);
                    break;
                case ConstructionType.CampFire:
                    BuildCampFire(raycastHit);
                    break;
                
            }
           
            
        }
    }

    public void BuildFloor(RaycastHit raycastHit)
    {
        Grid.Instance.GetXYZ(raycastHit.point,out int x,out int z,out float y);
        //instantiate floor 
        Instantiate(construction[0],Grid.Instance.GetWorldPosition(x+1,z), Quaternion.identity);
        //effect
        Destroy(Instantiate(effect, raycastHit.point, Quaternion.identity),3f);
    }

    public void BuildWall(RaycastHit raycastHit)
    {
        
        if(!raycastHit.transform.CompareTag("Floor"))return;
        WallDirection wallDirection = GetWallDirection(raycastHit.transform.position,raycastHit.point);
        GameObject buildingObj = null;
        switch (currentType)
        {
            case ConstructionType.Wall:
                buildingObj = construction[1];
                break;
            case ConstructionType.Door:
                buildingObj = construction[2];
                break;
            case ConstructionType.Window:
                buildingObj = construction[3];
                break;
            default:
                buildingObj = construction[3];
                break;
        }
        
        switch (wallDirection)
        {
            case WallDirection.Up:
                Instantiate(buildingObj, raycastHit.transform.position+new Vector3(0,0,1.5f), Quaternion.Euler(0f, 0f, 0f));
                break;
            case WallDirection.Down:
                //
                Instantiate(buildingObj, raycastHit.transform.position, Quaternion.Euler(0f, 0f, 0f));
                break;
            case WallDirection.Left:
                //
                Instantiate(buildingObj, raycastHit.transform.position+new Vector3(-1.5f,0,0), Quaternion.Euler(0f, 90f, 0f));
                break;
            case WallDirection.Right:
                Instantiate(buildingObj, raycastHit.transform.position, Quaternion.Euler(0f, 90f, 0f));
                break;
        }
        //effect
        Destroy(Instantiate(effect, raycastHit.point, Quaternion.identity),3f);

    }

    public void BuildRoof(RaycastHit raycastHit)
    {
        Grid.Instance.GetXYZ(raycastHit.point,out int x,out int z,out float y);
        Instantiate(construction[5],Grid.Instance.GetWorldPosition(x+1,z+1)+new Vector3(0,height,0), Quaternion.identity);
        //effect
        Destroy(Instantiate(effect, raycastHit.point, Quaternion.identity),3f);
    }

    public void BuildStair(RaycastHit raycastHit)
    {
        Grid.Instance.GetXYZ(raycastHit.point,out int x,out int z,out float y);
        //instantiate floor 
        Instantiate(construction[6],Grid.Instance.GetWorldPosition(x+1,z), Quaternion.identity);
        //effect
        Destroy(Instantiate(effect, raycastHit.point, Quaternion.identity),3f);
    }
    
    public void BuildStoneBase(RaycastHit raycastHit)
    {
        Grid.Instance.GetXYZ(raycastHit.point,out int x,out int z,out float y);
        //instantiate floor 
        Instantiate(construction[7],Grid.Instance.GetWorldPosition(x+1,z), Quaternion.identity);
        //effect
        Destroy(Instantiate(effect, raycastHit.point, Quaternion.identity),3f);
    }
    public void BuildCampFire(RaycastHit raycastHit)
    {
        Grid.Instance.GetXYZ(raycastHit.point,out int x,out int z,out float y);
        //instantiate floor 
        Instantiate(construction[4],Grid.Instance.GetWorldPosition(x+1,z), Quaternion.identity);
        //effect
        Destroy(Instantiate(effect, raycastHit.point, Quaternion.identity),3f);
    }
    //get rotation
    private WallDirection GetWallDirection(Vector3 floorPosition, Vector3 hitPoint)
    {
        float right = Mathf.Abs(hitPoint.x - floorPosition.x);
        float left = Mathf.Abs(hitPoint.x - (floorPosition.x -1.5f));
        float up = Mathf.Abs((floorPosition.z + 1.5f) - hitPoint.z);
        float down = Mathf.Abs(hitPoint.z - floorPosition.z);
        float min = Mathf.Min(right, left, up, down);
        WallDirection minDirection;
        switch (min)
        {
            case float val when val == right:
                minDirection = WallDirection.Right;
                break;
            case float val when val == left:
                minDirection = WallDirection.Left;
                break;
            case float val when val == up:
                minDirection = WallDirection.Up;
                break;
            case float val when val == down:
                minDirection = WallDirection.Down;
                break;
            default:
                minDirection = WallDirection.Up;
                break;
        }
        return minDirection;
    }
    
    
        



}
