﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Lomztein.Moduthulhu.Core.Bot.Client.Sharding.Guild.Config
{
    public class ConfigFunctionInfo
    {
        private readonly ConfigFunctionParam[] _parameters;
        public ConfigFunctionParam[] GetParameters () => _parameters;
        public Delegate Action { get; private set; }
        public Delegate Message { get; private set; }

        public string Name { get; private set; }
        public string Desc { get; private set; }
        public string Identifier { get; private set; }

        public ConfigFunctionInfo(string name, string description, string identifier, Delegate action, Delegate message, params string[] paramNames)
        {
            Action = action;
            Message = message;

            if (Action.Method.ReturnType != typeof(void))
            { // I couldn't find any non-generic Action object to use, best I got is a Delegate type.
                throw new ArgumentException("Action delegate must be without a return type.");
            }

            if (Message.Method.ReturnType != typeof (string))
            {
                throw new ArgumentException("Message delegate must have a string return type.");
            }

            Type[] generics = Action.GetType().GetGenericArguments();
            if (generics.Length != paramNames.Length)
            {
                throw new ArgumentException("A differing amount of parameter names was given in comparison to the actions generic arguments. Lengths need to be identical.");
            }

            _parameters = new ConfigFunctionParam[generics.Length];
            for (int i = 0; i < generics.Length; i++)
            {
                _parameters[i] = new ConfigFunctionParam(generics[i], paramNames[i]);
            }

            Name = name;
            Desc = description;
            Identifier = identifier;
        }

        public bool Matches(string identifier) => Identifier == identifier;
        public bool Matches(string name, string identifier) => Name == name && Identifier == identifier;
    }
}
