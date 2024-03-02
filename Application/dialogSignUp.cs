using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Essentials;
using MySqlConnector;
namespace Application
{
    public class OnSignUpEventArgs : EventArgs
    {
        private string mFirstName;
        private string mLastName;
        private string mEmail;
        private string mPassword;

        public string FirstName
        {
            get { return mFirstName; }
            set { mFirstName = value; }
        }

        public string LastName
        {
            get { return mLastName; }
            set { mLastName = value; }
        }

        public string Email
        {
            get { return mEmail; }
            set { mEmail = value; }
        }

        public string Password
        {
            get { return mPassword; }
            set { mPassword = value; }
        }

        public OnSignUpEventArgs(string firstName, string lastName, string email, string password) : base()
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
           
        }
    }

    [Obsolete]
    class Dialog_SignUp : DialogFragment
    {
        private EditText mTxtFirstName;
        private EditText mTxtLastName;
        private EditText mTxtEmail;
        private EditText mTxtPassword;
        private Button mBtnSignUp;
        private TextView error;

        public event EventHandler<OnSignUpEventArgs> mOnSignUpComplete;
        //Regex preverjanje - zanke
        public bool isValidEmail(string email)
        {
            return Android.Util.Patterns.EmailAddress.Matcher(email).Matches();
        }

        public bool isValidFirstName(string firstname)
        {
            
            var valid  = Regex.IsMatch(firstname, "^[a-zA-Z]+$");
            return valid;

        }

        public bool isValidLastName(string lastname)
        {
            var valid1 = Regex.IsMatch(lastname, "^[a-zA-Z]+$");
            return valid1;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.dialog_sign_up, container, false);
            //Things in the textfields
            mTxtFirstName = view.FindViewById<EditText>(Resource.Id.txtFirstName);
            mTxtLastName = view.FindViewById<EditText>(Resource.Id.txtLastName);
            mTxtEmail = view.FindViewById<EditText>(Resource.Id.txtEmail);
            mTxtPassword = view.FindViewById<EditText>(Resource.Id.txtPassword);
            mBtnSignUp = view.FindViewById<Button>(Resource.Id.button1);
            error = view.FindViewById<TextView>(Resource.Id.textView1);
            string connectionString = "server=spiritofsakura.eu;port=3306;uid=spiritof_sakura;pwd=1q2w3e4r5t6z7u8i9o;database=spiritof_app;Allow User Variables=True";


            //textView results
            var showfname = view.FindViewById<TextView>(Resource.Id.textresultFName);
            var showlname = view.FindViewById<TextView>(Resource.Id.textresultLName);
            var showemail = view.FindViewById<TextView>(Resource.Id.textresultEmail);

            //verifier
            int verify = 0;

            /*
                var emailResult = isValidEmail(userEmail);

                //Email Check


                if (userEmail == "")
                {
                    showemail.Text = "Please enter your Email";
                }
                else
                {
                    if (emailResult == true)
                    {
                        showemail.Text = "Email is valid.";
                    }

                    else
                    {
                        showemail.Text = "Email is incorrect. Please try again";
                    }
                }
               */

            //Verify text inputs
            mTxtFirstName.TextChanged += (s, e) =>
            {
                var firstNameResult = isValidFirstName(mTxtFirstName.Text);
                if (firstNameResult == true)
                {
                    showfname.Text = "";
                    verify = 1;
                }
                else
                {
                    showfname.Text = "You can't use special characters. Please try again!";
                    verify = 0;
                }
            };

            mTxtLastName.TextChanged += (s, e) =>
            {
                var lastNameResult = isValidLastName(mTxtLastName.Text);
                if (lastNameResult == true)
                {
                    showlname.Text = "";
                    verify = 1;
                }
                else
                {
                    showlname.Text = "You can't use special characters. Please try again!";
                    verify = 0;
                }
            };
            
            mTxtEmail.TextChanged += (sender, e) =>
            {
                var emailResult = isValidEmail(mTxtEmail.Text);

                //Email Check



                if (emailResult == true)
                {
                    showemail.Text = "";
                    verify = 1;
                }
                
                else
                {
                    showemail.Text = "Email is incorrect. Please try again";
                    verify = 0;

                }

            };
            mTxtEmail.FocusChange += (sender, e) =>
            {
                MySqlConnection connect = new MySqlConnection(connectionString);
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM user WHERE email = @userEmail", connect);
                cmd.Parameters.AddWithValue("@userEmail",mTxtEmail.Text);
                connect.Open();

                //preveri, ali obstaja email ali ne
                var result = cmd.ExecuteScalar();
                if(result != null)
                {
                    //Email je že bil uporabljen
                    showemail.Text = "This email has already been used by another user. Please use a different one.";
                    verify = 0;
                }
                else
                {
                    //Email še ni bil uporabljen
                    verify = 1;
                }
                cmd.Dispose();
                connect.Close();
            };



            mBtnSignUp.Click += (s, e) =>
            {


                if (verify == 1)
                {
                    mOnSignUpComplete.Invoke(this, new OnSignUpEventArgs(mTxtFirstName.Text, mTxtLastName.Text, mTxtEmail.Text, mTxtPassword.Text));
                    this.Dismiss();
                }
                else
                {
                    error.Text = "Please fill out all the fields.";
                }
            };
                
                

            

            
            return view;
        }

    }
}