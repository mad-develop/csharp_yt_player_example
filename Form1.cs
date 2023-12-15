using Microsoft.Web.WebView2.Core;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace YT_Player
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await InitializeAsync();

            // load HTML template to browser component
            this.webView.NavigateToString(this.getYtHtml());

        }

        private async Task InitializeAsync()
        {
            // Initialize webview2 component and add message receiver event
            await this.webView.EnsureCoreWebView2Async(null);
            this.webView.CoreWebView2.WebMessageReceived += MessageReceived;
        }

        async void loadVideo(string youtubeId)
        {
            // Load video
            await this.webView.CoreWebView2.ExecuteScriptAsync("createPlayer('" + youtubeId + "');");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Parse Uri from TextBox and load the video
            Uri uri = new Uri(this.textBox1.Text);
            string ytVideoId =  HttpUtility.ParseQueryString(uri.Query).Get("v");

            this.loadVideo(ytVideoId);
        }

        private void MessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            String content = args.TryGetWebMessageAsString();
            MessageBox.Show(content);
        }

        private string getYtHtml()
        {
            // simple html template for YT JS api implementation
            string html = @"<html>
            <head><meta content='IE=Edge' http-equiv='X-UA-Compatible'/></head>
            <body>
	            <div id='player'></div>
	            <script src='http://www.youtube.com/player_api'></script>
	            <script>
		            // create youtube player
		            var player;
		            var apiReady;

		            // autoplay video
		            function onPlayerReady(event) {
			            event.target.playVideo();
		            }

		            // when video ends
		            function onPlayerStateChange(event) {
			            if (event.data === 0) {
				            window.chrome.webview.postMessage('video play done');
			            }
		            }

		            function onYouTubePlayerAPIReady() {
			            apiReady = true;
		            }

		            function createPlayer(videoId) {
			            if (!apiReady) {
				            return;
			            }

			            if (player) {
				            player.destroy();
				            player = null;
			            }
			
			            player = new YT.Player('player', {
				            width: '640',
				            height: '390',
				            videoId: videoId,
				            events: {
					            onReady: onPlayerReady,
					            onStateChange: onPlayerStateChange
				            }
			            });
		            }
	            </script>
            </body>
            </html>";

            return html;
        }
    }
}
