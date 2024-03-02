using Android.App;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using Android.Content;
using Android.Widget;
using System;
using Android.Views;
using System.Threading;
using Xamarin.Essentials;
using System.IO;
using System.Linq;
using MySqlConnector;
using System.Data;
using System.Collections.Generic;

namespace Application
{
    [Activity(Label = "CoreActivity")]
    public class CoreActivity : Activity
    {
        //here we declare all the things that we will be working with
        private TextView fullname;
        private TextView mTimeStart;
        private TextView mTimeEnd;
        private TextView mGeoStart;
        private TextView mGeoEnd;
        private ProgressBar mProgressBar;
        private Button mBtnStart;
        private Button mBtnEnd;
        private Button mBtnSave;
        private Button mBtnDelete;
        private string line;
        string a = MainActivity.name;
        string email = MainActivity.email;
        //Function for creating a text.txt file

        //gets serial number of the phone

        string serialNumber = Android.Provider.Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);

        //gets application path

        string path = Android.App.Application.Context.GetExternalFilesDir(null).ToString();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_core);

            //Creation of a log.txt file
            string filepath = Path.Combine(path, "logs.txt");

            //Capture our elements (listed above)
            mTimeStart = FindViewById<TextView>(Resource.Id.time_start);
            mBtnStart = FindViewById<Button>(Resource.Id.button_start);
            mTimeEnd = FindViewById<TextView>(Resource.Id.time_end);
            mBtnEnd = FindViewById<Button>(Resource.Id.button_end);
            mGeoStart = FindViewById<TextView>(Resource.Id.geo_start);
            mGeoEnd = FindViewById<TextView>(Resource.Id.geo_end);
            mBtnSave = FindViewById<Button>(Resource.Id.button_save);
            mBtnDelete = FindViewById<Button>(Resource.Id.button_delete);
            mProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            fullname = FindViewById<TextView>(Resource.Id.textView1);
            
            mBtnEnd.Enabled = false;
            mBtnSave.Enabled = false;
            mBtnDelete.Enabled = false;

            //Displays name
            fullname.Text = "You're logged in as: " + a;

            




            //When user click on the button START -----------------------------------------
            mBtnStart.Click += async (object sender, EventArgs e) =>
            {
                try
                {
                    //Visibility of Progress Bar
                    mProgressBar.Visibility = ViewStates.Visible;
                    mBtnStart.Enabled = false;
                    //TIME-------


                    var start_result = await Geolocation.GetLocationAsync();

                    start_result = await Geolocation.GetLocationAsync(new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.Default,
                        Timeout = TimeSpan.FromSeconds(10),
                    });
                    string start_time = start_result.Timestamp.AddHours(2).DateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    mTimeStart.Text = start_time;





                    //LOCATION


                    if (start_result == null)
                    {
                        start_result = await Geolocation.GetLocationAsync(new GeolocationRequest
                        {
                            DesiredAccuracy = GeolocationAccuracy.Default,
                            Timeout = TimeSpan.FromSeconds(10),
                        });
                    }
                    //Displays Geolocation
                    if (start_result == null)
                    {
                        mGeoStart.Text = "No GPS!";
                    }
                    else
                    {
                        mGeoStart.Text = $"lat: {start_result.Latitude}, lng: {start_result.Longitude}";
                    }


                    if (mTimeStart.Text != null && mGeoStart.Text != null)
                    {
                        mProgressBar.Visibility = ViewStates.Invisible;
                    }
                    mBtnEnd.Enabled = true;
                }
                catch (Exception ex)
                {
                    mProgressBar.Visibility = ViewStates.Invisible;
                    Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                    alert.SetTitle("Warning!");
                    alert.SetMessage("Your device can't capture your location. Please turn ON your location or try again.");
                    alert.SetNeutralButton("Close", (senderAlert, args) =>
                    {
                        mBtnStart.Enabled = true;
                    });
                    Dialog dialog = alert.Create();
                    dialog.Show();
                }
            };

            //When user clicks on the buttom END
            mBtnEnd.Click += async (object sender, EventArgs e) =>
            {
                try
                {
                    mProgressBar.Visibility = ViewStates.Visible;
                    mBtnEnd.Enabled = false;
                    // Captures the current time in a DateTime object.
                    var end_result = await Geolocation.GetLocationAsync();
                    end_result = await Geolocation.GetLocationAsync(new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.Default,
                        Timeout = TimeSpan.FromSeconds(10),
                    });
                    string end_time = end_result.Timestamp.AddHours(2).DateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    mTimeEnd.Text = end_time;

                    //LOCATION



                    if (end_result == null)
                    {
                        end_result = await Geolocation.GetLocationAsync(new GeolocationRequest
                        {
                            DesiredAccuracy = GeolocationAccuracy.Default,
                            Timeout = TimeSpan.FromSeconds(10),
                        });
                    }
                    //Displays Geolocation
                    if (end_result == null)
                    {
                        mGeoEnd.Text = "No GPS!";
                    }
                    else
                    {
                        mGeoEnd.Text = $"lat: {end_result.Latitude}, lng: {end_result.Longitude}";
                    }

                    if (mTimeEnd.Text != null && mGeoEnd.Text != null)
                    {
                        mProgressBar.Visibility = ViewStates.Invisible;
                    }
                    mBtnSave.Enabled = true;
                    mBtnDelete.Enabled = true;
                }
                catch (Exception ex)
                {
                    mProgressBar.Visibility = ViewStates.Invisible;
                    Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                    alert.SetTitle("Warning!");
                    alert.SetMessage("Your device can't capture your location. Please turn ON your location or try again.");
                    alert.SetNeutralButton("Close", (senderAlert, args) =>
                    {
                        mBtnEnd.Enabled = true;
                    });
                    Dialog dialog = alert.Create();
                    dialog.Show();
                }
            };


            //KO uporabnik pritisne gumb SAVE

            mBtnSave.Click += (object sender, EventArgs e) =>
            {
                //Spremenljivka kjer se zabeleži stanje interneta
                var profiles = Connectivity.NetworkAccess;
                //combines all text files into text
                line = "(select id_u from user where email = '"+email+"'),'" + serialNumber + "','" + mTimeStart.Text + "','" + mGeoStart.Text + "','" + mTimeEnd.Text + "','" + mGeoEnd.Text + "'";

                //Alert Dialog
                Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);



                //inserts the line into the "logs.txt".
                using (var writer = System.IO.File.AppendText(filepath))
                {
                    writer.WriteLine(line);
                }

                //If the person is connected to the internet.
                if (profiles == NetworkAccess.Internet)
                {
                    // Active Wi-Fi connection.
                    alert.SetTitle("Network: Online");
                    alert.SetMessage("Your work has been saved into your device. Do you want to upload them to the server?");

                    alert.SetPositiveButton("Yes", (senderAlert, args) =>
                    {
                        //function inserted if needed.

                        foreach (string line in File.ReadLines(filepath))
                        {

                            string connectionString = "server=spiritofsakura.eu;port=3306;uid=spiritof_sakura;pwd=1q2w3e4r5t6z7u8i9o;database=spiritof_app;Allow User Variables=True";
                            //Database line
                            string query = "insert into work(id_u,serial_number,arrival_time,arrival_geo,departure_time,departure_geo) values(" + line + ");";
                            //Connection
                            MySqlConnection connect = new MySqlConnection(connectionString);
                            connect.Open();

                            MySqlCommand cmd = new MySqlCommand(query, connect);
                            //Execute
                            cmd.ExecuteNonQuery();

                        }


                        File.Delete(filepath);
                        mProgressBar.Visibility = ViewStates.Invisible;


                        //removes all fields with variables
                        mTimeStart.Text = "";
                        mTimeEnd.Text = "";
                        mGeoStart.Text = "";
                        mGeoEnd.Text = "";

                    });
                    alert.SetNegativeButton("Cancel", (senderAlert, args) =>
                    {
                        //removes all fields with variables
                        mTimeStart.Text = "";
                        mTimeEnd.Text = "";
                        mGeoStart.Text = "";
                        mGeoEnd.Text = "";


                    });

                    Dialog dialog = alert.Create();
                    dialog.Show();
                }
                else
                {


                    alert.SetTitle("Network: Offline");
                    alert.SetMessage("Your work has been saved to your device.");
                    alert.SetNeutralButton("OK", (senderAlert, args) =>
                    {
                        //function inserted if needed.
                        mTimeStart.Text = "";
                        mTimeEnd.Text = "";
                        mGeoStart.Text = "";
                        mGeoEnd.Text = "";
                    });
                    Dialog dialog = alert.Create();
                    dialog.Show();
                }


                mBtnSave.Enabled = false;
                mBtnDelete.Enabled = false;
                mBtnStart.Enabled = true;
            };

            //Ko uporabnik pritinse gumb delete
            mBtnDelete.Click += (object sender, EventArgs e) =>
            {

                //alert window pops up
                Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                alert.SetTitle("Warning!");
                alert.SetMessage("You're about to delete all the data of your current timestamp and geolocation. Are you sure, you want to do this?");
                alert.SetPositiveButton("Yes", (senderAlert, args) =>
                {
                    mTimeStart.Text = "";
                    mTimeEnd.Text = "";
                    mGeoStart.Text = "";
                    mGeoEnd.Text = "";
                    mBtnDelete.Enabled = false;
                    mBtnSave.Enabled = false;
                    mBtnStart.Enabled = true;
                });
                alert.SetNegativeButton("Cancel", (senderAlert, args) =>
                {
                    mBtnDelete.Enabled = true;
                    mBtnSave.Enabled = true;
                });
                Dialog dialog = alert.Create();
                dialog.Show();

            };

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}