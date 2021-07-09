using System;
using System.Net;
using Telegram.Bot;
using OpenWeatherMap;



namespace TelegramBot
{
    class Program
    {
        static TelegramBotClient telegramBot;
        static OpenWeatherMapClient OWM;
        static string response;
        static bool start = true;
        static bool first = true;
        static bool sumFirstTime = true;
        static bool counting = false;
        static bool consultingWeather = false;
        static bool weatherFirstTime = true;
        static int count = 0;
        static void Main(string[] args)
        {
            telegramBot = new TelegramBotClient("1865326814:AAG7dLdFvtDWbeODRF_d41Q-UGvIuDLbRyU");
            OWM = new OpenWeatherMapClient("c8204d6fa61c168c63574d11620db8a1");
            telegramBot.OnMessage += _botClient_OnMessage;
            telegramBot.StartReceiving();
            Console.ReadKey();
            telegramBot.StopReceiving();
        }
        private async static void _botClient_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            {
                if(response != null)
                {
                    start = false;
                }
                response = e.Message.Text;
                if (first || response == "0")
                {
                    await telegramBot.SendTextMessageAsync(e.Message.Chat.Id, "Hola! \nPara ver el clima presiona 1 \nPara contar presiona 2");
                    first = false;
                    counting = false;
                    consultingWeather = false;
                    weatherFirstTime = true;
                    count = 0;
                }
                if (response == "1" || consultingWeather)
                {
                    consultingWeather = true;
                    if(weatherFirstTime)
                    {
                        await telegramBot.SendTextMessageAsync(e.Message.Chat.Id, "Ingrese la ciudad que quiere consultar \n para volver al menu presione 0.");
                        weatherFirstTime = false;
                    }
                    else {
                        if (response != "0")
                        {
                            CurrentWeatherResponse currentweather = null;
                            try
                            {
                                currentweather = await OWM.CurrentWeather.GetByName(response, language: OpenWeatherMapLanguage.SP);
                            }
                            catch (Exception ex)
                            {
                                await telegramBot.SendTextMessageAsync(e.Message.Chat.Id, "No pudimos procesar su solicitud.");
                            }
                            if (currentweather != null)
                            {
                                string weather = currentweather.Weather.Value;
                                await telegramBot.SendTextMessageAsync(e.Message.Chat.Id, "El tiempo esta: " + weather + "!");
                            }
                        }
                    }
                }
                else if(response == "2" || counting)
                {
                    counting = true;
                    if(sumFirstTime)
                    {
                        sumFirstTime = false;
                        await telegramBot.SendTextMessageAsync(e.Message.Chat.Id, "Ingrese cualquier tecla para contar \n para volver al menu presione 0.");
                    }
                    else
                    {
                        if (counting)
                        {
                            count++;
                            if (response != "0")
                            {
                                await telegramBot.SendTextMessageAsync(e.Message.Chat.Id, "El contador va: " + count);
                            }
                        }
                    }
                }
                else if(response != "2" && response != "1" && response != "0" && !start)
                {
                    await telegramBot.SendTextMessageAsync(e.Message.Chat.Id, "Opcion no valida.");
                }
            }
        }
    }
}







