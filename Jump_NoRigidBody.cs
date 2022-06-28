using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump_NoRigidBody : MonoBehaviour
{
    float t = 0; // 시간변수
    public float _jumpPower = 3.5f; // 점프강도이자 점프가 끝나기까지의 길이
    int state = 0;  // 점프상태
    public float _accel, _gravity = 3.5f; // 액셀 올라갈떄 추가가속도, 중력 내려갈떄 가속도
    public float _feetPos; // 캐릭터가 서있을 y위치 캐싱변수
    const float Accell = 0.02f; // 고정 가속도, 타임델타는 고정된 값이 아니라 상수형태로 만들어둠
    private void Start()
    {
        _feetPos = transform.position.y ;
    }
    // y = -x^2 + ax  =  -x(x - a)
    // 현재 높이 = 시간제곱 + 점프힘X시간
    // TO DO : 레이위아래로 쏴서, 발딪을 땅과 머리 부딪힌 땅잇는지 작성
    void CheckJump()
    {
        // 점프위로향할떄 머리위로 레이쏴서 장애물 판단
        // 점프 하강할떄 피트아래 레이쏴서 장애물 판단
        // 레이함수 체크 여기!!
        float yy = transform.position.y;  // 현재 캐릭터 높이값에서 추가 점프 계산을위해 

        if (state == 0 && Input.GetKeyDown(KeyCode.Space))
        {
            state = 1;
        }
        myJump(ref yy);  //초기값이 있으면 ref 없으면 out으로 
        transform.position = new Vector3(transform.position.x, yy, transform.position.z);
    }

    void myJump(ref float yy)
    {
        switch (state)
        {
            case 0:  // Stay
                break;

            case 1:  // Jump Up
                t += Accell * _accel;
                yy = -t * (t - _jumpPower);
                if (t >= _jumpPower * 0.5f) { Debug.Log(yy); state = 2; }
                break;
            case 2:  // Jump Down
                t += Accell * _gravity;
                yy = -t * (t - _jumpPower);
                if (yy <= _feetPos) { t = state = 0;  yy = _feetPos; }
                break;
        }
    }
}
