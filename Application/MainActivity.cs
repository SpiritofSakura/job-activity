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
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;

namespace Application
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    
    public class MainActivity : AppCompatActivity
    {
        
       
        public static string name;
        public static string email;
       
        private Button mBtnSignUp;
        private Button mBtnSignIn;
        private ProgressBar mProgressBar;
        
        private string line;
        private string salt;
        
        string connectionString = "server=spiritofsakura.eu;port=3306;uid=spiritof_sakura;pwd=1q2w3e4r5t6z7u8i9o;database=spiritof_app;Allow User Variables=True";
        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            mBtnSignUp = FindViewById<Button>(Resource.Id.btnSignUp);
            mBtnSignIn = FindViewById<Button>(Resource.Id.btn_LogIn);
            mProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);

            
            //When user presses the SignUp button
            mBtnSignUp.Click += (object sender, EventArgs args) =>
            {
                //Pull dialog
                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                Dialog_SignUp signUpDialog = new Dialog_SignUp();
                signUpDialog.Show(transaction, "dialog fragment");

                signUpDialog.mOnSignUpComplete += signUpDialog_mOnSignUpComplete;
            };

            //When Sign IN button is pressed
            mBtnSignIn.Click += (object sender, EventArgs args) =>
            {
                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                Dialog_SignIn signInDialog = new Dialog_SignIn();
                signInDialog.Show(transaction, "dialog fragment");

                signInDialog.mOnSignInComplete += signInDialog_mOnSignInComplete;
            };
        }

        void signInDialog_mOnSignInComplete(object sender, OnSignInEventArgs e)
        {
            Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
            var profiles = Connectivity.NetworkAccess;
            if (profiles == NetworkAccess.Internet)
            {
                string userEmail = e.Email;
                string userPassword1 = e.Password;
                //Uporaba soli
                salt = "hva9AVDUCsP6nBPh";
                string userPassword = userPassword1 + salt;

                //KRIPTIRANJE MD5 Algoritem
                using (var md5Hash = MD5.Create())
                {
                    // Byte array representation of source string
                    var sourceBytes = Encoding.UTF8.GetBytes(userPassword);

                    // Generate hash value(Byte Array) for input data
                    var hashBytes = md5Hash.ComputeHash(sourceBytes);


                    // Convert hash byte array to string
                    var encryptedPassword = BitConverter.ToString(hashBytes).Replace("-", string.Empty);

                    userPassword = encryptedPassword;
                }
                //get first name and last name from the database
                MySqlConnection name1 = new MySqlConnection(connectionString);
                MySqlCommand cmd1 = new MySqlCommand("SELECT * FROM user WHERE email = @userEmail AND password = @userPassword", name1);
                cmd1.Parameters.AddWithValue("@userEmail", userEmail);
                cmd1.Parameters.AddWithValue("@userPassword", userPassword);
                name1.Open();
                //Prebere podatke in jih shrani, da se bodo lahko prikazali v jedru aplikacije
                MySqlDataReader reader = cmd1.ExecuteReader();
                while(reader.Read())
                {
                    string ime = reader.GetString(1);
                    string priimek = reader.GetString(2);
                    string eposta = reader.GetString(3);
                    name = ime + " " + priimek;
                    email = eposta;
                }
                
                cmd1.Dispose();
                name1.Close();
                
                //Preveri če uporabnik obstaja v bazi
                try
                {
                    MySqlConnection connect1 = new MySqlConnection(connectionString);
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM user WHERE email = @userEmail AND password = @userPassword", connect1);
                    cmd.Parameters.AddWithValue("@userEmail", userEmail);
                    cmd.Parameters.AddWithValue("@userPassword", userPassword);
                    connect1.Open();

                    var result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        //Succesful log in!
                        //Switches to a new page
                        StartActivity(typeof(CoreActivity));




                    }
                    else
                    {
                        //Not succesful log in!
                        alert.SetTitle("Denied!");
                        alert.SetMessage("Wrong Email or Password!");
                        alert.SetNeutralButton("Close", (senderAlert, args) =>
                        {

                        });
                        Dialog dialog = alert.Create();
                        dialog.Show();
                    }
                    //Execute
                    cmd.Dispose();
                    connect1.Close();

                }
                catch (Exception ex)
                { Console.WriteLine(ex.Message); }
            }
            else
            {
                alert.SetTitle("Error!");
                alert.SetMessage("You're not connected to the internet, therefore you can't log into the app.");
                alert.SetNeutralButton("Close", (sender, e) =>
                {

                });
                Dialog dialog = alert.Create();
                dialog.Show();
            }
            
            
            
        }

        void signUpDialog_mOnSignUpComplete(object sender, OnSignUpEventArgs e)
        {
            Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
            var profiles = Connectivity.NetworkAccess;
            if (profiles == NetworkAccess.Internet)
            {
                mProgressBar.Visibility = ViewStates.Visible;
                Thread thread = new Thread(ActLikeARequest);
                thread.Start();

                //Pridobivanje podatkov iz dialoga in se zabeleži v spremenljivko


                string userFirstName = e.FirstName;
                string userLastName = e.LastName;
                string userEmail = e.Email;
                string userPassword1 = e.Password;
                salt = "hva9AVDUCsP6nBPh";
                string userPassword = userPassword1 + salt;
                //KRIPTIRANJE MD5 Algoritem
                using (var md5Hash = MD5.Create())
                {
                    // Byte array representation of source string
                    var sourceBytes = Encoding.UTF8.GetBytes(userPassword);

                    // Generate hash value(Byte Array) for input data
                    var hashBytes = md5Hash.ComputeHash(sourceBytes);

                    //using SALT
                   
                    // Convert hash byte array to string
                    var encryptedPassword = BitConverter.ToString(hashBytes).Replace("-", string.Empty);

                    userPassword = encryptedPassword;
                }

                line = "'" + userFirstName + "','" + userLastName + "','" + userEmail + "','" + userPassword + "'";
            }
            else
            {
                alert.SetTitle("Error!");
                alert.SetMessage("You're not connected to the internet, therefore you can't create an account.");
                alert.SetNeutralButton("Close", (sender, e) =>
                {

                });
                Dialog dialog = alert.Create();
                dialog.Show();
            }





        }

        private void ActLikeARequest()
        {
            
            Thread.Sleep(3000);
            //POVEZOVANJE Z BAZO--------------------
            
            
            //Database line
            string query1 = "insert into user(first_name,last_name,email,password) values(" + line + ")";
            //Connection
            MySqlConnection connect1 = new MySqlConnection(connectionString);
            connect1.Open();

            MySqlCommand cmd = new MySqlCommand(query1, connect1);
            //Execute
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            connect1.Close();
            
            RunOnUiThread(() => { mProgressBar.Visibility = ViewStates.Invisible; });

            //Preusmeri uporabnika na log in page
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            Dialog_SignIn signInDialog = new Dialog_SignIn();
            signInDialog.Show(transaction, "dialog fragment");

            signInDialog.mOnSignInComplete += signInDialog_mOnSignInComplete;


        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}