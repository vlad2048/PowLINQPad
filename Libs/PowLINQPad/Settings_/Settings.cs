﻿using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using LINQPad;
using PowBasics.QueryExpr_;
using PowLINQPad.UtilsInternal.Json_;

namespace PowLINQPad.Settings_;

public interface ISettings { }

public static class Settings
{
	public static S Load<S>() where S : ISettings, new() => Jsoners.Common.LoadOrCreateDefault(File, new S());

	public static IRwVar<T> Get<S, T>(this S settings, Expression<Func<S, T>> expr) where S : ISettings, new()
	{
		var (get, set) = QueryExprUtils.RetrieveGetSet(expr);
		var settingVar = Var.Make(get(settings)).D(D);
		settingVar
			.Skip(1)
			.Subscribe(val =>
			{
				set(settings, val);
				RequestSave(settings);
			}).D(D);
		return settingVar;
	}



	internal static void Init()
	{
		whenSave = new Subject<object>().D(D);
		WhenSave
			.Throttle(DebounceTime)
			.Synchronize()
			.Subscribe(Save).D(D);
	}

	private static readonly TimeSpan DebounceTime = TimeSpan.FromMilliseconds(200);
	private static ISubject<object> whenSave = null!;
	private static IObservable<object> WhenSave => whenSave.AsObservable();	
	private static string File => FileUtils.GetSettingsFilename();
	private static void RequestSave<S>(S settings) where S : ISettings, new() => whenSave.OnNext(settings);
	private static void Save(object settings) => Jsoners.Common.Save(File, settings);
}





file static class FileUtils
{
	public static string GetSettingsFilename()
	{
		var queryFile = Util.CurrentQueryPath;
		var folder = Path.GetDirectoryName(queryFile)!;
		var name = Path.GetFileNameWithoutExtension(queryFile);
		return Path.Combine(folder, $"{name}.json");
	}

	/*public static string GetSettingsFilename() => Path.Combine(FindSettingsFolder(), $"{Path.GetFileNameWithoutExtension(Util.CurrentQueryPath)}.json");

	private static string FindSettingsFolder()
	{
		var folder = Path.GetDirectoryName(Util.CurrentQueryPath);
		bool Check(out string checkFolder)
		{
			checkFolder = Path.Combine(folder, "_settings");
			return Directory.Exists(checkFolder);
		}
		while (folder != null)
		{
			if (Check(out var settingsFolder)) return settingsFolder;
			folder = Path.GetDirectoryName(folder);
		}
		throw new ArgumentException("Failed to find the _settings folder");
	}*/
}