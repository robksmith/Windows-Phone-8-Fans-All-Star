
#region Usings

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;

#endregion

namespace Zengo.WP8.FAS
{
    public class IsolatedStorage
    {
        //public const string ProfileFilename = "userprofile.dat";

        //internal static ProfileViewModel GetProfile(string profileName)
        //{
        //    try
        //    {
        //        using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
        //        {
        //            if (file.FileExists(profileName))
        //            {
        //                DataContractSerializer serializer = new DataContractSerializer(typeof(ProfileViewModel));
        //                using (IsolatedStorageFileStream stream = file.OpenFile(profileName, System.IO.FileMode.Open))
        //                {
        //                    ProfileViewModel profile = serializer.ReadObject(stream) as ProfileViewModel;
        //                    return profile;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }

        //    return new ProfileViewModel();
        //}


        //internal static void SaveProfile(ProfileViewModel profile, string profileName)
        //{
        //    using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
        //    {
        //        using (IsolatedStorageFileStream stream = file.CreateFile(profileName))
        //        {
        //            DataContractSerializer serializer = new DataContractSerializer(typeof(ProfileViewModel));
        //            serializer.WriteObject(stream, profile);
        //            stream.Close();
        //        }
        //    }
        //}
    }
}
