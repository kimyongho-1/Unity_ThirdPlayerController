using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float _moveSpeed ,_rotSpeed;

    // Jump
    float y = 0.0f;
    float gravity = 0f;
    int direction = 0;
    const float jump_Speed = 0.2f;
    const float jump_accell = 0.01f;
    const float y_base = 1f;
    //

    Rigidbody _rb;
    public bool _isGround=true;
    float rotSpeed;
    Vector2 input,dir;
    void Start()
    {
        y = transform.position.y;

        _rb = GetComponent<Rigidbody>();
        Managers mng = Managers.INSTANCE;
        Managers._InputManger.POS = this.transform;
        Managers._InputManger.KeyAction -= OnKeyboad;
        Managers._InputManger.KeyAction += OnKeyboad;
     
    }
 
    public void OnKeyboad() // 키보드 입력으로 실행
    {
        Jump();


        rotSpeed = _rotSpeed;
        
        // 입력
         input = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        // 방향
         dir = input.normalized;
        
        //뒤로 가면서 좌우회전을할떄 배로 회전함 그래서 수정
        if (input.y < 0) { rotSpeed *= 0.25f; }
        

         this.transform.SetPositionAndRotation(
         this.transform.position + (transform.TransformDirection(new Vector3(0, 0, input.y)) * dir.magnitude * _moveSpeed * Time.deltaTime)
         , (input.x == 0) ? this.transform.rotation : this.transform.rotation *
         Quaternion.Euler(Vector3.up * Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg * rotSpeed * Time.deltaTime)
         );
    }
    void Jump()
    {
        JumpProcess();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DoJump();
        }
        Vector3 pos = transform.position;
        pos.y = y;
        transform.position = pos;
    }
    void DoJump() // 점프키 누를때 1회만 호출   
    { 
        direction = 1;       
        gravity = jump_Speed;   
    }
    void JumpProcess()
    {
        switch (direction)
        {
            case 0: // 2단 점프시 처리  
            {                
               //if (y > y_base)
               //{
               //  if (y >= jump_accell)         
               //  {                      
               //    y -= jump_accell;
               //    y -= gravity; 
               //  }                    
               //  else           
               //  {  y = y_base; }
               //}               
               break;         
            }

            case 1: // Up
            {  
                y += gravity;
                if (gravity <= 0.0f)
                { direction = 2; }
                else { gravity -= jump_accell; }
                    break;
            }

            case 2: //Down
            {
               y -= gravity;
               if (y > y_base)
               { gravity += jump_accell; }
               else { direction = 0; y = y_base; }
               break;
            }
        }
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
