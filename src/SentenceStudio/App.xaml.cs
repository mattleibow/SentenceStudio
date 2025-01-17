﻿using System.Globalization;
using CommunityToolkit.Mvvm.Messaging;
using SentenceStudio.Messages;
using SentenceStudio.Pages.Dashboard;
using SentenceStudio.Pages.Onboarding;
using SentenceStudio.Pages.Vocabulary;
using SentenceStudio.Resources.Styles;
using SentenceStudio.Services;
using Sharpnado.Tasks;

namespace SentenceStudio;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		Debug.WriteLine($"AppStart Culture: {CultureInfo.CurrentUICulture.Name}");
		Debug.WriteLine($"Manufacturer: {DeviceInfo.Model}");

		if(DeviceInfo.Model.Contains("NoteAir3C", StringComparison.CurrentCultureIgnoreCase)) // TODO expand this to detect any eInk device
		{
			Application.Current.Resources.MergedDictionaries.Add(new HighContrastColors());
		}
		else
		{
			Application.Current.Resources.MergedDictionaries.Add(new Resources.Styles.Colors());
		}
		
		Application.Current.Resources.MergedDictionaries.Add(new Resources.Styles.Fluent());
		Application.Current.Resources.MergedDictionaries.Add(new Resources.Styles.Styles());
		Application.Current.Resources.MergedDictionaries.Add(new Resources.Styles.Converters());

		// Application.Current.Resources.MergedDictionaries.Add(mergedResources);

		// CultureInfo.CurrentUICulture = new CultureInfo( "ko-KR", false );

		// Debug.WriteLine($"New Culture: {CultureInfo.CurrentUICulture.Name}");

		

		// Register a message in some module
		WeakReferenceMessenger.Default.Register<ConnectivityChangedMessage>(this, async (r, m) =>
		{
			if(!m.Value)
				await Shell.Current.CurrentPage.DisplayAlert("No Internet Connection", "Please connect to the internet to use this feature", "OK");
		});
        
	}

    

	protected override void OnStart()
	{
		
	}

	protected override void OnSleep()
	{
	}

	protected override void OnResume()
	{
	}
	
    protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window( new AppShell(activationState.Context.Services.GetService<AppShellModel>()) );
	}
}
