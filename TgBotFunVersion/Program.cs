using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class IsExternalInit { }
}
namespace TgBotFunVersion
{
    
    class Program
    {
        public static async void FunctionSendAnswer(string Answer, ITelegramBotClient botClient, long Id)
        {
            await botClient.SendTextMessageAsync(
            chatId: Id,
            text: Answer,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
            disableNotification: false);

        }
        private static TelegramBotClient client;
        private static ReceiverOptions receiverOptions;
        private static RegexForCheck RegexForCheck = new RegexForCheck();
        private static Person clientCred = new Person();
        
        static async Task Main()
        {
            ReadConfig settings = new ReadConfig();
            string token = settings.returnValue("token");
            client = new TelegramBotClient(token); // Присваиваем нашей переменной значение, в параметре передаем Token, полученный от BotFather
            receiverOptions = new ReceiverOptions // Также присваем значение настройкам бота
            {
                AllowedUpdates = new[] // Тут указываем типы получаемых Update`ов, о них подробнее расказано тут 
                {
                UpdateType.Message,
                UpdateType.CallbackQuery// Сообщения (текст, фото/видео, голосовые/видео сообщения и т.д.)
            },
                // Параметр, отвечающий за обработку сообщений, пришедших за то время, когда ваш бот был оффлайн
                // True - не обрабатывать, False (стоит по умолчанию) - обрабаывать
                ThrowPendingUpdates = true,
            };

            var cts = new CancellationTokenSource();

            // UpdateHander - обработчик приходящих Update`ов
            // ErrorHandler - обработчик ошибок, связанных с Bot API
            client.StartReceiving(UpdateHandler, ErrorHandler, receiverOptions, cts.Token); // Запускаем бота

            var me = await client.GetMeAsync(); // Создаем переменную, в которую помещаем информацию о нашем боте.
            Console.WriteLine($"{me.FirstName} запущен!");

            await Task.Delay(-1); // Устанавливаем бесконечную задержку, чтобы наш бот работал постоянно
        }
        private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            ReadConfig setting = new ReadConfig();

            DataBase dataBase = new DataBase(setting.returnValue("connectionString"));
            var LinqRequest = new ContextDb();
            // Обязательно ставим блок try-catch, чтобы наш бот не "падал" в случае каких-либо ошибок
            try
            {
                // Сразу же ставим конструкцию switch, чтобы обрабатывать приходящие Update
                switch (update.Type)
                {
                    case UpdateType.CallbackQuery:
                        {
                            var callbackQuery = update.CallbackQuery;

                            var userButton = callbackQuery.From;

                            var chatButton = callbackQuery.Message.Chat;

                            switch(callbackQuery.Data)
                            {
                                case "button1":
                                    {
                                        FunctionSendAnswer(LinqRequest.ReturnDebt(chatButton.Id).CreditSize.ToString(),
                                             botClient, chatButton.Id);

                                        return;
                                    }
                                case "button2":
                                    {
                                        FunctionSendAnswer(LinqRequest.ReturnDebt(chatButton.Id).DatePayment.ToString("dd.MM.yyyy"),
                                             botClient, chatButton.Id);
                                        return;
                                    }
                                case "button3":
                                    {
                                        FunctionSendAnswer(LinqRequest.ReturnDebt(chatButton.Id).SizePayment.ToString(),
                                             botClient, chatButton.Id);
                                        return;
                                    }
                                case "button4":
                                    {
                                        dataBase.UpdateLastAnswer(chatButton, "Погашение задолжности");
                                        FunctionSendAnswer("Введите сумму погашения",
                                             botClient, chatButton.Id);
                                        return;
                                    }
                            }
                            return;
                        }
                    case UpdateType.Message:
                        // эта переменная будет содержать в себе все связанное с сообщениями
                        var message = update.Message;
                        // From - это от кого пришло сообщение (или любой другой Update)
                        var user = message.From;
                        // Выводим на экран то, что пишут нашему боту, а также небольшую информацию об отправителе
                        //Console.WriteLine($"{user.FirstName} ({user.Id}) написал сообщение: {message.Text}");

                        // Chat - содержит всю информацию о чате
                        var chat = message.Chat;
                        dataBase.StateTable(message.Text, chat);
                        bool answerExist = LinqRequest.ExistIdOrNot(chat.Id);
                        bool debtAnswer  = LinqRequest.DebtOrNot(chat.Id);
                        List<string> state = new List<string>(dataBase.LastState(chat.Id.ToString()));
                        if (answerExist == true && debtAnswer == true) 
                        {
                            if (state[1].ToString() == "Погашение задолжности")
                            {
                                try
                                {
                                   string Answer = LinqRequest.UpdateCreditSize(chat.Id, Convert.ToDecimal(message.Text));
                                    if (Answer == "Оплата прошла успешно")
                                    {
                                        dataBase.UpdateLastAnswer(chat, "");
                                        FunctionSendAnswer(Answer,
                                                     botClient, chat.Id);
                                        return;
                                    }
                                    else
                                    {
                                        FunctionSendAnswer(Answer,
                                                     botClient, chat.Id);
                                        return;
                                    }
                                }
                                catch 
                                {
                                    FunctionSendAnswer("Введите сумму цифрами",
                                                 botClient, chat.Id);
                                    return;
                                }
                            }
                            // Тут создаем нашу клавиатуру
                            else
                            {
                                var inlineKeyboard = new InlineKeyboardMarkup(
                                    new List<InlineKeyboardButton[]>() // здесь создаем лист (массив), который содрежит в себе массив из класса кнопок
                                    {
                                        // Каждый новый массив - это дополнительные строки,
                                        // а каждая дополнительная строка (кнопка) в массиве - это добавление ряда

                                        new InlineKeyboardButton[] // тут создаем массив кнопок
                                        {
                                            InlineKeyboardButton.WithCallbackData("Проверить свою задолженность","button1"),
                                        },
                                        new InlineKeyboardButton[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("Узнать ближайшую дату платежа","button2"),
                                        },
                                        new InlineKeyboardButton[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("Узнать сумму платежа","button3"),
                                        },
                                        new InlineKeyboardButton[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("Внести месячную задолжность","button4"),
                                        }
                                    });
                                await botClient.SendTextMessageAsync(
                                        chat.Id,
                                        "Выберите нужное действие",
                                        replyMarkup: inlineKeyboard);
                                return;
                            }
                        }
                        else
                        {
                            switch (message.Type)
                            {
                                case MessageType.Text:
                                    {
                                        if (message.Text == "/start")
                                        {
                                            clientCred.IdTelegram = message.Chat.Id.ToString();
                                            FunctionSendAnswer("/Взять кредит\n",
                                             botClient, message.Chat.Id);
                                            return;
                                        }
                                        if (state[0].ToString() == "/Взять кредит")
                                        {
                                            dataBase.UpdateLastAnswer(chat, "Введите ваше имя");
                                            FunctionSendAnswer("Введите ваше имя", botClient, message.Chat.Id);
                                            return;
                                        }
                                        else if (state[1].ToString() == "Введите ваше имя")
                                        {
                                            if (RegexForCheck.CheckName(message.Text) == "Не корректный ввод")
                                            {
                                                FunctionSendAnswer("Не корректный ввод имени", botClient, message.Chat.Id);
                                                FunctionSendAnswer("Повторите попытку", botClient, message.Chat.Id);
                                                return;
                                            }
                                            else
                                            {
                                                clientCred.Name = message.Text;
                                                FunctionSendAnswer("Введите вашу фамилию", botClient, message.Chat.Id);
                                                dataBase.UpdateLastAnswer(chat, "Введите вашу фамилию");
                                                return;
                                            }

                                        }
                                        else if (state[1].ToString() == "Введите вашу фамилию")
                                        {
                                            if (RegexForCheck.CheckName(message.Text) == "Не корректный ввод")
                                            {
                                                FunctionSendAnswer("Не корректный ввод фамилии", botClient, message.Chat.Id);
                                                FunctionSendAnswer("Повторите попытку", botClient, message.Chat.Id);
                                                return;
                                            }
                                            else
                                            {
                                                clientCred.SecondName = message.Text;
                                                FunctionSendAnswer("Введите дату вашего рождения в формате дд.мм.гггг", botClient, message.Chat.Id);
                                                dataBase.UpdateLastAnswer(chat, "Введите дату вашего рождения в формате дд.мм.гггг");
                                                return;
                                            }
                                        }
                                        else if (state[1].ToString() == "Введите дату вашего рождения в формате дд.мм.гггг")
                                        {
                                            if (RegexForCheck.CheckDate(message.Text) == "Не корректный ввод даты рождения")
                                            {
                                                FunctionSendAnswer("Не корректный ввод даты рождения", botClient, message.Chat.Id);
                                                FunctionSendAnswer("Повторите попытку", botClient, message.Chat.Id);
                                                return;
                                            }
                                            else
                                            {
                                                clientCred.Birthday = message.Text;
                                                FunctionSendAnswer("Введите ваш номер телефона в формате 81234567890", botClient, message.Chat.Id);
                                                dataBase.UpdateLastAnswer(chat, "Введите ваш номер телефона");
                                                return;
                                            }

                                        }
                                        else if (state[1].ToString() == "Введите ваш номер телефона")
                                        {
                                            if (RegexForCheck.CheckNumber(message.Text) == "не корректный ввод номера")
                                            {
                                                FunctionSendAnswer("Не корректный ввод номера телефона", botClient, message.Chat.Id);
                                                FunctionSendAnswer("Повторите попытку", botClient, message.Chat.Id);
                                                return;
                                            }
                                            else
                                            {
                                                FunctionSendAnswer("Введите запасной номер телефона", botClient, message.Chat.Id);
                                                dataBase.UpdateLastAnswer(chat, "Введите запасной номер телефона");
                                                clientCred.Number = message.Text;
                                                return;
                                            }

                                        }
                                        else if (state[1].ToString() == "Введите запасной номер телефона")
                                        {
                                            if (RegexForCheck.CheckNumber(message.Text) == "не корректный ввод номера")
                                            {
                                                FunctionSendAnswer("Не корректный ввод запасного номера телефона", botClient, message.Chat.Id);
                                                FunctionSendAnswer("Повторите попытку", botClient, message.Chat.Id);
                                                return;
                                            }
                                            else
                                            {
                                                FunctionSendAnswer("Введите сумму кредита которую вы хотите взять", botClient, message.Chat.Id);
                                                dataBase.UpdateLastAnswer(chat, "Введите сумму кредита");
                                                clientCred.SecondNumber = message.Text;
                                                return;
                                            }
                                        }
                                        else if (state[1].ToString() == "Введите сумму кредита")
                                        {
                                            if (RegexForCheck.CheckMoney(message.Text) == "не коректный ввод денежной суммы")
                                            {
                                                FunctionSendAnswer("не коректный ввод денежной суммы", botClient, message.Chat.Id);
                                                FunctionSendAnswer("Повторите попытку", botClient, message.Chat.Id);
                                                return;
                                            }
                                            else
                                            {
                                                clientCred.CreditSize = Int64.Parse(message.Text);
                                                dataBase.UpdateLastAnswer(chat, "Введите на сколько месяцев вам нужен кредит");
                                                FunctionSendAnswer("Введите на сколько месяцев вам нужен кредит", botClient, message.Chat.Id);
                                                return;
                                            }
                                        }
                                        else if (state[1].ToString() == "Введите на сколько месяцев вам нужен кредит")
                                        {
                                            if (RegexForCheck.CheckMounth(message.Text) == "Срок кредита не может превышать 5 лет")
                                            {
                                                FunctionSendAnswer("Срок кредита не может превышать 5 лет", botClient, message.Chat.Id);
                                                FunctionSendAnswer("Введите меньший срок", botClient, message.Chat.Id);
                                                return;
                                            }
                                            else
                                            {
                                                clientCred.TimeCredMounth = Int32.Parse(message.Text);
                                                clientCred.DatePayment = DateTime.Now.AddMonths(+1);
                                                clientCred.InterestRate = "0,10";
                                                clientCred.Debt = 1;
                                                clientCred.CreditSize = (clientCred.CreditSize + (clientCred.CreditSize * Convert.ToDecimal(clientCred.InterestRate) * Math.Round(Convert.ToDecimal(clientCred.TimeCredMounth)/12, 0)));
                                                clientCred.SizePayment = Math.Round(clientCred.CreditSize / clientCred.TimeCredMounth, 0);
                                                LinqRequest.AddClient(clientCred);
                                                dataBase.UpdateLastAnswer(chat, "Клиент добавлен в базу данных");
                                                FunctionSendAnswer("Полная сумма с учетом процентов " + clientCred.CreditSize.ToString(), botClient, message.Chat.Id);
                                                FunctionSendAnswer("Ежемесячная сумма " + clientCred.SizePayment.ToString(), botClient, message.Chat.Id);
                                                FunctionSendAnswer("Дата следующего платежа " + (clientCred.DatePayment.AddMonths(+1).ToString("dd/MM/yyyy")), botClient, message.Chat.Id);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            FunctionSendAnswer("Не понимаю команды", botClient, message.Chat.Id);
                                            return;
                                        }     
                                    }
                                    
                            }
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                WriteLog writeSome = new WriteLog();
                writeSome.writeIN(ex.ToString());
                Console.WriteLine();
            }

        }
        private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
        {
            
            // Тут создадим переменную, в которую поместим код ошибки и её сообщение 
            var ErrorMessage = error switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => error.ToString()
            };
            return Task.CompletedTask;
        }
    }    
}
