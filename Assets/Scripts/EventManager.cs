using System;

namespace Unstable
{
    public static class EventManager
    {
        public static void DoAction(string command, string argument)
        {
            switch (command.ToUpperInvariant())
            {
                case "ADD":
                    {
                        string[] s = argument.Split(' ');
                        var trigram = s[0];
                        var cardTrigram = s[1];

                        for (int i = 0; i < int.Parse(s[2]); i++)
                        {
                            GameManager.Instance.AddCard(GameManager.Instance.GetCard(trigram == "null" ? null : trigram, cardTrigram), trigram);
                        }
                    }
                    break;

                default:
                    throw new NotImplementedException("Unknown command " + command);
            }
        }
    }
}
