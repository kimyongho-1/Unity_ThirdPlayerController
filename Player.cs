using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float _moveSpeed ,_rotSpeed;
    public Transform _localCamera;
    void Start()
    {
        Managers mng = Managers.INSTANCE;
        Managers._InputManger.POS = this.transform;
        Managers._InputManger.KeyAction -= OnKeyboad;
        Managers._InputManger.KeyAction += OnKeyboad;
    }
   
    public void OnKeyboad() // 키보드 입력으로 실행
    {
        float rotSpeed = _rotSpeed;
        // 입력
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        // 방향
        Vector2 dir = input.normalized;
        
        //뒤로 가면서 좌우회전을할떄 배로 회전함 그래서 수정
        if (input.y < 0) { rotSpeed *= 0.25f; }
       
        this.transform.SetPositionAndRotation( 
            this.transform.position +transform.TransformDirection(new Vector3(0,0,input.y))*dir.magnitude * _moveSpeed * Time.deltaTime
            , (input.x == 0) ? this.transform.rotation : this.transform.rotation *
            Quaternion.Euler(Vector3.up * Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg * rotSpeed * Time.deltaTime)
            );
    }
    public void OnMouse()
    { 
        

    }
    public void OnMousePos() // 레이캐스트 사용
    {
        if (!Input.GetMouseButton(0)) { return; }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform != null)
            {
                // 목적지
                Vector3 dest = new Vector3(hit.point.x, this.transform.position.y, hit.point.z);
                // 방향각도
                Quaternion degree = Managers._Rot.Rot_Q4(this.transform, hit.point);

                StopAllCoroutines();
                StartCoroutine(Managers._InputManger.Moving(this.transform, dest, 5f));
                StartCoroutine(Managers._InputManger.Rotating(this.transform, degree, 2f));
            }
        }
    }
    
}
