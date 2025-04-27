using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ArrowSpawner : MonoBehaviour
{
    public GameObject arrow;
    public GameObject notch;

    private XRGrabInteractable _bow;
    private bool _arrowNotched = false;
    private GameObject _currentArrow = null;

    private void Start() // Method to initialize the bow and subscribe to the event
    {
        _bow = GetComponent<XRGrabInteractable>();
        PullInteraction.PullActionReleased += NotchEmpty; 
    }

    private void OnDestroy() // Method to unsubscribe from the event when the object is destroyed
    {       
        PullInteraction.PullActionReleased -= NotchEmpty; 
    }

    void Update() // Method to check the state of the bow and arrow
     {
        if (_bow.isSelected && _arrowNotched == false)
        {
            _arrowNotched = true;
            StartCoroutine("DelayedSpawn");
        }
        if (!_bow.isSelected && _currentArrow != null)
        {
            Destroy(_currentArrow);
            NotchEmpty(1f); 
        }
    }

    private void NotchEmpty(float value) // Method to reset the arrow state 
    {
        _arrowNotched = false;
        _currentArrow = null;
    }
    IEnumerator DelayedSpawn() 
    {
        yield return new WaitForSeconds(0.1f);
        _currentArrow = Instantiate(arrow, notch.transform);

    }
}
