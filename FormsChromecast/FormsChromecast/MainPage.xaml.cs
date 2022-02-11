using SharpCaster.Controllers;
using SharpCaster.Models;
using SharpCaster.Services;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FormsChromecast
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void TestVideo_Clicked(object sender, EventArgs e)
        {
            var testVideoUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4";
            var chromecast = await PromptAndGetSelectedChromecastDevice();
            if (chromecast is null) return;
            await ConnectAndStreamToChromecast(chromecast, testVideoUrl);
        }

        private async void PickVideo_Clicked(object sender, EventArgs e)
        {
            var video = await MediaPicker.PickVideoAsync();
            if (video is null) return;

            var url = GetHostedUrl(video.FullPath);

            var chromecast = await PromptAndGetSelectedChromecastDevice();
            if (chromecast is null) return;
            await ConnectAndStreamToChromecast(chromecast, url);
        }

        private async void URLVideo_Clicked(object sender, EventArgs e)
        {
            var url = await DisplayPromptAsync("Enter video URL", string.Empty);
            if (string.IsNullOrEmpty(url)) return;

            var chromecast = await PromptAndGetSelectedChromecastDevice();
            if (chromecast is null) return;
            await ConnectAndStreamToChromecast(chromecast, url);
        }

        private async Task<Chromecast> PromptAndGetSelectedChromecastDevice()
        {
            var chromecasts = await ChromecastService.Current.StartLocatingDevices();

            var chromecastName = await DisplayActionSheet("Choose Chromecast", "Cancel", null, chromecasts.Select(x => x.FriendlyName).ToArray());

            return chromecasts.FirstOrDefault(x => x.FriendlyName == chromecastName);
        }

        private static async Task ConnectAndStreamToChromecast(Chromecast chromecast, string mediaUrl)
        {
            SharpCasterDemoController _controller = null;
            ChromecastService.Current.ChromeCastClient.ConnectedChanged += async delegate { if (_controller == null) _controller = await ChromecastService.Current.ChromeCastClient.LaunchSharpCaster(); };
            ChromecastService.Current.ChromeCastClient.ApplicationStarted +=
            async delegate
            {
                while (_controller == null)
                {
                    await Task.Delay(500);
                }

                await _controller.LoadMedia(mediaUrl, "video/mp4", null, "BUFFERED");
            };

            await ChromecastService.Current.ConnectToChromecast(chromecast);
        }

        private string GetHostedUrl(string path)
        {
            // Implement your logic here to upload your video
            // and retrieve the URL for it.
            var testVideoUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4";
            return testVideoUrl;
        }
    }
}
