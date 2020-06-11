using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum GunMode
{
    RIFLE,
    PISTOL,
    KNIFE,
    GRENADE
}

enum FireMode
{
    FULL,
    SEMI
}

public class PlayerController : MonoBehaviourPun, IPunObservable {


    public GameObject PlayerUIPrefab;

    public float total_rifle_ammo_in_one_clip = 30f;
    public float total_rifle_ammo_all_clips = 3f;

    public float total_pistol_ammo_in_one_clip = 12f;
    public float total_pistol_ammo_all_clips = 3f;

    public float cur_clip = 30f;
    public float total_ammo = 36f;

    public float pistol_ammo = 11f;
    public float rifle_ammo = 90f;

    public float pistol_armor_dmg = 15;
    public float pistol_health_dmg = 10f;

    public float rifle_armor_dmg = 30f;
    public float rifle_health_dmg = 20f;

    public Image reload_image;

    public Image[] images;
    public int image_index = 0;
    public Transform spawn_point;

    public Text text_obj_health;
    public Text text_obj_armor;
    public Text text_obj_ammo;

    public float health = 100f;
    public float armor = 100f;

    public float walkSpeed = 2;
    public float runSpeed = 6;
    public float gravity = -12;
    public float jumpHeight = 1;
    [Range(0, 1)]
    public float airControlPercent;

    public float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;
    public Vector2 inputDir;
    public float speedSmoothTime = 0.1f;
    float speedSmoothVelocity;
    float currentSpeed;
    float velocityY;
    bool IsFiring;

    public bool start_shooting;

    private LineRenderer tracer;

    public float delta_rotation;

    Animator animator;
    public Transform cameraT;
    CharacterController controller;

    public static GameObject LocalPlayerInstance;

    GunMode gun_mode;
    FireMode fire_mode;
    
    CountdownScript full_auto_fire_rate;
    CountdownScript pistol_fire_rate;
    CountdownScript shotgun_fire_rate;
    //knife slash and grenade throw rate

    //same as playermanager script on photon tutorial page

    public bool hasArmor = true;

    bool startfire = false;

    public bool need_to_reload = false;

    public float pistol_cooldown = 1f;
    public float pistol_cooldown_new = 1f;
    public GameObject _uiGo;
    void Awake()
    {
        //animator = GetComponent<Animator>();
        if(photonView.IsMine)
        {
            PlayerController.LocalPlayerInstance = this.gameObject;
        }
        DontDestroyOnLoad(this.gameObject);
        cameraT = Camera.main.transform;
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        if (PlayerUIPrefab != null)
        {
            GameObject _uiGo = Instantiate(PlayerUIPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }
        else
        {
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
        }

        images[0] = _uiGo.transform.GetChild(0).GetComponent<Image>(); //GameObject.Find("RIFLE").GetComponent<Image>();
        images[1] = _uiGo.transform.GetChild(1).GetComponent<Image>();
        images[2] = _uiGo.transform.GetChild(2).GetComponent<Image>();
        images[3] = _uiGo.transform.GetChild(3).GetComponent<Image>();

       
        pistol_fire_rate = new CountdownScript(0.5f, 0.5f, 0.02f, 0.02f);
        full_auto_fire_rate = new CountdownScript(0.1f, 0.1f, 0.02f, 0.02f);
        cur_clip = 30;
        ThirdPersonCamera tpc = this.gameObject.GetComponent<ThirdPersonCamera>();

        if (tpc != null)
        {
            if (photonView.IsMine)
            {
                tpc.OnStartFollowing();
            }
        }
        else
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> missing TPC on playerprefab");
        }
        if (GetComponent<LineRenderer>())
        {
            tracer = GetComponent<LineRenderer>();
        }
        gun_mode = GunMode.RIFLE;
    }

    void OnLevelWasLoaded(int level)
    {

        _uiGo = Instantiate(this.PlayerUIPrefab);
        _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
    }

    void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        //methods
        States();

        need_to_reload = cur_clip <= 0;
        hasArmor = armor > 0;
    }

    void FixedUpdate()
    {
        /*if(!photonView.IsMine)
        {
            return;
        }*/
        if(photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        ProcessInputs();
        
    }

    void ProcessInputs()
    {
        // input
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"),0f, Input.GetAxisRaw("Vertical"));
        inputDir = input.normalized;

        bool running = Input.GetKey(KeyCode.LeftShift);
        
        if(input != Vector3.zero)
        {
            transform.Translate(input);
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {

        }
        // animator
        //float animationSpeedPercent = ((running) ? currentSpeed / runSpeed : currentSpeed / walkSpeed * .5f);
        // animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);

        if(need_to_reload)
        {
            Reload();
        }

        if (health <= 0f)
        {
            GameManager.Instance.LeaveRoom();
        }
    }

    private void allweaponstates()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            gun_mode = GunMode.RIFLE;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            gun_mode = GunMode.PISTOL;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            gun_mode = GunMode.KNIFE;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            gun_mode = GunMode.GRENADE;
        }
    }

    void States()
    {
        switch (gun_mode)
        {
            case GunMode.RIFLE:
                allweaponstates();
                break;
            case GunMode.PISTOL:
                allweaponstates();
                break;
            case GunMode.KNIFE:
                allweaponstates();
                break;
            case GunMode.GRENADE:
                allweaponstates();
                break;
            default:
                break;
        }

        if(gun_mode == GunMode.RIFLE)
        {
            Rifle();
            image_index = 0;
            fire_mode = FireMode.FULL;
        }

        if(gun_mode == GunMode.PISTOL)
        {
            Pistol();
            image_index = 1;
            fire_mode = FireMode.SEMI;
        }

        if(gun_mode == GunMode.GRENADE)
        {
            image_index = 3;
        }

        if(gun_mode == GunMode.KNIFE)
        {
            image_index = 2;
        }

        switch(image_index)
        {
            case 0:
                ClearColorIndex();
                images[image_index].GetComponentInChildren<Text>().color = Color.red;
                break;
            case 1:
                ClearColorIndex();
                images[image_index].GetComponentInChildren<Text>().color = Color.red;
                break;
            case 2:
                ClearColorIndex();
                images[image_index].GetComponentInChildren<Text>().color = Color.red;
                break;
            case 3:
                ClearColorIndex();
                images[image_index].GetComponentInChildren<Text>().color = Color.red;
                break;
        }

    }

    void ClearColorIndex()
    {
        for (int i = 0; i < images.Length; i++)
        {
            images[i].GetComponentInChildren<Text>().color = Color.black;
        }
    }

    void Shoot()
    {
        if (tracer)
        {
            print("shooting from tracer");
            StartCoroutine(RenderTracer());
            
        }
        if (gun_mode == GunMode.RIFLE)
        {
            cur_clip -= 1f;
        }
        if (gun_mode == GunMode.PISTOL)
        {
            cur_clip -= 1f;
        }
    }

    void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            print("reloading");
            switch(gun_mode)
            {
                case GunMode.RIFLE:
                    cur_clip = total_rifle_ammo_in_one_clip;
                    print("done reloading");
                    break;
                case GunMode.PISTOL:
                    print("done reloading");
                    cur_clip = total_pistol_ammo_in_one_clip;
                    break;
            }
            need_to_reload = false;
        }
    }

    void Rifle()
    {
        FireMode fireMode;
        fireMode = FireMode.FULL;

        RaycastHit hit;

        Debug.DrawRay(transform.position, transform.forward * 10f, Color.red);

        if (Input.GetMouseButton(0))
        {
            full_auto_fire_rate.DoAction(Shoot);
            
            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                //print("hit: " + hit.collider.gameObject.name);
                if (hit.collider.gameObject.layer == 12)
                {
                    print("hit: " + hit.collider.gameObject.name);
                    hit.transform.GetComponent<PhotonView>().RPC("ApplyDamage",RpcTarget.All , 10f);
                   /* if (hit.collider.gameObject.GetComponent<PlayerController>().hasArmor)
                    {
                        hit.collider.gameObject.GetComponent<PhotonView>().RPC("ApplyDamage", RpcTarget.Others, 10f, hit.collider.gameObject);
                        hit.collider.gameObject.GetComponent<PlayerController>().armor -= rifle_armor_dmg;
                    }*/
                    
                }
            }
            if (!IsFiring)
            {
                IsFiring = true;
            }
            print("rifle");
        }
        if (Input.GetButtonUp("Fire1"))
        {
            if (IsFiring)
            {
                IsFiring = false;
            }
        }
    }

    void Pistol()
    {        
        if(startfire)
        {
            print("is on cooldown");
            pistol_cooldown -= Time.deltaTime;
            if(pistol_cooldown <= 0f)
            {
                pistol_cooldown = pistol_cooldown_new;
                startfire = false;
            }
        }

        RaycastHit hit;
        
        if (Input.GetMouseButtonDown(0))
        {
            if(!startfire)
            {
                print("is no longer on cooldown");
                Shoot();
                if (Physics.Raycast(transform.position, transform.forward, out hit))
                {
                    if (hit.collider.gameObject.layer == 12)
                    {
                        if(hit.collider.gameObject.GetComponent<PlayerController>().hasArmor)
                        {
                            hit.collider.gameObject.GetComponent<PlayerController>().ApplyDamage(pistol_health_dmg);
                            hit.collider.gameObject.GetComponent<PlayerController>().armor -= pistol_armor_dmg;
                        }
                    }
                }
                if (!IsFiring)
                {
                    IsFiring = true;
                }
                print("pistol");
                startfire = true;
            }
        }
        if (Input.GetButtonUp("Fire1"))
        {
            if (IsFiring)
            {
                IsFiring = false;
            }
        }
    }

    void Knife()
    {

    }

    void Grenade()
    {

    }
    

    private void OnTriggerEnter(Collider other)
    {
        if(!photonView.IsMine)
        {
            return;
        }
        if(!other.name.Contains("Bullet"))
        {
            return;
        }

        health -= 10f;
    }

    private void OnTriggerStay(Collider other)
    {
        if(!photonView.IsMine)
        {
            return;
        }
    }

    [PunRPC]
    public void ApplyDamage(float damage)
    {
        health -= damage;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(IsFiring);
            
        }
        else
        {
            this.IsFiring = (bool)stream.ReceiveNext();
        }
    }

    IEnumerator RenderTracer()
    {
        tracer.enabled = true;
        tracer.SetPosition(0, spawn_point.position);
        tracer.SetPosition(1, spawn_point.position + transform.forward * 10f);
        yield return null;
        tracer.enabled = false;
    }
}
