using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.UI;

public class RosterItem_ : MonoBehaviour
{
    public VivoxParticipant participant;
    public TMP_Text playerNameText;

    [SerializeField] Image chatStateImage;
    [SerializeField] Sprite mutedImage;
    [SerializeField] Sprite speakingImage;
    [SerializeField] Sprite notSpeakingImage;
    [SerializeField] Slider participantVolumeSlider;
    [SerializeField] Button muteButton;

    [SerializeField] int minSliderVolume;
    [SerializeField] int maxSliderVolume;

    readonly Color k_MutedColor = new Color(1, 0.3f, 0.3f, 1);

    private void UpdateChatStateImage()
    {
        if (participant.IsMuted)
        {
            chatStateImage.sprite = mutedImage;
            chatStateImage.gameObject.transform.localScale = Vector3.one;
        }
        else
        {
            if (participant.SpeechDetected)
            {
                chatStateImage.sprite = speakingImage;
                chatStateImage.gameObject.transform.localScale = Vector3.one;
            }
            else
            {
               chatStateImage.sprite = notSpeakingImage;
            }
        }
    }

    public void SetupRosterItem(VivoxParticipant participant)
    {
        this.participant = participant;
        playerNameText.text = participant.DisplayName;
        UpdateChatStateImage();

        participant.ParticipantMuteStateChanged += UpdateChatStateImage;
        participant.ParticipantSpeechDetected += UpdateChatStateImage;

        muteButton.onClick.AddListener(() =>
        {
            // If already muted, unmute, and vice versa.
            if (participant.IsMuted)
            {
                participant.UnmutePlayerLocally();
                muteButton.image.color = Color.white;
            }
            else
            {
                participant.MutePlayerLocally();
                muteButton.image.color = k_MutedColor;
            }
        });

        if (participant.IsSelf)
        {
            // Can't change our own participant volume, so turn off the slider
            participantVolumeSlider.gameObject.SetActive(false);
        }
        else
        {
            participantVolumeSlider.minValue = minSliderVolume;
            participantVolumeSlider.maxValue = maxSliderVolume;
            participantVolumeSlider.value = participant.LocalVolume;
            participantVolumeSlider.onValueChanged.AddListener((val) =>
            {
                OnParticipantVolumeChanged(val);
            });

            participantVolumeSlider.value = participant.LocalVolume;
        }
    }

    void OnParticipantVolumeChanged(float volume)
    {
        if (!participant.IsSelf)
        {
            participant.SetLocalVolume((int)volume);
        }
    }
}
