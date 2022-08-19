    /******************************************************************************
 * Copyright (C) Ultraleap, Inc. 2011-2020.                                   *
 *                                                                            *
 * Use subject to the terms of the Apache License 2.0 available at            *
 * http://www.apache.org/licenses/LICENSE-2.0, or another agreement           *
 * between Ultraleap and you, your company or other organization.             *
 ******************************************************************************/

using UnityEngine;

namespace Leap.Unity {

    /// <summary>
    /// Use this component on a Game Object to allow it to be manipulated by a pinch gesture.  The component
    /// allows rotation, translation, and scale of the object (RTS).
    /// </summary>
    public class LeapRTS : MonoBehaviour {

    [HideInInspector]
    public static bool RTSenabled = true;
    public enum RotationMethod {
      None,
      Single,
      Full
    }

    //***************************
    public GameObject floorPlane;
    public GameObject laserLineLeft;
    public GameObject laserLineRight;
    public float desfase= 1;

    private float initialAngle;
    //***************************

    //Mano Izquierda
    [SerializeField]
    private PinchDetector _pinchDetectorA;
    public PinchDetector PinchDetectorA {
      get {
        return _pinchDetectorA;
      }
      set {
        _pinchDetectorA = value;
      }
    }

    //Mano derecha
    [SerializeField]
    private PinchDetector _pinchDetectorB;
    public PinchDetector PinchDetectorB {
      get {
        return _pinchDetectorB;
      }
      set {
        _pinchDetectorB = value;
      }
    }

    [SerializeField]
    private RotationMethod _oneHandedRotationMethod;

    [SerializeField]
    private RotationMethod _twoHandedRotationMethod;

    [SerializeField]
    private bool _allowScale = true;

    [Header("GUI Options")]
    [SerializeField]
    private KeyCode _toggleGuiState = KeyCode.None;

    [SerializeField]
    private bool _showGUI = false;

    private Transform _anchor;

    private float _defaultNearClip;

    void Start() {
//      if (_pinchDetectorA == null || _pinchDetectorB == null) {
//        Debug.LogWarning("Both Pinch Detectors of the LeapRTS component must be assigned. This component has been disabled.");
//        enabled = false;
//      }


        GameObject pinchControl = new GameObject("RTS Anchor");
        laserLineLeft.SetActive(false);
        laserLineRight.SetActive(false);
        _anchor = pinchControl.transform;
        _anchor.transform.parent = transform.parent;
        transform.parent = _anchor;
    }

    void Update()
    {
        if (RTSenabled) { 
            if (Input.GetKeyDown(_toggleGuiState))
            {
                _showGUI = !_showGUI;
            }

            bool didUpdate = false;
            if (_pinchDetectorA != null)
                didUpdate |= _pinchDetectorA.DidChangeFromLastFrame;
            if (_pinchDetectorB != null)
                didUpdate |= _pinchDetectorB.DidChangeFromLastFrame;

            if (didUpdate)
            {
                transform.SetParent(null, true);
            }


            //*****DE LAS SIGUIENTES FUNCIONES, SOLO SE UTILIZA CUANDO SE PELLIZCA CON LAS DOS MANOS
            if (_pinchDetectorA != null && _pinchDetectorA.IsActive &&
                _pinchDetectorB != null && _pinchDetectorB.IsActive)
            {
                    transformDoubleAnchor(_pinchDetectorA, _pinchDetectorB);
                    laserLineLeft.SetActive(true);
                    laserLineRight.SetActive(true);
                    //transformDoubleAnchor();                                          //Se ejecutan las acciones cuando se pellizca con las dos manos
                }
            else if (_pinchDetectorA != null && _pinchDetectorA.IsActive && _pinchDetectorA.Position.y>0.90f)   //0.90f para que no detecte las manos en la cintura
            {
                //transformSingleAnchor(_pinchDetectorA);                           //Pellizco con mano izquierda
            }
            else if (_pinchDetectorB != null && _pinchDetectorB.IsActive && _pinchDetectorB.Position.y>0.90f)
            {
                //transformSingleAnchor(_pinchDetectorB);                           //Pellizco con mano derecha
            }
            else
            {
                laserLineLeft.SetActive(false);
                laserLineRight.SetActive(false);
                }
            //Debug.Log(_pinchDetectorA.Position.y);      
            //Debug con mano izquierda para saber altura

            if (didUpdate)
            {
                transform.SetParent(_anchor, true);
            }
        }
    }

    //Tonterias de un UI que sale en pantalla
    void OnGUI() {
      if (_showGUI) {
        GUILayout.Label("One Handed Settings");
        doRotationMethodGUI(ref _oneHandedRotationMethod);
        GUILayout.Label("Two Handed Settings");
        doRotationMethodGUI(ref _twoHandedRotationMethod);
        _allowScale = GUILayout.Toggle(_allowScale, "Allow Two Handed Scale");
      }
    }

    private void doRotationMethodGUI(ref RotationMethod rotationMethod) {
      GUILayout.BeginHorizontal();

      GUI.color = rotationMethod == RotationMethod.None ? Color.green : Color.white;
      if (GUILayout.Button("No Rotation")) {
        rotationMethod = RotationMethod.None;
      }

      GUI.color = rotationMethod == RotationMethod.Single ? Color.green : Color.white;
      if (GUILayout.Button("Single Axis")) {
        rotationMethod = RotationMethod.Single;
      }

      GUI.color = rotationMethod == RotationMethod.Full ? Color.green : Color.white;
      if (GUILayout.Button("Full Rotation")) {
        rotationMethod = RotationMethod.Full;
      }

      GUI.color = Color.white;

      GUILayout.EndHorizontal();
    }

    private void transformDoubleAnchor(PinchDetector singlePinchLeft, PinchDetector singlePinchRight)
    {
        Vector3 singlePinchPosition = new Vector3((singlePinchLeft.Position.x + singlePinchRight.Position.x)/2, 0, (singlePinchLeft.Position.z + singlePinchRight.Position.z) / 2);
        _anchor.position = singlePinchPosition*2f;

    }

    private void transformSingleAnchor(PinchDetector singlePinch) {
        Vector3 singlePinchPosition = new Vector3(singlePinch.Position.x, 0, singlePinch.Position.z);
        _anchor.position = singlePinchPosition;

        switch (_oneHandedRotationMethod)
        {
            case RotationMethod.None:
                break;
            case RotationMethod.Single:
                Vector3 p = singlePinch.Rotation * Vector3.right;
                p.y = _anchor.position.y;
                _anchor.LookAt(p);
                break;
            case RotationMethod.Full:
                _anchor.rotation = singlePinch.Rotation;
                break;
        }

        _anchor.localScale = Vector3.one;
    }

    /* ESTA FUNCION ES LA ANTIGUA DE TRANSOFRMDOUBLEANCHOR. LO QUE SE BUSCA ES QUE SE MUEVA AL PINCHAR CON LOS DOS Y ESTO NO OCURRIA
    private void transformDoubleAnchor() {        
        _anchor.position = transform.GetChild(0).transform.position;
        //_anchor.position = (_pinchDetectorA.Position + _pinchDetectorB.Position) / 2.0f;

        //Vector3 desfase = new Vector3(transform.GetChild(0).transform.position.x, 0, transform.GetChild(0).transform.position.z);

        switch (_twoHandedRotationMethod)
        {
            case RotationMethod.None:
                break;
            case RotationMethod.Single:
                Vector3 p = _pinchDetectorA.Position;
                p.y = _anchor.position.y;
                p.x = p.x * desfase;
                //p.z = p.z * desfase;
                //_anchor.LookAt(_anchor.position + (_pinchDetectorA.Position + _pinchDetectorB.Position));
                //_anchor.Rotate(Vector3.up, (0.036f*p.y)-initialAngle);
                _anchor.LookAt(p);
                break;
            case RotationMethod.Full:
                Quaternion pp = Quaternion.Lerp(_pinchDetectorA.Rotation, _pinchDetectorB.Rotation, 0.5f);
                Vector3 u = pp * Vector3.up;
                _anchor.LookAt(_pinchDetectorA.Position, u);
                break;
        }

        if (_allowScale)
        {
            _anchor.localScale = Vector3.one * Vector3.Distance(_pinchDetectorA.Position, _pinchDetectorB.Position);
        }

    }
    */
    }
}
