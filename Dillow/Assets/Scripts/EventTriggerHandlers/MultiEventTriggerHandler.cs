using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

[EventHandlerInfo("Triggers",
    "Multi Event Trigger",
    "Executes the block if a given number of target event triggers publish their events.")]
public class MultiEventTriggerHandler : EventHandler
{
    public List<EventTrigger> triggers;

    [Tooltip("The number of the events that must be triggered for the handler to trigger." +
        " Anything less than zero means as many events must be triggered as there are event triggers.")]
    public int target = -1;
    private int current = 0;

    [Tooltip("Should each Event Trigger only contribute to triggering this handler once?")]
    public bool singleUseTriggers = true;

    [Tooltip("Can this handler be triggered multiple times?")]
    public bool multiTrigger = false;
    private bool triggered = false;

    void Start() {
        if (target < 0) {
            target = triggers.Count;
        }

        foreach (EventTrigger trigger in triggers) {
            trigger.TriggerEvent += EventTriggered;
        }
    }

    void EventTriggered(EventTrigger _trigger) {
        current += 1;
        if (current >= target) {
            current = 0;
            TargetMet();
        }
        if (singleUseTriggers) {
            _trigger.TriggerEvent -= EventTriggered;
        }
    }

    void TargetMet() {
        if (multiTrigger || !triggered) {
            triggered = true;
            ExecuteBlock();
        }
    }
}
