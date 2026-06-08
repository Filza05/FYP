using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Vivox;
using UnityEngine;

public class RosterManager_ : MonoBehaviour
{
    public RosterItem_ rosterItemPrefab;
    List<RosterItem_> rosterObjects = new List<RosterItem_>();

    public void Awake()
    {
        VivoxService.Instance.ParticipantAddedToChannel += OnParticipantAdded;
        VivoxService.Instance.ParticipantRemovedFromChannel += OnParticipantRemoved;
    }

    private void OnDestroy()
    {
        VivoxService.Instance.ParticipantAddedToChannel -= OnParticipantAdded;
        VivoxService.Instance.ParticipantRemovedFromChannel -= OnParticipantRemoved;
    }

    private void OnParticipantAdded(VivoxParticipant participant)
    {
        if (participant.ChannelName != ClientSingleton.Instance.channelToJoin) { return; }

        RosterItem_ rosterItem_ = Instantiate(rosterItemPrefab, this.transform);
        rosterObjects.Add(rosterItem_);

        rosterItem_.SetupRosterItem(participant);

        AdjustRosterHeight();
    }

    private void OnParticipantRemoved(VivoxParticipant participant)
    {
        RemoveParticipant(participant);
    }

    void AdjustRosterHeight()
    {
        RectTransform rt = this.gameObject.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, rosterObjects.Count * 50);
    }

    private void RemoveParticipant(VivoxParticipant participant)
    {
        // Loop through all the children of the parent GameObject
        foreach (RosterItem_ child in rosterObjects)
        {
            if(participant == child.participant)
            {
                rosterObjects.Remove(child);
                Destroy(child.gameObject);

                AdjustRosterHeight();
                break;
            }
        }
    }
}
