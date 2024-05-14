using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ChakasBullyingAdventure;

class Program
{
    #region Commands from Telegram API

    private const string ChakaStatus = "chaka_satus";
    private const string ChakaAssBurned = "chaka_ass_burned";
    
    #endregion

    #region Configuration Parameters
    
    private const string TelegramBotToken = "TelegramBotToken";
    private const string AllowedIds = "AllowedIds";

    #endregion

    private static ChakaAssBurnedManager _assBurnedManager = new ChakaAssBurnedManager();
    private static string RageImageId = null;
    private static List<long> AllowedUserIds = new List<long>() {  };
    static async Task Main(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .AddJsonFile("appsettings.local.json")
               .Build();

        AllowedUserIds.AddRange(configuration.GetSection(AllowedIds).Get<long[]>());
               
        var botClient = new TelegramBotClient(configuration.GetSection(TelegramBotToken).Value);

        using CancellationTokenSource cts = new();

        // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
        };

        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        var me = await botClient.GetMeAsync();

        Console.WriteLine($"Start listening for @{me.Username}");
        await Task.Delay(-1);

        // Send cancellation request to stop bot
        cts.Cancel();
    }

    static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Only process Message updates: https://core.telegram.org/bots/api#message
        if (update.Message is not { } message)
            return;
        // Only process text messages
        if (message.Text is not { } messageText)
            return;

        var chatId = message.Chat.Id;

        Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

        if (messageText.Contains(ChakaStatus))
        {
            Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: _assBurnedManager.GetBurningMessage(),
                    cancellationToken: cancellationToken);
        }

        if (messageText.Contains(ChakaAssBurned))
        {
            if (message.From == null)
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Who the hell sent this message ffs",
                    cancellationToken: cancellationToken);

            }

            if (AllowedUserIds.Contains(message.From!.Id))
            {
                _assBurnedManager.AssBurned(DateTime.Now);
                
                if (RageImageId != null)
                {
                    Message sentImage = await botClient.SendPhotoAsync(
                       chatId: chatId,
                       photo: InputFile.FromFileId(RageImageId),
                       cancellationToken: cancellationToken);
                }
                else
                {
                    using FileStream fileStream = new FileStream("Assets/AAAAA.png", FileMode.Open);
                    Message sentImage = await botClient.SendPhotoAsync(
                        chatId: chatId,
                        photo: InputFile.FromStream(fileStream),
                        cancellationToken: cancellationToken);
                    RageImageId = sentImage.Photo!.Last().FileId;
                }
                
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "ДА ЯК ВИ МЕНЕ ДОЇБАЛИ БЛЯТЬ ААААААААААААААА",
                    cancellationToken: cancellationToken);
            }
            else
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Nonono mister fish you go to yobaniy tazik",
                    cancellationToken: cancellationToken);
            }
        }

    }

    static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}
