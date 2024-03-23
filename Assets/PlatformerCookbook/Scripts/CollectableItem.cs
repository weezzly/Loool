using UnityEngine;
using UnityEngine.Events;

public class CollectableItem : MonoBehaviour
{
    public UnityEvent action;
    public bool destroyAfterCollected = true;
    
    private void OnTriggerEnter(Collider other)
    {
        var controller = other.GetComponent<PlatformerCharacterController>();
        if (!controller) return;

        action.Invoke();
        
        if (!destroyAfterCollected) return;
        Destroy(gameObject);
    }
}
