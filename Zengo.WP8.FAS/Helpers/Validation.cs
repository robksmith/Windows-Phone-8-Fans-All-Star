
#region Usings

using Zengo.WP8.FAS.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

#endregion

namespace Zengo.WP8.FAS
{
    public class Validation
    {
        #region Fields

        //private static string emailRegex;
        private static string emailRegex = @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$";
        //const string mobileRegex = @"^[0-9]{0,5}[ ]{0,1}[0-9]{0,6}$";

        #endregion


        internal static bool ValidateExists(FieldTitleAndError message, TextBox textbox, string watermark, string errorMessage)
        {
            string text = textbox.Text == watermark ? string.Empty : textbox.Text;

            if (string.IsNullOrEmpty(text))
            {
                textbox.Focus();

                message.SetError(errorMessage);

                return false;
            }
            else
            {
                message.ClearError();
                return true;
            }
        }


        internal static bool ValidateSelectorChosen(FieldTitleAndError message, string id, string errorMessage)
        {
            if (id == "0")
            {
                message.SetError(errorMessage);
                return false;
            }
            else
            {
                message.ClearError();
                return true;
            }
        }

        internal static bool ValidateSelectorChosenForFreeEntry(FieldTitleAndError message, string id, string errorMessage)
        {
            if (id == "-1")
            {
                message.SetError(errorMessage);
                return false;
            }

            message.ClearError();
            return true;
        }

        internal static bool ValidateExists(FieldTitleAndError message, PasswordBox passwordbox, string watermark, string errorMessage)
        {
            string text = passwordbox.Password == watermark ? string.Empty : passwordbox.Password;

            if (string.IsNullOrEmpty(text))
            {
                passwordbox.Focus();

                message.SetError(errorMessage);

                return false;
            }
            else
            {
                message.ClearError();
                return true;
            }
        }

        internal static bool EmailValid(FieldTitleAndError message, TextBox textbox, string watermark, string errorMessage)
        {
            string email = textbox.Text == watermark ? string.Empty : textbox.Text;

            if (!Regex.IsMatch(email, emailRegex))
            {
                textbox.Focus();

                message.SetError(errorMessage);

                return false;
            }
            else
            {
                message.ClearError();
                return true;
            }
        }

        internal static bool SameEmail(FieldTitleAndError message, TextBox textbox, string originalEmail, string watermark, string errorMessage)
        {
            string email = textbox.Text == watermark ? string.Empty : textbox.Text;

            if (email != originalEmail)
            {
                textbox.Focus();

                message.SetError(errorMessage);

                return false;
            }
            else
            {
                message.ClearError();
                return true;
            }
        }

        internal static bool SamePassword(FieldTitleAndError message, PasswordBox confirmPasswordbox, string originalPassword, string passwordConfirmWatermark, string errorMessage)
        {
            string text = confirmPasswordbox.Password == passwordConfirmWatermark ? string.Empty : confirmPasswordbox.Password;

            if (text != originalPassword)
            {
                confirmPasswordbox.Focus();

                message.SetError(errorMessage);

                return false;
            }
            else
            {
                message.ClearError();
                return true;
            }
        }

        internal static bool MinLength(FieldTitleAndError message, PasswordBox passwordbox, string watermark, string errorMessage)
        {
            string text = passwordbox.Password == watermark ? string.Empty : passwordbox.Password;

            if (text.Length < 6)
            {
                passwordbox.Focus();

                message.SetError(errorMessage);

                return false;
            }
            else
            {
                message.ClearError();
                return true;
            }
        }

    }
}
