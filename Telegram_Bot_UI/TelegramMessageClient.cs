using System;
using System.Linq;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Args;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace Telegram_Bot_UI
{
    class TelegramMessageClient
    {
        private MainWindow mainWindow; //Main Window reference

        static TelegramBotClient bot; //Bot itself

        public ObservableCollection<MessageLog> BotMessageLog { get; set; } //Chat message LIST

        public static List<MessageLog> botLogCopy = new List<MessageLog>(); //History LIST

        List<long> listOfId = new List<long>(); // ID LIST

        string savePath = @"history.json"; // Path to SVAE and LOAD History

        /// <summary>
        /// Bot connection
        /// </summary>
        /// <param name="window">Main Window reference</param>
        public TelegramMessageClient(MainWindow window)
        {
            this.mainWindow = window;
            this.BotMessageLog = new ObservableCollection<MessageLog>();


            string token = File.ReadAllText(@"TelegramBotToken.txt");

            #region Proxy;

            var proxy = new WebProxy()
            {
                Address = new Uri($"http://109.173.26.83:1080"),
                UseDefaultCredentials = false,
            };

            var httpClientHandler = new HttpClientHandler() { Proxy = proxy };

            HttpClient httpClient = new HttpClient(httpClientHandler);

            bot = new TelegramBotClient(token, httpClient);

            #endregion Proxy

            bot = new TelegramBotClient(token);

            OpenHistory();

            bot.OnMessage += MessageListener;

            bot.StartReceiving();
        }

        /// <summary>
        /// Method to do when Messages start
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessageListener(object sender, MessageEventArgs e)
        {
            string separator = "-------o-------";

            Debug.WriteLine(separator);

            string text = $"{DateTime.Now.ToLongTimeString()} : {e.Message.Chat.FirstName} : {e.Message.Chat.Id} : {e.Message.Text}";

            Debug.WriteLine($"{text} " +
                            $"\n {e.Message.Type.ToString()}");

            if (e.Message.Text == null) return;

            var messageTxt = e.Message.Text;

            mainWindow.Dispatcher.Invoke(() =>
            {
                BotMessageLog.Add(new MessageLog(DateTime.Now.ToLongTimeString(), 
                                                 e.Message.Chat.Id, 
                                                 e.Message.Chat.FirstName, 
                                                 messageTxt));
            });
        }

        /// <summary>
        /// Bot sending Messages to chosen User
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Id"></param>
        public void SendMessage(string Text, string Id)
        {
            long id = Convert.ToInt64(Id);
            bot.SendTextMessageAsync(id, Text);

            mainWindow.Dispatcher.Invoke(() =>
            {
                BotMessageLog.Add(new MessageLog(DateTime.Now.ToLongTimeString(), id, "BOT", Text));
            });
        }

        /// <summary>
        /// Saving History when Window is closed
        /// </summary>
        public void SaveHistory()
        {
            botLogCopy.AddRange(BotMessageLog);

            listOfId.Clear();

            int usersCount = 0;

            JObject history = new JObject();
            JArray allUsersJSON = new JArray();

            foreach (MessageLog message in botLogCopy)
            {
                if (!listOfId.Contains(message.ID))
                {
                    if (message.FirstName == "BOT")
                        continue;

                    JObject userJSON = new JObject();
                    userJSON["id"] = message.ID;
                    userJSON["name"] = message.FirstName;

                    listOfId.Add(message.ID);
                    allUsersJSON.Add(userJSON);
                    ++usersCount;
                }

                history["Users ID"] = allUsersJSON;
            }

            

            for (int i = 0; i < usersCount; i++)
            {
                JArray messagesJSON = new JArray();

                foreach (MessageLog message in botLogCopy)
                {
                    if (message.ID == listOfId[i])
                    {
                        if (message.FirstName == "BOT")
                            continue;

                        JObject messageJSON = new JObject();
                        messageJSON["time"] = message.Time;
                        messageJSON["message"] = message.Message;

                        messagesJSON.Add(messageJSON);                        
                    }
                }

                history["Users ID"][i]["Message"] = messagesJSON;            
            }

            string json = JsonConvert.SerializeObject(history);

            File.WriteAllText(savePath, json);
        }

        private void OpenHistory()
        {
            string json = File.ReadAllText(savePath);

            var usersJSON = JObject.Parse(json)["Users ID"].ToArray();

            for (int i = 0; i < usersJSON.Length; i++)
            {
                var userDataJson = JObject.Parse(json)["Users ID"][i]["Message"];

                foreach(var user in userDataJson)
                {
                    botLogCopy.Add(new MessageLog(user["time"].ToString(),
                                                     Convert.ToInt64(usersJSON[i]["id"]),
                                                     usersJSON[i]["name"].ToString(),
                                                     user["message"].ToString()));                        
                }
            }
        }    

        /// <summary>
        /// UPLOAD chosen Document to chosen User
        /// </summary>
        /// <param name="chatID"></param>
        /// <param name="filePath"></param>
        public static async void UploadDocument(long chatID, string filePath)
        {
            FileStream fileStream = File.OpenRead(filePath);

            InputOnlineFile inputOnlineFile = new InputOnlineFile(fileStream, filePath);
            await bot.SendDocumentAsync(chatID, inputOnlineFile);

            fileStream.Close();
            fileStream.Dispose();
        }
    }
}

