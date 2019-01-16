using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTrigger : EventTrigger
{
    public string triggerTag;

    private void OnTriggerEnter(Collider collider) {
        if ((triggerTag == null || collider.CompareTag(triggerTag)) && TriggerEvent != null) {
            Debug.Log("Bork");
            TriggerEvent(this);
        }
    }
}
