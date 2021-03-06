﻿using Newtonsoft.Json;
using System.IO;
using TGS.Interface;

namespace TGS.Server
{
	/// <summary>
	/// Configuration settings for a <see cref="Instance"/>
	/// </summary>
	public interface IInstanceConfig
	{
		/// <summary>
		/// The <see cref="Instance"/> directory this <see cref="IInstanceConfig"/> is for
		/// </summary>
		string Directory { get; }

		/// <summary>
		/// Actual version of the <see cref="IInstanceConfig"/>. Migrated up via <see cref="DeprecatedInstanceConfig"/>
		/// </summary>
		ulong Version { get; }

		/// <summary>
		/// The name of the <see cref="Instance"/>
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// If the <see cref="Instance"/> is active
		/// </summary>
		bool Enabled { get; set; }

		/// <summary>
		/// The name of the .dme/.dmb the <see cref="Instance"/> uses
		/// </summary>
		string ProjectName { get; set; }

		/// <summary>
		/// The port the <see cref="Instance"/> runs on
		/// </summary>
		ushort Port { get; set; }

		/// <summary>
		/// The <see cref="DreamDaemonSecurity"/> level for the <see cref="Instance"/>
		/// </summary>
		DreamDaemonSecurity Security { get; set; }

		/// <summary>
		/// Whether or not the <see cref="Instance"/> should immediately start DreamDaemon when activated
		/// </summary>
		bool Autostart { get; set; }

		/// <summary>
		/// Whether or not DreamDaemon allows connections from webclients
		/// </summary>
		bool Webclient { get; set; }

		/// <summary>
		/// Author and committer name for synchronize commits
		/// </summary>
		string CommitterName { get; set; }
		/// <summary>
		/// Author and committer e-mail for synchronize commits
		/// </summary>
		string CommitterEmail { get; set; }

		/// <summary>
		/// Encrypted serialized <see cref="ChatSetupInfo"/>s
		/// </summary>
		string ChatProviderData { get; set; }

		/// <summary>
		/// Entropy for <see cref="ChatProviderData"/>
		/// </summary>
		string ChatProviderEntropy { get; set; }

		/// <summary>
		/// If the <see cref="Instance"/> should reattach to a running DreamDaemon <see cref="System.Diagnostics.Process"/>
		/// </summary>
		bool ReattachRequired { get; set; }

		/// <summary>
		/// The <see cref="System.Diagnostics.Process.Id"/> of the runnning DreamDaemon <see cref="System.Diagnostics.Process"/>
		/// </summary>
		int ReattachProcessID { get; set; }

		/// <summary>
		/// The port the runnning DreamDaemon <see cref="System.Diagnostics.Process"/> was launched on
		/// </summary>
		ushort ReattachPort { get; set; }

		/// <summary>
		/// The serviceCommsKey the runnning DreamDaemon <see cref="System.Diagnostics.Process"/> was launched on
		/// </summary>
		string ReattachCommsKey { get; set; }

		/// <summary>
		/// The API version of the runnning DreamDaemon <see cref="System.Diagnostics.Process"/>
		/// </summary>
		string ReattachAPIVersion { get; set; }

		/// <summary>
		/// The user group allowed to use the <see cref="Instance"/>
		/// </summary>
		string AuthorizedUserGroupSID { get; set; }

		/// <summary>
		/// The auto update interval for the <see cref="Instance"/>
		/// </summary>
		ulong AutoUpdateInterval { get; set; }

		/// <summary>
		/// Whether or not testmerge commits are published to a temporary remote branch
		/// </summary>
		bool PushTestmergeCommits { get; set; }

		/// <summary>
		/// Saves the <see cref="IInstanceConfig"/> to it's <see cref="Instance"/> <see cref="Directory"/>
		/// </summary>
		void Save();
	}

	/// <inheritdoc />
	public class InstanceConfig : IInstanceConfig
	{
		/// <summary>
		/// The name the file is saved as in the <see cref="Directory"/>
		/// </summary>
		[JsonIgnore]
		public const string JSONFilename = "Instance.json";

		/// <summary>
		/// The current version of the config
		/// </summary>
		[JsonIgnore]
		protected const ulong CurrentVersion = 0;   //Literally any time you add/deprecated a field, this number needs to be bumped

		/// <inheritdoc />
		[JsonIgnore]
		public string Directory { get; private set; }

		/// <inheritdoc />
		public ulong Version { get; protected set; } = CurrentVersion;

		/// <inheritdoc />
		public string Name { get; set; } = "TG Station Server";

		/// <inheritdoc />
		public bool Enabled { get; set; } = true;

		/// <inheritdoc />
		public string ProjectName { get; set; } = "tgstation";

		/// <inheritdoc />
		public ushort Port { get; set; } = 1337;

		/// <inheritdoc />
		public DreamDaemonSecurity Security { get; set; } = DreamDaemonSecurity.Trusted;

		/// <inheritdoc />
		public bool Autostart { get; set; } = false;

		/// <inheritdoc />
		public bool Webclient { get; set; } = false;

		/// <inheritdoc />
		public string CommitterName { get; set; } = "tgstation-server";

		/// <inheritdoc />
		public string CommitterEmail { get; set; } = "tgstation-server@tgstation13.org";

		/// <inheritdoc />
		public string ChatProviderData { get; set; } = Instance.UninitializedString;

		/// <inheritdoc />
		public string ChatProviderEntropy { get; set; }

		/// <inheritdoc />
		public bool ReattachRequired { get; set; } = false;

		/// <inheritdoc />
		public int ReattachProcessID { get; set; }

		/// <inheritdoc />
		public ushort ReattachPort { get; set; }

		/// <inheritdoc />
		public string ReattachCommsKey { get; set; }

		/// <inheritdoc />
		public string ReattachAPIVersion { get; set; }

		/// <inheritdoc />
		public string AuthorizedUserGroupSID { get; set; } = null;

		/// <inheritdoc />
		public ulong AutoUpdateInterval { get; set; } = 0;

		/// <inheritdoc />
		public bool PushTestmergeCommits { get; set; } = false;

		/// <summary>
		/// Construct a <see cref="InstanceConfig"/> for a <see cref="Instance"/> at <paramref name="path"/>
		/// </summary>
		/// <param name="path">The path to the <see cref="Instance"/></param>
		public InstanceConfig(string path)
		{
			Directory = path;
		}

		/// <inheritdoc />
		public void Save()
		{
			var data = JsonConvert.SerializeObject(this, Formatting.Indented);
			var path = Path.Combine(Directory, JSONFilename);
			File.WriteAllText(path, data);
		}

		/// <summary>
		/// Loads and migrates an <see cref="IInstanceConfig"/> from a <see cref="Instance"/> at <paramref name="path"/>
		/// </summary>
		/// <param name="path">The path to the <see cref="Instance"/> directory</param>
		/// <returns>The migrated <see cref="IInstanceConfig"/></returns>
		public static IInstanceConfig Load(string path)
		{
			var configtext = File.ReadAllText(Path.Combine(path, JSONFilename));
			var res = JsonConvert.DeserializeObject<DeprecatedInstanceConfig>(configtext);
			res.Directory = path;
			res.MigrateToCurrentVersion();
			return res;
		}
	}
}
