using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using TwitchToolkit;
using TwitchToolkit.IRC;
using TwitchToolkit.PawnQueue;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public class CommandBase : CommandDriver
    {
        private const int MessageLimit = 500;

        public static Pawn FindPawn(string username)
        {
            return Find.ColonistBar.Entries
                .Where(c => ((NameTriple) c.pawn.Name).Nick.EqualsIgnoreCase(username))
                .Select(c => c.pawn)
                .FirstOrDefault();
        }

        public static Pawn GetOrFindPawn(string username)
        {
            var safe = GetPawn(username);

            if (safe != null)
            {
                return safe;
            }

            var result = FindPawn(username);

            if (result == null)
            {
                Logger.Warn($"Viewer \"{username}\" was unlinked from their pawn!  Reassigning...");

                var component = Current.Game.GetComponent<GameComponentPawns>();

                component.pawnHistory[username] = result;
                component.viewerNameQueue.Remove(username);
            }

            return result;
        }

        public static Pawn GetPawn(string username)
        {
            var component = Current.Game.GetComponent<GameComponentPawns>();
            var query = component.pawnHistory.Keys
                .Where(k => k.EqualsIgnoreCase(username))
                .Select(p => component.pawnHistory[p]);

            return query.Any() ? query.First() : null;
        }

        public static string GetTranslatedEmoji(string emoji, string text = null)
        {
            if(text == null)
            {
                text = $"{emoji}.Text";
            }

            if(TKSettings.Emojis)
            {
                return emoji;
            }

            return text;
        }

        public static void SendCommandMessage(string viewer, string message, bool separateRoom)
        {
            SendMessage(
                "TKUtils.Formats.CommandBase".Translate(
                    viewer.Named("VIEWER"),
                    message.Named("MESSAGE")
                ),
                separateRoom
            );
        }

        public static void SendCommandMessage(string message, IRCMessage ircMessage) => SendCommandMessage(ircMessage.User, message, CommandsHandler.SendToChatroom(ircMessage));

        public static void SendMessage(string message, bool separateRoom)
        {
            if (message.NullOrEmpty())
            {
                return;
            }

            var words = message.Split(new[] {' '}, System.StringSplitOptions.None);
            var builder = new StringBuilder();
            var messages = new List<string>();
            var chars = 0;

            foreach (var word in words)
            {
                if (chars + word.Length <= MessageLimit - 3)
                {
                    builder.Append($"{word} ");
                    chars += word.Length + 1;
                }
                else
                {
                    builder.Append("...");
                    messages.Add(builder.ToString());
                    builder.Clear();
                    chars = 0;
                }
            }

            if (builder.Length > 0)
            {
                messages.Add(builder.ToString());
                builder.Clear();
            }

            if (messages.Count <= 0)
            {
                return;
            }

            foreach (var m in messages)
            {
                Toolkit.client.SendMessage(m.Trim());
            }
        }
    }
}
