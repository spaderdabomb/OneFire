using GogoGaga.OptimizedRopesAndCables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VHierarchy.Libs;
using GinjaGaming.FinalCharacterController;

public class FishingBob : MonoBehaviour
{
    [SerializeField] private int waterLayer;
    [SerializeField] private GameObject fishingLineAsset;

    [SerializeField] private float radius = 0.1f;
    [SerializeField] private float speed = 0.1f;

    private float angle = 0f;

    private GameObject _fishingLine = null;
    private Rope _fishingLineRope = null;
    private GameObject _activeItem = null;
    private Rigidbody _rb;

    private bool hitWater = false;
    private bool fishHooked = false;
    private Quaternion _originalRotation;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _originalRotation = transform.rotation;
    }

    private void OnEnable()
    {
        FishingManager.Instance.OnFishHooked += FishHooked;
        FishingManager.Instance.OnBobHitWater += BobHitWater;
    }

    private void OnDisable()
    {
        FishingManager.Instance.OnFishHooked -= FishHooked;
        FishingManager.Instance.OnBobHitWater -= BobHitWater;
    }

    private void Start()
    {
        _fishingLine = Instantiate(fishingLineAsset, GameObjectManager.Instance.effectsContainer.transform);
        _fishingLineRope = _fishingLine.GetComponent<Rope>();
        _activeItem = GameObjectManager.Instance.playerEquippedItem.ActiveItemObject;

        InitializeRope();
        GetDistanceFromFishingRod();
    }

    void Update()
    {
        GetDistanceFromFishingRod();

        if (fishHooked)
            MoveInCircle();
    }

    private void FixedUpdate()
    {
        if (fishHooked)
            MoveInCircle();
    }

    private void FishHooked(FishData fishData)
    {
        fishHooked = true;
        _rb.isKinematic = false;
        _rb.useGravity = false;
        GetComponent<SphereCollider>().enabled = false;
    }

    private void MoveInCircle()
    {
        angle += speed * Time.fixedDeltaTime;

        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        Vector3 targetPosition = new Vector3(x, 0f, z) + transform.position;
        Vector3 moveDirection = (targetPosition - _rb.position) / Time.fixedDeltaTime;
        _rb.linearVelocity = moveDirection;
    }
    
    public void Hide()
    {
        GetComponent<MeshRenderer>().enabled = false;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z);
    }

    public void BobHitWater(WorldLake worldLake)
    {
        _rb.isKinematic = true;
        transform.rotation = _originalRotation;
        transform.position = new Vector3(transform.position.x, worldLake.transform.position.y, transform.position.z);
    }

    private void InitializeRope()
    {
        if (_activeItem == null || _activeItem.GetComponent<FishingRodHeld>() == null)
        {
            Debug.Log("Trying to get distance of line to fishing rod but fishing rod not equipped");
            return;
        }

        FishingRodHeld fishingRodHeld;
        if (_activeItem.TryGetComponent<FishingRodHeld>(out fishingRodHeld))
        {
            _fishingLineRope.SetStartPoint(fishingRodHeld.bobSpawnLocation);
            _fishingLineRope.SetEndPoint(transform);
            _fishingLineRope.GetComponent<RopeWindEffect>().windDirectionDegrees = Random.Range(-180f, 180f);
        }
    }

    private void GetDistanceFromFishingRod()
    {
        if (_activeItem == null)
        {
            Debug.Log("Trying to get distance of line to fishing rod but fishing rod not equipped");
            return;
        }

        FishingRodHeld fishingRodHeld;
        if (_activeItem.TryGetComponent<FishingRodHeld>(out fishingRodHeld))
        {
            Vector3 distance = fishingRodHeld.bobSpawnLocation.position - transform.position;
            _fishingLineRope.ropeLength = distance.magnitude;
        }
    }

    private void OnDestroy()
    {
        Destroy(_fishingLine);
        _fishingLine = null;
        _fishingLineRope = null;
        _activeItem = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == waterLayer && !hitWater)
        {
            hitWater = true;

            WorldLake worldLake = other.gameObject.GetComponent<WorldLake>();
            BobHitWater(worldLake);
            FishingManager.Instance.BobHitWater(other.gameObject.GetComponent<WorldLake>());
        }
        else if (other.gameObject.layer != waterLayer)
        {
            FishingManager.Instance.StopFishing();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("Here");
        if (collision.gameObject.layer == waterLayer)
        {
            Debug.Log("colling with water layer - did you forget to set water collider to a trigger?");
        }
        else
        {
            FishingManager.Instance.StopFishing();
        }
    }

}
