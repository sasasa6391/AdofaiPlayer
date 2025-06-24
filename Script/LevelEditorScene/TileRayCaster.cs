using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRayCaster : MonoBehaviour
{
    public Floor currentFloor;
    public bool isShift = false;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (currentFloor != null)
            {
                isShift = !isShift;
                currentFloor.SetEditorCanvas(isShift);
            }
        }
        // ���콺 ���� ��ư Ŭ���� ����
        if (Input.GetMouseButtonDown(0))
        {
            // ���콺 ��ġ���� ȭ���� ���� ����ĳ��Ʈ �߻�
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            // ����ĳ��Ʈ�� Collider�� �����ߴ��� Ȯ��
            if (hit.collider != null)
            {
                // ����ĳ��Ʈ�� ���� ��ü�� GameObject ��������
                GameObject hitObject = hit.collider.gameObject;

                // ���� ��ü�� Ư�� ��ũ��Ʈ�� �ִٸ� �޼��� ȣ��
                Floor floor = hitObject.GetComponent<Floor>();
                if (floor != null)
                {
                    if(currentFloor != null && floor != currentFloor)
                    {
                        currentFloor.OnRayCastExit();
                    }
                    currentFloor = floor;
                    isShift = false;
                    floor.OnRayCastEnter();
                }
                else
                {
                    currentFloor.OnRayCastExit();
                    currentFloor = null;
                }
            }
            else
            {
                if (currentFloor != null)
                {
                    currentFloor.OnRayCastExit();
                    currentFloor = null;
                }
            }
        }
    }
}
