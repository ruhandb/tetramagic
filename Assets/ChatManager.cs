using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public string username;

    public int maxMessages = 50;

    public GameObject chatPanel, textObject;
    public InputField chatBox;
    public Color playerColor, infoColor;

    private FirebaseManager fbManager;
    [SerializeField]
    List<Text> messageList = new List<Text>();

    private void Awake()
    {
        fbManager = FindObjectOfType<FirebaseManager>();
        username = fbManager.Auth.CurrentUser.Email;
    }

    void Start()
    {
        fbManager.DB.GetReference("messages").OrderByChild("ts").LimitToLast(maxMessages).ChildAdded += ChatManager_ChildAdded;
        chatBox.interactable = true;
        SendMessageToChat($"{username} connected!", Message.MessageType.InfoMessage);
    }

    public void Logout()
    {
        SendMessageToChat($"{username} disconnected!", Message.MessageType.InfoMessage);
        fbManager.Auth.SignOut();
    }

    public void GoToScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    private void ChatManager_ChildAdded(object sender, ChildChangedEventArgs e)
    {
        if (e.Snapshot.Exists)
        {
            IDictionary value = (IDictionary)e.Snapshot.Value;
            Text newText = Instantiate(textObject, chatPanel.transform).GetComponent<Text>(); ;
            newText.text = (value["tp"].ToString() == "0" ? $"{value["user"]}: " : String.Empty) + value["txt"];
            newText.color = TypeToColor((Message.MessageType)Int32.Parse(value["tp"].ToString()));

            if (messageList.Count >= maxMessages)
            {
                Destroy(messageList[0].gameObject);
                messageList.RemoveAt(0);
            }
            var message = new Message();
            messageList.Add(newText.GetComponent<Text>());
        }
    }

    void Update()
    {
        if (chatBox.text != "")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendMessageToChat(chatBox.text, Message.MessageType.PlayerMessage);                
            }
        }
        else
        {
            if (!chatBox.isFocused && Input.GetKeyDown(KeyCode.Return))
            {
                chatBox.ActivateInputField();
            }
        }

        if (!chatBox.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
               SendMessageToChat("Space bar is pressed!!", Message.MessageType.InfoMessage);
            }             
        }
    }
    public void SendMessageToChat(string text)
    {
        SendMessageToChat(text, Message.MessageType.PlayerMessage);
    }

    public void SendMessageToChat(string text, Message.MessageType messageType)
    {
        if(text != "")
        {
            var messageRef = fbManager.DB.GetReference("messages");
            // Push a new document to the database
            var messageKey = messageRef.Push().Key;

            Message message = new Message
            {
                user = username,
                txt = text,
                tp = messageType,
                ts = ServerValue.Timestamp
            };

            messageRef.Child(messageKey)
                .SetRawJsonValueAsync(JsonUtility.ToJson(message), 1).ContinueWith(t => {
                    if (t.IsCompleted)
                    {
                        // Set the Firebase server timestamp on the datetime object
                        messageRef.Child(messageKey).UpdateChildrenAsync(new Dictionary<string, object> {
                            { "ts", ServerValue.Timestamp }
                        });
                    }
                }); 
            chatBox.text = "";
        }
    }

    Color TypeToColor(Message.MessageType type)
    {
        switch (type)
        {
            case Message.MessageType.PlayerMessage: return playerColor;
            case Message.MessageType.InfoMessage: return infoColor;
        }
        return Color.red;
    }
}

[System.Serializable]
public class Message
{
    public string user;
    public string txt;
    public object ts;
    public MessageType tp;

    public enum MessageType
    {
        PlayerMessage,
        InfoMessage
    }
}
