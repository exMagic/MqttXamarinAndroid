using Android.App;
using Android.Widget;
using Android.OS;
using uPLibrary.Networking.M2Mqtt;
using System;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MqttXamarinAndroid
{
	[Activity (Label = "MqttXamarinAndroid", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		private EditText edtMessage;
		private Button bttPub;
		private Spinner spnQOS;
		private TextView txtResult;
		private MqttClient mqttClient;
        string topic = "b";
        private void initWidget()
		{
			this.txtResult = FindViewById<TextView> (Resource.Id.result);
			this.edtMessage = FindViewById<EditText> (Resource.Id.edtMessage);
			this.bttPub = FindViewById<Button> (Resource.Id.bttPublishMes);
			this.spnQOS = FindViewById<Spinner> (Resource.Id.spnQOS);
			var adapter = ArrayAdapter.CreateFromResource (this, Resource.Array.qos, Android.Resource.Layout.SimpleSpinnerItem);
			adapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spnQOS.Adapter = adapter;
            ConnectServer();
            mqttClient.Subscribe(new string[] { topic }, new byte[] { (byte)spnQOS.SelectedItemPosition });
        }

        private void initControl()
		{
			bttPub.Click += (object sender, EventArgs e) => {
                try {
					if(edtMessage.Text != null || edtMessage.Text.Length > 0 ){
                        if (mqttClient!=null && mqttClient.IsConnected){
                            mqttClient.Publish(topic,System.Text.Encoding.UTF8.GetBytes( edtMessage.Text));
							txtResult.Text = "publish message "+edtMessage.Text+" to topic " + topic + " ok";
						}
					}else{
						Toast.MakeText (this, "topic or message wrong", ToastLength.Short).Show();
					}
				} catch (Exception ex) {					
				}
			};
		}

		private void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
		{
			string result = System.Text.Encoding.UTF8.GetString(e.Message);
			RunOnUiThread (() => {
				txtResult.Text = "Receiver message: "+result;
			});
		}

		private void client_ConnectionClosedEvent (object sender, EventArgs e)
		{
			RunOnUiThread (() => {
				txtResult.Text = "Connection lost";
			});
		}

		private void ConnectServer()
		{
			try {
                    mqttClient = new MqttClient("m21.cloudmqtt.com", 12004, false, null, MqttSslProtocols.None);
                    mqttClient.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
                    mqttClient.ConnectionClosed += client_ConnectionClosedEvent;
                    mqttClient.Connect("xamek", "uugymqbo", "pSUnBCfynYB8", false, 9999);
                    if (mqttClient.IsConnected)
                    {
                        txtResult.Text = "Connect OK -- let's sub topic";
                    }
            } catch (Exception ex) {
				txtResult.Text = "Connect ERROR";
			}
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			Xamarin.Insights.Initialize (XamarinInsights.ApiKey, this);
			base.OnCreate (savedInstanceState);
			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
			initWidget ();
			initControl ();
		}
	}
}
