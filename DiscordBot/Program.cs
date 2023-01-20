using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using UbiServices;
using UbiServices.Public;

namespace DiscordBot
{
    internal class Program
    {
        public string token = "";
        public ulong guildId = 0;
        public static Task ReadKeyAsync()
        {
            Console.ReadLine();
            return Task.CompletedTask;
        }
#pragma warning disable 4014
        public static void Main() => new Program().AsyncMain();
#pragma warning restore 4014

#pragma warning disable 8618
        private DiscordSocketClient _client;
#pragma warning restore 8618
        public async Task AsyncMain()
        {
            if (!File.Exists("token.txt"))
            {
                Console.WriteLine("No token");
                return;
            }
            else
            {
                token = File.ReadAllLines("token.txt")[0];

            }

            if (!File.Exists("guild.txt"))
            {
                Console.WriteLine("No guild");
                return;
            }
            else
            {
                guildId = ulong.Parse(File.ReadAllLines("guild.txt")[0]);

            }

            _client = new DiscordSocketClient();
            _client.Log += Log;
            _client.Ready += Ready;
            _client.SlashCommandExecuted += SlashCommandExecuted;

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await ReadKeyAsync();
            await _client.StopAsync();
            Console.WriteLine("End");
        }
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }


        private async Task Ready()
        {
            SocketGuild guild = _client.GetGuild(guildId);
            if (guild == null) { Console.WriteLine("Failed to get guild ID!"); return; }

            List<SlashCommandProperties> vCmd = new();

            // Use this to remove old commands
            //await guild.DeleteApplicationCommandsAsync(new RequestOptions() { AuditLogReason = "Reset" });

            await _client.SetGameAsync("Uplay Failure", "", ActivityType.Watching);



            //https://discordnet.dev/guides/int_basics/application-commands/slash-commands/subcommands.html
            vCmd.Add(new SlashCommandBuilder()
            .WithName("info")
            .WithDescription("Print information about the Bot.")
            .Build()
            );
            vCmd.Add(new SlashCommandBuilder()
            .WithName("space")
            .WithDescription("Get space by SpaceId.")
            .AddOption("spaceid", ApplicationCommandOptionType.String, "The SpaceId.", true)
            .Build()
            );

            vCmd.Add(new SlashCommandBuilder()
            .WithName("spaceparam")
            .WithDescription("Get space parameters by SpaceId.")
            .AddOption("spaceid", ApplicationCommandOptionType.String, "The SpaceId.", true)
            .Build()
            );

            vCmd.Add(new SlashCommandBuilder()
            .WithName("sandbox")
            .WithDescription("Get space parameters by SpaceId.")
            .AddOption("spaceid", ApplicationCommandOptionType.String, "The SpaceId.", true)
            .Build()
            );

            vCmd.Add(new SlashCommandBuilder()
            .WithName("betas")
            .WithDescription("Get the Betas.")
            .AddOption("email", ApplicationCommandOptionType.String, "Your UBI Email", true)
            .AddOption("password", ApplicationCommandOptionType.String, "Your UBI Password", true)
            .Build()
            );

            vCmd.Add(new SlashCommandBuilder()
            .WithName("betasb64")
            .WithDescription("Get the Betas.")
            .AddOption("b64", ApplicationCommandOptionType.String, "Your b64 Stuff", true)
            .Build()
            );

            vCmd.Add(new SlashCommandBuilder()
            .WithName("betaphases")
            .WithDescription("Get the Betas.")
            .AddOption("email", ApplicationCommandOptionType.String, "Your UBI Email", true)
            .AddOption("password", ApplicationCommandOptionType.String, "Your UBI Password", true)
            .AddOption("betacode", ApplicationCommandOptionType.String, "Beta Code", true)
            .Build()
            );
            
            vCmd.Add(new SlashCommandBuilder()
            .WithName("betaphasesb64")
            .WithDescription("Get the Betas.")
            .AddOption("b64", ApplicationCommandOptionType.String, "Your b64 Stuff", true)
            .AddOption("betacode", ApplicationCommandOptionType.String, "Beta Code", true)
            .Build()
            );

            vCmd.Add(new SlashCommandBuilder()
            .WithName("betaphase")
            .WithDescription("Get the Betas.")
            .AddOption("email", ApplicationCommandOptionType.String, "Your UBI Email", true)
            .AddOption("password", ApplicationCommandOptionType.String, "Your UBI Password", true)
            .AddOption("betacode", ApplicationCommandOptionType.String, "Beta Code", true)
            .AddOption("phaseid", ApplicationCommandOptionType.String, "Beta Phase Code", true)
            .Build()
            );
            
            vCmd.Add(new SlashCommandBuilder()
            .WithName("betaphaseb64")
            .WithDescription("Get the Betas.")
            .AddOption("b64", ApplicationCommandOptionType.String, "Your b64 Stuff", true)
            .AddOption("betacode", ApplicationCommandOptionType.String, "Beta Code", true)
            .AddOption("phaseid", ApplicationCommandOptionType.String, "Beta Phase Code", true)
            .Build()
            );

            vCmd.Add(new SlashCommandBuilder()
            .WithName("store")
            .WithDescription("Get Store Info.")
            .AddOption("storeref", ApplicationCommandOptionType.String, "The store reference.", true)
            .Build()
            );

            vCmd.Add(new SlashCommandBuilder()
            .WithName("pingpong")
            .WithDescription("Pong")
            .Build()
            );

            foreach (var cmd in vCmd)
            {
                await guild.CreateApplicationCommandAsync(cmd);
            }
            Console.WriteLine("Slash commands successfully created!");


        }
        private async Task SlashCommandExecuted(SocketSlashCommand cmd)
        {
            var curDB = _client.CurrentUser;
            var embedBuiler = new EmbedBuilder()
            .WithAuthor(curDB.ToString().Split("#")[0], curDB.GetAvatarUrl() ?? curDB.GetDefaultAvatarUrl())
            .WithTitle("")
            .WithDescription("")
            .WithColor(Color.Green)
            .WithCurrentTimestamp();
            switch (cmd.Data.Name)
            {
                case "info":
                    embedBuiler.Title = "Information";
                    embedBuiler.Description = "I am not affiliated with Ubisoft or any trademark.\nI do not save any user information.";
                    await cmd.RespondAsync(embed: embedBuiler.Build());
                    break;
                case "space":
                    embedBuiler = getspace(embedBuiler, cmd);
                    await cmd.RespondAsync(embed: embedBuiler.Build(), ephemeral: true);
                    break;
                case "spaceparam":
                    var config = getspaceparam(cmd);
                    File.WriteAllText("tmp.json", config);
                    await cmd.RespondWithFileAsync("tmp.json", "spaces.json", "Your Space Parameters here:", ephemeral: true);
                    break;
                case "sandbox":
                    var sandbox = sandboxes(cmd);
                    File.WriteAllText("tmp.json", sandbox);
                    await cmd.RespondWithFileAsync("tmp.json", "sandbox.json", "Your Space Sandbox here:", ephemeral: true);
                    break;
                case "betas":
                    var betas = getbetas(cmd);
                    File.WriteAllText("tmp.json", betas);
                    await cmd.RespondWithFileAsync("tmp.json", "betas.json", "Your Betas here:", ephemeral: true);
                    break;
                case "betaphases":
                    var betacode = getbetaphases(cmd);
                    File.WriteAllText("tmp.json", betacode);
                    await cmd.RespondWithFileAsync("tmp.json", "betas.json", "Your Betas here:", ephemeral: true);
                    break;
                case "betaphase":
                    File.WriteAllText("tmp.json", getbetaphase(cmd));
                    await cmd.RespondWithFileAsync("tmp.json", "betas.json", "Your Betas here:", ephemeral: true);
                    break;
                case "betasb64":
                    var betas64 = getbetasb64(cmd);
                    File.WriteAllText("tmp.json", betas64);
                    await cmd.RespondWithFileAsync("tmp.json", "betas.json", "Your Betas here:", ephemeral: true);
                    break;
                case "betaphasesb64":
                    var betacode64 = getbetaphasesb64(cmd);
                    File.WriteAllText("tmp.json", betacode64);
                    await cmd.RespondWithFileAsync("tmp.json", "betas.json", "Your Betas here:", ephemeral: true);
                    break;
                case "betaphaseb64":
                    File.WriteAllText("tmp.json", getbetaphaseb64(cmd));
                    await cmd.RespondWithFileAsync("tmp.json", "betas.json", "Your Betas here:", ephemeral: true);
                    break;
                case "store":
                    File.WriteAllText("tmp.json", store(cmd));
                    await cmd.RespondWithFileAsync("tmp.json", "store.json", "Your Store here:", ephemeral: true);
                    break;
                case "pingpong":
                    embedBuiler = pong(embedBuiler, cmd);
                    await cmd.RespondAsync(embed: embedBuiler.Build(), ephemeral: true);
                    break;
                default:
                    break;
            }
            File.WriteAllText("tmp.json", "");
        }


        private EmbedBuilder getspace(EmbedBuilder embed, SocketSlashCommand cmd)
        {
            var desc = "";
            embed.Title = "Getting Spaces!";
            //Todo check for pattern
            var callback = V1.Spaces.GetSpaces((string)cmd.Data.Options.ToList()[0].Value);
            if (callback == null)
            {
                desc = "Nothing was recieved :(";
            }
            else
            {
                desc = JsonConvert.SerializeObject(callback, Formatting.Indented);
            }
            embed.Description = desc;
            return embed;
        }

        private string sandboxes(SocketSlashCommand cmd)
        {
            var desc = "";
            //Todo check for pattern
            var callback = V1.Spaces.GetSpaceSandbox((string)cmd.Data.Options.ToList()[0].Value);
            if (callback == null)
            {
                desc = "Nothing was recieved :(";
            }
            else
            {
                desc = JsonConvert.SerializeObject(callback, Formatting.Indented);
            }
            return desc;
        }

        private string store(SocketSlashCommand cmd)
        {
            var desc = "";
            //Todo check for pattern
            var callback = UbiServices.Store.Products.GetStoreFrontByProducts(countrycode: Enums.CountryCode.ie, new() { (string)cmd.Data.Options.ToList()[0].Value }, null, false);
            if (callback == null)
            {
                desc = "Nothing was recieved :(";
            }
            else
            {
                desc = JsonConvert.SerializeObject(callback, Formatting.Indented);
            }
            return desc;
        }

        private string getspaceparam(SocketSlashCommand cmd)
        {
            var callback = V1.Spaces.GetSpaceParameters((string)cmd.Data.Options.ToList()[0].Value);
            if (callback == null)
            {
                return "";
            }
            else
            {
                return JsonConvert.SerializeObject(callback, Formatting.Indented);
            }
        }

        private string getbetas(SocketSlashCommand cmd)
        {
            var x = V3.Login((string)cmd.Data.Options.ToList()[0].Value, (string)cmd.Data.Options.ToList()[1].Value);
            var callback = Betas.GetBetas(x.Ticket);
            if (callback == null)
            {
                return "";
            }
            else
            {
                return JsonConvert.SerializeObject(callback, Formatting.Indented);
            }
        }
        
        private string getbetasb64(SocketSlashCommand cmd)
        {
            var x = V3.LoginBase64((string)cmd.Data.Options.ToList()[0].Value);
            var callback = Betas.GetBetas(x.Ticket);
            if (callback == null)
            {
                return "";
            }
            else
            {
                return JsonConvert.SerializeObject(callback, Formatting.Indented);
            }
        }

        private string getbetaphases(SocketSlashCommand cmd)
        {
            var x = V3.Login((string)cmd.Data.Options.ToList()[0].Value, (string)cmd.Data.Options.ToList()[1].Value);
            var callback = Betas.GetBetasPhases(x.Ticket, (string)cmd.Data.Options.ToList()[2].Value);
            if (callback == null)
            {
                return "";
            }
            else
            {
                return JsonConvert.SerializeObject(callback, Formatting.Indented);
            }
        }
        
        private string getbetaphasesb64(SocketSlashCommand cmd)
        {
            var x = V3.LoginBase64((string)cmd.Data.Options.ToList()[0].Value);
            var callback = Betas.GetBetasPhases(x.Ticket, (string)cmd.Data.Options.ToList()[1].Value);
            if (callback == null)
            {
                return "";
            }
            else
            {
                return JsonConvert.SerializeObject(callback, Formatting.Indented);
            }
        }

        private string getbetaphase(SocketSlashCommand cmd)
        {
            var x = V3.Login((string)cmd.Data.Options.ToList()[0].Value, (string)cmd.Data.Options.ToList()[1].Value);
            var callback = Betas.GetBetasPhasePlayergroups(x.Ticket, (string)cmd.Data.Options.ToList()[2].Value, (string)cmd.Data.Options.ToList()[3].Value);
            if (callback == null)
            {
                return "";
            }
            else
            {
                return JsonConvert.SerializeObject(callback, Formatting.Indented);
            }
        }
        
        private string getbetaphaseb64(SocketSlashCommand cmd)
        {
            var x = V3.LoginBase64((string)cmd.Data.Options.ToList()[0].Value);
            var callback = Betas.GetBetasPhasePlayergroups(x.Ticket, (string)cmd.Data.Options.ToList()[1].Value, (string)cmd.Data.Options.ToList()[2].Value);
            if (callback == null)
            {
                return "";
            }
            else
            {
                return JsonConvert.SerializeObject(callback, Formatting.Indented);
            }
        }

        private EmbedBuilder pong(EmbedBuilder embed, SocketSlashCommand cmd)
        {
            embed.Title = "Pong";
            embed.Description = "Pong";
            return embed;
        }
    }
}
