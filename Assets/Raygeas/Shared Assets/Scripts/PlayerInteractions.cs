using UnityEngine;
using UnityEngine.UI;

//Interacting with physics objects and interactive objects
namespace Raygeas
{
    public class PlayerInteractions : MonoBehaviour
    {
        [Header("Interaction variables")]
        [Tooltip("Maximum distance from player to object of interaction")]
        [SerializeField] private float interactionDistance = 3f;
        [Tooltip("The player's main camera")]
        [SerializeField] private Camera mainCamera;
        [Tooltip("Parent object where the object to be lifted becomes")]
        [SerializeField] private Transform pickupParent;

        [Header("Keybinds")]
        [Tooltip("Interaction key")]
        [SerializeField] private KeyCode interactionKey = KeyCode.E;

        [Header("Object Following")]
        [Tooltip("Minimum speed of the lifted object")]
        [SerializeField] private float minSpeed = 0;
        [Tooltip("Maximum speed of the lifted object")]
        [SerializeField] private float maxSpeed = 3000f;

        [Header("UI")]
        [Tooltip("Background object for text")]
        [SerializeField] private Image uiPanel;
        [Tooltip("Text holder")]
        [SerializeField] private Text panelText;
        [Tooltip("Text when an object can be lifted")]
        [SerializeField] private string itemPickUpText;
        [Tooltip("Text when an object can be drop")]
        [SerializeField] private string itemDropText;
        [Tooltip("Text when an interactive object can be opened")]
        [SerializeField] private string interactiveOpenText;
        [Tooltip("Text when an interactive object can be closed")]
        [SerializeField] private string interactiveCloseText;

        //Private variables.
        private PhysicsObject _physicsObject;
        private PhysicsObject _currentlyPickedUpObject;
        private PhysicsObject _lookObject;
        private Quaternion _lookRotation;
        private Rigidbody _pickupRigidBody;
        private Interactive _lookInteractive;
        private float _currentSpeed = 0f;
        private float _currentDistance = 0f;
        private CharacterController _characterController;
        private Collider _selection;


        private void Start()
        {
            mainCamera = Camera.main;
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            Interactions();
            LegCheck();
        }

        //Determine which object we are now looking at, depending on the component
        private void Interactions()
        {
            Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            if (_selection == null)
            {
                _lookInteractive = null;
                _lookObject = null;
            }

            if (Physics.Raycast(ray, out RaycastHit interactionHit, interactionDistance))
            {

                Collider selection = interactionHit.collider;

                if (selection.GetComponent<PhysicsObject>() != null)
                {
                    _lookObject = selection.gameObject.GetComponent<PhysicsObject>();
                    ShowItemUI();
                }
                else if (selection.GetComponent<Interactive>() != null)
                {
                    _lookInteractive = selection.gameObject.GetComponent<Interactive>();
                    ShowInteractiveUI();
                    if (Input.GetKeyDown(interactionKey))
                    {
                        _lookInteractive.PlayInteractiveAnimation();
                    }
                }
                selection = _selection;
            }

            if (_lookObject == null && _lookInteractive == null)
            {
                uiPanel.gameObject.SetActive(false);
            }

            if (Input.GetKeyDown(interactionKey))
                    {
                        if (_currentlyPickedUpObject == null && _lookObject != null)
                        {
                            PickUpObject();

                        }
                        else
                        {
                            BreakConnection();
                        }
                    }
        }

        //Disconnects from the object when the player attempts to step on the object, prevents flight on the object
        private void LegCheck()
        {
            Vector3 spherePosition = _characterController.center + transform.position;
            RaycastHit legCheck;
            if (Physics.SphereCast(spherePosition, 0.3f, Vector3.down, out legCheck, 2.0f))
            {
                if (legCheck.collider.GetComponent<PhysicsObject>())
                {
                    BreakConnection();
                }
            }
        }

        //Velocity movement toward pickup parent
        private void FixedUpdate()
        {
            if (_currentlyPickedUpObject != null)
            {
                _currentDistance = Vector3.Distance(pickupParent.position, _pickupRigidBody.position);
                _currentSpeed = Mathf.SmoothStep(minSpeed, maxSpeed, _currentDistance / interactionDistance);
                _currentSpeed *= Time.fixedDeltaTime;
                Vector3 direction = pickupParent.position - _pickupRigidBody.position;
                _pickupRigidBody.velocity = direction.normalized * _currentSpeed;
            }
        }

        //Picking up an looking object
        public void PickUpObject()
        {
            _physicsObject = _lookObject.GetComponentInChildren<PhysicsObject>();
            _currentlyPickedUpObject = _lookObject;
            _lookRotation = _currentlyPickedUpObject.transform.rotation;
            _pickupRigidBody = _currentlyPickedUpObject.GetComponent<Rigidbody>();
            _pickupRigidBody.constraints = RigidbodyConstraints.FreezeRotation;
            _pickupRigidBody.transform.rotation = _lookRotation;
            _physicsObject.playerInteraction = this;
            StartCoroutine(_physicsObject.PickUp());
        }

        //Release the object
        public void BreakConnection()
        {
            if (_currentlyPickedUpObject)
            {
                _pickupRigidBody.constraints = RigidbodyConstraints.None;
                _physicsObject.pickedUp = false;
                _currentlyPickedUpObject = null;
                _physicsObject = null;
                _lookObject = null;
                _currentDistance = 0;
            }
        }

        //Show interface elements when hovering over an object
        private void ShowInteractiveUI()
        {

            uiPanel.gameObject.SetActive(true);

            if (_lookInteractive.interactiveObjectOpen)
            {
                panelText.text = interactiveCloseText;
            }
            else
            {
                panelText.text = interactiveOpenText;
            }

        }

        private void ShowItemUI()
        {
            uiPanel.gameObject.SetActive(true);

            if (_currentlyPickedUpObject == null)
            {
                panelText.text = itemPickUpText;
            }
            else if (_currentlyPickedUpObject != null)
            {
                panelText.text = itemDropText;
            }
        }

    }
}