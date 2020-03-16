using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //스피드 조정 변수
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float crouchSpeed;
    private float applySpeed; // 런스피드와 워크스피드 스위칭

    [SerializeField]
    private float jumpForce; //점프세기

    //상태 변수
    private bool isWalk = false;
    private bool isCrouch = false;
    private bool isRun = false;
    private bool isGround = true;



    //움직임 체크 변수
    private Vector3 lastPos;


    //앉을 때 얼마나 앉을지 결정하는 변수
    [SerializeField]
    private float crouchPosY; //얼마나 앉을지
    private float originPosY; //원래 높이
    private float applyCrouchPosY; //

    //플레이어와 땅의 착지를 확인하기 위한 컴포넌트 선언
    private CapsuleCollider capsuleColider;

    //민감도
    [SerializeField]
    private float lookSensitivity; //카메라 움직임의 민감도


    //카메라 한계

    [SerializeField]
    private float cameraRotationLimit; //카메라 회전 제한 -> 카메라가 360도 회전하면 안되기 때문에
    private float currentCameraRotationX = 0; //현재카메라  회전값;

    //필요한 컴포넌트
    [SerializeField]
    private Camera thecamera; //카메라 컴포넌트 추가
    private GunController theGunController;
    private Crosshair theCrosshair;
    private StatusController theStatusController;

    private Rigidbody myRigid;
    // Start is called before the first frame update
    void Start()
    {
        capsuleColider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
        theGunController = FindObjectOfType<GunController>(); //하이라키에 건컨트롤러 스크립트 있는거 싹다 뒤져서 찾아서 건컨트롤러에 넣어준다.
        theCrosshair = FindObjectOfType<Crosshair>();
        theStatusController = FindObjectOfType<StatusController>();

        //초기화.
        applySpeed = walkSpeed;
        originPosY = thecamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;
    }

  
    // Update is called once per frame$
    void Update() //1초에 대략 60번 실행되는 함수 , 매 프레임마다 호출
    {
        IsGround(); //땅에 닿는지 안 닿는지 체크
        TryJump(); //점프시도 스페이스바를 누른다.
        TryRun(); //쉬프트키를 누른다
        TryCrouch();
        Move(); //움직인다
        MoveCheck();

        if (!Inventory.inventoryActivated) 
        {
            CameraRotation(); //위아래 카메라 로테이션
            CharacterRotation(); //좌우 플레이어 로테이션
        }
        
    }

    private void TryCrouch()//앉기 시도
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }
    
    private void Crouch()//앉기 동작
    {
        isCrouch = !isCrouch;

        if (isWalk)
        {

            isWalk = false;

        }
        theCrosshair.CrouchingAnimation(isCrouch);


        if (isCrouch)
        {
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        }
        else
        {
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;

        }
        StartCoroutine(CrouchCoroutine());
    }

    IEnumerator CrouchCoroutine() //병렬처리 cpu가 처리를 왔다갔다 빠르게하면서 병렬처리 하는 것 처럼 보임. 아직 유니티는 다중 처리를 지원하지 않는다. 부드러운 앉기 동작
    {
        float _PosY = thecamera.transform.localPosition.y;
        int count=0;
        while(_PosY != applyCrouchPosY)
        {
            count++;
            _PosY = Mathf.Lerp(_PosY, applyCrouchPosY, 0.3f); //선형 보간 출발지, 목적지, 속도, 높을수록 빨리 증가
            thecamera.transform.localPosition = new Vector3(0, _PosY, 0); //코루틴을 쓰지 않아서 한프레임의 대기 없이 실행되었으면 엄청 빨리실행되어서 부드러운 앉기가 되지 않음;

            if (count > 15)
                break;

            yield return null;// 한프레임 대기로 인해서 부드러운 동작 가능

        }

        thecamera.transform.localPosition = new Vector3(0, applyCrouchPosY, 0);
    }
    
    private void IsGround() //지면 체크 
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleColider.bounds.extents.y + 0.2f);
        theCrosshair.JumpingAnimation(!isGround);
        //transform.position에서, vector3.down의 방향으로 capsuleColider.bounds.extents.y의 길이만큼 쏜다. 
        //bounds는  캡슐콜라이더의 전체, extends는 콜라이더의 절반,즉 y의 값을 콜라이더의 절반만큼 밑으로 쏜다
    } 
    
    
    //점프시도
    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround && theStatusController.GetCurrentSP() > 0)
        {
            AkSoundEngine.PostEvent("Player_Jump", gameObject);
            Jump();
        }
    }

    //점프 동작
    private void Jump()
    {
        if (isCrouch) //앉은 상태에서 점프시 앉은 상태 해제
            Crouch();
        theStatusController.DecreaseStamina(100);
        myRigid.velocity = transform.up * jumpForce;    //transform.up (0, 1, 0) velocity란? 속력??
    }

    //달리기 시도
    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift) && theStatusController.GetCurrentSP() > 0)
        {
            Running();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || theStatusController.GetCurrentSP() <= 0)
        {
            RunningCancel();
        }
    }

    //달리기 동작
    private void Running()
    {
        if (isCrouch)
            Crouch();

        theGunController.CancelFineSight();
        isRun = true;
        theCrosshair.RunningAnimation(isRun);
        theStatusController.DecreaseStamina(10);
        applySpeed = runSpeed;


    }

    //달리기 취소
    private void RunningCancel()
    {
        isRun = false;
        theCrosshair.RunningAnimation(isRun);

        applySpeed = walkSpeed;
    }
    private void Move() //함수 안에서 생성된 변수는 함수 호출이 끝나면 파괴되어 사라진다.
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal"); // Input.GetAxisRaw를 하면 0, 1, -1 값 리턴
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;
        //transform.right = (1, 0, 0)
        //transform.forward = (0, 0, 1)

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;

        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    }

    private void MoveCheck() // 움직이는지 안움직이는지 확인만!
    {
        if (!isRun && !isCrouch && isGround)
        {
            if (Vector3.Distance(lastPos, transform.position) >= 0.02f)
                isWalk = true;

            else
                isWalk = false;


            lastPos = transform.position;

        }

        if (isWalk)
        {
            //AkSoundEngine.PostEvent("Player_Walk", gameObject);
        }

        theCrosshair.WalkingAnimation(isWalk);

    }
    private void CharacterRotation() //상하 카메라 회전
    {
 
            float _yRotation = Input.GetAxisRaw("Mouse X");
            Vector3 _charactorRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
            myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_charactorRotationY));
            //Quaternion.Euler -> Rotation은 쿼터니언 값인데 오일러 값을 쿼터니언 값으로 변환.. 쿼터니언 * 쿼터니언
        

    }

  


    private void CameraRotation()
    {
        if (!pauseCameraRotation)
        {
            float _xRotation = Input.GetAxisRaw("Mouse Y");
            float _cameraRotationX = _xRotation * lookSensitivity;
            currentCameraRotationX -= _cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit); // 카메라 각도 제한

            thecamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f); // 쿼터니언인 로테이션  값을 오일러 값으로!
        }
    }

    private bool pauseCameraRotation = false;

    public IEnumerator TreeLookCoroutine(Vector3 _target)
    {
        pauseCameraRotation = true;


        Quaternion direction = Quaternion.LookRotation(_target - thecamera.transform.position);
        Vector3 eulerValue = direction.eulerAngles;

        float destinationX = eulerValue.x;

        while (Mathf.Abs(destinationX - currentCameraRotationX) >= 0.5f)
        {
            eulerValue = Quaternion.Lerp(thecamera.transform.localRotation, direction, 0.3f).eulerAngles;
            thecamera.transform.localRotation = Quaternion.Euler(eulerValue.x, 0f, 0f);
            currentCameraRotationX = thecamera.transform.localEulerAngles.x;

            yield return null;
        }

        pauseCameraRotation = false;
    }

 }
