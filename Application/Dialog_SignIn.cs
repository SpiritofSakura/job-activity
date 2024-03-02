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

namespace Application
{
    public class OnSignInEventArgs : EventArgs
    {
        private string mEmail;
        private string mPassword;


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

        public OnSignInEventArgs(string email, string password) : base()
        {
            Email = email;
            Password = password;
        }
    }
    
    [Obsolete]
    class Dialog_SignIn : DialogFragment
    {
        public bool isValidEmail(string email)
        {
            return Android.Util.Patterns.EmailAddress.Matcher(email).Matches();
        }
        private EditText mTxtEmail1;
        private EditText mTxtPassword1;
        private Button mButton_SignIn;
        private TextView showemail;
        private TextView error;

        public event EventHandler<OnSignInEventArgs> mOnSignInComplete;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.dialog_sign_in, container, false);

            
            mTxtEmail1 = view.FindViewById<EditText>(Resource.Id.txtEmail1);
            mTxtPassword1 = view.FindViewById<EditText>(Resource.Id.txtPassword1);
            mButton_SignIn = view.FindViewById<Button>(Resource.Id.button_signIn);
            showemail = view.FindViewById<TextView>(Resource.Id.textView1);
            error = view.FindViewById<TextView>(Resource.Id.textView2);

            int verify = 0;

            mTxtEmail1.TextChanged += (sender, e) =>
            {
                var emailResult = isValidEmail(mTxtEmail1.Text);

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

            
            mButton_SignIn.Click += (s, e) =>
            {
                if(verify == 1)
                {
                    mOnSignInComplete.Invoke(this, new OnSignInEventArgs(mTxtEmail1.Text, mTxtPassword1.Text));
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