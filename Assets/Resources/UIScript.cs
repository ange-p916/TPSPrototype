using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour {

    #region Private Fields

    public GameObject PlayerUI;

    private PlayerController target;

    [SerializeField]
    private Vector3 screenOffset = new Vector3(0f, 30f, 0f);

    float characterControllerHeight = 0f;
    Transform targetTransform;
    Renderer targetRenderer;
    CanvasGroup _canvasGroup;
    Vector3 targetPosition;

    #endregion

    public Text player_health;
    public Text player_ammo;
    public Text player_armor;

    public Image reload_image;

    public Text player_name;
    public Slider playerHealthSlider;

    #region MonoBehaviour Callbacks

    void Awake()
    {
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        _canvasGroup = this.GetComponentInChildren<CanvasGroup>();
        
        //target = FindObjectOfType<PlayerController>();
        player_ammo = transform.GetChild(6).GetComponentInChildren<Text>();
        player_armor = transform.GetChild(5).transform.GetChild(0).GetComponent<Text>();
        reload_image = transform.GetChild(7).GetComponent<Image>();
        PlayerUI = transform.Find("PlayerUI").gameObject;
    }

    void Start()
    {
        player_ammo.text = target.total_rifle_ammo_in_one_clip.ToString();
        reload_image.gameObject.SetActive(false);
    }

    private void Update()
    {
        reload_image.gameObject.SetActive(target.need_to_reload);
        
        if(player_health != null)
        {
            player_health.text = " " + target.health;
        }
        if(player_ammo != null)
        {
            player_ammo.text = " " + target.cur_clip;
        }
        if (player_armor != null)
        {
            player_armor.text = " " + target.armor;
        }

        if(playerHealthSlider != null)
        {
            playerHealthSlider.value = target.health;
        }

        if (target == null)
        {
            Destroy(this.gameObject);
            return;
        }
    }
    #endregion


    #region Public Methods
    public void SetTarget(PlayerController _target)
    {
        if (_target == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
            return;
        }
        // Cache references for efficiency
        target = _target;

        targetTransform = this.target.GetComponent<Transform>();
        targetRenderer = this.target.GetComponent<Renderer>();
        CharacterController characterController = _target.GetComponent<CharacterController>();

        if(characterController != null)
        {
            characterControllerHeight = characterController.height;
        }

        if (player_name != null)
        {
            player_name.text = target.photonView.Owner.NickName;
        }
    }

    #endregion

    void LateUpdate()
    {
        if(targetRenderer != null)
        {
            this._canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
        }

        if(targetTransform != null)
        {
            targetPosition = targetTransform.position;
            targetPosition.y += characterControllerHeight;
            PlayerUI.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
        }
    }

}
