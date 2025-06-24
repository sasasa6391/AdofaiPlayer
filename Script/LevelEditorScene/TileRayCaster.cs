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
        // 마우스 왼쪽 버튼 클릭을 감지
        if (Input.GetMouseButtonDown(0))
        {
            // 마우스 위치에서 화면을 향해 레이캐스트 발사
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            // 레이캐스트가 Collider를 감지했는지 확인
            if (hit.collider != null)
            {
                // 레이캐스트에 맞은 물체의 GameObject 가져오기
                GameObject hitObject = hit.collider.gameObject;

                // 맞은 물체에 특정 스크립트가 있다면 메서드 호출
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
