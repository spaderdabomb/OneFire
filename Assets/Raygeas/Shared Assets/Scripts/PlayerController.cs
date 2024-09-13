using System.Collections.Generic;
using UnityEngine;

/*Simple player movement controller, based on character controller component, 
with footstep system based on check the current texture of the component*/
namespace Raygeas
{
    public class PlayerController : MonoBehaviour
    {
        //Variables for footstep system list
        [System.Serializable]
        public class GroundLayer
        {
            public string layerName;
            public Texture2D[] groundTextures;
            public AudioClip[] footstepSounds;
        }

        [Header("Movement")]

        [Tooltip("Basic controller speed")]
        [SerializeField] private float walkSpeed = 5.0f;

        [Tooltip("Running controller speed")]
        [SerializeField] private float runMultiplier = 3.0f;

        [Tooltip("Force of the jump with which the controller rushes upwards")]
        [SerializeField] private float jumpForce = 5.0f;

        [Tooltip("Gravity, pushing down controller when it jumping")]
        [SerializeField] private float gravity = -9.81f;

        [Header("Mouse Look")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float mouseSensivity = 1.0f;
        [SerializeField] private float mouseVerticalClamp = 90.0f;

        [Header("Keybinds")]
        [SerializeField] private KeyCode jumpKey = KeyCode.Space;
        [SerializeField] private KeyCode runKey = KeyCode.LeftShift;


        [Header("Footsteps")]
        [Tooltip("Footstep source")]
        [SerializeField] private AudioSource footstepSource;
        [SerializeField][Range(0f, 1f)] private float walkFootstepVolume = 0.4f;
        [SerializeField][Range(0f, 1f)] private float runFootstepVolume = 0.8f;

        [Tooltip("Distance for ground texture checker")]
        [SerializeField] private float groundCheckDistance = 1.5f;

        [Tooltip("Footsteps playing rate")]
        [SerializeField][Range(1f, 4f)] private float footstepRate = 1f;

        [Tooltip("Footstep rate when player running")]
        [SerializeField][Range(1f, 4f)] private float runningFootstepRate = 1.5f;

        [Tooltip("Add textures for this layer and add sounds to be played for this texture")]
        public List<GroundLayer> groundLayers = new List<GroundLayer>();

        //Private movement variables
        private float _horizontalMovement;
        private float _verticalMovement;
        private float _currentSpeed;
        private Vector3 _moveDirection;
        private Vector3 _velocity;
        private CharacterController _characterController;
        private bool _isRunning;

        //Private mouselook variables
        private float _verticalRotation;
        private float _yAxis;
        private float _xAxis;
        private bool _activeRotation;

        //Private footstep system variables
        private Terrain[] _terrains;
        private Dictionary<Terrain, TerrainData> _terrainDataMap = new();
        private Dictionary<Terrain, TerrainLayer[]> _terrainLayersMap = new();
        private AudioClip _previousClip;
        private Texture2D _currentTexture;
        private RaycastHit _groundHit;
        private float _nextFootstep;

        private void Awake()
        {
            GetTerrainData();
            _characterController = GetComponent<CharacterController>();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        //Getting all terrain data for footstep system
        private void GetTerrainData()
        {
            _terrains = Terrain.activeTerrains;
            foreach (Terrain terrain in _terrains)
            {
                _terrainDataMap[terrain] = terrain.terrainData;
                _terrainLayersMap[terrain] = terrain.terrainData.terrainLayers;
            }
        }

        private void Update()
        {
            Movement();
            MouseLook();
            GroundChecker();
        }

        private void FixedUpdate()
        {
            SpeedCheck();
        }

        //Character controller movement
        private void Movement()
        {
            if (_characterController.isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }

            if (Input.GetKey(jumpKey) && _characterController.isGrounded)
            {
                _velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            }

            _horizontalMovement = Input.GetAxis("Horizontal");
            _verticalMovement = Input.GetAxis("Vertical");

            _moveDirection = transform.forward * _verticalMovement + transform.right * _horizontalMovement;

            _isRunning = Input.GetKey(runKey);
            _currentSpeed = walkSpeed * (_isRunning ? runMultiplier : 1f);
            _characterController.Move(_moveDirection * _currentSpeed * Time.deltaTime);

            _velocity.y += gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);

        }

        private void MouseLook()
        {
            _xAxis = Input.GetAxis("Mouse X");
            _yAxis = Input.GetAxis("Mouse Y");

            _verticalRotation += -_yAxis * mouseSensivity;
            _verticalRotation = Mathf.Clamp(_verticalRotation, -mouseVerticalClamp, mouseVerticalClamp);
            playerCamera.transform.localRotation = Quaternion.Euler(_verticalRotation, 0, 0);
            transform.rotation *= Quaternion.Euler(0, _xAxis * mouseSensivity, 0);
        }

        //Playing footstep sound when controller moves and grounded
        private void SpeedCheck()
        {
            if (_characterController.isGrounded && (_horizontalMovement != 0 || _verticalMovement != 0))
            {
                float currentFootstepRate = (_isRunning ? runningFootstepRate : footstepRate);

                if (_nextFootstep >= 100f)
                {
                    {
                        PlayFootstep();
                        _nextFootstep = 0;
                    }
                }
                _nextFootstep += (currentFootstepRate * walkSpeed);
            }
        }


        //Check where the controller is now and identify the texture of the component
        private void GroundChecker()
        {
            Ray checkerRay = new Ray(transform.position + (Vector3.up * 0.1f), Vector3.down);

            if (Physics.Raycast(checkerRay, out _groundHit, groundCheckDistance))
            {
                foreach (Terrain terrain in _terrains)
                {
                    if (_groundHit.collider.gameObject == terrain.gameObject)
                    {
                        _currentTexture = _terrainLayersMap[terrain][GetTerrainTexture(terrain, transform.position)].diffuseTexture;
                        break;
                    }
                }
                if (_groundHit.collider.GetComponent<Renderer>())
                {
                    _currentTexture = GetRendererTexture();
                }
            }
        }

        //Play a footstep sound depending on the specific texture
        private void PlayFootstep()
        {
            for (int i = 0; i < groundLayers.Count; i++)
            {
                for (int k = 0; k < groundLayers[i].groundTextures.Length; k++)
                {
                    if (_currentTexture == groundLayers[i].groundTextures[k])
                    {
                        footstepSource.PlayOneShot(RandomClip(groundLayers[i].footstepSounds));
                    }
                }
            }
        }

        //Returns an audio clip from an array, prevents a newly played clip from being repeated and randomize pitch
        private AudioClip RandomClip(AudioClip[] clips)
        {
            int attempts = 2;
            footstepSource.pitch = Random.Range(0.9f, 1.1f);

            if (_isRunning)
            {
                footstepSource.volume = runFootstepVolume;
            }
            else
            {
                footstepSource.volume = walkFootstepVolume;
            }

            AudioClip selectedClip = clips[Random.Range(0, clips.Length)];

            while (selectedClip == _previousClip && attempts > 0)
            {
                selectedClip = clips[Random.Range(0, clips.Length)];

                attempts--;
            }
            _previousClip = selectedClip;
            return selectedClip;
        }

        //Return an array of textures depending on location of the controller on terrain
        private float[] GetTerrainTexturesArray(Terrain terrain, Vector3 controllerPosition)
        {
            TerrainData terrainData = _terrainDataMap[terrain];
            Vector3 terrainPosition = terrain.transform.position;

            int positionX = (int)(((controllerPosition.x - terrainPosition.x) / terrainData.size.x) * terrainData.alphamapWidth);
            int positionZ = (int)(((controllerPosition.z - terrainPosition.z) / terrainData.size.z) * terrainData.alphamapHeight);

            float[,,] layerData = terrainData.GetAlphamaps(positionX, positionZ, 1, 1);

            float[] texturesArray = new float[layerData.GetUpperBound(2) + 1];
            for (int n = 0; n < texturesArray.Length; ++n)
            {
                texturesArray[n] = layerData[0, 0, n];
            }
            return texturesArray;
        }

        //Returns the zero index of the prevailing texture based on the controller location on terrain
        private int GetTerrainTexture(Terrain terrain, Vector3 controllerPosition)
        {
            float[] array = GetTerrainTexturesArray(terrain, controllerPosition);
            float maxArray = 0;
            int maxArrayIndex = 0;

            for (int n = 0; n < array.Length; ++n)
            {
                if (array[n] > maxArray)
                {
                    maxArrayIndex = n;
                    maxArray = array[n];
                }
            }
            return maxArrayIndex;
        }

        //Returns the current main texture of renderer where the controller is located now
        private Texture2D GetRendererTexture()
        {
            Texture2D texture;
            texture = (Texture2D)_groundHit.collider.gameObject.GetComponent<Renderer>().material.mainTexture;
            return texture;
        }
    }
}
