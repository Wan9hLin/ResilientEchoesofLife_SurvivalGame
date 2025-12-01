using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RadialMenuManager : MonoBehaviour
{
    public Transform center;
    public Transform selectObject;
    public GameObject RadialMenuRoot;

    bool isRadialMenuActive;
    public TextMeshProUGUI HighlightBuildName;
    public TextMeshProUGUI selectedBuildName;
    public ConstructionController.ConstructionType[] constructionTypes;

    public Transform[] itemSlots;

    // ����ƶ���������
    public float mouseSensitivity;

    private Vector2 previousMousePosition;

    private void OnEnable()
    {
        isRadialMenuActive = true;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("cursor");
    }
    

    // Update is called once per frame
    void LateUpdate()
    {   
        
        previousMousePosition = Input.mousePosition;
        
        if (isRadialMenuActive)
        {
            // ��ȡ�����ˮƽ�ʹ�ֱ�����ϵ�ԭʼ�ƶ�����
            float mouseX = Input.GetAxisRaw("Mouse X");
            float mouseY = Input.GetAxisRaw("Mouse Y");

            // ���������ȵ�������ƶ�����
            mouseX *= mouseSensitivity;
            mouseY *= mouseSensitivity;

            // �������λ��
            Vector2 mousePosition = previousMousePosition + new Vector2(mouseX, mouseY);

            Vector2 delta = mousePosition - (Vector2)center.position;
            float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            angle += 180;

            if (angle < 0)
            {
                angle += 360;
            }

            int currentbuild = Mathf.FloorToInt(angle / 45);
            int adjustedIndex = (8 - currentbuild) % 8;

            selectObject.eulerAngles = new Vector3(0, 0, currentbuild * 45);
            // HighlightBuildName.text = InventoryBuildName[adjustedIndex];

            for (int i = 0; i < itemSlots.Length; i++)
            {
                itemSlots[i].localScale = Vector3.one;
            }

            itemSlots[adjustedIndex].localScale = new Vector3(1.2f, 1.2f, 1.2f);
            HighlightBuildName.text = constructionTypes[adjustedIndex].ToString();
            if (Input.GetMouseButtonUp(1))
            {
                // selectedBuildName.text = InventoryBuildName[adjustedIndex];
                ConstructionController.Instance.currentType = constructionTypes[adjustedIndex];
                Debug.Log("Selected Build Name: " + constructionTypes[adjustedIndex]);
                Cursor.lockState = CursorLockMode.Locked;
                gameObject.SetActive(false);
            }

            previousMousePosition = mousePosition;
        }
    }
}


