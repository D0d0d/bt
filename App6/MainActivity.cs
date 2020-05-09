using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

using System.Collections.Generic;
using Android.Content;
using Android.Bluetooth;
using System.Collections.ObjectModel;


namespace App6
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public static TextView textview;
        public Button button;
        public readonly static Receiver1 receiver = new Receiver1();
        public static IntentFilter Filter { get; set; }
        private BluetoothAdapter ba = BluetoothAdapter.DefaultAdapter;
        private readonly static int RequstEnableBt = 1;
        bool toStart;

        ///
        void Button_Click(object sender, EventArgs e)
        {
            if (toStart)
            {
                textview.Text = String.Empty;
                ba.StartDiscovery();
                button.Text = "Идет поиск..";
                toStart = false;
                BluetoothDevice[] device = new BluetoothDevice[ba.BondedDevices.Count];
                ba.BondedDevices.CopyTo(device, 0);
                // Если список спаренных устройств не пуст
                if (ba.BondedDevices.Count > 0)
                {
                    textview.Text += "Сохраненные:" + "\n";
                    // проходимся в цикле по этому списку
                    for (int i = 0; i < ba.BondedDevices.Count; i++)
                    {
                        // Добавляем имена и адреса в mArrayAdapter, чтобы показать
                        // через ListView
                        textview.Text += device[i].Name + "\n" + device[i].Address;
                    }
                }
                else
                {
                    Toast.MakeText(this, "Сопряженных нет", ToastLength.Long).Show();
                }
            }
            else
            {
                ba.CancelDiscovery();
                toStart = true;
                textview.Text = "Начать поиск!";
                button.Text = "Поиск..";
            }
        }
        [BroadcastReceiver]
        public class Receiver1 : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {

                string action = intent.Action;
                ObservableCollection<MainActivity> devices = new ObservableCollection<MainActivity>();
                if (BluetoothDevice.ActionFound.Equals(action))
                {
                    textview.Text += "\n\n Поблизости:";
                    BluetoothDevice device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
                    string deviceName = device.Name;
                    string deviceHardwareAddress = device.Address;
                    textview.Text += "\n Name:" + deviceName + " Mac:" + deviceHardwareAddress + System.Environment.NewLine;

                }
            }
        }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);


            ///

            button = FindViewById<Button>(Resource.Id.button1); ///

            textview = FindViewById<TextView>(Resource.Id.textView1);
            textview.Text = String.Empty;

            button.Click += Button_Click;
            toStart = true;

            Filter = new IntentFilter();
            Filter.AddAction(BluetoothAdapter.ActionStateChanged);
            Filter.AddAction(BluetoothAdapter.ActionDiscoveryStarted);
            
            Filter.AddAction(BluetoothAdapter.ActionDiscoveryFinished);
            Filter.AddAction(BluetoothDevice.ActionFound);

            RegisterReceiver(receiver, Filter);
         
            if (!ba.IsEnabled)
            {
                Intent enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                this.StartActivityForResult(enableBtIntent, RequstEnableBt);
             
            }

            String status;
            
                String mydeviceaddress = ba.Address;
                String mydevicename = ba.Name;
                status = mydevicename + " : " + mydeviceaddress;
            
            Toast.MakeText(this, status, ToastLength.Long).Show();

            
            
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

      
	}
}

