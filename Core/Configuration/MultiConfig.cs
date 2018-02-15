﻿using Discord;
using Discord.WebSocket;
using Lomztein.ModularDiscordBot.Core.Bot;
using Lomztein.ModularDiscordBot.Core.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lomztein.ModularDiscordBot.Core.Configuration
{
    /// <summary>
    /// This is designed to be easy to use for multiserver modules.
    /// </summary>
    public class MultiConfig : Config {

        public MultiConfig (string _name) : base (_name) { }

        public T GetEntry<T> (SocketGuild guild, string key, T fallback) {
            return GetEntry (guild.Id, key, fallback);
        }

        public MultiEntry<T> GetEntries<T> (IEnumerable<IEntity<ulong>> entities, string key, T fallback) {
            Dictionary<ulong, T> entries = new Dictionary<ulong, T> ();
            foreach (SocketGuild entity in entities) {
                entries.Add (entity.Id, GetEntry (entity, key, fallback));
            }
            return new MultiEntry<T> (entries);
        }

        public void SetEntries (IEnumerable<IEntity<ulong>> entities, string key, object value, bool save) {
            Dictionary<ulong, object> entries = new Dictionary<ulong, object> ();
            foreach (SocketGuild entity in entities) {
                SetEntry (entity.Id, key, value, save);
            }
        }

        public MultiEntry<T> GetEntries<T>(IEnumerable<IEntity<ulong>> entities, string key, IEnumerable<T> fallback) {
            Dictionary<ulong, T> entries = new Dictionary<ulong, T> ();

            for (int i = 0; i < entities.Count (); i++) {
                entries.Add (entities.ElementAt (i).Id, GetEntry (entities.ElementAt (i).Id, key, fallback.ElementAt (i)));
            }

            return new MultiEntry<T> (entries);
        }

        public void SetEntries<T>(IEnumerable<IEntity<ulong>> entities, string key, MultiEntry<T> entry, bool save) {
            Dictionary<ulong, object> entries = new Dictionary<ulong, object> ();
            for (int i = 0; i < entities.Count (); i++) {
                SetEntry (entry.values.ElementAt (i).Key, key, entry.values.ElementAt (i).Value, save);
            }
        }

        public override void Load() {
            try {
                string [ ] files = Directory.GetFiles (GetPath ());
                var loadedEntries = new Dictionary<ulong, Dictionary<string, object>> ();

                foreach (string file in files) {
                    Dictionary<string, object> entry = JSONSerialization.DeserializeFile<Dictionary<string, object>> (file);
                    loadedEntries.Add (ulong.Parse (Path.GetFileNameWithoutExtension (file)), entry);
                }

                entries = loadedEntries;
            } catch (Exception exc) {
                Log.Write (exc);
            }
        }

        public override void Save() {
            foreach (var value in entries) {
                JSONSerialization.SerializeObject (value.Value, GetPath (value.Key), true);
            }
        }

        public string GetPath (ulong id) {
            return configRootDirectory + name + "/" + id.ToString ();
        }
    }
}
