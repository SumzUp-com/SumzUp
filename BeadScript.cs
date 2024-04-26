using UnityEngine;
using System.Collections.Generic;

public class BeadScript : MonoBehaviour
{
    public Numby Numby;
    private bool _isDragging = false;
    private Rigidbody2D _rigidbody;
    private Dictionary<int, GameObject> _fingerIdToGameObjectMap = new Dictionary<int, GameObject>();
    private Vector3 _startPos;
    public Vector3 _currentPos;
    public Vector3 _pointPos;
    private Vector3 _pointPostwo;
    public Vector3 startPosition;
    private float _distanceThreshold = 2f; // Move to point pos
    private float _distanceThresholdtwo = 65f; // Set bead value
    private float _distanceThresholdthree = 78f; // Move to start pos
    public int gameObjectValue = 0;
    private float gameObjectValuestart;
    private string _gameObjectName;
    public AudioClip upSound;
    public AudioClip downSound;
    public AudioSource audioSource;
    public Vector3 PointPos { get { return _pointPos; } }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _gameObjectName = gameObject.name;
        _startPos = _rigidbody.position;
        startPosition = _rigidbody.position;
        _pointPos = new Vector3(_startPos.x, _startPos.y + 80, _startPos.z);
        _pointPostwo = new Vector3(_startPos.x, _startPos.y - 80, _startPos.z);
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        HandleTouches();
        MoveBead();
        SetBeadValue();
        PreventGhostTouches();
    }

    private void HandleTouches()
    {
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);

                if (touch.phase == TouchPhase.Began)
                {
                    BoxCollider2D collider = GetComponent<BoxCollider2D>();
                    if (collider.OverlapPoint(touchPos))
                    {
                        HandleTouchBegin(touch);
                    }
                }

                if (_fingerIdToGameObjectMap.ContainsKey(touch.fingerId))
                {
                    HandleTouchMove(touch);
                    if (touch.phase == TouchPhase.Ended)
                    {
                        HandleTouchEnd(touch);
                    }
                }
            }
        }
    }

    private void HandleTouchBegin(Touch touch)
    {
        _fingerIdToGameObjectMap[touch.fingerId] = gameObject;
        _isDragging = true;
        gameObjectValuestart = gameObjectValue;
        Numby.numbyup.enabled = false;
        Numby.numbyside.enabled = true;
        Numby.numbydown.enabled = false;
        Numby.numbysmile.enabled = false;
    }

    private void HandleTouchMove(Touch touch)
    {
        // check if the GameObject is being dragged and the GameObject is named '1 Bead'
            if (_gameObjectName == "1 Bead" || _gameObjectName == "10 Bead" || _gameObjectName == "100 Bead" || _gameObjectName == "1000 Bead")
            {
                // check if the fingerId is in the dictionary and corresponds to this game object
                foreach (var fingerId in _fingerIdToGameObjectMap.Keys)
                {
                    if (_fingerIdToGameObjectMap[fingerId] == gameObject)
                    {
                        if (Input.touchCount > 0 && fingerId < Input.touchCount && _fingerIdToGameObjectMap[fingerId] == gameObject)
                        {
                            Vector3 screenPos = touch.position;
                            screenPos.z = 10.0f;
                            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
                            _currentPos = new Vector3(worldPos.x, worldPos.y, 0f);

                            // check if the GameObject is within the max distance of 80px from its starting position and above it's starting position
                            if (Vector3.Distance(_startPos, _currentPos) > 80f && _currentPos.y > _startPos.y)
                            {
                                _currentPos = Vector3.MoveTowards(_startPos, _currentPos, 80f);
                            }
                            // if the gameobject goes below starting position, set it's position to the starting position
                            else if(_currentPos.y < _startPos.y)
                            {
                                _currentPos = _startPos;
                            }
                                _rigidbody.MovePosition(_currentPos);
                        }
                    }
                }
            }
        
        // check if the GameObject is being dragged and the GameObject is named '5 Bead'
            if (_gameObjectName == "5 Bead" || _gameObjectName == "50 Bead" || _gameObjectName == "500 Bead" || _gameObjectName == "5000 Bead")
            {
                // check if the fingerId is in the dictionary and corresponds to this game object
                foreach (var fingerId in _fingerIdToGameObjectMap.Keys)
                {
                    if (Input.touchCount > 0 && fingerId < Input.touchCount && _fingerIdToGameObjectMap[fingerId] == gameObject)
                    {
                        Vector3 screenPos = touch.position;
                        screenPos.z = 10.0f;
                        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
                        _currentPos = new Vector3(worldPos.x, worldPos.y, 0f);

                        // check if the GameObject is within the max distance of 80px from its starting position and above it's starting position
                        if (Vector3.Distance(_startPos, _currentPos) > 80f && _currentPos.y > _startPos.y)
                        {   
                            _currentPos = Vector3.MoveTowards(_startPos, _currentPos, 80f);
                        }
                            _rigidbody.MovePosition(_currentPos);
                    }
                }
            }
    }

    private void HandleTouchEnd(Touch touch)
    {
        switch (_gameObjectName)
            {
            case "5 Bead":
            case "50 Bead":
            case "500 Bead":
            case "5000 Bead":
                // check if the touch has ended, and if the 5 bead is in the '0' position
                if (gameObjectValuestart == 0)
                {
                List<int> fingerIds = new List<int>(_fingerIdToGameObjectMap.Keys);
                for (int i = 0; i < fingerIds.Count; i++)
                {
                    _currentPos = _rigidbody.position;
                    float distance = Vector3.Distance(_startPos, _currentPos);
                        if (distance < _distanceThreshold)
                        {
                            _rigidbody.MovePosition(_startPos);
                        }
                        else
                        {
                            _rigidbody.MovePosition(_pointPostwo);
                            audioSource.clip = upSound;
                            audioSource.Play();
                        }
                }
                }

                // check if the touch has ended, and if the 5 bead is in the '5' position
                if (gameObjectValuestart == 5 || gameObjectValuestart == 50 || gameObjectValuestart == 500 || gameObjectValuestart == 5000)
                {
                List<int> fingerIds = new List<int>(_fingerIdToGameObjectMap.Keys);
                for (int i = 0; i < fingerIds.Count; i++)
                {
                            _currentPos = _rigidbody.position;
                            float distance = Vector3.Distance(_startPos, _currentPos);
                            if (distance < _distanceThresholdthree)
                            {
                                _rigidbody.MovePosition(_startPos);
                                audioSource.clip = downSound;
                                audioSource.Play(); 
                            }
                            else
                            {
                                _rigidbody.MovePosition(_pointPostwo);
                            }
                }
                }
            break;
            case "1 Bead":
            case "10 Bead":
            case "100 Bead":
            case "1000 Bead":
                // check if the touch has ended, and if the 1 bead is in the '0' position
                if (gameObjectValuestart == 0)
                {
                List<int> fingerIds = new List<int>(_fingerIdToGameObjectMap.Keys);
                for (int i = 0; i < fingerIds.Count; i++)
                {
                            _currentPos = _rigidbody.position;
                            float distance = Vector3.Distance(_startPos, _currentPos);
                            if (distance < _distanceThreshold)
                            {
                                _rigidbody.MovePosition(_startPos);
                            }
                            else
                            {
                                _rigidbody.MovePosition(_pointPos);
                                audioSource.clip = upSound;
                                audioSource.Play();
                            }
                }
                }

                // check if the touch has ended, and if the 1 bead is in the '1' position
                if (gameObjectValuestart == 1 || gameObjectValuestart == 10 || gameObjectValuestart == 100 || gameObjectValuestart == 1000)
                {
                List<int> fingerIds = new List<int>(_fingerIdToGameObjectMap.Keys);
                for (int i = 0; i < fingerIds.Count; i++)
                {
                            _currentPos = _rigidbody.position;
                            float distance = Vector3.Distance(_startPos, _currentPos);
                            if (distance < _distanceThresholdthree)
                            {
                                _rigidbody.MovePosition(_startPos);
                                audioSource.clip = downSound;
                                audioSource.Play();                        
                            }
                            else
                            {
                                _rigidbody.MovePosition(_pointPos);
                            }
                }
                }
            break;
            }
        _fingerIdToGameObjectMap.Remove(touch.fingerId);
        _isDragging = false;
    }

    private void MoveBead()
    {
        // check if a gameobject is being pushed, and if so, move it in that direction
                if (_isDragging.Equals(false) && _rigidbody.velocity.y > 1)
                {
                    _rigidbody.MovePosition(_pointPos);
                    audioSource.clip = upSound;
                    audioSource.Play();   
                }
                else if (_isDragging.Equals(false) && _rigidbody.velocity.y < -1)
                {
                    _rigidbody.MovePosition(_startPos);
                    audioSource.clip = downSound;
                    audioSource.Play();   
                }
    }

    void SetBeadValue()
    {
    // assign points
        _currentPos = _rigidbody.position;
        float distancetwo = Vector3.Distance(_startPos, _currentPos);
        if (distancetwo > _distanceThresholdtwo)
        {
            switch (_gameObjectName)
            {
                case "5 Bead":
                case "5Bead":
                    gameObjectValue = 5;
                    break;
                case "1 Bead":
                case "1Bead1":
                case "1Bead2":
                case "1Bead3":
                case "1Bead4":
                    gameObjectValue = 1;
                    break;
                case "10 Bead":
                    gameObjectValue = 10;
                    break;
                case "50 Bead":
                case "50Bead":
                    gameObjectValue = 50;
                    break;
                case "100 Bead":
                    gameObjectValue = 100;
                    break;
                case "500 Bead":
                case "500Bead":
                    gameObjectValue = 500;
                    break;
                case "1000 Bead":
                    gameObjectValue = 1000;
                    break;
                case "5000 Bead":
                case "5000Bead":
                    gameObjectValue = 5000;
                break;
            }
        }
        else if (distancetwo < _distanceThresholdtwo)
        {
            gameObjectValue = 0;
        }
    }

    void PreventGhostTouches()
    {
        if (_fingerIdToGameObjectMap.Count != Input.touchCount && Input.touchCount == 0)
        {
            ClearTouch();
        }
    }

    public void ClearTouch()
    {
        _fingerIdToGameObjectMap.Clear();
    }
}