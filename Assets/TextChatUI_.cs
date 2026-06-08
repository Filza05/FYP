using System.Linq;
using System;
using System.Threading.Tasks;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using Michsky.UI.Heat;

public class TextChatUI_ : MonoBehaviour
{
    private IList<KeyValuePair<string, MessageObjectUI_>> m_MessageObjPool = new List<KeyValuePair<string, MessageObjectUI_>>();

    [SerializeField] TMP_InputField messageInputField;
    [SerializeField] ButtonManager sendMessageButton;
    [SerializeField] GameObject chatContentObj;
    [SerializeField] MessageObjectUI_ messageObj;
    [SerializeField] Toggle toggleTTS;
    [SerializeField] ButtonManager sendTTSMessageButton;

    private ScrollRect textChatScrollRect;
    private Task fetchMessages = null;
    private DateTime? oldestMessage = null;

    private void Awake()
    {
        Setup();
    }

    public void Setup()
    {
        VivoxService.Instance.ChannelJoined += OnChannelJoined;
        VivoxService.Instance.ChannelMessageReceived += OnChannelMessageReceived;
        VivoxService.Instance.ChannelMessageDeleted += OnChannelMessageDeleted;
        VivoxService.Instance.ChannelMessageEdited += OnChannelMessageEdited;

        sendMessageButton.onClick.AddListener(SendMessage);
        toggleTTS.onValueChanged.AddListener(TTSToggleValueChanged);
        sendTTSMessageButton.onClick.AddListener(SubmitTTSMessageToVivox);

        textChatScrollRect = GetComponent<ScrollRect>();
    }

    private void OnEnable()
    {
        StartCoroutine(SendScrollRectToBottom());
    }

    private void OnDestroy()
    {
        VivoxService.Instance.ChannelJoined -= OnChannelJoined;
        VivoxService.Instance.ChannelMessageReceived -= OnChannelMessageReceived;
        VivoxService.Instance.ChannelMessageDeleted -= OnChannelMessageDeleted;
        VivoxService.Instance.ChannelMessageEdited -= OnChannelMessageEdited;

        sendMessageButton.onClick.RemoveAllListeners();
        messageInputField.onEndEdit.RemoveAllListeners();
        sendTTSMessageButton.onClick.RemoveAllListeners();
        toggleTTS.onValueChanged.RemoveAllListeners();
    }

    async void SendMessage()
    {
        if (string.IsNullOrEmpty(messageInputField.text))
        {
            return;
        }

        await VivoxService.Instance.SendChannelTextMessageAsync(ClientSingleton.Instance.channelToJoin, messageInputField.text);
        ClearTextField();
    }

    private async Task FetchHistory(bool scrollToBottom = false)
    {
        try
        {
            var chatHistoryOptions = new ChatHistoryQueryOptions()
            {
                TimeEnd = oldestMessage
            };
            var historyMessages =
                await VivoxService.Instance.GetChannelTextMessageHistoryAsync(ClientSingleton.Instance.channelToJoin, 10,
                    chatHistoryOptions);
            var reversedMessages = historyMessages.Reverse();
            foreach (var historyMessage in reversedMessages)
            {
                AddMessageToChat(historyMessage, true, scrollToBottom);
            }

            // Update the oldest message ReceivedTime if it exists to help the next fetch get the next batch of history
            oldestMessage = historyMessages.FirstOrDefault()?.ReceivedTime;
        }
        catch (TaskCanceledException e)
        {
            Debug.Log($"Chat history request was canceled, likely because of a logout or the data is no longer needed: {e.Message}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Tried to fetch chat history and failed with error: {e.Message}");
        }
    }

    void AddMessageToChat(VivoxMessage message, bool isHistory = false, bool scrollToBottom = false)
    {
        var newMessageObj = Instantiate(messageObj, chatContentObj.transform);
        var newMessageTextObject = newMessageObj.GetComponent<MessageObjectUI_>();
        if (isHistory)
        {
            m_MessageObjPool.Insert(0, new KeyValuePair<string, MessageObjectUI_>(message.MessageId, newMessageTextObject));
            newMessageObj.transform.SetSiblingIndex(0);
        }
        else
        {
            m_MessageObjPool.Add(new KeyValuePair<string, MessageObjectUI_>(message.MessageId, newMessageTextObject));
        }

        newMessageTextObject.SetTextMessage(message);
        if (scrollToBottom && isActiveAndEnabled)
        {
            StartCoroutine(SendScrollRectToBottom());
        }

        if (!message.FromSelf)
        {
            if (toggleTTS.isOn)
            {
                VivoxService.Instance.TextToSpeechSendMessage($"{message.SenderDisplayName} said, {message.MessageText}", TextToSpeechMessageType.LocalPlayback);
            }
        }
    }

    private void OnChannelJoined(string obj)
    {
        fetchMessages = FetchHistory(true);
    }

    private void ClearTextField()
    {
        messageInputField.text = string.Empty;
        messageInputField.Select();
        messageInputField.ActivateInputField();
    }

    IEnumerator SendScrollRectToBottom()
    {
        yield return new WaitForEndOfFrame();

        textChatScrollRect.normalizedPosition = new Vector2(0, 0);
        yield return null;
    }

    void OnDirectedMessageReceived(VivoxMessage message)
    {
        AddMessageToChat(message, false, true);
    }

    private void OnChannelMessageReceived(VivoxMessage message)
    {
        AddMessageToChat(message, false, true);
    }

    private void OnChannelMessageDeleted(VivoxMessage message)
    {
        var editedMessage = m_MessageObjPool?.FirstOrDefault(x => x.Key == message.MessageId).Value;
        // If we have the message that's been deleted we will destroy it if not we do nothing.
        editedMessage?.SetTextMessage(message, true);
    }

    private void OnChannelMessageEdited(VivoxMessage message)
    {
        var editedMessage = m_MessageObjPool?.FirstOrDefault(x => x.Key == message.MessageId).Value;
        // If we have the message that's been edited we will update if not we do nothing.
        editedMessage?.SetTextMessage(message);
    }

    void TTSToggleValueChanged(bool toggleTTS)
    {
        if (!this.toggleTTS.isOn)
        {
            VivoxService.Instance.TextToSpeechCancelMessages(TextToSpeechMessageType.LocalPlayback);
        }
    }

    void SubmitTTSMessageToVivox()
    {
        if (string.IsNullOrEmpty(messageInputField.text))
        {
            return;
        }

        VivoxService.Instance.TextToSpeechSendMessage(messageInputField.text, TextToSpeechMessageType.RemoteTransmissionWithLocalPlayback);
        ClearTextField();
    }
}
