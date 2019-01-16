using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

[EventHandlerInfo("Triggers",
    "Single Event Trigger",
    "Executes the block if a target event trigger publishes its event.")]
public class EventTriggerHandler : EventHandler
{
    public EventTrigger trigger;

    private void Start() {
        trigger.TriggerEvent += EventTriggered;
    }

    void EventTriggered(EventTrigger _trigger) {
        ExecuteBlock();
    }

}
