using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Move")] [SerializeField] float _moveSpeed;
    [Header("Rotate")] [SerializeField] float _rotSpeed;
    [Header("Jump")] [SerializeField][Range(1,5)] float _jumpPower = 1;
    public int jumpState;
    float _gravity, _feetPos, _obstacleDist = 0; // 캐릭터가 서있을 y위치 캐싱변수
    const float Accell = 0.01f; // 고정 가속도, 타임델타는 고정된 값이 아니라 상수형태로 만들어둠
    // y = -x^2 + ax  =  -x(x - a)
    float t = 0;
    public bool _considering =false;
    float rotSpeed;
    Vector2 input,dir;
    void Start()
    {
        _feetPos = transform.position.y;
        jumpState = 0;
        Managers mng = Managers.INSTANCE;
        Managers._InputManger.POS = this.transform;
        Managers._InputManger.KeyAction -= OnKeyboad;
        Managers._InputManger.KeyAction += OnKeyboad;
        
    }
 
    public void OnKeyboad() // 키보드 입력으로 실행
    {
        CheckJump();

        rotSpeed = _rotSpeed;
        
        // 입력
         input = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        // 방향
         dir = input.normalized;
        
        //뒤로 가면서 좌우회전을할떄 배로 회전함 그래서 수정
        if (input.y < 0) { rotSpeed *= 0.25f; }

         this.transform.SetPositionAndRotation(
         this.transform.position + (transform.TransformDirection
         (new Vector3(0,0, input.y)) * dir.magnitude * _moveSpeed * Time.deltaTime)
         , (input.x == 0) ? this.transform.rotation : this.transform.rotation *
         Quaternion.Euler(Vector3.up * Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg * rotSpeed * Time.deltaTime)
         );
    }
    void CheckJump()
    {
        float y = transform.position.y ;  // 현재 캐릭터 높이값에서 추가 점프 계산을위해 

        if (jumpState == 0 && Input.GetKeyDown(KeyCode.Space))
        {
            _gravity = (_jumpPower * 0.1f);
            _obstacleDist = checkHight();
            jumpState = (_considering == false) ? 1 : 2 ; 
        }
        Jump(ref y , ref _obstacleDist);  //초기값이 있으면 ref 없으면 out으로 
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
  
    void Jump(ref float y, ref float obstacleDist)
    {
        switch (jumpState)
        {
            case 0:
                break;
            case 1:  // Jump Up+ 장애물없음
                t += _gravity;
                y += _gravity;
                if (_gravity <= 0)
                {
                    Debug.Log("t : "+t + " y :"+y); t = 0;
                    _feetPos = checkDown();
                    jumpState = 3;
                }
                else { _gravity -= Accell; }
                break;

            case 2:  // Jump Up + 장애물있음
                t += _gravity;
                y += _gravity;
                if ( y >= obstacleDist-0.1f)
                {
                    y = obstacleDist - Mathf.Epsilon;
                    _gravity = 0;
                    Debug.Log("t : " + t + " y :" + y); t = 0;
                    _feetPos = checkDown();
                    jumpState = 3;
                }
                else { _gravity -= Accell; }
                break;

            case 3:  // Jump Down 
                y += _gravity;
                if (y <= _feetPos) 
                {
                    if (checkDown() != _feetPos) // 한번더 체크
                    { 
                        _feetPos = checkDown(); // 땅위치 변경
                        break;
                    }
                    else { y = _feetPos; jumpState = 0;  }
                }
                else { _gravity -= Accell * 1.2f; }  // 내려올떄 더 빠르게              
                break;
        }
    }
    float checkDown()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 50f))
        {
            if (hit.transform.position.y + 1 != _feetPos) // 착지지점이 변했다면
                { return hit.transform.position.y + 1; } // 캐릭터의 feet은 캐릭터의 1아래에 있기에 1을 더해준값을 반환
        }
        return _feetPos;
    }
   
    float checkHight()
    {
        float f = (_jumpPower * 0.1f);
        float count = f;
        while (f > 0)
        {
            f -= Accell;
            count += f;
        }
        Debug.Log("count : "+count);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.up, out hit ,count  ))
        {
            if (hit.collider != null) // 머리위에 걸리는게 있음
            {
                Debug.Log(hit.transform.name);
                _considering = true;
                return (count);
            }
        }
        // 머리위에 걸리는게 없음
        _considering = false;
        return 0;
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
