using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varjo.XR;
using System;


public class VarjoMarkerManager : MonoBehaviour
{
    public static bool detected = false;

    [Serializable]
    public struct TrackedObject
    {
        public long id;
        public GameObject gameObject;
        public bool dynamicTracking;
    }

    public float offSet;

    // An public array for all the tracked objects. 
    public TrackedObject[] trackedObjects = new TrackedObject[1];

    // A list for found markers.
    private List<VarjoMarker> markers = new List<VarjoMarker>();

    // A list for IDs of removed markers.
    private List<long> removedMarkerIds = new List<long>();
    

    // Start is called before the first frame update
    void Start()
    {
        // Enable Varjo Marker tracking.
        VarjoMarkers.EnableVarjoMarkers(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Check if Varjo Marker tracking is enabled and functional.
        if (VarjoMarkers.IsVarjoMarkersEnabled())
        {
            // Get a list of markers with up-to-date data.
            VarjoMarkers.GetVarjoMarkers(out markers);  //EL PARAMETRO OUT HACE QUE SE PASE POR REFERENCIA

            // Loop through found markers and update gameobjects matching the marker ID in the array.
            foreach (var marker in markers)
            {
                for (var i = 0; i < trackedObjects.Length; i++)
                {
                    if (trackedObjects[i].id == marker.id)
                    {
                        detected = true;
                        // This simple marker manager controls only visibility and pose of the GameObjects.
                        trackedObjects[i].gameObject.SetActive(true);
                        trackedObjects[i].gameObject.transform.localPosition = new Vector3(marker.pose.position.x, marker.pose.position.y - offSet, marker.pose.position.z);
                        //trackedObjects[i].gameObject.transform.localPosition.Set(marker.pose.position.x, offSet, marker.pose.position.z);

                        /****************************************************************************/
                        //trackedObjects[i].gameObject.transform.localRotation = marker.pose.rotation;
                        //Debug.Log("VarjoMarkerManager: Object placed over marker");
                        /**************EN ESTE CASO, LA ROTACIÓN DEL MARKER NO ES RELEVANTE***********/

                        // Set the marker tracking mode
                        if ((marker.flags == VarjoMarkerFlags.DoPrediction) != trackedObjects[i].dynamicTracking)
                        {
                            if (trackedObjects[i].dynamicTracking)
                            {
                                VarjoMarkers.AddVarjoMarkerFlags(marker.id, VarjoMarkerFlags.DoPrediction);
                            }
                            else
                            {
                                VarjoMarkers.RemoveVarjoMarkerFlags(marker.id, VarjoMarkerFlags.DoPrediction);
                            }
                        }
                    }
                }
            }

            // Get a list of IDs of removed markers.
            VarjoMarkers.GetRemovedVarjoMarkerIds(out removedMarkerIds);

            // Loop through removed marker IDs and deactivate gameobjects matching the marker IDs in the array.
            foreach (var id in removedMarkerIds)
            {
                for (var i = 0; i < trackedObjects.Length; i++)
                {
                    if (trackedObjects[i].id == id)
                    {
                        trackedObjects[i].gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    /*
     * private void OnEnable()
    {
        // Enable Varjo Marker tracking.
        VarjoMarkers.EnableVarjoMarkers(true);
    }

    private void OnDisable()
    {
        // Disable Varjo Marker tracking.
        VarjoMarkers.EnableVarjoMarkers(false);
    }
    */

    public void ManualEnable()
    {
        // Enable Varjo Marker tracking.
        VarjoMarkers.EnableVarjoMarkers(true);
    }

    public void ManualDisable()
    {
        // Enable Varjo Marker tracking.
        VarjoMarkers.EnableVarjoMarkers(false);
    }
}

