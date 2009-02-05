﻿using agsXMPP.protocol.client;
using ChatsworthLib.Entity;

namespace ChatsworthLib.Commands
{
    public class AliasCommand : ICommand
    {
        private readonly ICommunicator _communicator;
        private readonly IMemberDirectory _directory;

        public AliasCommand(ICommunicator communicator, IMemberDirectory directory)
        {
            _communicator = communicator;
            _directory = directory;
        }

        public void Execute(Message message)
        {
            ChatMember fromMember = _directory.LookUp(message.From.Bare);
            if (fromMember == null)
                return;

            string alias = ExtractAlias(message.Body);

            if (string.IsNullOrEmpty(alias))
            {
                _communicator.SendMessage(fromMember.Jid, "usage: /alias <NEW_ALIAS_WITH_NO_SPACES>");
            }
            else
            {
                fromMember.Alias = alias;
                foreach (var chatMember in _directory.GetToListForSubscriber(fromMember.Jid))
                {
                    _communicator.SendMessage(chatMember.Jid,
                                       string.Format("{0} is now known as {1}", fromMember.Jid, fromMember.Alias));
                }
            }
        }

        private string ExtractAlias(string messageBody)
        {
            string[] words = messageBody.Split(' ');
            return words.Length > 1 ? words[1] : string.Empty;
        }

        public string CommandName
        {
            get { return "alias"; }
        }
    }
}
